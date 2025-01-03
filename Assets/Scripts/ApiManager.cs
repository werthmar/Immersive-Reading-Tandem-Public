using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Metadata;
using UnityEngine.Networking;

/// <summary>
/// Made Using the Unity documentation and ChatGPT
/// </summary>
/// 

public class InsecureCertificateHandler : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        // Always return true to bypass certificate validation
        // WARNING: This is insecure and should only be used for testing or development
        return true;
    }
}


public class ApiManager : MonoBehaviour
{
    // Ping response json data structure
    [Serializable]
    public class ApiResponse
    {
        public string data;
    }


    /// <summary>
    /// Api Call
    /// </summary>
    /// <param name="route">e.g. /ping or /exercise</param>
    /// <param name="onSuccess">what happens when call runs succesfully</param>
    /// <param name="onFail">what happens when call fails</param>
    /// <param name="payload">=post.body as string</param>

    // Make a call to the SpeechAPI, route = '/pong'. API url set in home screen -> settings, post request, payload optional
    public void ApiCall(string route, Action<string> onSuccess, Action<string> onFail, string payload = "")
    {
        string api_url = PlayerPrefs.GetString("ASR4DVV_API_URL", "http://192.168.0.204:8000/v7"); // Provide a default value if not found
        if (api_url == "") api_url = "http://192.168.0.204:8000/v7"; // Default setter in PlayerPrefs dosn't work everytime
        //string api_url = "http://192.168.0.204:8000/v7"; // Provide a default value if not found
        Debug.Log("Loaded API: " + api_url);
        // Generate the complete URL for the call
        api_url += route;

        StartCoroutine(
            ApiCall(
                api_url,
                payload,
                result => {

                    if (result.error != null) {
                        Debug.Log("ASR4DVV: " + result.error);
                        onFail?.Invoke(result.error);
                    }
                    else {
                        var data = result.downloadHandler.text;
                        string jsonData;

                        // For ping call
                        try
                        {
                            jsonData = JsonUtility.FromJson<ApiResponse>(data).data;
                        }
                        // For excercise call
                        catch
                        {
                            jsonData = data;
                        }

                        onSuccess?.Invoke(jsonData);
                    }


                        // ApiResponse jsonData;
                        // Option 1: Parse using JsonUtility (If response is in Json)
                        //stringData = JsonUtility.FromJson<ApiResponse>(data).data;
                        
                        //Debug.Log("TEST!!!! " + result);
                        //Debug.Log("Output: " + jsonData.data);
                        
                        // Pass the data back
                        //onSuccess?.Invoke(stringData);
                }
            )
        );
    }

    private IEnumerator ApiCall( string route, string payload, Action<UnityWebRequest> callback)
    {

        //using (UnityWebRequest www = UnityWebRequest.Post("http://192.168.0.204:8000/v7/ping", "{ \"field1\": 1, \"field2\": 2 }", "application/json"))
        using (UnityWebRequest www = new UnityWebRequest( route, "POST" ))
        {
            // Set the request body
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(payload);
            www.uploadHandler = new UploadHandlerRaw(jsonToSend);
            
            // Attach the custom certificate handler to bypass SSL validation
            www.certificateHandler = new InsecureCertificateHandler();

            // Optional: You may need to set a content type header
            www.SetRequestHeader("Content-Type", "application/json" );
            www.SetRequestHeader("Accept", "application/json");

            // No need to set a request body for this POST request
            www.downloadHandler = new DownloadHandlerBuffer();

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
                callback?.Invoke(www);
            }
            else
            {
                Debug.Log("Form upload complete!");
                callback?.Invoke(www);
            }
        }

    }

}
