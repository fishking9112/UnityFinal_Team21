using System;
using System.Collections.Generic;

[Serializable]
public struct SaveData
{
    public PlayerData player;
    public SettingsData settings;
}

[Serializable]
public struct PlayerData
{
    public string nickName;
    public int level;
    public int coin;
}

[Serializable]
public struct SettingsData
{
    public float bgmVolume;
    public float sfxVolume;
   // public string language;
}

// 이후 필요한 데이터 구조체로 추가