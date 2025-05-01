using DG.Tweening;
using UnityEngine;

public class SlotChange : MonoBehaviour
{
    public CanvasGroup queenActiveSkillGroup;
    public CanvasGroup monsterGroup;
    public RectTransform queenActiveSkillGroupTransform;
    public RectTransform monsterGroupTransform;

    public GameObject queenActiveSkillGauge;
    public GameObject summonGauge;

    public float duration;
    public float arcHeight;

    private bool isChange = false;

    private QueenController controller;

    private void Start()
    {
        controller = GameManager.Instance.queen.controller;
        InitOrder();
    }

    private void InitOrder()
    {
        monsterGroupTransform.SetAsLastSibling();
        summonGauge.SetActive(true);
        queenActiveSkillGauge.SetActive(false);
        monsterGroup.alpha = 1f;
        queenActiveSkillGroup.alpha = 0.5f;
    }

    // 버튼에 등록할 함수. 버튼을 누르면 몬스터슬롯과 권능 슬롯이 변경됨
    public void OnChangeSlots()
    {
        if (isChange)
        {
            return;
        }

        isChange = true;

        Vector3 queenActiveSkillPos = queenActiveSkillGroupTransform.anchoredPosition;
        Vector3 monsterPos = monsterGroupTransform.anchoredPosition;

        Vector3[] queenActiveSkillPath = CreateArc(queenActiveSkillPos, monsterPos, controller.curSlot == QueenSlot.MONSTER ? arcHeight : -arcHeight);
        Vector3[] monsterPath = CreateArc(monsterPos, queenActiveSkillPos, controller.curSlot == QueenSlot.MONSTER ? -arcHeight : arcHeight);

        Sequence seq = DOTween.Sequence();

        // 시작할 때
        seq.Append(queenActiveSkillGroupTransform.DOLocalPath(queenActiveSkillPath, duration, PathType.CatmullRom).SetEase(Ease.InOutQuad))
           .Join(monsterGroupTransform.DOLocalPath(monsterPath, duration, PathType.CatmullRom).SetEase(Ease.InOutQuad));

        // 포물선 최고점에 이르렀을 때
        seq.InsertCallback(duration / 2f, SetOrder);

        // 끝날 때
        seq.OnComplete(CheangeEnd);
    }

    // 슬롯의 순서를 바꿈. 현재 선택된 슬롯이 아니면 반 투명해지면서 현재슬롯에 가려지도록 렌더링 순서 변경
    private void SetOrder()
    {
        controller.selectedMonsterId = -1;
        controller.selectedQueenActiveSkill = null;
        controller.cursorIcon.GetComponent<SpriteRenderer>().sprite = null;

        if (controller.curSlot == QueenSlot.MONSTER)
        {
            monsterGroupTransform.SetAsFirstSibling();
            summonGauge.SetActive(false);
            queenActiveSkillGauge.SetActive(true);
            queenActiveSkillGroup.DOFade(1f, 0.2f);
            monsterGroup.DOFade(0.5f, 0.2f);
        }
        else if (controller.curSlot == QueenSlot.QueenActiveSkill)
        {
            monsterGroupTransform.SetAsLastSibling();
            summonGauge.SetActive(true);
            queenActiveSkillGauge.SetActive(false);
            queenActiveSkillGroup.DOFade(0.5f, 0.2f);
            monsterGroup.DOFade(1f, 0.2f);
        }
    }

    // 슬롯 변경이 끝날 때 호출. 현재 슬롯의 상태를 바꿔줌
    private void CheangeEnd()
    {
        controller.curSlot = controller.curSlot == QueenSlot.MONSTER ? QueenSlot.QueenActiveSkill : QueenSlot.MONSTER;
        isChange = false;
    }

    // 포물선을 만들어주는 함수
    private Vector3[] CreateArc(Vector3 start, Vector3 end, float height)
    {
        Vector3 mid = (start + end) / 2f;
        mid.y += height;
        return new Vector3[] { start, mid, end };
    }
}
