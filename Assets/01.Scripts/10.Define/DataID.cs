/// <summary>
/// 작성 규칙 : 퀸의 타입
/// EX) 기본 퀸 => NORMAL
/// </summary>
public enum IDQueenStatus
{
    NORMAL = 11,
    SLIME,
    ORC,
    SKELETON,
}

/// <summary>
/// 작성 규칙 : 이름
/// EX) 검 => SWORD
/// </summary>
public enum IDHeroAbility
{
    // 히어로 어빌리티 이름은 게임에 맞게 추후 수정 필요
    SWORD = 101,
    WAND,
    BIBLE,
    GARLIC,
    AXE,
    CHAIN,
}

/// <summary>
/// 작성 규칙 : 이름
/// EX) 용사1 => HERO1
/// </summary>
public enum IDHeroStatus
{
    HERO1 = 201,
    HERO2,
    HERO3,
}

/// <summary>
/// 작성 규칙 : 종족_타입명
/// EX) 기본 슬라임 => SLIME_NORMAL
/// </summary>
public enum IDMonster
{
    // 슬라임은 아직 가명
    SLIME_NORMAL = 1001,
    SLIME_NORMAL2,
    SLIME_BIG,
    SLIME_FAST,
    SLIME_POISON,
    SLIME_POISON2,

    ORC_NORMAL = 1101,
    ORC_WARRIOR,
    ORC_SHAMAN,
    ORC_WARRIOR2,
    ORC_BERSERKER,
    ORC_SHAMAN2,

    SKELETON_NORMAL = 1201,
    SKELETON_WARRIOR,
    SKELETON_ARCHER,
    SKELETON_MAGE,
    SKELETON_KNIGHT,
    SKELETON_RANGER,
    SKELETON_WIZARD,
}


/// <summary>
/// 작성 규칙 : 주어_목적어_동사
/// EX) XXX의 OOO을 증가 => XXX_OOO_UP
/// </summary>
public enum IDQueenEnhance
{
    EVOLUTIONPOINT_GAIN = 10001,
    QUEEN_MANA_GAUGE_RECOVERY_SPEED_UP,
    QUEEN_SUMMON_GAUGE_RECOVERY_SPEED_UP,

    SLIME_MAXHEALTH_UP = 11001,
    SLIME_ATTACKDAMAGE_UP,
    SLIME_MOVESPEED_UP,

    ORC_MAXHEALTH_UP = 12001,
    ORC_ATTACKDAMAGE_UP,
    ORC_MOVESPEED_UP,

    SKELETON_MAXHEALTH_UP = 13001,
    SKELETON_ATTACKDAMAGE_UP,
    SKELETON_MOVESPEED_UP,
}

/// <summary>
/// 작성 규칙 : 퀸 스킬 = 퀸 이름, 공용 스킬 = 스킬 이름
/// EX) 기본 퀸 스킬 = NORMALQUEEN, 범위 공격 = RANGEATTACK
/// </summary>
public enum IDQueenActiveSkill
{
    NORMALQUEEN = 20001,
    SLIMEQUEEN,
    ORCQUEEN,
    SKELETONQUEEN,

    RANGEATTACK = 21001,
    SUMMON,
    ATTACKDAMAGEUP,
    RANGEHEAL,
    RANGESLOW,
    METEOR,
    ALLHEAL,
}

/// <summary>
/// 작성 규칙 : 주어_목적어_동사
/// EX) XXX의 OOO을 증가 => XXX_OOO_UP
/// </summary>
public enum IDQueenAbility
{
    MONSTER_ATTACKDAMAGE_UP = 30001,
    MONSTER_MOVESPEED_UP,
    GOLD_GAINEDAMOUNT_UP,
    EXP_GAINEDAMOUNT_UP,
    CASTLE_HEALTH_UP,
    CASTLE_HEALTHRECOVERY_UP,
    SUMMONGAUGE_RECOVERYSPEED_UP,
    SUMMONGAUGE_MAXGAUGE_UP,
    QUEENACTIVESKILLGAUGE_RECOVERYSPEED_UP,
    QUEENACTIVESKILLGAUGE_MAXGAUGE_UP,
    EVOLUTIONPOINT_STARTAMOUNT_UP,
}

/// <summary>
/// 작성 규칙 : 이름
/// EX) 공격력 증가 => ATTACKDAMAGEUP
/// </summary>
public enum IDBuff
{
    ATTACKDAMAGEUP = 100001,
    SLOW,
    BURN,
    POISON,
    MOVESPEEDUP,
}

/// <summary>
/// 작성 규칙 : 이름
/// EX) 업적1 : Trophy1
/// </summary>
public enum IDTrophy
{
    // 업적 이름 추후 수정 필요
    Trophy1 = 1000001,
    Trophy2,
    Trophy3,
    Trophy4,
}