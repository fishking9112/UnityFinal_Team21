using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NicknameRegisterUI : MonoBehaviour
{
    [SerializeField] private GameObject nicknamePanel;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button confirmButton;
    [SerializeField] private GameObject showWarningPopup;
    [SerializeField] private Button closePopupButton;

    private void Start()
    {
        nicknamePanel.SetActive(false);
        showWarningPopup.SetActive(false);
        UGSManager.Instance.OnRequireNickname += Show;
        confirmButton.onClick.AddListener(OnClickConfirm);
        closePopupButton.onClick.AddListener(Show);
    }

    private void Show()
    {
        nicknamePanel.SetActive(true);
        showWarningPopup.SetActive(false);
    }
    private void ShowWarningPopup()
    {
        nicknamePanel.SetActive(false);
        showWarningPopup.SetActive(true);
    }

    public async void OnClickConfirm()
    {
        string nickname = inputField.text.Trim();

        if (!IsValidNickname(nickname))
        {
            ShowWarningPopup();
            return;
        }

        await UGSManager.Instance.Auth.SaveNicknameAsync(nickname);
        nicknamePanel.SetActive(false);

        // UGSManager에게 등록 완료 알림
        UGSManager.Instance.CompleteNicknameRegistration();
    }

    private bool IsValidNickname(string nickname)
    {
        if (string.IsNullOrWhiteSpace(nickname))
            return false;

        // 알파벳, 숫자, 한글만 허용 (특수문자 반려)
        foreach (char c in nickname)
        {
            if (!(char.IsLetterOrDigit(c) || IsKorean(c)))
                return false;
        }

        return true;
    }

    private bool IsKorean(char c)
    {
        return c >= 0xAC00 && c <= 0xD7A3;
    }
}