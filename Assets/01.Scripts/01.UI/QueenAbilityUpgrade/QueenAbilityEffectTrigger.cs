using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueenAbilityEffectTrigger : MonoBehaviour
{
    void Start()
    {
        QueenAbilityUpgradeManager.Instance.ApplyAllEffects();
    }
}
