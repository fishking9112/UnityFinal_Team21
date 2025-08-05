using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Image gaugeImg;

    public void SetAmount(float _gauge)
    {
        gaugeImg.fillAmount = _gauge;
    }
}
