using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu]
public class SessionSaveMailSender : ScriptableObject
{
    const int Port = 587;
    const bool EnableSSL = true;
    const string Host = "smtp.gmail.com";
    const string Subject = "[RETRATO] seu REFLEXO";
    [SerializeField] [Required] AssetReferenceContainer _htmlDocumentAssetReference;
    readonly CancellationTokenContainer _cancellationToken = new();
    readonly MailAddress _mailAddress = new("reflexo.retrato@gmail.com");
    readonly NetworkCredential _networkCredential = new("reflexo.retrato@gmail.com", "retrato666");

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
        return;

        var mail = new MailMessage();

        mail.From = new MailAddress("gabr.j.t@gmail.com");
        mail.To.Add("gabr.j.t@gmail.com");
        mail.Subject = "Test Mail";
        mail.Body = $"This is for testing SMTP mail from GMAIL <img src='data:image/png;base64,{result.ID}' />";

        var smtpServer = new SmtpClient("smtp.gmail.com");
        smtpServer.Port = 587;
        smtpServer.Credentials = new NetworkCredential("gabr.j.t@gmail.com", "G@butops2029");
        smtpServer.EnableSsl = true;
        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

        smtpServer.Send(mail);
        Debug.Log("success");
    }

    bool ServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslpolicyerrors)
    {
        return true;
    }

#if UNITY_EDITOR
    [Button]
    async void TestSendMail()
    {
        var (success, htmlDocument) = await _htmlDocumentAssetReference.LoadCurrentAsset<TextAsset>();

        if (!success)
        {
            return;
        }

        using var mailMessage = new MailMessage {From = _mailAddress};

        const string to = "gabr.j.t@gmail.com";

        mailMessage.To.Add(to);
        mailMessage.Subject = Subject;
        mailMessage.IsBodyHtml = true;
        mailMessage.Body = htmlDocument.text;

        _htmlDocumentAssetReference.TryUnloadCurrentAsset();

        using var smtpClient = new SmtpClient(Host) {Port = Port, Credentials = _networkCredential, EnableSsl = EnableSSL};
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
                Debug.LogWarning($"{nameof(SessionSaveMailSender)}::{nameof(TestSendMail)} cancelled mail send to {to}");

                return;
            }

            mailSentResult.SetSent();

            Debug.Log($"{nameof(SessionSaveMailSender)}::{nameof(TestSendMail)} successfully sent mail to {to}");
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