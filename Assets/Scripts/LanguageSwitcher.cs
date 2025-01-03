using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using Unity.VisualScripting;

public class LanguageSwitcher : MonoBehaviour
{
    

    public void SetLanguage()
    {
        var languageIdentifier = LocalizationSettings.SelectedLocale.Identifier.ToString();

        print(languageIdentifier);

        switch (languageIdentifier)
        {
            case "German(de)":
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[1];
                break;
            case "English(en)":
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
                break;
        }

        //languageIdentifier = LocalizationSettings.SelectedLocale.Identifier.ToString();
        
        //var selectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
        //LocalizationSettings.SelectedLocale = selectedLocale;
    }
}