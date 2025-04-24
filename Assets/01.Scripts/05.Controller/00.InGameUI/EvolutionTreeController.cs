using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(EvolutionTreeUI))]
public class EvolutionTreeController : MonoBehaviour
{
    [SerializeField] private EvolutionTreeUI windowUI;

    private int curIndex;

    private void OnEnable()
    {
        curIndex = 0;
        windowUI.ShowPage(curIndex);
    }

    private void Start()
    {
        windowUI.SetButtonCallbacks(OnClickLeftButton, OnClickRightButton);
    }

    public void OnClickLeftButton()
    {
        if (curIndex > 0)
        {
            curIndex--;
            windowUI.ShowPage(curIndex);
        }
    }

    public void OnClickRightButton()
    {
        if (curIndex < windowUI.PageCount - 1)
        {
            curIndex++;
            windowUI.ShowPage(curIndex);
        }
    }
}
