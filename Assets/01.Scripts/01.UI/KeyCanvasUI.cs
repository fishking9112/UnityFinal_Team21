using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyCanvasUI : MonoBehaviour
{
    private float destroyDuration = 60f;     // 없어지는 시간 60초

    private float curTime = 0f;

    private void Update()
    {
        curTime += Time.deltaTime;

        if (curTime > destroyDuration)
        {
            Destroy(this.gameObject);
        }
    }
}
