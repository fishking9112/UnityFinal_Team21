using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointManager : MonoSingleton<SpawnPointManager>
{
    public SpawnPointController heroPoint;
    public SpawnPointController MiniCastlePoint;
    public SpawnPointController BarrackPoint;
    public SpawnPointController MonsterPoint;

    public SpriteRenderer areaVisualizerPrefab;
}
