using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "SoundDataSO", menuName = "Scriptable Objects/SoundDataSO")]
public class SoundDataSO : ScriptableObject
{
    public AudioResource AudioResource;
    public AudioMixerGroup AudioMixerGroup;
    public bool IsLooping;
    public bool PlayOnAwake;
}
