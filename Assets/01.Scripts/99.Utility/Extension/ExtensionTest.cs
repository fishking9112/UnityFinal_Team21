using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExtensionTest : MonoBehaviour
{
    Text text;
    // Start is called before the first frame update
    void Start()
    {
        text=gameObject.GetOrAddComponent<Text>();
        Debug.Log(text);
        text.text = "테스트";
    }

}
