using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

[Serializable]
public struct SaveData
{
    [JsonProperty("version")]
    public int version;

    [JsonProperty("player")]
    public PlayerData player;

    [JsonProperty("settings")]
    public SettingsData settings;

    [JsonProperty("queenUpgrades")]
    public QueenAbilityUpgradeData queenUpgrades;

    [JsonExtensionData]
    public Dictionary<string, JToken> extraRootFields;
}

[Serializable]
public struct PlayerData
{
    [JsonProperty("gold")]
    public int gold;


    [JsonExtensionData]
    public Dictionary<string, JToken> extraPlayerFields;
}

[Serializable]
public struct SettingsData
{
    [JsonProperty("bgmVolume")]
    public float bgmVolume;

    [JsonProperty("sfxVolume")]
    public float sfxVolume;
    // public string language;


    [JsonExtensionData]
    public Dictionary<string, JToken> extraSettingsFields;
}

[Serializable]
public struct QueenAbilityUpgradeData
{
    [JsonProperty("upgrades")]
    public List<QueenAbilityUpgradeInfo> upgrades;

    [JsonExtensionData]
    public Dictionary<string, JToken> extraQueenUpgradeFields;
}


[Serializable]
public struct LeaderBoardData
{
    [JsonProperty("leaderboardQueenID")]
    public int queenID;

    [JsonExtensionData]
    public Dictionary<string, JToken> extraLeaderboardResultFields;
}

// 이후 필요한 데이터 구조체로 추가