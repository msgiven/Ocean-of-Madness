using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundEmitter : MonoBehaviour
{
    private AudioSource _audioSource;
    private Coroutine _playingCoroutine;

    // 1. Добавляем флаг паузы
    private bool _isPaused = false;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public SoundEmitter Initialize(SoundDataSO soundData)
    {
        _audioSource.resource = soundData.AudioResource;
        _audioSource.outputAudioMixerGroup = soundData.AudioMixerGroup;
        _audioSource.loop = soundData.IsLooping;
        _audioSource.playOnAwake = soundData.PlayOnAwake;

        return this;
    }

    public void Play()
    {
        // 2. Сбрасываем флаг при новом воспроизведении
        _isPaused = false;

        if (_playingCoroutine != null) { StopCoroutine(_playingCoroutine); }
        _audioSource.Play();
        _playingCoroutine = StartCoroutine(WaitForSoundToEnd());
    }

    public void Paused()
    {
        // 3. Поднимаем флаг при паузе
        _isPaused = true;
        _audioSource.Pause();
    }

    public void UnPaused()
    {
        // 4. Опускаем флаг при снятии с паузы
        _isPaused = false;
        _audioSource.UnPause();
    }

    public void Stop()
    {
        if (_playingCoroutine != null) { StopCoroutine(_playingCoroutine); }
        _playingCoroutine = null;

        _audioSource.Stop();
        SoundManager.Instance.Realise(this);
    }

    private IEnumerator WaitForSoundToEnd()
    {
        // 5. Ждем, пока звук ИГРАЕТ или пока он НА ПАУЗЕ
        yield return new WaitWhile(() => _audioSource.isPlaying || _isPaused);

        SoundManager.Instance.Realise(this);
    }
}