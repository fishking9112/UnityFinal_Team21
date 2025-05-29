using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResultController : MonoBehaviour
{
    private WaitForSeconds waitSec1 = new WaitForSeconds(1f);
    private WaitForSeconds waitSec2 = new WaitForSeconds(2f);
    private WaitForSeconds waitHalf = new WaitForSeconds(0.25f);
    private WaitForSeconds waitHalfHalf = new WaitForSeconds(0.1f);

    public Animator goVictory;
    public Animator goDefeat;

    public void Awake()
    {
        GameManager.Instance.gameResultController = this;
        goVictory.gameObject.SetActive(false);
        goDefeat.gameObject.SetActive(false);
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
        StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().HUDGroup.SetActive(false);
        goVictory.gameObject.SetActive(true);
        goVictory.Play("Clear");

        yield return waitSec1;
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

        List<Vector3> usedPositions = new List<Vector3>();
        float minimumDistance = 1.0f; // 최소 거리 조건

        for (int i = 0; i < 10; i++)
        {
            Vector3 spawnPos = GetNonOverlappingPosition(usedPositions, -4f, 4f, minimumDistance);
            usedPositions.Add(spawnPos);

            Transform tf = ParticleManager.Instance.SpawnParticle("Burn", spawnPos, Vector3.one, Quaternion.identity).transform;
            tf.localScale = new Vector3(0.5f, 0.5f);

            yield return waitHalfHalf;
        }

        GameManager.Instance.cameraController.StartCutScene(new Vector2(0f, -2f), 1.5f);

        yield return waitSec2;


        usedPositions.Clear(); // 다시 초기화

        for (int i = 0; i < 20; i++)
        {
            int index = i;
            Vector3 spawnPos = GetNonOverlappingPosition(usedPositions, -4f, 4f, minimumDistance);
            usedPositions.Add(spawnPos);

            Transform tf = ParticleManager.Instance.SpawnParticle("HeroFireball_Explode", spawnPos, Vector3.one, Quaternion.identity).transform;
            tf.localScale = new Vector3(0.65f, 0.65f);

            if(index == 10)
            {
                StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().HUDGroup.SetActive(false);
                goDefeat.gameObject.SetActive(true);
                goDefeat.Play("Defeat");
            }

            yield return waitHalfHalf;
        }
        yield return waitSec2;

        StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().gameResultUI.isClear = false;
        StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().ShowWindow<GameResultUI>(isTimeOverOpen: true);
    }

    private Vector3 GetNonOverlappingPosition(List<Vector3> existingPositions, float min, float max, float minDistance, int maxTries = 30)
    {
        for (int i = 0; i < maxTries; i++)
        {
            Vector3 candidate = new Vector3(Random.Range(min, max), Random.Range(min, max), 0f);
            bool tooClose = false;

            foreach (var pos in existingPositions)
            {
                if (Vector3.Distance(candidate, pos) < minDistance)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose)
                return candidate;
        }

        // 실패 시 마지막 후보 반환 (최소 거리 보장 못할 수 있음)
        return new Vector3(Random.Range(min, max), Random.Range(min, max), 0f);
    }

}
