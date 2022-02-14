using System;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu]
public class SessionSaveMailSender : ScriptableObject
{
    const int Port = 587;
    const bool EnableSSL = true;
    const string Host = "smtp.gmail.com";
    const string Subject = "[RETRATO] seu REFLEXO";
    [SerializeField] [Required] AssetReferenceContainer _htmlDocumentAssetReference;
    readonly CancellationTokenContainer _cancellationToken = new();
    readonly NetworkCredential _credentials = new("reflexo.retrato@gmail.com", "retrato666");
    readonly MailAddress _mailAddress = new("reflexo.retrato@gmail.com");

    void OnEnable()
    {
        ServicePointManager.ServerCertificateValidationCallback = ServerCertificateValidationCallback;
    }

    void OnDisable()
    {
        Cancel();

        _htmlDocumentAssetReference.TryUnloadCurrentAsset();
    }

    [Button]
    public void Cancel()
    {
        _cancellationToken.Cancel();
    }

    public void OnResultSaved(SessionSave.Result result)
    {
        SendMail(result);
    }

    bool ServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslpolicyerrors)
    {
        return true;
    }

    string GetFormattedMailBody(SessionSave.Result result, string htmlDocumentText)
    {
        return htmlDocumentText;
    }

    async void SendMail(SessionSave.Result result)
    {
        var to = result.Contact;

        if (string.IsNullOrEmpty(to))
        {
            return;
        }

        Assert.IsFalse(string.IsNullOrEmpty(result.Name));
        Assert.IsFalse(string.IsNullOrEmpty(to));

        var (success, htmlDocument) = await _htmlDocumentAssetReference.LoadCurrentAsset<TextAsset>();

        if (!success)
        {
            return;
        }

        MailMessage mailMessage;

        try
        {
            mailMessage = new MailMessage
            {
                From = _mailAddress,
                Subject = Subject,
                IsBodyHtml = true,
                Body = GetFormattedMailBody(result, htmlDocument.text),
                To = {to}
            };
        }
        catch (FormatException exception)
        {
            Debug.LogException(exception);

            return;
        }

        using (mailMessage)
        {
            _htmlDocumentAssetReference.TryUnloadCurrentAsset();

            using var smtpClient = new SmtpClient(Host) {Port = Port, Credentials = _credentials, EnableSsl = EnableSSL};
            var mailSentResult = new MailSentResult();

            smtpClient.SendCompleted += (sender, args) =>
            {
                if (args.Error != null)
                {
                    Debug.LogException(args.Error);

                    return;
                }

                if (args.Cancelled || mailSentResult.IsUnsent())
                {
                    return;
                }

                mailSentResult.SetSent();
            };

            void CancelSend()
            {
                mailSentResult.SetUnsent();
                smtpClient.SendAsyncCancel();
            }

            _cancellationToken.CancellationToken.Register(CancelSend);

            smtpClient.SendMailAsync(mailMessage);

            var cancelled = await UniTask.WaitUntil(mailSentResult.IsCompleted, PlayerLoopTiming.Update, _cancellationToken.CancellationToken).SuppressCancellationThrow();

            if (!cancelled)
            {
                _cancellationToken.Reset();
            }

            if (mailSentResult.IsSent())
            {
                Debug.Log($"{nameof(SessionSaveMailSender)}::{nameof(SendMail)} successfully sent mail to {to}");
            }
            else
            {
                Debug.LogWarning($"{nameof(SessionSaveMailSender)}::{nameof(SendMail)} cancelled mail send to {to}");
            }
        }
    }

#if UNITY_EDITOR
    [Button]
    void TestSendMail()
    {
        var result = new SessionSave.Result {Contact = "gabr.j.t@gmail.com"};

        SendMail(result);
    }
#endif

    class MailSentResult
    {
        bool _completed;
        bool _sent;

        public bool IsSent()
        {
            return _sent;
        }

        public bool IsUnsent()
        {
            return !_sent && _completed;
        }

        public void SetSent()
        {
            _completed = _sent = true;
        }

        public void SetUnsent()
        {
            _sent = false;
            _completed = true;
        }

        public bool IsCompleted()
        {
            return _completed;
        }
    }
}