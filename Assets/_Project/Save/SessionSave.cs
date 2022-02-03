using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using NaughtyAttributes;
using UnityEngine;
using Debug = UnityEngine.Debug;

[CreateAssetMenu]
public class SessionSave : ScriptableObject
{
    [SerializeField] [Required] CharacterBodyParts _characterBodyParts;
    [SerializeField] UnityEventSessionSaveResult _resultSaved;
    readonly Dictionary<int, string> _resultsPathDictionary = new();
    readonly SemaphoreSlim _semaphore = new(1, 1);
    readonly Stopwatch _stopwatch = new();

    void OnDisable()
    {
        _resultsPathDictionary.Clear();
        _stopwatch.Reset();
        _semaphore.Dispose();
    }

    [Button]
    public void Begin()
    {
        _characterBodyParts.Dispose();
        _stopwatch.Restart();
    }

    [Button]
    public void End()
    {
        End(string.Empty, string.Empty);
    }

    public void End(string username, string contact)
    {
        _stopwatch.Stop();

        Save(username, contact, _stopwatch.Elapsed.Minutes, _characterBodyParts.Data);
    }

    string GetResultsPath()
    {
        var key = DateTime.Now.DayOfYear;

        if (!_resultsPathDictionary.TryGetValue(key, out var path))
        {
            _resultsPathDictionary[key] = path = $"{Application.persistentDataPath}{Path.DirectorySeparatorChar}RESULTADOS_{DateTime.Now.Date:dd-MM-yyyy}.json";
        }

        return path;
    }

    async void Save(string username, string contact, int minutes, CharacterBodyParts.CharacterBodyPartsData characterBodyPartsData)
    {
        var path = GetResultsPath();

        Results results;

        await _semaphore.WaitAsync();

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

        var result = new Result(results.Values.Count, username, contact, minutes, characterBodyPartsData);

        results.Values.Add(result);

        var file = new StreamWriter(path);

        await file.WriteAsync(JsonUtility.ToJson(results));

        Debug.Log($"{nameof(Save)}@{path}");

        file.Close();

        await file.DisposeAsync();

        _semaphore.Release();

        _resultSaved.Invoke(result);
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
        public CharacterBodyParts.CharacterBodyPartsData CharacterBodyPartsData;

        public Result(int id, string name, string contact, int minutes, CharacterBodyParts.CharacterBodyPartsData characterBodyPartsData)
        {
            const string notAvailable = "N/D";

            ID = id;
            Name = string.IsNullOrEmpty(name) ? notAvailable : name;
            Contact = string.IsNullOrEmpty(contact) ? notAvailable : contact;
            Minutes = minutes;
            CharacterBodyPartsData = characterBodyPartsData;
        }
    }
}