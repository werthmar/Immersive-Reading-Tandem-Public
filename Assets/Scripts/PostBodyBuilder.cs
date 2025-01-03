using System;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;

public class PostBodyBuilder
{
    // Send --- Json structure for req.body
    [Serializable]
    public class PostBody
    {
        public string lesson_input_id;
        public int alpha_level;
        public string sub_level;
        public Sample[] samples;
    }

    [Serializable]
    public class Sample
    {
        public string text;
        public string type;
        public VoiceRecording[] voice_recordings;

    }

    [Serializable]
    public class VoiceRecording
    {
        public string voice_record;
    }

    // Build the Json according to the API requirements. Variables like alpha level could be changed for other use cases
    public string BuildBody(string text, string base64audio)
    {
        PostBody postBody = new PostBody{
            lesson_input_id = "a1",
            alpha_level = 4,
            sub_level = "b",
            samples = new Sample[] {
                new Sample{
                    text = text,
                    type = "text",
                    voice_recordings = new VoiceRecording[] {
                        new VoiceRecording{
                            voice_record = base64audio
                        }
                    }
                }
            }
        };
        return JsonConvert.SerializeObject(postBody, Formatting.Indented);
    }

    // Receive ---
    /// <summary>
    ///  Data Structures
    /// </summary>
    // Exercuse response json data structure
    [Serializable]
    public class VoiceRecordingMetric
    {
        public int sample_word_count;
        public double word_accuracy_relative;
        public double word_accuracy_absolute;
        public int words_per_minute;
        public string text_with_annotated_errors;
        public string transcription;
        public int halting_count;
    }

    [Serializable]
    public class SampleMetric
    {
        public string text;
        public VoiceRecordingMetric[] voice_recording_metrics;
        public string type;
    }

    [Serializable]
    public class ExerciseResponse
    {
        public string lesson_input_id;
        public SampleMetric[] sample_metrics;
    }

    public List<ExerciseResponse> UnpackJsonString(string data)
    {
        // Deserialize the JSON array directly into a List<Lesson>
        List<ExerciseResponse> unpackedJson = JsonConvert.DeserializeObject<List<ExerciseResponse>>(data);

        // Access the parsed data
        foreach (var lesson in unpackedJson)
        {
            Debug.Log($"Lesson ID: {lesson.lesson_input_id}");
            foreach (var sample in lesson.sample_metrics)
            {
                Debug.Log($"Text: {sample.text}, Type: {sample.type}");
                foreach (var metric in sample.voice_recording_metrics)
                {
                    Debug.Log($"WPM: {metric.words_per_minute}, Halting Count: {metric.halting_count}");
                }
            }
        }
        return unpackedJson;
    }

}