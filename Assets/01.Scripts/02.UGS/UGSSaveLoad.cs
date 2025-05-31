<<<<<<< Updated upstream
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;
using Unity.Services.CloudSave.Models.Data.Player;
using UnityEngine;

public class UGSSaveLoad : MonoBehaviour
{
    private const string SaveKey = "PlayerSaveData";


    #region 저장

    /// <summary>
    /// 저장
    /// </summary>
    public async UniTask SaveAsync()
    {
        try
        {
            var saveData = Collect();
            var saveJson = JsonConvert.SerializeObject(saveData);
            var saveDict = new Dictionary<string, object> { { SaveKey, saveJson } };

            await CloudSaveService.Instance.Data.Player.SaveAsync(saveDict, new Unity.Services.CloudSave.Models.Data.Player.SaveOptions(new PublicWriteAccessClassOptions()));
            Utils.Log("저장 완료");
        }
        catch (Exception e)
        {
            Utils.Log($"저장 실패: {e.Message}");
        }
    }

    /// <summary>
    /// 저장할 변수를 SaveData에 입력
    /// </summary>
    private SaveData Collect()
    {
        SaveData data = new SaveData
        {
            player = CollectPlayerData(),
            settings = CollectSettingsData()
        };

        return data;
    }
    private PlayerData CollectPlayerData()
    {
        return new PlayerData
        {
            // TODO: 실제 저장 내용 넣기
            nickName = "GameManager.instance.playerName",
            level = 151,
            coin = 500
        };
    }
    private SettingsData CollectSettingsData()
    {
        return new SettingsData
        {
            bgmVolume = SoundManager.Instance.BGMVolume,
            sfxVolume = SoundManager.Instance.SFXVolume,
            //  language = SettingsManager.Instance.CurrentLanguage
        };
    }

    #endregion

    #region 불러오기

    /// <summary>
    /// 저장된 내용 불러오기
    /// </summary>
    public async UniTask LoadAsync()
    {
        try
        {
            var playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { SaveKey }, new LoadOptions(new PublicReadAccessClassOptions()));

            if (playerData.TryGetValue(SaveKey, out var savedValue))
            {
                var json = savedValue.Value.GetAsString();
                var saveData = JsonConvert.DeserializeObject<SaveData>(json);

                OnLoadComplete(saveData);
            }
            else
            {
                Utils.Log("저장된 데이터가 없음.");
            }
        }
        catch (Exception e)
        {
            Utils.Log($"로드 실패: {e.Message}");
        }
    }

    /// <summary>
    /// 불러온 내용 실제 적용 시키는 함수
    /// </summary>
    private void OnLoadComplete(SaveData saveData)
    {
        SoundManager.Instance.SetBGMVolume(saveData.settings.bgmVolume);
        SoundManager.Instance.SetSFXVolume(saveData.settings.sfxVolume);
    }

    #endregion
}
=======
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models.Data.Player;
using UnityEngine;

public class UGSSaveLoad : MonoBehaviour
{
    private const int CurrentVersion = 4; // 최신 데이터 버전

    private const string SaveKey = "PlayerSaveData";
    private const string RankDataKey = "PlayerRankDataKey";

    #region 저장

    /// <summary>
    /// 저장
    /// </summary>
    public async UniTask SaveAsync()
    {
        try
        {
            var saveData = Collect();
            saveData.version = CurrentVersion; // 항상 최신 버전으로 설정
            ValidateSaveData(ref saveData); // 데이터 유효성 검사

            var saveJson = JsonConvert.SerializeObject(saveData);
            var saveDict = new Dictionary<string, object> { { SaveKey, saveJson } };

            await CloudSaveService.Instance.Data.Player.SaveAsync(
                saveDict,
                new Unity.Services.CloudSave.Models.Data.Player.SaveOptions(new PublicWriteAccessClassOptions())
            );

            PlayerPrefs.SetFloat("BGM_VOLUME", SoundManager.Instance.BGMVolume);
            PlayerPrefs.SetFloat("SFX_VOLUME", SoundManager.Instance.SFXVolume);
            PlayerPrefs.Save();

            Utils.Log("저장 완료");
        }
        catch (Exception e)
        {
            Utils.Log($"저장 실패: {e.Message}");
        }
    }

    /// <summary>
    /// 게임 정보(랭크)를 업로드
    /// </summary>
    /// <param name="rankInfo"></param>
    /// <returns></returns>
    public async UniTask UploadRankDataAsync(int QueenID)
    {
        try
        {
            var leaderBoardData = new LeaderBoardData
            {
                queenID = QueenID
            };

            var saveJson = JsonConvert.SerializeObject(leaderBoardData);
            var saveDict = new Dictionary<string, object> { { RankDataKey, saveJson } };

            await CloudSaveService.Instance.Data.Player.SaveAsync(
                saveDict,
                new Unity.Services.CloudSave.Models.Data.Player.SaveOptions(new PublicWriteAccessClassOptions())
            );
            Utils.Log("랭크 데이터 업로드 완료");
        }
        catch (Exception e)
        {
            Utils.Log($"랭크 데이터 업로드 실패: {e.Message}");
        }
    }

    /// <summary>
    /// 저장할 변수를 SaveData에 입력
    /// </summary>
    private SaveData Collect()
    {
        SaveData data = new SaveData
        {
            version = CurrentVersion,
            player = CollectPlayerData(),
            settings = CollectSettingsData(),
            queenUpgrades = CollectQueenAbilityUpgradeData(),
            extraRootFields = new Dictionary<string, JToken>()
        };

        return data;
    }
    private PlayerData CollectPlayerData()
    {
        return new PlayerData
        {
            gold = Mathf.Max(0, GameManager.Instance.GetGold()),
            extraPlayerFields = new Dictionary<string, JToken>()
        };
    }
    private SettingsData CollectSettingsData()
    {
        return new SettingsData
        {
            // bgmVolume = Mathf.Clamp(SoundManager.Instance.BGMVolume, 0f, 1f),
            // sfxVolume = Mathf.Clamp(SoundManager.Instance.SFXVolume, 0f, 1f),
            //  language = SettingsManager.Instance.CurrentLanguage
            extraSettingsFields = new Dictionary<string, JToken>()
        };
    }
    private QueenAbilityUpgradeData CollectQueenAbilityUpgradeData()
    {
        var data = QueenAbilityUpgradeManager.Instance.SetSaveData();
        data.extraQueenUpgradeFields = new Dictionary<string, JToken>();
        return data;
    }


    /// <summary>
    /// 저장 데이터 유효성 검사
    /// </summary>
    private void ValidateSaveData(ref SaveData data)
    {
        // 금액 음수 방지
        if (data.player.gold < 0)
            data.player.gold = 0;

        // 볼륨 범위 제한
        // data.settings.bgmVolume = Mathf.Clamp(data.settings.bgmVolume, 0f, 1f);
        // data.settings.sfxVolume = Mathf.Clamp(data.settings.sfxVolume, 0f, 1f);

        // 언어 null 체크
        // if (string.IsNullOrEmpty(data.settings.language))
        //     data.settings.language = "en";
    }

    #endregion

    #region 불러오기

    /// <summary>
    /// 저장된 내용 불러오기
    /// </summary>
    public async UniTask LoadAsync()
    {
        try
        {
            var playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(
                new HashSet<string> { SaveKey },
                new LoadOptions(new PublicReadAccessClassOptions())
            );

            if (playerData.TryGetValue(SaveKey, out var savedValue))
            {
                var json = savedValue.Value.GetAsString();

                SaveData saveData;
                try
                {
                    saveData = JsonConvert.DeserializeObject<SaveData>(json);
                }
                catch (Exception ex)
                {
                    Utils.Log($"역직렬화 실패: {ex.Message}");
                    saveData = CreateDefaultSaveData();
                }

                // 데이터 마이그레이션
                saveData = MigrateData(saveData);

                // 데이터 적용
                OnLoadComplete(saveData);
                Utils.Log("저장된 데이터 적용 완료");
            }
            else
            {
                Utils.Log("저장된 데이터가 없음.");
                OnLoadComplete(CreateDefaultSaveData());
            }
        }
        catch (Exception e)
        {
            Utils.Log($"클라우드 로드 실패: {e.Message}");
            OnLoadComplete(CreateDefaultSaveData());
        }
    }

    /// <summary>
    /// 데이터 마이그레이션
    /// </summary>
    private SaveData MigrateData(SaveData data)
    {
        // 버전 정보가 없는 경우 (최초 데이터)
        if (data.version == 0)
        {
            data.version = 1;
            data.player = new PlayerData
            {
                gold = Mathf.Max(0, data.player.gold),
                extraPlayerFields = new Dictionary<string, JToken>()
            };

            data.settings = new SettingsData
            {
                // bgmVolume = Mathf.Clamp(data.settings.bgmVolume, 0f, 1f),
                // sfxVolume = Mathf.Clamp(data.settings.sfxVolume, 0f, 1f),
                extraSettingsFields = new Dictionary<string, JToken>()
            };
            data.queenUpgrades = data.queenUpgrades.upgrades != null
                ? data.queenUpgrades
                : new QueenAbilityUpgradeData { upgrades = new List<QueenAbilityUpgradeInfo>(), extraQueenUpgradeFields = new Dictionary<string, JToken>() };
            data.extraRootFields = new Dictionary<string, JToken>();
        }

        // 버전 1 -> 버전 2 (예시로 language 필드 추가)
        if (data.version == 1)
        {
            data.settings = new SettingsData
            {
                // bgmVolume = Mathf.Clamp(data.settings.bgmVolume, 0f, 1f),
                // sfxVolume = Mathf.Clamp(data.settings.sfxVolume, 0f, 1f),
                // 언어 부분 추가
                extraSettingsFields = data.settings.extraSettingsFields ?? new Dictionary<string, JToken>()
            };
            data.version = 2;
        }

        // 버전 2 -> 버전 3 (PlayerData의 nickName 삭제)
        if (data.version == 2)
        {
            data.player = new PlayerData
            {
                gold = Mathf.Max(0, data.player.gold),
                extraPlayerFields = new Dictionary<string, JToken>()
            };
            data.version = 3;
        }

        // 버전 3 -> 버전 4 (bgmVolume/sfxVolume 제거)
        if (data.settings.extraSettingsFields != null)
        {
            data.settings.extraSettingsFields.Remove("bgmVolume");
            data.settings.extraSettingsFields.Remove("sfxVolume");

            data.version = 4;
        }

        // 추가 버전 마이그레이션은 여기에 구현
        return data;
    }

    /// <summary>
    /// 기본 SaveData 생성
    /// </summary>
    private SaveData CreateDefaultSaveData()
    {
        return new SaveData
        {
            version = CurrentVersion,
            player = new PlayerData
            {
                gold = 0,
                extraPlayerFields = new Dictionary<string, JToken>()
            },
            settings = new SettingsData
            {
                // bgmVolume = 0.1f,
                // sfxVolume = 0.1f,
                // 언어 부분 추가
                extraSettingsFields = new Dictionary<string, JToken>()
            },
            queenUpgrades = new QueenAbilityUpgradeData
            {
                upgrades = new List<QueenAbilityUpgradeInfo>(),
                extraQueenUpgradeFields = new Dictionary<string, JToken>()
            },
            extraRootFields = new Dictionary<string, JToken>()
        };
    }

    /// <summary>
    /// 불러온 내용 실제 적용
    /// </summary>
    private void OnLoadComplete(SaveData saveData)
    {
        // 플레이어 정보
        try
        {
            GameManager.Instance.SetGold(saveData.player.gold);
            Utils.Log($"Gold 적용 완료: {saveData.player.gold}");
        }
        catch (Exception e)
        {
            Utils.Log($"Gold 적용 실패: {e.Message}");
        }

        // 옵션
        // try
        // {
        //     SoundManager.Instance.SetBGMVolume(saveData.settings.bgmVolume);
        //     SoundManager.Instance.SetSFXVolume(saveData.settings.sfxVolume);
        //     // 언어 부분 추가
        //     Utils.Log("사운드 및 언어 설정 적용 완료");
        // }
        // catch (Exception e)
        // {
        //     Utils.Log($"사운드/언어 설정 적용 실패: {e.Message}");
        // }

        // 어빌리티
        try
        {
            QueenAbilityUpgradeManager.Instance.ApplyUpgradeData(saveData.queenUpgrades);
            Utils.Log("여왕 강화 데이터 적용 완료");
        }
        catch (Exception e)
        {
            Utils.Log($"여왕 강화 적용 실패: {e.Message}");
        }
    }


    /// <summary>
    /// playerId에 해당하는 공개 데이터를 읽어 nickname과 queenID를 반환합니다.
    /// </summary>
    public async Task<(string nickname, int queenID)> LoadPublicDataWithQueenId(string playerId)
    {
        try
        {
            var rankData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { RankDataKey }, new LoadOptions(new PublicReadAccessClassOptions(playerId)));

            string nickname = await UGSManager.Instance.Auth.LoadPublicDataByPlayerId(playerId);

            if (rankData.TryGetValue(RankDataKey, out var savedValue))
            {
                var json = savedValue.Value.GetAsString();

                LeaderBoardData leaderBoardData;
                try
                {
                    leaderBoardData = JsonConvert.DeserializeObject<LeaderBoardData>(json);
                }
                catch (Exception ex)
                {
                    Utils.Log($"역직렬화 실패: {ex.Message}");
                    leaderBoardData = CreateDefaultLeaderBoardData();
                }

                // 데이터 마이그레이션 (필요하면 추가하기)
                // leaderBoardData = MigrateData(leaderBoardData);

                Utils.Log("랭크 데이터 반환 완료");
                return (nickname, leaderBoardData.queenID);
            }
            else
            {
                Utils.Log("저장된 랭크 데이터가 없음.");
                return (nickname, -1);
            }

        }
        catch (Exception e)
        {
            Debug.LogWarning($"랭크 데이터 로드 실패: {e.Message}");
            return ("Unknown", -1);
        }
    }

    /// <summary>
    /// 기본 LeaderBoardData 생성
    /// </summary>
    private LeaderBoardData CreateDefaultLeaderBoardData()
    {
        return new LeaderBoardData
        {
            queenID = -1,
            extraLeaderboardResultFields = new Dictionary<string, JToken>()
        };
    }
    #endregion
}
>>>>>>> Stashed changes
