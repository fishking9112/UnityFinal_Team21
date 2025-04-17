using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class iconInfo
{
    public string outfitName;
    public Sprite sprite;
}

[CreateAssetMenu(fileName = "IconData", menuName = "Scriptable Object/New IconData")]
public class IconData : ScriptableObject
{
    public List<iconInfo> iconList;
    private Dictionary<string, Sprite> iconDic;

    public Sprite GetSprite(string outfit)
    {
        if(iconDic == null)
        {
            iconDic = new Dictionary<string, Sprite>();

            foreach (iconInfo icon in iconList)
            {
                iconDic[icon.outfitName] = icon.sprite;
            }
        }

        return iconDic.TryGetValue(outfit, out Sprite sprite) ? sprite : null;
    }
}
