using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Localization;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.SceneManagement;
using UnityEngine.Localization.Tables;
using UnityEngine.Localization.Settings;

public class TestSceneAction : EditorWindow
{
    private bool fold_abi;
    private bool fold_enh;
    private bool fold_hero;
    private bool fold_kill;

    private bool fold_etc;
    private bool isMortal=false;
    private bool isInfinity=false;
    private bool prevMortal=false;
    private bool prevInfinity=false;

    private GameHUD gameHud;
    private TestStart tStart;


    private Vector2 scroll = Vector2.zero;

    private Dictionary<int, int> heroWeapon = new Dictionary<int, int>();

    private StringTableCollection stringTableCollection;
    StringTable stringTable;

    [MenuItem("Window/TestScene")]
    public static void ShowEditor()
    {
        GetWindow<TestSceneAction>("TestFunc");
    }

    public void OnEnable()
    {

        isMortal = false;
        isInfinity = false;
        prevMortal = false;
        prevInfinity = false;
    }

    private void OnGUI()
    {
        stringTable = GetFixedStringTable("ko");
        scroll = GUILayout.BeginScrollView(scroll);

        ShowAbility();
        ShowEnhance();
        ShowHeroSet();
        KillAllUnit();

        ShowETC();

        GUILayout.EndScrollView();
        if (Event.current.type == EventType.ScrollWheel)
        {
            scroll.y += Event.current.delta.y * 20; 
            Repaint(); 
        }

    }

    #region Func
    private void ShowAbility()
    {
        fold_abi = EditorGUILayout.Foldout(fold_abi, "QueenAbility");

        //if (SceneManager.GetActiveScene().name != "TestScene")
        if(GameManager.Instance.isTest==false)
        {
            fold_abi = false;
            return;
        }
        if (fold_abi)
        {
            EditorGUILayout.BeginVertical("box");
            foreach (var kvp in DataManager.Instance.queenAbilityDic)
            {

                int currentValue = QueenAbilityUpgradeManager.Instance.GetLevel(kvp.Value.id);
                string value = stringTable[kvp.Value.name].Value;
                int maxLv = DataManager.Instance.queenAbilityDic[kvp.Value.id].maxLevel;
                currentValue = (int)EditorGUILayout.Slider(value, currentValue, 0, maxLv);

                QueenAbilityUpgradeManager.Instance.TrySetLevel(kvp.Value.id, currentValue);
            }

            if (GUILayout.Button("어빌리티 적용"))
            {
                ResetStat();
                QueenAbilityUpgradeManager.Instance.ApplyAllEffects();
                MonsterManager.Instance.WaitUntilInitCompleteAndSetup().Forget();
            }
            EditorGUILayout.EndVertical();
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

        if (GameManager.Instance.isTest == false)
        {
            fold_enh = false;
            return;
        }    



        if (fold_enh)
        {
            EditorGUILayout.BeginVertical("box");
            if (gameHud == null)
            {
                gameHud = GameObject.Find("GameHUD(Clone)")?.GetComponent<GameHUD>();
            }

            foreach (var key in DataManager.Instance.queenEnhanceDic)
            {
                int currentValue = gameHud.queenEnhanceUI.GetEnhanceLevel(key.Key);
                string value = stringTable[key.Value.name].Value;

                currentValue = EditorGUILayout.IntField(value, currentValue);
                if (GUILayout.Button("+"))
                {
                    
                    QueenEnhanceInfo info = (DataManager.Instance.queenEnhanceDic[key.Value.id]);

                    if (info.maxLevel > currentValue)
                    {
                        gameHud.queenEnhanceUI.ApplyInhance(info);
                    }
                }

            }
            EditorGUILayout.EndVertical();


        }
    }

    private void ShowHeroSet()
    {
        fold_hero = EditorGUILayout.Foldout(fold_hero, "HeroSetting");
        if (GameManager.Instance.isTest == false)
        {
            fold_hero = false;
            return;
        }
        var a =DataManager.Instance.heroAbilityDic;

        if(fold_hero)
        {
            EditorGUILayout.BeginVertical("box");

            int sum = 0;
            foreach(var ab in a)
            {
                if(!heroWeapon.ContainsKey(ab.Key))
                {
                    heroWeapon.Add(ab.Key, 0);
                }
                string value = stringTable[ab.Value.name].Value;

                heroWeapon[ab.Key] = (int)EditorGUILayout.Slider(value, heroWeapon[ab.Key], 0, 8);
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
                if (GameManager.Instance.isTest == false)
                {

                }
                else if(sum==0)
                {

                }
                else
                {

                    HeroStatusInfo statusInfo=HeroManager.Instance.SetTestHero(sum);
                    HeroController hero = HeroPoolManager.Instance.GetObject(SpawnPointManager.Instance.heroPoint.GetRandomPosition());
                    hero?.StatInit(statusInfo, HeroManager.Instance.isHealthUI,heroWeapon);
                }

            }
            EditorGUILayout.EndVertical();

        }
    }


    private void KillAllUnit()
    {

        fold_kill = EditorGUILayout.Foldout(fold_kill, "Kill");

        if (GameManager.Instance.isTest == false)
        {
            fold_kill = false;
            return;
        }

        if (fold_kill)
        {
            EditorGUILayout.BeginVertical("box");

            if (GUILayout.Button("Kill All Hero"))
            {
                foreach (var h in HeroManager.Instance.hero)
                {
                    h.Value.Die();
                }
            }
            EditorGUILayout.EndVertical();
        }
    }

    private void ShowETC()
    {
        fold_etc= EditorGUILayout.Foldout(fold_etc, "ETC");

        if(GameManager.Instance.isTest==false)
        {
            fold_etc = false;
            return;
        }

        if(fold_etc)
        {
            EditorGUILayout.BeginVertical("box");

            isMortal = EditorGUILayout.Toggle("성채 무적", isMortal);
            isInfinity = EditorGUILayout.Toggle("소환 무한", isInfinity);

            if(prevMortal!=isMortal)
            {
                prevMortal = isMortal;
                GameManager.Instance.isMortal = isMortal;
            }

            if(prevInfinity!=isInfinity)
            {
                prevInfinity = isInfinity;
                GameManager.Instance.isInf=isInfinity;
            }
            EditorGUILayout.EndVertical();
        }

    }



    #endregion







    private StringTable GetFixedStringTable(string localeCode)
    {
        StringTableCollection collection = LocalizationEditorSettings.GetStringTableCollections().Count > 0
            ? LocalizationEditorSettings.GetStringTableCollections()[0]
            : null;

        if (collection == null) return null;

        Locale fixedLocale = LocalizationSettings.AvailableLocales.GetLocale(localeCode);

        if (fixedLocale == null)
        {
            return null;
        }

        return (StringTable)collection.GetTable(fixedLocale.Identifier);
    }


}
