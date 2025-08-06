using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameSettingManager : MonoSingleton<GameSettingManager>
{
    [Name("피격 시 점등 시간")]
    [Range(0f, 0.5f)]
    public float takeDamagedRendererTimer = 0.5f;
}
