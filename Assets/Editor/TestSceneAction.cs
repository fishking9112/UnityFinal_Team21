using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestSceneAction : EditorWindow
{
    private bool fold_abi;
    private bool fold_enh;


    private GameHUD gameHud;
    private TestStart tStart;


    private Vector2 scroll = Vector2.zero;

    [MenuItem("Window/TestScene")]
    public static void ShowEditor()
    {
        GetWindow<TestSceneAction>("TestFunc");
    }


    private void OnGUI()
    {
        scroll = GUILayout.BeginScrollView(scroll);

        ShowAbility();
        ShowEnhance();


        GUILayout.EndScrollView();
        if (Event.current.type == EventType.ScrollWheel)
        {
            scroll.y += Event.current.delta.y * 20; 
            Repaint(); 
        }

    }

    private void ShowAbility()
    {
        fold_abi = EditorGUILayout.Foldout(fold_abi, "QueenAbility");


        if (fold_abi)
        {

            foreach (var kvp in DataManager.Instance.queenAbilityDic)
            {
                int currentValue = QueenAbilityUpgradeManager.Instance.GetLevel(kvp.Value.id);

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
        if(tStart==null)
            tStart = GameObject.Find("StartTrigger").GetComponent<TestStart>();
        tStart.ResetAllMonsterStats();
    }

    private void ShowEnhance()
    {
        fold_enh = EditorGUILayout.Foldout(fold_enh, "Enhance");

        if (SceneManager.GetActiveScene().name != "TestScene")
        {
            fold_enh = false;
            return;
        }    



        if (fold_enh)
        {
            if (gameHud == null)
            {
                gameHud = GameObject.Find("GameHUD(Clone)")?.GetComponent<GameHUD>();
            }
            foreach (var key in DataManager.Instance.queenEnhanceDic)
            {
                EditorGUILayout.BeginHorizontal();
                int currentValue = gameHud.queenEnhanceUI.GetEnhanceLevel(key.Key);


                currentValue = EditorGUILayout.IntField(key.Value.id.ToString(), currentValue);
                if (GUILayout.Button("+"))
                {
                    
                    QueenEnhanceInfo info = (DataManager.Instance.queenEnhanceDic[key.Value.id]);

                    if (info.maxLevel > currentValue)
                    {
                        gameHud.queenEnhanceUI.ApplyInhance(info);
                        return;
                    }
                }
                EditorGUILayout.EndHorizontal();

            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();

        }
    }

}
