using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class MonsterInfo
{
    public int id;
}

[CreateAssetMenu(fileName ="MonsterData", menuName ="Scriptable Object/New MonsterData")]
public class MonsterData : SheetDataReaderBase
{
    public List<MonsterInfo> infoList = new List<MonsterInfo>();

    private MonsterInfo monsterInfo;

    public void UpdateStat(List<GSTU_Cell> list)
    {
        monsterInfo = new MonsterInfo();

        foreach(var cell in list)
        {
            switch (cell.columnId)
            {
                case "id":
                    monsterInfo.id = int.Parse(cell.value);
                    break;
            }
        }
        infoList.Add(monsterInfo);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MonsterData))]
public class DataReaderEditor : Editor
{
    MonsterData data;

    private void OnEnable()
    {
        data = (MonsterData)target;
    }

    /// <summary>
    /// 커스텀 에디터 버튼. 누르면 데이터를 읽어옴
    /// </summary>
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(20);

        if (GUILayout.Button("구글 시트 데이터 읽어오기"))
        {
            ReadSheet(UpdateData);
            data.infoList.Clear();
        }
    }

    // 구글 시트의 데이터를 읽어옴
    private void ReadSheet(UnityAction<GstuSpreadSheet> callback, bool mergedcells = false)
    {
        SpreadsheetManager.Read(new GSTU_Search(data.sheetURL, data.sheetName), callback, mergedcells);
    }

    // 지정한 시작열부터 끝열까지의 데이터를 전부 가져옴
    private void UpdateData(GstuSpreadSheet sheet)
    {
        for (int i = data.startRowIndex; i <= data.endRowIndex; ++i)
        {
            data.UpdateStat(sheet.rows[i]);
        }
        EditorUtility.SetDirty(target);
    }
}
#endif