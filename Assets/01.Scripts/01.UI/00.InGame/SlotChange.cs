using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class SlotChange : MonoBehaviour
{
    public CanvasGroup queenActiveSkillGroup;
    public CanvasGroup monsterGroup;
    public RectTransform queenActiveSkillGroupTransform;
    public RectTransform monsterGroupTransform;
    public RectTransform panelTransform;
    public GameObject skillSlotNumGroup;
    public GameObject monsterSlotNumGroup;

    public GameObject queenActiveSkillGauge;
    public GameObject summonGauge;

    private float duration = 0.2f;
    public float arcHeight;

    private bool isChange = false;

    private QueenController controller;
    private InputAction inputAction;

    private int saveMonster = -1;
    private QueenActiveSkillBase saveSkill = null;

    public void Init(QueenController queenController, InputAction slotChangeAction)
    {
        controller = queenController;
        inputAction = slotChangeAction;
        inputAction.started += OnChangeSlots;
        InitOrder();
    }

    private void InitOrder()
    {
        panelTransform.SetAsFirstSibling();
        monsterGroupTransform.SetAsLastSibling();

        summonGauge.SetActive(true);
        queenActiveSkillGauge.SetActive(false);

        monsterSlotNumGroup.SetActive(true);
        skillSlotNumGroup.SetActive(false);

        monsterGroup.alpha = 1f;
        queenActiveSkillGroup.alpha = 0.5f;
    }

    // Tab 키를 누르면 몬스터슬롯과 권능 슬롯이 변경됨
    public void OnChangeSlots(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started)
        {
            return;
        }

        if (isChange)
        {
            return;
        }

        if (StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().isPaused)
        {
            return;
        }

        if (queenActiveSkillGroupTransform == null || monsterGroupTransform == null)
        {
            return;
        }

        isChange = true;

        // curSlot을 바로 바꿈
        controller.curSlot = controller.curSlot == QueenSlot.Monster ? QueenSlot.QueenActiveSkill : QueenSlot.Monster;

        if (controller.curSlot == QueenSlot.Monster)
        {
            saveMonster = controller.selectedMonsterId;
        }
        else
        {
            saveSkill = controller.selectedQueenActiveSkill;
        }

        if (controller.curSlot == QueenSlot.Monster)
        {
            controller.selectedMonsterId = saveMonster;
            if (DataManager.Instance.monsterDic.TryGetValue(saveMonster, out var monsterData))
            {
                var icon = DataManager.Instance.iconAtlas.GetSprite(monsterData.icon);
                controller.cursorIcon.GetComponent<SpriteRenderer>().sprite = icon;
            }
            else
            {
                controller.cursorIcon.GetComponent<SpriteRenderer>().sprite = null;
            }

            summonGauge.SetActive(true);
            queenActiveSkillGauge.SetActive(false);
            monsterSlotNumGroup.SetActive(true);
            skillSlotNumGroup.SetActive(false);
        }
        else
        {
            controller.selectedQueenActiveSkill = saveSkill;
            controller.cursorIcon.GetComponent<SpriteRenderer>().sprite = null;

            summonGauge.SetActive(false);
            queenActiveSkillGauge.SetActive(true);
            monsterSlotNumGroup.SetActive(false);
            skillSlotNumGroup.SetActive(true);
        }

        Vector3 queenActiveSkillPos = queenActiveSkillGroupTransform.anchoredPosition;
        Vector3 monsterPos = monsterGroupTransform.anchoredPosition;

        Vector3[] queenActiveSkillPath = CreateArc(
            queenActiveSkillPos,
            monsterPos,
            controller.curSlot == QueenSlot.Monster ? arcHeight : -arcHeight
        );
        Vector3[] monsterPath = CreateArc(
            monsterPos,
            queenActiveSkillPos,
            controller.curSlot == QueenSlot.Monster ? -arcHeight : arcHeight
        );

        Sequence seq = DOTween.Sequence();

        seq.Append(queenActiveSkillGroupTransform.DOLocalPath(queenActiveSkillPath, duration, PathType.CatmullRom).SetEase(Ease.InOutQuad))
           .Join(monsterGroupTransform.DOLocalPath(monsterPath, duration, PathType.CatmullRom).SetEase(Ease.InOutQuad));

        // 포물선 최고점에 이르렀을 때 순서 변경
        seq.InsertCallback(duration / 2f, SetOrder);

        seq.OnComplete(() =>
        {
            isChange = false;
        });
    }

    // 슬롯의 순서를 바꿈. 현재 선택된 슬롯이 아니면 반 투명해지면서 현재슬롯에 가려지도록 렌더링 순서 변경
    private void SetOrder()
    {
        if (controller.curSlot == QueenSlot.Monster)
        {
            monsterGroupTransform.SetAsLastSibling();
            panelTransform.SetAsFirstSibling();
            summonGauge.SetActive(true);
            queenActiveSkillGauge.SetActive(false);
            monsterSlotNumGroup.SetActive(true);
            skillSlotNumGroup.SetActive(false);
            queenActiveSkillGroup.DOFade(0.5f, 0.2f);
            monsterGroup.DOFade(1f, 0.2f);
        }
        else
        {
            monsterGroupTransform.SetAsFirstSibling();
            panelTransform.SetAsFirstSibling();
            summonGauge.SetActive(false);
            queenActiveSkillGauge.SetActive(true);
            monsterSlotNumGroup.SetActive(false);
            skillSlotNumGroup.SetActive(true);
            queenActiveSkillGroup.DOFade(1f, 0.2f);
            monsterGroup.DOFade(0.5f, 0.2f);
        }
    }

    // 포물선을 만들어주는 함수
    private Vector3[] CreateArc(Vector3 start, Vector3 end, float height)
    {
        Vector3 mid = (start + end) / 2f;
        mid.y += height;
        return new Vector3[] { start, mid, end };
    }

    private void OnDestroy()
    {
        if (inputAction != null)
        {
            inputAction.started -= OnChangeSlots;
        }
    }
}