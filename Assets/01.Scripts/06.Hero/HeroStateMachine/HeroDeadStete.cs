public class HeroDeadStete : HeroBaseState
{
    public HeroDeadStete(HeroState state) : base(state)
    {
    }

    public override void StateEnter()
    {
        base.StateEnter();
        // 사망 애니메이션, 사운드

        // 전투(자동전투 포함) 중지
        hero.ResetAbility();

        // 보상 떨구기/획득하기
    }

    public override void StateExit()
    {
        base.StateExit();
    }

}
