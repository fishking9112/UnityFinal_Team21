using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestProperty : MonoBehaviour
{
    ReactiveProperty<int> p;
    Text tmp;

    // Start is called before the first frame update
    void Start()
    {
        tmp = GetComponent<Text>();

        p=new ReactiveProperty<int>();
        p.Value= 0;
        p.AddAction(_=>TestAction(p.Value));
    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            p.Value++;
        }   
    }


    void TestAction(int v)
    {
        tmp.text = v.ToString();
    }
}
