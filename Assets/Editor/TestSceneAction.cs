using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Codice.Utils.Buffers.SizeBufferPool;
using static PlasticGui.WorkspaceWindow.Merge.MergeInProgress;

public class TestSceneAction : EditorWindow
{
    private bool fold_abi;
    private bool fold_enh;
    private bool fold_hero;


    private GameHUD gameHud;
    private TestStart tStart;


    private Vector2 scroll = Vector2.zero;

    private Dictionary<int, int> heroWeapon = new Dictionary<int, int>();


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
        ShowHeroSet();

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

    private void ShowHeroSet()
    {
        fold_hero = EditorGUILayout.Foldout(fold_hero, "HeroSetting");

        var a =DataManager.Instance.heroAbilityDic;

        if(fold_hero)
        {
            int sum = 0;
            foreach(var ab in a)
            {
                if(!heroWeapon.ContainsKey(ab.Key))
                {
                    heroWeapon.Add(ab.Key, 0);
                }
                heroWeapon[ab.Key] = (int)EditorGUILayout.Slider(ab.Key.ToString(), heroWeapon[ab.Key], 0, 8);
                sum += heroWeapon[ab.Key];
                if(sum>30)
                {
                    heroWeapon[ab.Key] = (int)EditorGUILayout.Slider(ab.Key.ToString(), 0, 0, 8);

                }
            }
            
            EditorGUILayout.IntField("레벨", sum);
            EditorGUILayout.HelpBox("최대레벨에 주의하세요, 현재 테이블상 최대레벨은 30입니다", MessageType.Warning);
            if(GUILayout.Button("소환"))
            {
                if (SceneManager.GetActiveScene().name != "TestScene")
                {
                    return;
                }
                if(sum==0)
                {
                    return;
                }
                HeroStatusInfo statusInfo=HeroManager.Instance.SetTestHero(sum);
                HeroController hero = HeroPoolManager.Instance.GetObject(SpawnPointManager.Instance.heroPoint.GetRandomPosition());
                hero?.StatInit(statusInfo, HeroManager.Instance.isHealthUI,heroWeapon);

            }
        }
    }

}
