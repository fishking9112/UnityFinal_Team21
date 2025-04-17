using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroManager : MonoSingleton<HeroManager>
{
    public Dictionary<GameObject, HeroController> hero = new Dictionary<GameObject, HeroController>();


    // Start is called before the first frame update
    void Start()
    {
        
    }

}
