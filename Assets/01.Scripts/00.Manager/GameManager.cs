using UnityEngine;
using UnityEngine.Windows;

public class GameManager : MonoSingleton<GameManager>
{


    private void Update()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.G))
        {
            _ = UGSManager.Instance.SaveLoad.LoadAsync();
        }
    }


    private async void OnApplicationQuit()
    {
        await UGSManager.Instance.SaveLoad.SaveAsync();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            _ = UGSManager.Instance.SaveLoad.SaveAsync();
        }
    }
}
