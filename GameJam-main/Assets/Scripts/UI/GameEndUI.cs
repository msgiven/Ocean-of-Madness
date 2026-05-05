using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameEndUI : MonoBehaviour
{
    [SerializeField] GameObject popup;
    [SerializeField] Button exit;
    [SerializeField] TextMeshProUGUI coinsAmount;
    [SerializeField] TextMeshProUGUI totalTime;
    [SerializeField] SoundDataSO gameEndSoundData;
    [SerializeField] SoundDataSO backgroundSoundData;

    private void Awake()
    {
        Hide();
        exit.onClick.AddListener(OnExit);        
    }
    private void Start()
    {
        ShipManager.Instance.OnGameEnd.AddListener(Show);
    }

    private void OnExit()
    {
        SoundManager.Instance.StopAllSound();
        SceneManager.LoadScene("MainMenuScene");
    }
    private void Hide()
    {
        SoundManager.Instance.StopAllSound();
        Time.timeScale = 1f;
        popup.SetActive(false);
    }
    private void Show()
    {
        SoundManager.Instance.StopAllSound();
        SoundManager.Instance.Get().Initialize(gameEndSoundData).Play();
        SoundManager.Instance.Get().Initialize(backgroundSoundData).Play();
        Time.timeScale = 0f;
        coinsAmount.text = (ShipManager.Instance.CurrentCoins).ToString();
        totalTime.text = ((int)ShipManager.Instance.TimeFromStart).ToString();
        popup.SetActive(true);
    }
}
