using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using UnityEngine;

public class UGSSaveLoad : MonoBehaviour
{
    private const string SaveKey = "PlayerSaveData";

    #region 저장

    /// <summary>
    /// 저장
    /// </summary>
    public async Task SaveAsync()
    {
        var saveData = Collect();

        var saveJson = JsonConvert.SerializeObject(saveData);

        var saveDict = new Dictionary<string, object>
        {
            { SaveKey, saveJson }
        };

        await CloudSaveService.Instance.Data.Player.SaveAsync(saveDict);
        Debug.Log("저장 완료");
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
    public async Task LoadAsync()
    {
        try
        {
            var result = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { SaveKey });
            if (result.TryGetValue(SaveKey, out var savedValue))
            {
                var json = savedValue.Value.GetAsString();
                var saveData = JsonConvert.DeserializeObject<SaveData>(json);

                OnLoadComplete(saveData);
            }
            else
            {
                Debug.LogWarning("저장된 데이터가 없음.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"로드 실패: {e.Message}");
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
