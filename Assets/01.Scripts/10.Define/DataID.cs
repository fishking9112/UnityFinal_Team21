public enum IDQueenStatus
{
    NORMAL = 11,
    SLIME,
    ORC,
    SKELETON,
}

public enum IDHeroAbility
{
    // 히어로 어빌리티 이름은 게임에 맞게 추후 수정 필요
    SWORD = 101,
    WAND,
    BIBLE,
    GARLIC,
    AXE,
    CHAIN,
    TARGETTING,
    BURST,
    SPEAR,
    HOLD,
    THUNDER,
}

public enum IDHeroAbility_LevelUp
{
    SWORD = 201,
    WAND,
    BIBLE,
    GARLIC,
    AXE,
    CHAIN,
    TARGETTING,
    BURST,
    SPEAR,
    HOLD,
    THUNDER,
}

public enum IDHeroStatus
{
    HERO1 = 301,
    HERO2,
    HERO3,
    HERO4,
    HERO5,
    HERO6,
    HERO7,
    HERO8,
    HERO9,
    HERO10,
    HERO11,
    HERO12,
}

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

    ELF_SCOUT = 1401,
    ELF_ARCHER,
    ELF_HUNTER,
    ELF_SENTINEL,
    ELF_GUARDIAN,

    DARKELF_DARKELF = 1501,
    DARKELF_THIEF,
    DARKELF_ASSASSIN,
    DARKELF_NIGHT,
    DARKELF_NIGHTLOAD,
}

public enum IDQueenEnhance
{
    EVOLUTIONPOINT_GAIN = 10001, // 진화 포인트
    QUEEN_MANA_GAUGE_RECOVERY_SPEED_UP, // 마나 회복 속도
    QUEEN_SUMMON_GAUGE_RECOVERY_SPEED_UP, // 소환 게이지 회복 속도
    QUEEN_MAX_MANA_GAUGE_UP,                 // 여왕 최대 마나 게이지 증가
    QUEEN_MAX_SUMMON_GAUGE_UP,              // 여왕 최대 소환 게이지 증가
    CASTLE_HEALTH_RECOVERY_SPEED_UP,         // 성벽 체력 회복 속도
    CASTLE_MAX_HEALTH_UP,                    // 성벽 최대 체력

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

public enum IDQueenActiveSkill
{
    SKELETON_LEGION = 21001,
    ATTACK_DAMAGE_UP,
    HEAL_WAVE,
    SLOW,
    FIRE_EXPLOSION,
    HEAL_RAIN,
    SACRIFICE,
    MANA_RECYCLE,
    OVERWORK,
    RECALL,
    CASTLE_INVINCIBLE,
    SUMMON_MILITIA,
    SUMMON_OBSTACLE,
    SUMMON_SCARECROW,
    SUMMON_ZOMBIE,
    DEATH_SYMBOL,
    WARCRY,
    BLOOD_ROAR,
    GIANT_FORM,
    DECAY,
    LASER,
    LIGHTNING_STORM,
    GRAVITYBALL,
}

public enum IDQueenAbility
{
    MONSTER_ATTACK_DAMAGE_UP = 30001,
    MONSTER_MOVE_SPEED_UP,
    GOLD_GAIN_AMOUNT_UP,
    EXP_GAIN_AMOUNT_UP,
    CASTLE_HEALTH_UP,
    CASTLE_HEALTH_RECOVERY_SPEED_UP,
    SUMMON_GAUGE_RECOVERY_SPEED_UP,
    SUMMON_GAUGE_MAXGAUGE_UP,
    QUEEN_MANA_GAUGE_RECOVERY_SPEED_UP,
    QUEEN_SUMMON_GAUGE_RECOVERY_SPEED_UP,
    EVOLUTION_POINT_START_AMOUNT_UP,
}

public enum IDBuff
{
    ATTACK_DAMAGE_UP = 100001,
    MOVE_SPEED_UP,
    ATTACK_SPEED_UP,
    ATTACK_DAMAGE_AND_MOVE_SPEED_UP,
    ATTACK_DAMAGE_AND_ATTACK_SPEED_UP,
    MOVE_SPEED_AND_ATTACK_SPEED_UP,
    ATTACK_DAMAGE_AND_MOVE_SPEED_AND_ATTACK_SPEED_UP,
    SLOW,
    BURN,
    POISON,
    DEATHSYMBOL,
    DECAY,
    MILITIA,
    GIANT_FORM,
}

public enum IDTrophy
{
    // 업적 이름 추후 수정 필요
    Trophy1 = 1000001,
    Trophy2,
    Trophy3,
    Trophy4,
}

public class GameLog
{
    public const string funnel = "Funnel_Step";
    public const string tutorial = "Tutorial";
    public const string enhance = "Enhance";

    public const string funnel_step = "Funnel_Step_Num";

    public enum Contents
    {
        NONE=0,
        App=1,
        Account,
        Play,
        Character,
        Collection,
        Archieve,
        QueenAbility,
        Leaderboard,
        
        Funnel,
    }

    public enum LogType
    {
        NONE=0,

        //App
        AppStart=1,

        //Account
        Join,
        Login,
        CreateNickName,
        Tutorial,   // 분리 가능성 있음


        //Play
        GameStart,
        Enhance,    // 분리 가능성 있음
        Mission,
        GameExit,
        GameClear,
        GameFail,

        // Character
        ChangeCharacter,

        // Collection
        OpenCollection,

        // Archive
        OpenArchieve,
        ClearArchieve,
        RewardArchieve,

        // QueenAbility
        UpgradeAbility,
        DegradeAbility,
        ResetAbility,


        // LeaderBoard
        OpenLeaderBoard,

        // Funnel
        FunnelStep,     // 분리 가능성 있음
    }

    public enum FunnelType
    {
        NONE=0,
        GameStart,

    }
}