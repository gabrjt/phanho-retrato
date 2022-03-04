using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;

[CreateAssetMenu]
public class SessionSaveMailSender : ScriptableObject
{
    readonly CancellationTokenContainer _cancellationToken = new();

    void OnDisable()
    {
        Cancel();
    }

    [Button]
    void Cancel()
    {
        _cancellationToken.Cancel();
    }

    public void OnResultSaved(SessionSave.Result result)
    {
        SendMail(ref result);
    }

    static string GetFormattedMailBody(SessionSave.Result result, string htmlDocumentText)
    {
        return htmlDocumentText.Replace("%NAME%", result.Name).Replace("%RESULT%", result.CharacterBodyPartsData.CharacterID.ToString());
    }

    void SendMail(ref SessionSave.Result result)
    {
        var to = result.Contact;
        var toName = result.Name;
        var characterID = result.CharacterBodyPartsData.CharacterID;

        if (string.IsNullOrEmpty(to))
        {
            return;
        }

        Assert.IsFalse(string.IsNullOrEmpty(toName));
        Assert.IsFalse(string.IsNullOrEmpty(to));

        SendMailServer(to, toName, characterID);
    }

    async void SendMailServer(string to, string toName, int characterID)
    {
        var wwwForm = new WWWForm();

        wwwForm.AddField("to", to);
        wwwForm.AddField("name", toName);
        wwwForm.AddField("characterID", characterID.ToString());

        using var unityWebRequest = UnityWebRequest.Post("https://raphaelrodrigues.art.br/mail-sender.php", wwwForm);

        var (cancelled, webRequest) = await unityWebRequest.SendWebRequest().WithCancellation(_cancellationToken.CancellationToken).SuppressCancellationThrow();

        if (cancelled)
        {
            return;
        }

        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"{nameof(SessionSaveMailSender)}::{nameof(SendMail)}: {webRequest.error}");

            return;
        }

        Debug.Log($"{nameof(SessionSaveMailSender)}::{nameof(SendMail)}: {webRequest.result} | Response: {webRequest.downloadHandler.text}");
    }

#if UNITY_EDITOR
    [Button]
    void TestSendMail()
    {
        var result = new SessionSave.Result
        {
            Name = _testName, Contact = _testMails[_testMailIndex], CharacterBodyPartsData = new CharacterBodyParts.CharacterBodyPartsData { CharacterID = _testID }
        };

        SendMail(ref result);
    }

    [SerializeField] string[] _testMails = { "gabr.j.t@gmail.com", "reflexo.retrato@gmail.com", "artesvisuaisrr@gmail.com" };
    [SerializeField] int _testMailIndex;
    [SerializeField] string _testName = "Gabiru";
    [SerializeField] int _testID = 64;
#endif
}