using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NaughtyAttributes;
using UnityEngine;
using Debug = UnityEngine.Debug;

[CreateAssetMenu]
public class SessionSave : ScriptableObject
{
    [SerializeField] [Required] CharacterBodyParts _characterBodyParts;
    readonly Stopwatch _stopwatch = new();

    [Button]
    public void Begin()
    {
        _characterBodyParts.Dispose();
        _stopwatch.Restart();
    }

    [Button]
    public void End()
    {
        _stopwatch.Stop();

        Save(string.Empty, string.Empty);
    }

    async void Save(string username, string contact)
    {
        var path = $"{Application.persistentDataPath}{Path.DirectorySeparatorChar}RESULTADOS_{DateTime.Now.Date:dd-MM-yyyy}.json";

        Results results;

        if (File.Exists(path))
        {
            var streamReader = new StreamReader(path);

            try
            {
                results = JsonUtility.FromJson<Results>(await streamReader.ReadToEndAsync());
            }
            catch (ArgumentException argumentException)
            {
                Debug.LogWarning(argumentException);

                File.Delete(path);

                results = new Results {Values = new List<Result>()};
            }
            finally
            {
                streamReader.Close();
                streamReader.Dispose();
            }
        }
        else
        {
            results = new Results {Values = new List<Result>()};
        }

        results.Values.Add(new Result(results.Values.Count, username, contact, _stopwatch.Elapsed.Minutes, _characterBodyParts));

        var file = new StreamWriter(path);

        await file.WriteAsync(JsonUtility.ToJson(results));

        Debug.Log($"{nameof(Save)}@{path}");

        file.Close();

        await file.DisposeAsync();
    }

    [Serializable]
    public struct Results
    {
        public List<Result> Values;
    }

    [Serializable]
    public struct Result
    {
        public int ID;
        public string Name;
        public string Contact;
        public int Minutes;
        public int TailID;
        public string TailName;
        public int LegsID;
        public string LegsName;
        public int ArmsID;
        public string ArmsName;
        public int HeadID;
        public string HeadName;
        public int CharacterID;

        public Result(int id, string name, string contact, int minutes, CharacterBodyParts characterBodyParts)
        {
            const string notAvailable = "N/D";

            ID = id;
            Name = string.IsNullOrEmpty(name) ? notAvailable : name;
            Contact = string.IsNullOrEmpty(contact) ? notAvailable : contact;
            Minutes = minutes;
            TailID = characterBodyParts.Tail ? characterBodyParts.Tail.ID : -1;
            TailName = characterBodyParts.Tail ? characterBodyParts.Tail.Name : string.Empty;
            LegsID = characterBodyParts.Legs ? characterBodyParts.Legs.ID : -1;
            LegsName = characterBodyParts.Legs ? characterBodyParts.Legs.Name : string.Empty;
            ArmsID = characterBodyParts.Arms ? characterBodyParts.Arms.ID : -1;
            ArmsName = characterBodyParts.Arms ? characterBodyParts.Arms.Name : string.Empty;
            HeadID = characterBodyParts.Head ? characterBodyParts.Head.ID : -1;
            HeadName = characterBodyParts.Head ? characterBodyParts.Head.Name : string.Empty;
            CharacterID = characterBodyParts.ID;
        }
    }
}