using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using NaughtyAttributes;
using UnityEngine;
using Debug = UnityEngine.Debug;

[CreateAssetMenu]
public class SessionSave : ScriptableObject, IDisposable
{
    [SerializeField] [Required] CharacterBodyParts _characterBodyParts;
    [SerializeField] UnityEventSessionSaveResult _resultSaved;
    readonly CancellationTokenContainer _cancellationToken = new();
    readonly Dictionary<int, string> _resultsPathDictionary = new();
    readonly SemaphoreSlim _semaphore = new(1, 1);
    readonly Stopwatch _stopwatch = new();
    bool _disposed;

    public bool IsCreated => !_disposed;

    void OnDisable()
    {
        Dispose();
    }

    public void Dispose()
    {
        _disposed = true;
        _cancellationToken.Cancel();
        _resultsPathDictionary.Clear();
        _stopwatch.Reset();
        _semaphore.Dispose();
    }

    public void Clear()
    {
        _disposed = false;
        _cancellationToken.Reset();
        _stopwatch.Reset();
    }

    public void Cancel()
    {
        _cancellationToken.Cancel();
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

        try
        {
            await _semaphore.WaitAsync(_cancellationToken.CancellationToken);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);

            return;
        }

        Results results;

        if (File.Exists(path))
        {
            using var streamReader = new StreamReader(path);

            try
            {
                results = JsonUtility.FromJson<Results>(await streamReader.ReadToEndAsync());
            }
            catch (ArgumentException exception)
            {
                Debug.LogException(exception);

                File.Delete(path);

                results = new Results {Values = new List<Result>()};
            }
        }
        else
        {
            results = new Results {Values = new List<Result>()};
        }

        var result = new Result(results.Values.Count, username, contact, minutes, characterBodyPartsData);

        results.Values.Add(result);

        await using var streamWriter = new StreamWriter(path);

        await streamWriter.WriteAsync(JsonUtility.ToJson(results));

        Debug.Log($"{nameof(Save)} {nameof(Results)} @ {path}");

        if (_disposed)
        {
            return;
        }

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