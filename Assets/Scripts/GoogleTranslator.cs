using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using System.IO;
using System;

/// <summary>
/// Handles word translation
/// Generated using chatgpt
/// Using google cloud api, api key is to be placed in Assets/API_Keys/api_key.json
///     -> {"api-key": "xxx"}
/// </summary>

public class GoogleTranslateManager : MonoBehaviour
{
    private string apiKey; // Set your API key here

    void Start()
    {
        try
        {
            // Load the default key in env if no key was inputed in main menu.
            LoadApiKey();
        }
        catch(Exception e)
        {
            Debug.Log(e);
        }

        // Example call to test translation functionality
        //StartCoroutine(TranslateText("Hello, world!", "es", (translatedText) =>
        //{
        //    Debug.Log("Translated Text: " + translatedText);
        //}));
    }

    public void Initialize()
    {
        LoadApiKey();
    }

    public void TranslateWord(string word, Action<string> callback)
    {
        StartCoroutine(TranslateTextAPICall(word, "en", callback));
    }

    // Method to load the API key from a JSON file
    private void LoadApiKey()
    {
        apiKey = PlayerPrefs.GetString("GOOGLE_API_KEY");

        /* .env dosnt work on headset so its hardcorded instead
        if (apiKey == "") 
        {
            try
            {
                // Construct the full path to the API key JSON file
                string path = Path.Combine(Application.dataPath, "API_Keys/api_key.json");

                if (!File.Exists(path))
                {
                    Debug.LogError("Cannot find the API key JSON file.");
                    return;
                }

                // Read the JSON file content
                string jsonText = File.ReadAllText(path);

                // Parse JSON to get the API key
                var json = JObject.Parse(jsonText);
                apiKey = json["api-key"]?.ToString();

                if (string.IsNullOrEmpty(apiKey))
                {
                    Debug.LogError("API key not found in JSON.");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error reading API key: " + ex.Message);
            }
        }
        */

    }

    private IEnumerator TranslateTextAPICall(string textToTranslate, string targetLanguage, System.Action<string> callback)
    {
        string url = $"https://translation.googleapis.com/language/translate/v2?key={apiKey}";
        string jsonData = "{\"q\": \"" + textToTranslate + "\",\"source\": \"de\",\"target\": \"" + targetLanguage + "\"}";

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
                callback?.Invoke("Here would be a translation. Please update the Google API Key");
            }
            else
            {
                // Parse JSON response using Json.NET
                var jsonResponse = JObject.Parse(request.downloadHandler.text);
                var translatedText = jsonResponse["data"]["translations"][0]["translatedText"].ToString();
                callback?.Invoke(translatedText);
            }
        }
    }
}
