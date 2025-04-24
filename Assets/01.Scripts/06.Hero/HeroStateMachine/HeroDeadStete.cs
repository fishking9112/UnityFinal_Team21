public class HeroDeadStete : HeroBaseState
{
    public HeroDeadStete(HeroState state) : base(state)
    {
    }

    public override void Enter()
    {
        base.Enter();
        // 사망 애니메이션, 사운드

        // 전투(자동전투 포함) 중지
        state.hero.ResetAbility();
        // 보상 떨구기/획득하기

        RewardGold gold= ObjectPoolManager.Instance.GetObject<RewardGold>("GoldReward", state.hero.gameObject.transform.position);
        gold.rewardAmount = state.hero.statusInfo.reward;
    }

    public override void Exit()
    {
        base.Exit();
    }

}
