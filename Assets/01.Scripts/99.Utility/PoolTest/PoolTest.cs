using UnityEngine;

public class PoolTest : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            print(1);
            ObjectPoolManager.Instance.GetObject("Circle", new Vector3(0, 0, 0), Quaternion.identity);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            print(2);
            ObjectPoolManager.Instance.GetObject("Capsule", new Vector3(0, 0, 0), Quaternion.identity);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            print(3);
            ObjectPoolManager.Instance.GetObject("Hexagon Flat-Top", new Vector3(0, 0, 0), Quaternion.identity);
        }
    }
}
