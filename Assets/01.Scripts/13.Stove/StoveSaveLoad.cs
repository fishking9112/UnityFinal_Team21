using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Stove.PCSDK.NET;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveSaveLoad : MonoBehaviour
{
    public async UniTask LoadPlayerDataAsync()
    {
        Utils.Log("플레이어 데이터 불러오기");
        StovePC.GetStat("SAVE_DATA");
        await UniTask.Delay(500); // 콜백 기다리는 코드 추가 필요
    }

    public void SavePlayerData(int value)
    {
        Utils.Log($"플레이어 데이터 저장: {value}");
        StovePC.SetStat("SAVE_DATA", value);
    }



    /*

    private const string SaveFileName = "PlayerSaveData.json";
    private const string RankFileName = "PlayerRankData.json";

    #region 저장

    public async UniTask SaveAsync()
    {
        try
        {
            var saveData = Collect();
            var json = JsonConvert.SerializeObject(saveData);

            var uploadResult = StovePC.UploadStorageFile(SaveFileName, System.Text.Encoding.UTF8.GetBytes(json));
            if (uploadResult != StovePCResult.NoError)
            {
                Debug.LogWarning($"스토브 클라우드 저장 실패: {uploadResult}");
                return;
            }

            PlayerPrefs.SetFloat("BGM_VOLUME", SoundManager.Instance.BGMVolume);
            PlayerPrefs.SetFloat("SFX_VOLUME", SoundManager.Instance.SFXVolume);
            PlayerPrefs.Save();

            Debug.Log("스토브 클라우드 저장 성공");
        }
        catch (Exception e)
        {
            Debug.LogWarning($"스토브 저장 중 예외 발생: {e.Message}");
        }
        await UniTask.Yield();
    }

    public async UniTask UploadRankDataAsync(int queenID)
    {
        try
        {
            var rankData = new LeaderBoardData
            {
                queenID = queenID
            };
            var json = JsonConvert.SerializeObject(rankData);

            var uploadResult = StovePC.UploadStorageFile(RankFileName, System.Text.Encoding.UTF8.GetBytes(json));
            if (uploadResult != StovePCResult.NoError)
            {
                Debug.LogWarning($"랭크 데이터 저장 실패: {uploadResult}");
                return;
            }

            Debug.Log("랭크 데이터 저장 성공");
        }
        catch (Exception e)
        {
            Debug.LogWarning($"랭크 저장 중 예외 발생: {e.Message}");
        }
        await UniTask.Yield();
    }

    #endregion

    #region 불러오기

    public async UniTask LoadAsync()
    {
        try
        {
            var downloadResult = StovePC.DownloadStorageFile(SaveFileName, out byte[] data);
            if (downloadResult != StovePCResult.NoError)
            {
                Debug.LogWarning($"스토브 클라우드 데이터 다운로드 실패: {downloadResult}");
                OnLoadComplete(CreateDefaultSaveData());
                return;
            }

            var json = System.Text.Encoding.UTF8.GetString(data);

            var saveData = JsonConvert.DeserializeObject<SaveData>(json);
            if (saveData == null)
            {
                Debug.LogWarning("저장된 데이터 역직렬화 실패, 기본 데이터 생성");
                saveData = CreateDefaultSaveData();
            }

            OnLoadComplete(saveData);
            Debug.Log("스토브 클라우드 데이터 불러오기 완료");
        }
        catch (Exception e)
        {
            Debug.LogWarning($"스토브 데이터 불러오기 예외 발생: {e.Message}");
            OnLoadComplete(CreateDefaultSaveData());
        }
        await UniTask.Yield();
    }

    public async UniTask<(string nickname, int queenID)> LoadRankDataAsync()
    {
        try
        {
            var downloadResult = StovePC.DownloadStorageFile(RankFileName, out byte[] data);
            if (downloadResult != StovePCResult.NoError)
            {
                Debug.LogWarning($"랭크 데이터 다운로드 실패: {downloadResult}");
                return ("Unknown", -1);
            }

            var json = System.Text.Encoding.UTF8.GetString(data);

            var rankData = JsonConvert.DeserializeObject<LeaderBoardData>(json);
            if (rankData == null)
            {
                Debug.LogWarning("랭크 데이터 역직렬화 실패");
                return ("Unknown", -1);
            }

            // 닉네임은 별도 API에서 받아오거나 STOVE 유저 데이터에서 받아와야 합니다.
            string nickname = StoveManager.Instance.User?.Nickname ?? "Unknown";

            Debug.Log("랭크 데이터 불러오기 완료");
            return (nickname, rankData.queenID);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"랭크 데이터 불러오기 예외 발생: {e.Message}");
            return ("Unknown", -1);
        }
    }

    #endregion

    #region 데이터 수집 및 적용

    private SaveData Collect()
    {
        return new SaveData
        {
            version = 4,
            player = new PlayerData
            {
                gold = Mathf.Max(0, GameManager.Instance.GetGold()),
                extraPlayerFields = new Dictionary<string, JToken>()
            },
            settings = new SettingsData
            {
                extraSettingsFields = new Dictionary<string, JToken>()
            },
            queenUpgrades = QueenAbilityUpgradeManager.Instance.SetSaveData(),
            extraRootFields = new Dictionary<string, JToken>()
        };
    }

    private void OnLoadComplete(SaveData saveData)
    {
        try
        {
            GameManager.Instance.SetGold(saveData.player.gold);
            QueenAbilityUpgradeManager.Instance.ApplyUpgradeData(saveData.queenUpgrades);
            Debug.Log("저장 데이터 적용 완료");
        }
        catch (Exception e)
        {
            Debug.LogWarning($"저장 데이터 적용 실패: {e.Message}");
        }
    }

    private SaveData CreateDefaultSaveData()
    {
        return new SaveData
        {
            version = 4,
            player = new PlayerData { gold = 0, extraPlayerFields = new Dictionary<string, JToken>() },
            settings = new SettingsData { extraSettingsFields = new Dictionary<string, JToken>() },
            queenUpgrades = new QueenAbilityUpgradeData
            {
                upgrades = new List<QueenAbilityUpgradeInfo>(),
                extraQueenUpgradeFields = new Dictionary<string, JToken>()
            },
            extraRootFields = new Dictionary<string, JToken>()
        };
    }

    #endregion
    */
}
