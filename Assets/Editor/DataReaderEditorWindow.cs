using UnityEngine;
using UnityEditor;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using GoogleSheetsToUnity;
using UnityEngine.Events;
using System.Collections.Generic;

public class DataReaderEditorWindow : EditorWindow
{
    private class DataInfo
    {
        public string addressableKey;
        public string buttonName;
        public ScriptableObject dataSo;
        public SheetDataReaderBase sheetData => dataSo as SheetDataReaderBase;

        public DataInfo(string buttonName, string addressableKey)
        {
            this.buttonName = buttonName;
            this.addressableKey = addressableKey;
        }
    }

    private List<DataInfo> dataInfo;

    [MenuItem("Window/Google Sheet Reader")]
    public static void ShowWindow()
    {
        GetWindow<DataReaderEditorWindow>("Google Sheet Reader");
    }

    private void OnEnable()
    {
        dataInfo = new List<DataInfo>()
        {
            new DataInfo("Monster 데이터", "MonsterData"),
            new DataInfo("QueenAbility 데이터", "QueenAbilityData"),
            new DataInfo("HeroAbility 데이터", "HeroAbilityData"),
            new DataInfo("Enhance 데이터", "QueenEnhanceData"),
            new DataInfo("HeroStatus 데이터","HeroStatusData"),
        };

        foreach (var info in dataInfo)
        {
            LoadAsset(info);
        }
    }

    // Addressable을 이용하여 에셋을 로드
    private void LoadAsset(DataInfo info)
    {
        Addressables.LoadAssetAsync<ScriptableObject>(info.addressableKey).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                info.dataSo = handle.Result;
            }
            else
            {
                Utils.Log($"[{info.addressableKey}]를 불러오지 못했습니다.");
            }
        };
    }

    // 커스텀 윈도우 창에 띄워질 UI
    private void OnGUI()
    {
        GUILayout.Label("데이터 불러오기", EditorStyles.boldLabel);
        GUILayout.Space(10);

        foreach (var info in dataInfo)
        {
            GUI.enabled = info.dataSo != null;

            if (GUILayout.Button(info.buttonName))
            {
                info.sheetData.ClearInfoList();
                ReadSheet(sheet => UpdateData(sheet, info.sheetData), info.sheetData);
            }
        }

        GUI.enabled = true;
    }

    // 구글 시트의 데이터를 읽어옴
    private void ReadSheet(UnityAction<GstuSpreadSheet> callback, SheetDataReaderBase data, bool mergedcells = false)
    {
        SpreadsheetManager.Read(new GSTU_Search(data.sheetURL, data.sheetName), callback, mergedcells);
    }

    // 지정한 시작열부터 끝열까지의 데이터를 전부 가져옴
    private void UpdateData(GstuSpreadSheet sheet, SheetDataReaderBase data)
    {
        for (int i = data.startRowIndex; i <= data.endRowIndex; ++i)
        {
            data.UpdateStat(sheet.rows[i]);
        }

        // 데이터 저장
        EditorUtility.SetDirty(data);
        AssetDatabase.SaveAssets();

        Utils.Log($"[{data.name}] 시트를 불러왔습니다.");
    }
}