using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;


public class StringManager : MonoSingleton<StringManager>
{

    // Start is called before the first frame update
    void Start()
    {
        LocalizeStringEvent a=new LocalizeStringEvent();
        
        a.StringReference.Arguments = new[] {"10"};
        a.SetEntry("9902303");
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

}
