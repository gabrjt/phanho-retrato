using System.Net;
using System.Net.Mail;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu]
public class SessionSaveEmailSender : ScriptableObject
{
    [Button]
    public void Test()
    {
        var mail = new MailMessage();

        mail.From = new MailAddress("gabr.j.t@gmail.com");
        mail.To.Add("gabr.j.t@gmail.com");
        mail.Subject = "Test Mail";
        mail.Body = $"This is for testing SMTP mail from GMAIL <img src='data:image/png;base64,{123}' />";

        var smtpServer = new SmtpClient("smtp.gmail.com");
        smtpServer.Port = 587;
        smtpServer.Credentials = new NetworkCredential("gabr.j.t@gmail.com", "G@butops2029");
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