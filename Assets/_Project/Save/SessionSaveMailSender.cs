using System;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;

[CreateAssetMenu]
public class SessionSaveMailSender : ScriptableObject
{
    const string Subject = "[RETRATO] seu REFLEXO";
    [SerializeField] string _host = "smtp.gmail.com";
    [SerializeField] int _port = 587;
    [SerializeField] bool _enableSSL = true;
    [SerializeField] bool _useWebServer = true;
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
        return htmlDocumentText.Replace("%NAME%", result.Name).Replace("%RESULT%", result.CharacterBodyPartsData.CharacterID.ToString());
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

        if (_useWebServer)
        {
            SendMailServer(result, to, htmlDocument);
        }
        else
        {
            SendMailSMTP(result, to, htmlDocument);
        }
    }

    async void SendMailSMTP(SessionSave.Result result, string to, TextAsset htmlDocument)
    {
        MailMessage mailMessage;

        try
        {
            mailMessage = new MailMessage
            {
                From = _mailAddress,
                Subject = Subject,
                IsBodyHtml = true,
                Body = GetFormattedMailBody(result, htmlDocument.text),
                To = { to }
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

            using var smtpClient = new SmtpClient(_host, _port) { Credentials = _credentials, EnableSsl = _enableSSL, UseDefaultCredentials = false };
            var mailSentResult = new MailSentResult();

            smtpClient.SendCompleted += OnSendCompleted;

            void OnSendCompleted(object sender, AsyncCompletedEventArgs args)
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
            }

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

    async void SendMailServer(SessionSave.Result result, string to, TextAsset htmlDocument)
    {
        var wwwForm = new WWWForm();

        wwwForm.AddField("toEmail", to);

        using var unityWebRequest = UnityWebRequest.Post("https://raphaelrodrigues.art.br/mail-sender.php", wwwForm);

        await unityWebRequest.SendWebRequest();

        if (unityWebRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"{nameof(SessionSaveMailSender)}::{nameof(SendMail)}: {unityWebRequest.error}");

            return;
        }

        Debug.Log($"{nameof(SessionSaveMailSender)}::{nameof(SendMail)}: {unityWebRequest.result}");

        foreach (var responseHeader in unityWebRequest.GetResponseHeaders())
        {
            Debug.Log($"{responseHeader.Key} {responseHeader.Value}");
        }

        Debug.Log(unityWebRequest.downloadHandler.text);
    }

#if UNITY_EDITOR
    [Button]
    void TestSendMail()
    {
        SendMail(new SessionSave.Result { Name = "Gabiru", Contact = "gabr.j.t@gmail.com", CharacterBodyPartsData = new CharacterBodyParts.CharacterBodyPartsData { CharacterID = 128 } });
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