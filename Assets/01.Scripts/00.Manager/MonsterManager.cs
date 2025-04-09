using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoSingleton<MonsterManager>
{
    public Dictionary<GameObject, MonsterController> monsters = new Dictionary<GameObject, MonsterController>(); // 몬스터가 나오면 자동으로 

    [Header("SO 데이터")]
    public MonsterData soData;

    // 테스트 코드 주석처리
    // void Update()
    // {
    //     if (Input.GetMouseButtonDown(0))
    //     {
    //         Vector2 randomPos = GetRandomWorldPositionInCamera();
    //         var monster = ObjectPoolManager.Instance.GetObject("Orc_Warrior", randomPos);
    //         monster.GetComponent<MonsterController>().Setting(soData.infoList[0]);
    //     }
    // }
    // Vector2 GetRandomWorldPositionInCamera()
    // {
    //     // Viewport 좌표: (0, 0) = 왼쪽 아래, (1, 1) = 오른쪽 위
    //     float randomX = Random.Range(0f, 1f);
    //     float randomY = Random.Range(0f, 1f);

    //     Vector3 viewportPos = new Vector3(randomX, randomY, Camera.main.nearClipPlane); // z는 필요없지만 있어야 함
    //     Vector3 worldPos = Camera.main.ViewportToWorldPoint(viewportPos);

    //     return new Vector2(worldPos.x, worldPos.y);
    // }
}
