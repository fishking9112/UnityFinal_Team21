using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NicknameRegisterUI : MonoBehaviour
{
    [SerializeField] private GameObject nicknamePanel;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button ConfirmButton;

    private void Start()
    {
        nicknamePanel.SetActive(false);
        UGSManager.Instance.OnRequireNickname += Show;
        ConfirmButton.onClick.AddListener(OnClickConfirm);
    }

    private void Show()
    {
        nicknamePanel.SetActive(true);
    }

    public async void OnClickConfirm()
    {
        string nickname = inputField.text;
        await UGSManager.Instance.Auth.SaveNicknameAsync(nickname);
        nicknamePanel.SetActive(false);

        // UGSManager에게 등록 완료 알림
        UGSManager.Instance.CompleteNicknameRegistration();
    }
}
