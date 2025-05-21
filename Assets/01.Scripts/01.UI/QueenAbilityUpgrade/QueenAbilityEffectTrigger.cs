using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueenAbilityEffectTrigger : MonoBehaviour
{
    void Awake()
    {
        QueenAbilityUpgradeManager.Instance.ApplyAllEffects();
    }
}
