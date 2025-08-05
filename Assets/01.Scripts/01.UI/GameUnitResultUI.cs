using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Components;

public class GameUnitResultUI : MonoBehaviour
{
    public Image unitIcon;
    public TextMeshProUGUI unitNameText;
    [SerializeField] private LocalizeStringEvent unitNameLocalize;
    public TextMeshProUGUI unitSpawnCountText;
    public TextMeshProUGUI unitAllDamageText;
    public TextMeshProUGUI unitDamagePerSpawnCountText;

    public void Init(string icon, string name, int spawnCount, float allDamage)
    {
        unitIcon.sprite = DataManager.Instance.iconAtlas.GetSprite(icon);
        // unitNameText.text = name;
        StringManager.Instance.SetString(name, unitNameLocalize);
        unitSpawnCountText.text = spawnCount.ToString("N0");
        unitAllDamageText.text = allDamage.ToString("N0");
        unitDamagePerSpawnCountText.text = ((float)allDamage / spawnCount).ToString("N2");
    }
}
