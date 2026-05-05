using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button play;
    [SerializeField] private Button exit;
    [SerializeField] private SoundDataSO mainMenuSoundData;
    private void Start()
    {
        SoundManager.Instance.StopAllSound();
        SoundManager.Instance.Get().Initialize(mainMenuSoundData).Play();
        play.onClick.AddListener(OnPlay);
        exit.onClick.AddListener(() => Application.Quit());
    }
    private void OnPlay()
    {
        SoundManager.Instance.StopAllSound();
        SceneManager.LoadScene("2");
    }
}
