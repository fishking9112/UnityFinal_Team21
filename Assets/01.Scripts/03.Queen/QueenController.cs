using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class QueenController : MonoBehaviour
{
    private QueenCondition condition;
    private ObjectPoolManager objectPoolManager;
    
    private Vector3 worldMousePos;
    private List<string> monsterSlot = new List<string>();
    private Dictionary<string, GameObject> monsterPrefabs = new Dictionary<string, GameObject>();
    private string selectedMonsterName;
    private GameObject selectedMonsterPrefab;

    [SerializeField] private float summonGaugeRecoverySpeed = 10f;
    [SerializeField] private float magicGaugeRecoverySpeed = 5f;

    public GameObject[] test;

    private void Start()
    {
        condition = GameManager.Instance.queen.condition;
        objectPoolManager = ObjectPoolManager.Instance;

        AddMonsterToSlot("Circle", test[0]);
        AddMonsterToSlot("Capsule", test[1]);
        AddMonsterToSlot("Hexagon Flat-Top", test[2]);
    }

    private void Update()
    {
        SelectedMonsterCursor();
        SummonMonster();
        RecoveryGauge();
    }

    // 선택한 슬롯의 몬스터를 마우스커서에 붙힘
    private void SelectedMonsterCursor()
    {
        Vector2 mousePos = Pointer.current.position.ReadValue();
        worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
        worldMousePos.z = 0f;

        if (selectedMonsterPrefab != null)
        {
            selectedMonsterPrefab.transform.position = worldMousePos;
        }
    }

    /// <summary>
    /// 번호 키를 누르면 해당 슬롯에 저장되어 있는 몬스터를 소환할 준비
    /// </summary>
    /// <param name="context"> 1,2,3,4,5,6번 키 </param>
    public void OnPressSlotNumber(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            int key = Mathf.RoundToInt(context.ReadValue<float>());

            if (key > monsterSlot.Count)
            {
                return;
            }

            selectedMonsterName = monsterSlot[key - 1];
            Destroy(selectedMonsterPrefab);
            selectedMonsterPrefab = Instantiate(monsterPrefabs[monsterSlot[key - 1]]);
        }
    }

    // 마우스의 월드좌표를 계산해서 해당 위치에 몬스터를 소환함
    private void SummonMonster()
    {
        if (Pointer.current.press.isPressed)
        {
            if(selectedMonsterName == null)
            {
                return;
            }

            float monsterRadius = 0.5f;

            int layerMask = ~(1<<(LayerMask.NameToLayer("CameraLimit")));


            Collider2D hit = Physics2D.OverlapCircle(worldMousePos, monsterRadius, layerMask);

            if(hit != null)
            {
                return;
            }

            objectPoolManager.GetObject(selectedMonsterName, worldMousePos);
        }
    }

    //public void AddMonsterToSlot(Monster monster)
    //{
    //    monsterSlot.Add(monster.monsterData.name);
    //    monsterPrefabs.Add(monster.monsterData.name, monster.monsterData.outfit);
    //}

    //public void RemoveMonsterFromSlot(Monster monster)
    //{
    //    monsterSlot.Remove(monster.monsterData.name);
    //    monsterPrefabs.Remove(monster.monsterData.name);
    //}

    // 슬롯에 몬스터 추가
    public void AddMonsterToSlot(string key, GameObject obj)
    {
        monsterSlot.Add(key);
        monsterPrefabs.Add(key, obj);
    }

    // 슬롯에서 몬스터 제거
    public void RemoveMonsterFromSlot(string key)
    {
        monsterSlot.Remove(key);
        monsterPrefabs.Remove(key);
    }

    // 자동 게이지 회복
    private void RecoveryGauge()
    {
        condition.AdjustCurMagicGauge(magicGaugeRecoverySpeed * Time.deltaTime);
        condition.AdjustCurSummonGauge(summonGaugeRecoverySpeed * Time.deltaTime);
    }
}
