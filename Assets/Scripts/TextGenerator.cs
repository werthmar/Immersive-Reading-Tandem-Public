using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Made using ChatGPT and manually edited. Generates Buttons used to display the text and enable Translation of unique words.
/// </summary>
public class TextGenerator : MonoBehaviour
{
    public RectTransform buttonPrefab; // Assign your button prefab with TextMeshPro inside
    public Transform buttonContainer;
    public GameObject loadingSpinner;
    public GoogleTranslateManager translator;
    public StoryDataManager storyDataManager;
    public Canvas tutorialCanvas;
    public Canvas translationCanvas;
    public TextMeshProUGUI translatedWordDisplay;
    public TextMeshProUGUI translatedTextDisplay;
    // Enable or Disable Buttons based on scene and state
    public Button nextSceneButton;
    public Toggle microphoneButton;
    public Button audioButton;
    public TextMeshProUGUI interactiveTaskResultText;


    //public float spacing = 2; // Space between elements

    // List to store all the textbuttons to be able to change their state later on
    private List<Button> storyTextButtons = new();

    void Start()
    {
        Loading(true);
        Initialize(interactable: false, lastScene: false);
        Loading(false);
    }

    public void Initialize(bool interactable, bool lastScene = false)
    {
        // Make sure the right pages are displayed
        tutorialCanvas.gameObject.SetActive(true);
        translatedWordDisplay.gameObject.SetActive(true);
        translatedTextDisplay.gameObject.SetActive(false);
        translationCanvas.gameObject.SetActive(false);
        interactiveTaskResultText.gameObject.SetActive(false);

        // You have to read 70% (changed from 80%) of the text right to unlock next scene or complete the interactive task
        nextSceneButton.interactable = false;

        // Logic if next scene is interactable -> disable microphone button
        microphoneButton.interactable = !interactable;

        if (lastScene)
        {
            microphoneButton.interactable = false;
            audioButton.interactable = false;
        }
        
        // Hide obj while its being build and display the loader instead
        //Loading(true);

        // Clear the old story data
        storyTextButtons.Clear();
        foreach (Transform child in buttonContainer.transform)
        {
            Destroy(child.gameObject); // Destroy each button GameObject
        }

        // Load the current story
        string story = storyDataManager.GetStoryText();

        string[] words = story.Split(' ');

        foreach (string word in words)
        {
            Button storyButton = CreateButtonForWord(word);
            storyTextButtons.Add(storyButton);
            
            //RectTransform rt = storyButton.GetComponent<RectTransform>();
            //StartCoroutine(GetHeightAfterLayout(rt));
        }

        // Arange Elements in horizantally and vertically to make a readable text out of the buttons
        StartCoroutine(ArrangeElements());
    }

    Button CreateButtonForWord(string word)
    {
        // Instantiate button prefab
        RectTransform buttonTransform = Instantiate(buttonPrefab, buttonContainer);
        
        // Set the button's text to be the word
        TextMeshProUGUI wordText = buttonTransform.GetComponentInChildren<TextMeshProUGUI>();
        wordText.text = word;


        // Add a click listener to the button
        Button button = buttonTransform.GetComponent<Button>();
        button.onClick.AddListener(() => TranslateWord(wordText));

        // Set the size of the button to be the size of the text (Because content size fitter adds wierd margin)
        float preferredWidth = wordText.GetPreferredValues().x;
        float preferredHeight = wordText.GetPreferredValues().y;

        // Access the RectTransform of the button
        RectTransform buttonRectTransform = button.GetComponent<RectTransform>();

        // Set the width to the preferred width of the TextMeshPro text
        buttonRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, preferredWidth);
        buttonRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, preferredHeight);

        return button;
    }

    void TranslateWord(TextMeshProUGUI textElement)
    {
        // Define a pattern to match punctuation characters
        string pattern = @"[.,!?\-""()]";

        // Use Regex to replace these characters with an empty string
        string cleanedWord = Regex.Replace(textElement.text, pattern, string.Empty);

        translator.TranslateWord(cleanedWord, (translatedWord) =>
        {
            // Implement your translation logic or API call here
            Debug.Log("Translating: " + translatedWord);

            // Display translation
            if(tutorialCanvas.isActiveAndEnabled) //Canvas visible
            {
                tutorialCanvas.gameObject.SetActive(false);
                translationCanvas.gameObject.SetActive(true); 
            }
            if(!translatedWordDisplay.isActiveAndEnabled) //Textobj visible
            {
                translatedWordDisplay.gameObject.SetActive(true);
                translatedTextDisplay.gameObject.SetActive(false);
            }
            translatedWordDisplay.text = $"{cleanedWord} = {translatedWord}";
        });
    }

    // Show a translation of the entire Text
    public void TranslateCompleteText()
    {
        if(!translatedTextDisplay.isActiveAndEnabled) //Textobj visible
        {
            translatedWordDisplay.gameObject.SetActive(false);
            translatedTextDisplay.gameObject.SetActive(true);
        }
        string translatedText = storyDataManager.GetStoryTextTranslation();
        translatedTextDisplay.text = translatedText;
    }

    IEnumerator ArrangeElements()
    {
        float spacing = 3f; // Distance between words
        float leftPadding = 2f; // Padding on the left side
        float verticalPadding = 2f; //padding at the start and between lines

        // Optionally force canvas update if necessary -> it is necessary
        Canvas.ForceUpdateCanvases();
        // Wait for end of frame
        yield return new WaitForEndOfFrame();
        RectTransform rt = buttonContainer.GetComponent<RectTransform>();
        float panelWidth = rt.rect.width;
        float xOffset = 0 + leftPadding;
        float yOffset = 0;
        float rowHeight = 0;

        foreach (Button element in storyTextButtons)
        {
            RectTransform childRect = element.GetComponent<RectTransform>();
            float elementWidth = childRect.rect.width;
            float elementHeight = childRect.rect.height;
            //Debug.Log($"Width: {elementWidth}, Height: {elementHeight}");

            if (xOffset + elementWidth > panelWidth) // Wrap to next line
            {
                xOffset = 0 + leftPadding;
                yOffset -= rowHeight + verticalPadding; // + spacing;
                rowHeight = 0;
            }

            childRect.anchoredPosition = new Vector2(xOffset, yOffset);

            xOffset += elementWidth + spacing;
            rowHeight = Mathf.Max(rowHeight, elementHeight);
        }

        // Make it visible again and disable loadingSpinner
        //Loading(false);
    }

    public void Loading(bool state)
    {
        loadingSpinner.SetActive(state);
        buttonContainer.gameObject.SetActive(!state);
    }

    // --- Get text, show results ---------------------------
    public string GetStoryText()
    {
        string completeText ="";
        foreach( Button button in storyTextButtons)
        {
            // Get the TextMeshProUGUI component child of the Button
            TextMeshProUGUI textMeshPro = button.GetComponentInChildren<TextMeshProUGUI>();
            completeText += " " + textMeshPro.text;
        }
        return completeText;
    }

    // Used to convert resultString 
    [System.Serializable]
    public class WordResult
    {
        public string word;
        public bool correct;

        public WordResult(string word, bool correct)
        {
            this.word = word;
            this.correct = correct;
        }
    }
    private List<WordResult> ParseString(string input)
    {
        List<WordResult> results = new List<WordResult>();
        string pattern = @"<(\w+)>(.*?)<\/\1>";

        // Use Regex to find all matches of the pattern
        MatchCollection matches = Regex.Matches(input, pattern);
        foreach (Match match in matches)
        {
            bool resultType = (match.Groups[1].Value == "correct") ? true : false; // "correct" or "error"
            string word = match.Groups[2].Value;           // The word itself
            
            // Create a new WordResult and add it to the list
            results.Add(new WordResult(word, resultType));
        }

        return results;
    }
    private static string CleanString(string input)
    {
        // Replace punctuation with empty strings
        char[] punctuation = { '.', ',', '!', '?', ';', ':' };
        foreach (char ch in punctuation)
        {
            input = input.Replace(ch.ToString(), "");
        }

        // cleaned string
        return input.Replace(" ", "").Trim();  // Remove spaces as well if needed
    }

    public void DisplayReadingResults(string resultString)
    {
        List<WordResult> resultDictionary = ParseString(resultString);
        int correctWordCount = 0;

        foreach(Button button in storyTextButtons)
        {
            // Get the TextMeshProUGUI component child of the Button
            TextMeshProUGUI textMeshPro = button.GetComponentInChildren<TextMeshProUGUI>();

            foreach (WordResult result in resultDictionary)
            {
                // Check if there is a result for the wordButton, clean punctuation and ignore case
                if( string.Equals( CleanString(textMeshPro.text), result.word, StringComparison.OrdinalIgnoreCase) )
                {
                    if (result.correct == true)
                    {
                        textMeshPro.color = Color.green; 
                        correctWordCount++;
                        break;
                    }
                    else
                    {
                        textMeshPro.color = Color.blue;
                    }
                }
            }
        }

        // Enable nextSceneButton when 80% of words are read correctly
        try
        {
            float percentage = 0;
            // Ensure total is not zero to avoid division by zero.
            if (resultDictionary.Count != 0)
            {
                // Calculate percentage
                percentage = (float)correctWordCount / resultDictionary.Count * 100;
                Debug.Log("Percentage: " + percentage + "%");
            }
            
            if (percentage >= 70)
            {
                nextSceneButton.interactable = true;
            }
        }
        catch(Exception e)
        {
            Debug.Log("Error while calculating correct word %" + e);
        }

    }

    public void DisplayInteractiveTaskResult(bool success, string text)
    {
        if (success)
        {
            interactiveTaskResultText.text = text;
            interactiveTaskResultText.color = Color.green;
            interactiveTaskResultText.gameObject.SetActive(true);
            nextSceneButton.interactable = true;
        }
        else
        {
            interactiveTaskResultText.text = text;
            interactiveTaskResultText.color = Color.blue;
            interactiveTaskResultText.gameObject.SetActive(true);
        }

    }

}
