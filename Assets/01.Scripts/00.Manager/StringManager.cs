using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Components;


public class StringManager : MonoSingleton<StringManager>
{

    // Start is called before the first frame update
    void Start()
    {
        SetBasicLocalLanguage();
    }

    public void SetString(string key, LocalizeStringEvent comp)
    {
        comp.SetEntry(key);
    }

    public void SetString(string key, LocalizeStringEvent comp, string arg)
    {
        comp.StringReference.Arguments.Clear();
        comp.StringReference.Arguments.Add(arg);
        comp.SetEntry(key);
    }
    public void SetString(string key, LocalizeStringEvent comp, string arg,string arg2)
    {
        comp.StringReference.Arguments.Clear();
        comp.StringReference.Arguments.Add(arg);
        comp.StringReference.Arguments.Add(arg2);
        comp.SetEntry(key);
    }



    public void SetBasicLocalLanguage()
    {
        int selectLang;
        if(PlayerPrefs.HasKey("language"))
        {
            selectLang = PlayerPrefs.GetInt("language");
        }
        else
        {
            selectLang = (int)Application.systemLanguage;
        }

        string code = selectLang == (int)SystemLanguage.Korean ? "ko" : "en";

        Locale lang = GetLocaleCode(code);
        LocalizationSettings.SelectedLocale = lang;
        //Utils.LogError(lang.LocaleName);
    }

    private Locale GetLocaleCode(string code)
    {
        foreach (var locale in LocalizationSettings.AvailableLocales.Locales)
        {
            if(locale.Identifier.Code==code)
            {
                return locale;
            }
        }
        return null;
    }

    public void ChangeLocale(int index)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
    }
}
