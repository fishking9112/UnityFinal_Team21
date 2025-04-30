using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionIcon : MonoBehaviour
{
    public Image iconImg;
    public Image lockImg;
    private Action<Sprite, string, string> action = null;
    private IInfo info;
    public void Init(IInfo _info, Action<Sprite, string, string> action)
    {
        info = _info;
        // TODO : 업적 확인해서 lock걸기

        // icon선택
        if (_info.Icon != null)
        {
            Debug.Log(_info.Icon);
            if (DataManager.Instance.iconAtlas.GetSprite(_info.Icon) != null)
            {
                iconImg.sprite = DataManager.Instance.iconAtlas.GetSprite(_info.Icon);
            }
        }

        if (this.action == null)
        {
            this.action = action;
        }
    }

    public void OnClickIcon()
    {
        if (action != null)
        {
            action(iconImg.sprite, info.Name, info.Description);
        }
    }
}
