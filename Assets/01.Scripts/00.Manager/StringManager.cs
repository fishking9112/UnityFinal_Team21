using System.Collections;
using System.Collections.Generic;
using UnityEngine;


enum Localization
{
    Ko=1,
    En
}

public class StringManager : MonoSingleton<StringManager>
{
    private Dictionary<long, CLocalStringData> StringDicLangs;

    private Localization local;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void SetStringTable()
    {

    }


    public string GetString(int key)
    {

        if(StringDicLangs.Count==0)
        {
            return string.Empty;
        }

        if(!StringDicLangs.TryGetValue(key, out CLocalStringData val))
        {
            return string.Empty;
        }

        switch(local)
        {
            case Localization.Ko:
                return val.ko;
            case Localization.En:
                return val.en;
        }



    }
}

public class CLocalStringData
{
    public int Key { get; set; }

    public string ko { get; set; }
    public string en { get; set; }

}
