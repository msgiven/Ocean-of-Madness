using Spine.Unity;
using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "TentacleAnimName", menuName = "Scriptable Objects/TentacleAnimName")]
public class TentacleAnimName : ScriptableObject
{
    public string animName;
    public SoundDataSO breakSoundData;
    public SoundDataSO fixSoundData;
}
