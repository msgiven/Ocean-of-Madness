using UI.Components;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject popup;
    [SerializeField] Button exit;
    [SerializeField] private Button resume;
    [SerializeField] SoundDataSO backgroundSoundData;

    private SoundEmitter soundEmitter;

    private InputSystem_Actions input;

    private bool isOnPause = false;
    private void Awake()
    {
        Hide();
        
        input = new();
        input.Player.Enable();
        input.Player.Pause.performed += Show;

        exit.onClick.AddListener(OnExit);
        resume.onClick.AddListener(Hide);        
    }

    private void OnExit()
    {
        SoundManager.Instance.StopAllSound();
        SceneManager.LoadScene("MainMenuScene");
    }
    private void Hide()
    {
        SoundManager.Instance.UnPausedAllSounds();

        if (isOnPause) 
        {
            soundEmitter.Stop();
            isOnPause = false;
        }
        Time.timeScale = 1f;
        popup.SetActive(false);
    }
    private void Show(InputAction.CallbackContext context)
    {
        isOnPause = true;
        SoundManager.Instance.PausedAllSounds();
        soundEmitter = SoundManager.Instance.Get().Initialize(backgroundSoundData);
        soundEmitter.Play();
        Time.timeScale = 0f;
        popup.SetActive(true);
    }
}
