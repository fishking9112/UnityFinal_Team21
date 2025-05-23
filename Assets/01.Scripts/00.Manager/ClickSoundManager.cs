using UnityEngine;

public class ClickSoundManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SoundManager.Instance.PlaySFX("SFX_UI_Click_Organic_Metallic_Thin_Select_1");
        }
    }
}
