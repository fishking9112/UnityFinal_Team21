using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MonsterManager : MonoSingleton<MonsterManager>
{
    public Dictionary<GameObject, MonsterController> monsters = new Dictionary<GameObject, MonsterController>(); // 몬스터가 나오면 자동으로 이곳에 저장(전체)

    // SO에서 받아온 데이터에서 변화된 값 (이곳에서 스텟을 상승시키면 몬스터 스텟 자동으로 업그레이드 완료)
    public Dictionary<int, MonsterInfo> monsterInfoList = new();
    // Health값 바뀌면 BaseController의 HealthStatUpdate 실행 필요
    public Dictionary<int, List<MonsterController>> idByMonsters = new Dictionary<int, List<MonsterController>>(); // 몬스터가 나오면 자동으로 이곳에 저장(종류별)

    [Header("SO 데이터")]
    public MonsterData monsterData;
    public int testSpawnNumber = 0;
    public BaseController testTarget;

    void Start()
    {
        for (int i = 0; i < monsterData.infoList.Count; i++)
        {
            // 깊은 복사로 저장해서 들고 있음
            monsterInfoList[monsterData.infoList[i].id] = new MonsterInfo(monsterData.infoList[i]);
            idByMonsters[i] = new List<MonsterController>();
        }

        testTarget.StatInit(monsterInfoList[0]);
    }

    // 테스트 코드 주석처리
    // void Update()
    // {
    //     if (Input.GetMouseButtonDown(0))
    //     {
    //         Vector2 randomPos = GetRandomWorldPositionInCamera();
    //         var monster = ObjectPoolManager.Instance.GetObject<MonsterController>(monsterInfoList[testSpawnNumber].outfit, randomPos);
    //         monster.StatInit(monsterInfoList[testSpawnNumber]);
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
