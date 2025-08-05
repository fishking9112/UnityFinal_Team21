using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointController : MonoBehaviour
{
    [SerializeField] private List<Rect> spawnAreas; // 적을 생성할 영역 리스트
    [SerializeField] private Color gizmoColor = new Color(1, 0, 0, 0.3f); // 기즈모 색상

    private List<SpriteRenderer> visualizers = new List<SpriteRenderer>();


    private void Start()
    {
        CreateVisualizers();
    }


    private void CreateVisualizers()
    {
        foreach (var area in spawnAreas)
        {
            SpriteRenderer sr = Instantiate(SpawnPointManager.Instance.areaVisualizerPrefab, transform);
            sr.gameObject.transform.position = new Vector3(area.x + area.width / 2, area.y + area.height / 2, 0);
            sr.gameObject.transform.localScale = new Vector3(area.width, area.height, 1);
            sr.color = gizmoColor;
            sr.enabled = false; // 처음엔 비활성화

            visualizers.Add(sr);
        }
    }

    public Vector2 GetRandomPosition(bool isCenter = false)
    {
        // 랜덤한 영역 선택
        Rect randomArea = spawnAreas[Random.Range(0, spawnAreas.Count)];

        if (isCenter)
        {
            // Rect 영역 내부의 랜덤 위치 계산
            Vector2 centerPosition = new Vector2(
                randomArea.x + randomArea.width / 2,
                randomArea.y + randomArea.height / 2
            );
            return centerPosition;
        }

        // Rect 영역 내부의 랜덤 위치 계산
        Vector2 randomPosition = new Vector2(
            Random.Range(randomArea.xMin, randomArea.xMax),
            Random.Range(randomArea.yMin, randomArea.yMax)
        );

        return randomPosition;
    }

    // 기즈모를 그려 영역을 시각화 (선택된 경우에만 표시)
    private void OnDrawGizmosSelected()
    {
        if (spawnAreas == null) return;

        Gizmos.color = gizmoColor;
        foreach (var area in spawnAreas)
        {
            Vector3 center = new Vector3(area.x + area.width / 2, area.y + area.height / 2);
            Vector3 size = new Vector3(area.width, area.height);
            Gizmos.DrawCube(center, size);
        }
    }
    public bool IsAreaIn(Vector2 areaPoint)
    {
        foreach (var area in spawnAreas)
        {
            if (area.Contains(areaPoint))
                return true;
        }
        return false;
    }
    public void ShowAndHideAreas()
    {
        StopAllCoroutines(); // 중복 방지
        StartCoroutine(ShowAndFadeCoroutine());
    }

    // 1초보여주고 2초간 천천히 감추기
    private IEnumerator ShowAndFadeCoroutine()
    {
        foreach (var sr in visualizers)
        {
            sr.color = gizmoColor;
            sr.enabled = true;
        }

        yield return new WaitForSeconds(1f);

        float fadeDuration = 1f;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(gizmoColor.a, 0f, elapsed / fadeDuration);

            foreach (var sr in visualizers)
            {
                Color c = sr.color;
                sr.color = new Color(c.r, c.g, c.b, alpha);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        foreach (var sr in visualizers)
        {
            sr.enabled = false;
        }
    }
}
