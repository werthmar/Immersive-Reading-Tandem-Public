using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

/// <summary>
/// Code Generated using ChatGPT and edited and expanded manually.
/// </summary>

public class RecordAudio : MonoBehaviour
{
    public GameObject loadingSpinner;
    public GameObject storyTextContainer;
    public TextMeshProUGUI asr4dvvErrorText;
    public TextGenerator textGenerator;
    private AudioSource audioSource;
    private AudioClip microphoneClip;
    private string microphoneName;
    private bool isRecording;
    private ApiManager apiManager;
    PostBodyBuilder postBodyBuilder = new PostBodyBuilder();
    // Set to satisfy API conditions
    private const int SampleRate = 16000;

    void Start()
    {
        // Audio source used for testing only
        audioSource = GetComponent<AudioSource>();

        // Handles API requests
        apiManager = FindObjectOfType<ApiManager>();
        
        isRecording = false;

        // Check if the microphone is available
        if (Microphone.devices.Length > 0)
        {
            // Get the default microphone
            microphoneName = Microphone.devices[0];
        }
        else
        {
            Debug.LogError("No microphone found!");
        }
    }

    public void toggleRecording()
    {
        if (!isRecording)
        {
            StartRecording();
        }
        else
        {
            StopRecording();
        }
    }

    void StartRecording()
    {
        if (microphoneName != null)
        {
            isRecording = true;
            // Start recording with the microphone.
            microphoneClip = Microphone.Start(microphoneName, true, 10, SampleRate);
            Debug.Log("Recording started...");
        }
    }

    void StopRecording()
    {
        isRecording = false;
        Microphone.End(microphoneName);
        Debug.Log("Recording stopped.");

        // Display loading animation
        storyTextContainer.SetActive(false);
        asr4dvvErrorText.gameObject.SetActive(false);
        loadingSpinner.SetActive(true);

        // Assign the clip to the AudioSource and play it (for testing)
        //audioSource.clip = microphoneClip;
        //audioSource.Play();
        //Debug.Log("Playing the recorded audio...");

        // Encode audio to base64
        string base64Audio = ConvertAudioClipToBase64(microphoneClip);

        // Get the text that was being read
        string storyText = textGenerator.GetStoryText();

        // Build post request body/payload
        string payload = postBodyBuilder.BuildBody(storyText, base64Audio);
        //string payload = JsonConvert.SerializeObject(jsonPayload); //convert to string
        Debug.Log(payload);

        // Sent recording to API for analasys
        apiManager.ApiCall(
            "/exercise", //"/ping"
            (result) => DisplayReadingResult(result),
            (error) => DisplayConnectionError(error),
            payload
        );

    }

    void DisplayConnectionError(string error)
    {
        Debug.Log("Error: " + error);
        asr4dvvErrorText.text = error;
        loadingSpinner.SetActive(false);
        asr4dvvErrorText.gameObject.SetActive(true);
    }

    void DisplayReadingResult(string result)
    {
        // Unwrap the result to json
        List<PostBodyBuilder.ExerciseResponse> resultJson = postBodyBuilder.UnpackJsonString(result);
        Debug.Log(resultJson);
        
        // Display results here
        string resultString = resultJson[0].sample_metrics[0].voice_recording_metrics[0].text_with_annotated_errors;
        textGenerator.DisplayReadingResults(resultString);

        // Disable loading animation
        loadingSpinner.SetActive(false);
        storyTextContainer.SetActive(true);
    }

    /// <summary>
    /// Audio to Base 64 Converter (Audio -> .wav -> base64)
    /// </summary>
    /// <param name="recordedClip"></param>
    /// <returns>base64 audio in string format</returns>
    string ConvertAudioClipToBase64(AudioClip recordedClip)
    {
        // Convert to WAV and Base64
        byte[] wavData = ConvertClipToWav(recordedClip);
        string base64Audio = Convert.ToBase64String(wavData);
        return base64Audio;
    }

    private byte[] ConvertClipToWav(AudioClip clip)
    {
        int totalSamples = clip.samples;
        float[] samples = new float[totalSamples];
        clip.GetData(samples, 0);
        byte[] byteArray = new byte[samples.Length * 2];
        
        int index = 0;
        foreach (float sample in samples)
        {
            short intSample = (short)(sample * short.MaxValue);
            byteArray[index++] = (byte)(intSample & 0xFF);
            byteArray[index++] = (byte)((intSample >> 8) & 0xFF);
        }

        byte[] wavHeader = CreateWavHeader(byteArray.Length, SampleRate);
        byte[] wavFile = new byte[wavHeader.Length + byteArray.Length];
        Buffer.BlockCopy(wavHeader, 0, wavFile, 0, wavHeader.Length);
        Buffer.BlockCopy(byteArray, 0, wavFile, wavHeader.Length, byteArray.Length);

        return wavFile;
    }

    private byte[] CreateWavHeader(int audioLength, int sampleRate)
    {
        byte[] header = new byte[44];
        Buffer.BlockCopy(Encoding.UTF8.GetBytes("RIFF"), 0, header, 0, 4);
        BitConverter.GetBytes(36 + audioLength).CopyTo(header, 4);
        Buffer.BlockCopy(Encoding.UTF8.GetBytes("WAVE"), 0, header, 8, 4);
        Buffer.BlockCopy(Encoding.UTF8.GetBytes("fmt "), 0, header, 12, 4);
        BitConverter.GetBytes(16).CopyTo(header, 16);
        BitConverter.GetBytes((short)1).CopyTo(header, 20);
        BitConverter.GetBytes((short)1).CopyTo(header, 22);
        BitConverter.GetBytes(sampleRate).CopyTo(header, 24);
        BitConverter.GetBytes(sampleRate * 2).CopyTo(header, 28);
        BitConverter.GetBytes((short)2).CopyTo(header, 32);
        BitConverter.GetBytes((short)16).CopyTo(header, 34);
        Buffer.BlockCopy(Encoding.UTF8.GetBytes("data"), 0, header, 36, 4);
        BitConverter.GetBytes(audioLength).CopyTo(header, 40);
        return header;
    }
}
