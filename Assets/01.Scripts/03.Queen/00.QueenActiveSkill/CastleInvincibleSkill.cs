public class CastleInvincibleSkill : QueenActiveSkillBase
{
    public override void Init()
    {
        base.Init();

        //info 초기화
    }

    public override void UseSkill()
    {
        GameManager.Instance.castle.condition.SetInvincible(true);
        Invoke("EndSkill", info.value);
    }

    private void EndSkill()
    {
        GameManager.Instance.castle.condition.SetInvincible(false);
    }
}
