using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QueenSelectItem : MonoBehaviour
{
    [SerializeField] private Toggle thisToggle;
    public Toggle ThisToggle => thisToggle;
    [SerializeField] private Image queenIcon;
    private int queenID;

    public void SetQueenSelectItem(int queenID, ToggleGroup queenSelectToggleGroup)
    {
        this.queenID = queenID;
        thisToggle.group = queenSelectToggleGroup;

        queenIcon.sprite = DataManager.Instance.iconAtlas.GetSprite(DataManager.Instance.queenStatusDic[queenID].Icon);
    }

}
