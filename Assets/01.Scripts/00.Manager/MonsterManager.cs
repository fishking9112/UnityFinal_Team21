using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MonsterManager : MonoSingleton<MonsterManager>
{
    public Dictionary<GameObject, MonsterController> monsters = new(); // 몬스터가 나오면 자동으로 이곳에 저장(전체)

    // Data에서 받아온 데이터에서 변화된 값 (이곳에서 스텟을 상승시키면 몬스터 스텟 자동으로 업그레이드 완료)
    public Dictionary<int, MonsterInfo> monsterInfoList = new();
    // Health값 바뀌면 BaseController의 HealthStatUpdate 실행 필요
    public Dictionary<int, List<MonsterController>> idByMonsters = new(); // 몬스터가 나오면 자동으로 이곳에 저장(종류별)

    public bool isHealthUI = false;
    public bool InitComplete = false;

    private async void Start()
    {
        await WaitUntilInitCompleteAndSetup();
    }

    private async UniTask WaitUntilInitCompleteAndSetup()
    {
        await UniTask.WaitUntil(() => InitComplete);

        foreach (var monsterdata in DataManager.Instance.queenAbilityMonsterStatDic.Values)
        {
            monsterInfoList[monsterdata.ID] = new MonsterInfo(monsterdata);
            idByMonsters[monsterdata.ID] = new List<MonsterController>();
        }
    }

    public void OnClickHealthUITest()
    {
        isHealthUI = !isHealthUI;
        foreach (var monster in monsters)
        {
            monster.Value.SetHealthUI(isHealthUI);
        }
    }

    // 테스트 코드 주석처리
    // void Update()
    // {
    //     if (Input.GetMouseButtonDown(0))
    //     {
    //         Vector2 randomPos = GetRandomWorldPositionInCamera();
    //         var monster = ObjectPoolManager.Instance.GetObject<MonsterController>(monsterInfoList[1101].outfit, randomPos);
    //         monster.StatInit(monsterInfoList[1101]);
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
