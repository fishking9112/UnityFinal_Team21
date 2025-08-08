using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TestSceneAction : EditorWindow
{
    private bool fold_abi;
    private bool fold_enh;
    List<int> abi_list = new List<int>();



    [MenuItem("Window/TestScene")]
    public static void ShowEditor()
    {
        GetWindow<TestSceneAction>("TestFunc");
    }


    private void OnGUI()
    {
        ShowAbility();
        ShowEnhance();
    }

    private void ShowAbility()
    {
        fold_abi = EditorGUILayout.Foldout(fold_abi, "QueenAbility");


        if (fold_abi)
        {

            foreach (var kvp in DataManager.Instance.queenAbilityDic)
            {
                // 현재 Dictionary의 value 값을 가져옵니다.
                int currentValue = QueenAbilityUpgradeManager.Instance.GetLevel(kvp.Value.id);

                // IntField를 그리고, 변경된 값을 다시 임시 변수에 할당합니다.
                currentValue = EditorGUILayout.IntField(kvp.Value.id.ToString(), currentValue);

                QueenAbilityUpgradeManager.Instance.TrySetLevel(kvp.Value.id, currentValue);
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            if (GUILayout.Button("어빌리티 적용"))
            {
                ResetStat();
                QueenAbilityUpgradeManager.Instance.ApplyAllEffects();
                MonsterManager.Instance.WaitUntilInitCompleteAndSetup().Forget();
            }
        }
    }

    private void ResetStat()
    {
        TestStart t = GameObject.Find("StartTrigger").GetComponent<TestStart>();
        t.ResetAllMonsterStats();
    }

    private void ShowEnhance()
    {
        fold_enh = EditorGUILayout.Foldout(fold_abi, "Enhance");


        if (fold_enh)
        {
            foreach (var key in DataManager.Instance.queenEnhanceDic)
            {
                int a;
                a = EditorGUILayout.IntField(key.Value.id.ToString(), 0);

            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            if (GUILayout.Button("인핸스 적용"))
            {

            }
        }
    }

}
