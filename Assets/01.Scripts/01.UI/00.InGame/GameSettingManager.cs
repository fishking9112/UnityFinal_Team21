using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameSettingManager : MonoSingleton<GameSettingManager>
{
    [Name("유닛 피격 시 점등 시간")]
    [Range(0f, 0.5f)]
    public float unitTakeDamagedRendererTimer = 0.5f;

    [Name("성 피격 시 점등 시간")]
    [Range(0f, 0.5f)]
    public float castleTakeDamagedRendererTimer = 0.5f;
}
