using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private Image timerFill;
    private BaseController target;

    public void Init(BaseController target)
    {
        this.target = target;
        target.deathTimer.AddAction(UpdateBar);

        UpdateBar(target.deathTimer.Value);
    }

    private void UpdateBar(float ratio)
    {
        timerFill.fillAmount = ratio;
    }

    private void OnDestroy()
    {
        if (target != null)
        {
            target.deathTimer.RemoveAction(UpdateBar);
        }
    }
}
