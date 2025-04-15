using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class QueenEnhanceManager : MonoSingleton<QueenEnhanceManager>
{
    [Header("설정")]
    [SerializeField] private int numberOfOptions = 3; // 레벨업 시 보여줄 옵션 개수

  //  private List<QueenEnhanceInfo> cachedInhanceList = new();


    private QueenEnhanceUIController queenEnhanceUIController;
    public QueenEnhanceUIController QueenEnhanceUIController => queenEnhanceUIController;


    private void Start()
    {
        // 모든 강화 옵션 데이터를 미리 캐싱
    //    cachedInhanceList.AddRange(DataManager.Instance.queenInhanceDic.Values);
    }

    /// <summary>
    /// 외부에서 호출되는 레벨업 진입 함수
    /// </summary>
    public void OnLevelUp()
    {
     //   var randomOptions = GetRandomInhanceOptions(numberOfOptions);

        // UIController에게 전달
      //  QueenEnhanceUIController.ShowOptions(randomOptions);
    }

    /// <summary>
    /// 현재 전체 강화 목록 중에서 랜덤하게 n개를 뽑는다.
    /// </summary>
   /* private List<QueenInhanceInfo> GetRandomInhanceOptions(int count)
    {
        List<QueenInhanceInfo> result = new List<QueenInhanceInfo>();
        List<QueenInhanceInfo> tempList = new List<QueenInhanceInfo>(cachedInhanceList);

        for (int i = 0; i < count && tempList.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, tempList.Count);
            result.Add(tempList[randomIndex]);
            tempList.RemoveAt(randomIndex);

        }

        return result;
    }*/

    /// <summary>
    /// 버튼에서 전달된 선택된 옵션 처리
    /// </summary>
    /*public void OnOptionSelected(QueenEnhanceInfo selectedInfo)
   {

        // 선택된 능력 적용 처리 로직 (TODO)
        // ex. 플레이어 능력 강화

        // UI 닫기
        QueenEnhanceUIController.Instance.HideUI();
    }*/



    /// <summary>
    /// UI 스크립트를 등록하고 능력 목록 UI 아이템을 생성합니다.
    /// </summary>
    /// <param name="script">UI 패널 스크립트</param>
    public void SetQueenInhanceUIController(QueenEnhanceUIController script)
    {
        queenEnhanceUIController = script;
    }


    /// <summary>
    /// 강화 항목 선택 시 실제 적용되는 함수
    /// </summary>
  /*  public void ApplyInhance(QueenInhanceInfo info)
    {

        // TODO: 능력 적용 로직 구현
    }*/

}
