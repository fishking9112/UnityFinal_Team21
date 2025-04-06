using UnityEngine;

public class PoolTest : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            ObjectPoolManager.Instance.GetObject(0, new Vector3(0, 0, 0), Quaternion.identity);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            ObjectPoolManager.Instance.GetObject(1, new Vector3(0, 0, 0), Quaternion.identity);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            ObjectPoolManager.Instance.GetObject(2, new Vector3(0, 0, 0), Quaternion.identity);
        }
    }
}
