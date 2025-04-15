using GoogleSheetsToUnity;
using System.Collections.Generic;
using UnityEngine;


public abstract class SheetDataReaderBase : ScriptableObject
{
    [Header("Sheet URL")]
    public string sheetURL;

    [Header("Sheet Name")]
    public string sheetName;

    [Header("Start Row Index")]
    public int startRowIndex;

    [Header("End Row Index")]
    public int endRowIndex;

    public abstract void UpdateStat(List<GSTU_Cell> list);
    public abstract void ClearInfoList();
}