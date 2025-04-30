using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionIcon : MonoBehaviour
{
    public Image iconImg;
    public Image lockImg;
    private Action<Sprite, string, string, bool> action = null;
    private IInfo info;
    public void Init(IInfo _info, Action<Sprite, string, string, bool> action)
    {
        info = _info;
        // TODO : 업적 확인해서 lock걸기

        // icon선택
        if (_info.Icon != null)
        {
            if (DataManager.Instance.iconAtlas.GetSprite(_info.Icon) != null)
            {
                iconImg.sprite = DataManager.Instance.iconAtlas.GetSprite(_info.Icon);
            }
        }

        if (TrophyManager.Instance.IsCollectionWithUnlockID(info.ID))
        {
            lockImg.gameObject.SetActive(false);
        }
        else
        {
            lockImg.gameObject.SetActive(true);
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

            if (TrophyManager.Instance.IsCollectionWithUnlockID(info.ID))
            {
                action(iconImg.sprite, info.Name, info.Description, true);
            }
            else
            {
                string desc = DataManager.Instance.trophyDic[TrophyManager.Instance.unlockIdToTrophyIds[info.ID]].description + " 필요";
                action(null, "잠금", desc, false);
            }
        }
    }
}
