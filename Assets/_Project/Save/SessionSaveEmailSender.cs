using System.Net;
using System.Net.Mail;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu]
public class SessionSaveEmailSender : ScriptableObject
{
    [SerializeField] AssetReference _htmlDocumentAssetReference;

    [Button]
    public async void Test()
    {
        var mail = new MailMessage();

        mail.From = new MailAddress("reflexo.retrato@gmail.com");
        mail.To.Add("artesvisuaisrr@gmail.com");
        mail.Subject = "[RETRATO] seu REFLEXO";
        
        await _htmlDocumentAssetReference.LoadAssetAsync<TextAsset>();

        mail.IsBodyHtml = true;
        mail.Body = ((TextAsset)_htmlDocumentAssetReference.Asset).text;

        var smtpServer = new SmtpClient("smtp.gmail.com");
        smtpServer.Port = 587;
        smtpServer.Credentials = new NetworkCredential("reflexo.retrato@gmail.com", "retrato666");
        smtpServer.EnableSsl = true;
        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

        smtpServer.Send(mail);
        Debug.Log("success");
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
}