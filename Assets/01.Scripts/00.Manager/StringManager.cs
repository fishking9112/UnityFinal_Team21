using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Components;
using System.Threading.Tasks;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;


public class StringManager : MonoSingleton<StringManager>
{

    // Start is called before the first frame update
    void Start()
    {
        SetBasicLocalLanguage();
    }

    public async Task<string> GetString(string key)
    {
        var str = new LocalizedString("StringTable", key);
        AsyncOperationHandle<string> handle = str.GetLocalizedStringAsync();
        await handle.Task;

        return handle.Result;
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


        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[selectLang];


        //string code = selectLang == (int)SystemLanguage.Korean ? "ko" : "en";

        //Locale lang = GetLocaleCode(code);
        //LocalizationSettings.SelectedLocale = lang;
        ////Utils.LogError(lang.LocaleName);
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
        PlayerPrefs.SetInt("language", index);
    }
}
