using DG.Tweening;
using UnityEngine;

public class SlotChange : MonoBehaviour
{
    public CanvasGroup magicGroup;
    public CanvasGroup monsterGroup;
    public RectTransform magicGroupTransform;
    public RectTransform monsterGroupTransform;
    public RectTransform magicSlotTransform;
    public RectTransform monsterSlotTransform;

    public GameObject magicGauge;
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
        magicGauge.SetActive(false);
        monsterGroup.alpha = 1f;
        magicGroup.alpha = 0.5f;
    }

    // 버튼에 등록할 함수. 버튼을 누르면 몬스터슬롯과 권능 슬롯이 변경됨
    public void OnChangeSlots()
    {
        if (isChange)
        {
            return;
        }

        isChange = true;

        Vector3 magicPos = magicSlotTransform.localPosition;
        Vector3 monsterPos = monsterSlotTransform.localPosition;

        Vector3[] magicPath = CreateArc(magicPos, monsterPos, controller.curSlot == QueenSlot.MONSTER ? arcHeight : -arcHeight);
        Vector3[] monsterPath = CreateArc(monsterPos, magicPos, controller.curSlot == QueenSlot.MONSTER ? -arcHeight : arcHeight);

        Sequence seq = DOTween.Sequence();

        // 시작할 때
        seq.Append(magicSlotTransform.DOLocalPath(magicPath, duration, PathType.CatmullRom).SetEase(Ease.InOutQuad))
           .Join(monsterSlotTransform.DOLocalPath(monsterPath, duration, PathType.CatmullRom).SetEase(Ease.InOutQuad));

        // 포물선 최고점에 이르렀을 때
        seq.InsertCallback(duration / 2f, SetOrder);

        // 끝날 때
        seq.OnComplete(CheangeEnd);
    }

    // 슬롯의 순서를 바꿈. 현재 선택된 슬롯이 아니면 반 투명해지면서 현재슬롯에 가려지도록 렌더링 순서 변경
    private void SetOrder()
    {
        controller.selectedMonster = null;
        //controller.selectedMagic = null;
        controller.cursorIcon.GetComponent<SpriteRenderer>().sprite = null;

        if (controller.curSlot == QueenSlot.MONSTER)
        {
            monsterGroupTransform.SetAsFirstSibling();
            summonGauge.SetActive(false);
            magicGauge.SetActive(true);
            magicGroup.DOFade(1f, 0.2f);
            monsterGroup.DOFade(0.5f, 0.2f);
        }
        else if (controller.curSlot == QueenSlot.MAGIC)
        {
            monsterGroupTransform.SetAsLastSibling();
            summonGauge.SetActive(true);
            magicGauge.SetActive(false);
            magicGroup.DOFade(0.5f, 0.2f);
            monsterGroup.DOFade(1f, 0.2f);
        }
    }

    // 슬롯 변경이 끝날 때 호출. 현재 슬롯의 상태를 바꿔줌
    private void CheangeEnd()
    {
        controller.curSlot = controller.curSlot == QueenSlot.MONSTER ? QueenSlot.MAGIC : QueenSlot.MONSTER;
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
