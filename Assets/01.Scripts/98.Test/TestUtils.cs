using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUtils : MonoBehaviour
{
    string comma;
    int a = 127;
    string ts;
    int time = 18;
    // Start is called before the first frame update
    void Start()
    {
        comma = Utils.GetThousandCommaText(a);
        Utils.Log(comma);

        ts = Utils.GetMMSSTime(time);
        Utils.Log(ts);
    }

}
