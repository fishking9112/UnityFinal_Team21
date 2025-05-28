using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResultController : MonoBehaviour
{
    private WaitForSeconds waitSec2 = new WaitForSeconds(2f);
    private WaitForSeconds waitHalf = new WaitForSeconds(0.25f);

    public GameObject goVictory;
    public GameObject goDefeat;

    public void Awake()
    {
        GameManager.Instance.gameResultController = this;
        goVictory.SetActive(false);
        goDefeat.SetActive(false);
    }

    public void GameClear()
    {
        StartCoroutine(GameClearProcess());
    }

    public IEnumerator GameClearProcess()
    {
        StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().HideWindow();
        GameManager.Instance.cameraController.StartCutScene(new Vector2(0f, -2f), 1.5f);

        yield return waitSec2; // 2초의 기다림
        goVictory.SetActive(true);

        yield return waitSec2; // 2초의 기다림
        StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().gameResultUI.isClear = true;
        StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().ShowWindow<GameResultUI>(isTimeOverOpen: true);
    }

    public void GameOver()
    {
        StartCoroutine(GameOverProcess());
    }

    public IEnumerator GameOverProcess()
    {
        StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().HideWindow();
        ParticleManager.Instance.SpawnParticle("Burn", Vector3.zero, Vector3.one, Quaternion.identity);
        GameManager.Instance.cameraController.StartCutScene(new Vector2(0f, -2f), 1.5f);

        yield return waitSec2; // 2초의 기다림
        goDefeat.SetActive(true);
        ParticleManager.Instance.SpawnParticle("HeroFireball_Explode", Vector3.zero + new Vector3(Random.Range(-4, 0), Random.Range(-4, 0)), Vector3.one, Quaternion.identity);

        yield return waitHalf; // 잠깐의 기다림
        ParticleManager.Instance.SpawnParticle("HeroFireball_Explode", Vector3.zero + new Vector3(Random.Range(0, 4), Random.Range(0, 4)), Vector3.one, Quaternion.identity);

        yield return waitHalf;// 잠깐의 기다림
        ParticleManager.Instance.SpawnParticle("HeroFireball_Explode", Vector3.zero + new Vector3(Random.Range(-4, 4), Random.Range(-4, 4)), Vector3.one, Quaternion.identity);

        yield return waitHalf;// 잠깐의 기다림
        StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().gameResultUI.isClear = false;
        StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().ShowWindow<GameResultUI>(isTimeOverOpen: true);
    }
}
