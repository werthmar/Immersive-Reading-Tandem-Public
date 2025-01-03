using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace TMPro{

    public class TestConnectionButton : MonoBehaviour
    {
        [SerializeField] public TMP_InputField googleField;
        [SerializeField] public TMP_InputField asr4dvvField;
        [SerializeField] public TMP_Text asrResultDisplay;
        [SerializeField] public TMP_Text googleResultDisplay;

        private ApiManager apiManager;
        public GoogleTranslateManager googleTranslateManager;

         private void Start()
        {
            apiManager = FindObjectOfType<ApiManager>();
        }
        
        public void TestConnectionAndSave()
        {   
            if(asr4dvvField.text != "")
            {
                // Save API in settings
                PlayerPrefs.SetString("GOOGLE_API_KEY", googleField.text);
                PlayerPrefs.SetString("ASR4DVV_API_URL", asr4dvvField.text);
                PlayerPrefs.Save(); // Ensure data is saved
                
                // Test ASR4DVV API
                apiManager.ApiCall(
                    "/ping",
                    (string result) =>  asrResultDisplay.text = "ASR4DVV response: " + result,
                    (string error) => asrResultDisplay.text =  "ASR4DVV: " + error
                );

                // Test Google API Key
                googleTranslateManager.Initialize();
                googleTranslateManager.TranslateWord("käse", (result) => googleResultDisplay.text = (result == "cheese") ? "Google response: connection succesfull" : "Google response: " + result );
            }
            else
            {
                asrResultDisplay.text = "Please Input a URL first";
            }

        }

        
    }

}
