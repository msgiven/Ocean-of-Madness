using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private bool _collectionCheck = true;
    [SerializeField] private int _defaultCapacity = 10;
    [SerializeField] private int _maxPoolSize = 100;
    [SerializeField] private int _maxSoundInstance = 100;

    private IObjectPool<SoundEmitter> _soundEmitterPool;
    private List<SoundEmitter> _activeSoundEmitters;
    private Dictionary<SoundData, int> _soundCount;

    [SerializeField] private SoundEmitter _soundEmitterPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        _activeSoundEmitters = new List<SoundEmitter>();
        _soundCount = new Dictionary<SoundData, int>();

        DontDestroyOnLoad(gameObject);
    }

    private void Start() => InitializePool();

    public SoundEmitter Get() { return _soundEmitterPool.Get(); }
    public void Realise(SoundEmitter soundEmitter) { _soundEmitterPool.Release(soundEmitter); }
    public bool CanPlaySound(SoundData soundData) { return _soundCount.TryGetValue(soundData, out int count) && count < _maxSoundInstance; }

    public void StopAllSound()
    {
        var cop = new List<SoundEmitter>(_activeSoundEmitters);
        foreach (var e in cop)
        {
            e.Stop();
        }
    }

    public void PausedAllSounds()
    {
        var cop = new List<SoundEmitter>(_activeSoundEmitters);
        foreach (var e in cop)
        {
            e.Paused();
        }
    }

    public void UnPausedAllSounds()
    {
        var cop = new List<SoundEmitter>(_activeSoundEmitters);
        foreach (var e in cop)
        {
            e.UnPaused();
        }
    }

    private void InitializePool()
    {
        _soundEmitterPool = new ObjectPool<SoundEmitter>
            (
                CreateSoundEmitter,
                OnTakeSoundEmitter,
                OnReleaseSoundEmitter,
                OnDestroySoundEmitter,
                _collectionCheck,
                _defaultCapacity,
                _maxPoolSize
            );
    }

    private SoundEmitter CreateSoundEmitter()
    {
        SoundEmitter soundEmitter = Instantiate(_soundEmitterPrefab, transform);
        soundEmitter.gameObject.SetActive(false);
        return soundEmitter;
    }

    private void OnTakeSoundEmitter(SoundEmitter soundEmitter)
    {
        soundEmitter.gameObject.SetActive(true);
        _activeSoundEmitters.Add(soundEmitter);
    }

    private void OnReleaseSoundEmitter(SoundEmitter soundEmitter)
    {
        soundEmitter.gameObject.SetActive(false);
        _activeSoundEmitters.Remove(soundEmitter);
    }

    private void OnDestroySoundEmitter(SoundEmitter soundEmitter)
    {
        Destroy(soundEmitter.gameObject);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
