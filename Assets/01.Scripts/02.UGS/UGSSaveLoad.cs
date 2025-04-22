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
            settings = CollectSettingsData(),
            queenUpgrades = CollectQueenAbilityUpgradeData()
        };

        return data;
    }
    private PlayerData CollectPlayerData()
    {
        return new PlayerData
        {
            // TODO: 실제 저장 내용 넣기
            nickName = "GameManager.instance.playerName",
            gold = GameManager.Instance.GetGold()
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
    private QueenAbilityUpgradeData CollectQueenAbilityUpgradeData()
    {
        return QueenAbilityUpgradeManager.Instance.SetSaveData();
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
                    return;
                }

                // 역직렬화가 성공한 후에 적용
                OnLoadComplete(saveData);
            }
            else
            {
                Utils.Log("저장된 데이터가 없음.");
            }
        }
        catch (Exception e)
        {
            Utils.Log($"클라우드 로드 실패: {e.Message}");
        }
    }

    /// <summary>
    /// 불러온 내용 실제 적용 시키는 함수
    /// </summary>
    private void OnLoadComplete(SaveData saveData)
    {
        try
        {
            GameManager.Instance.SetGold(saveData.player.gold);
            Utils.Log($"Gold 적용 완료: {saveData.player.gold}");
        }
        catch (Exception e)
        {
            Utils.Log($"Gold 적용 실패: {e}");
        }

        try
        {
            SoundManager.Instance.SetBGMVolume(saveData.settings.bgmVolume);
            SoundManager.Instance.SetSFXVolume(saveData.settings.sfxVolume);
            Utils.Log("사운드 설정 적용 완료");
        }
        catch (Exception e)
        {
            Utils.Log($"사운드 설정 적용 실패: {e}");
        }

        try
        {
            QueenAbilityUpgradeManager.Instance.ApplyUpgradeData(saveData.queenUpgrades);
            Utils.Log("여왕 강화 데이터 적용 완료");
        }
        catch (Exception e)
        {
            Utils.Log($"여왕 강화 적용 실패: {e}");
        }
    }

    #endregion
}
