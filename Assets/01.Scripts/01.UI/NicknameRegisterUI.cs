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

        // 닉네임 등록 완료 후 게임 흐름 재시작
        await UGSManager.Instance.StartGameFlowAsync();
    }
}
