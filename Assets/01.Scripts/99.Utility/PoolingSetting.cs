#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class PoolSettingGenerator_Addressables
{
    [MenuItem("Tools/Pooling/Pooling SO 최신화")]
    public static void GenerateMultiPoolSetting()
    {
        string saveFolder = "Assets/06.Data/00.ScriptableObject/99.PoolObject";

        if (!Directory.Exists(saveFolder))
            Directory.CreateDirectory(saveFolder);

        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;

        if (settings == null)
        {
            Debug.LogError("Addressables Settings 를 찾을 수 없습니다.");
            return;
        }

        var entries = settings.groups
            .SelectMany(group => group.entries)
            .Where(entry => entry.labels.Contains("poolObj"))
            .ToList();

        Pooling multiSetting = ScriptableObject.CreateInstance<Pooling>();
        multiSetting.poolList = new List<PoolObject>();

        foreach (var entry in entries)
        {
            string path = entry.AssetPath;
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab == null)
            {
                Debug.LogWarning($"{path} 경로의 프리팹을 불러올 수 없습니다.");
                continue;
            }

            PoolObject poolEntry = new PoolObject
            {
                key = prefab.name,
                prefab = prefab,
                initSize = 5 // 기본 풀 크기
            };

            multiSetting.poolList.Add(poolEntry);
        }

        string fileName = $"Pooling.asset";
        string assetPath = Path.Combine(saveFolder, fileName);

        if (File.Exists(assetPath))
            File.Delete(assetPath);

        AssetDatabase.CreateAsset(multiSetting, assetPath);
        Debug.Log("MultiPoolSetting 생성 완료");

        AssetDatabase.SaveAssets();
    }

}
#endif
