using UnityEngine;
using UnityEditor;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using GoogleSheetsToUnity;
using UnityEngine.Events;

public class DataReaderEditorWindow : EditorWindow
{
    private MonsterData monsterData;

    [MenuItem("Window/Google Sheet Reader")]
    public static void ShowWindow()
    {
        GetWindow<DataReaderEditorWindow>("Google Sheet Reader");
    }

    private void OnEnable()
    {
        Addressables.LoadAssetAsync<MonsterData>("MonsterData").Completed += OnLoaded;
    }

    private void OnLoaded(AsyncOperationHandle<MonsterData> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            monsterData = handle.Result;
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("데이터 읽어오기", EditorStyles.boldLabel);
        GUILayout.Space(10);

        if (GUILayout.Button("몬스터 데이터"))
        {
            ReadSheet((sheet) => UpdateData(sheet, monsterData), monsterData);
            monsterData.infoList.Clear();
        }

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
    }
}