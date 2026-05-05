using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ChangeAtmosphere : MonoBehaviour
{

    // для воды
    [SerializeField] private Renderer waterRenderer;
    private Color32 DeepNormal = new Color32(12, 104, 192, 255);
    private Color32 DeepMadness = new Color32(12, 192, 73, 255);
    int DeepID = Shader.PropertyToID("Color_7D9A58EC");
    Material WaterMat;

    // для тумана
    [SerializeField] Color32 ColorFogNormal = new Color32(106, 145, 219, 255);
    [SerializeField] Color32 ColorFogMadness = new Color32(148, 114, 225, 255);
    [SerializeField] float DensityFogNormal = 0.02f;
    [SerializeField] float DensityFogMadness = 0.025f;

    // источники света 3д
    [Header("3d lights")]
    [SerializeField] private Light dirLight;
    [SerializeField] Light[] nightLamps;


    [Header("3d light maps")]
    [SerializeField] private Texture2D[] nightColorMaps; // Lightmap-*_light.exr
    [SerializeField] private Texture2D[] nightDirMaps; // Lightmap-*_dir.png

    // свет для фазы безумия 3д
    [SerializeField] private Color32 DirColorMadness = new Color32(110, 83, 233, 255);
    [SerializeField] private float DirIntensityMadness = 0.5f;
    [SerializeField] private float DirIndirectMadness = 5f;
    [SerializeField] private Vector3 DirRotationMadness = new Vector3(76.487f, -897.816f, -1325.621f);
    
    //свет для фазы разума 3д
    [SerializeField] private Color32 DirColorNormal = new Color32(255, 255, 255, 255);
    [SerializeField] private float DirIntensityNormal = 1f;
    [SerializeField] private float DirIndirectNormal = 1f;
    [SerializeField] private Vector3 DirRotationNormal = new Vector3 (63.597f, -545.525f, -725.78f);

    // global volume для затемнения вообще всего
    [SerializeField] private Volume globalVolume;
    [SerializeField] private UnityEngine.Color GlobalColorNormal = new UnityEngine.Color(1f, 1f, 1f, 1f);
    [SerializeField] private UnityEngine.Color GlobalColorMadness = new UnityEngine.Color(0.7f, 0.8f, 1f, 1f);
    private ColorAdjustments _colorAdj;

    void OnEnable()               
    {
        // вода
        WaterMat = waterRenderer.material;
        if (waterRenderer == null) {
            Debug.Log($"No renderer!");
            return;
        }
        if (WaterMat == null) {
            Debug.Log($"No material!");
            return;
        }
        waterRenderer.SetPropertyBlock(null);
        Debug.Log($"MAT: {WaterMat.name} | SHADER: {WaterMat.shader.name}");
        DumpProps(WaterMat);

        // проверка глобал вольюм на всякий
        var profile = globalVolume.profile ?? globalVolume.sharedProfile;
        if (!profile)
        {
            Debug.LogError("У Volume нет профиля.");
            return;
        }
        if (!profile.TryGet(out _colorAdj))
        {
            _colorAdj = profile.Add<ColorAdjustments>(true); // true = сразу включить все overrides
            Debug.LogWarning("В профиле не было Color Adjustments — добавили.");
        }
        _colorAdj.colorFilter.overrideState = true;

        //свет
        SetDay();

    }

    void Update()
    {

    }

    public void ToNormal()
    {
        Debug.Log($"To normal state!");
        WaterMat.SetColor(DeepID, DeepNormal);
        RenderSettings.fogColor = ColorFogNormal;
        RenderSettings.fogDensity = DensityFogNormal;
        SetDay();
    }

    public void ToMadness()
    {
        Debug.Log($"To madness state!");
        WaterMat.SetColor(DeepID, DeepMadness);
        RenderSettings.fogColor = ColorFogMadness;
        RenderSettings.fogDensity = DensityFogMadness;
        SetNight();
    }

    void SetDay()
    {
        
        dirLight.color = DirColorNormal;
        dirLight.intensity = DirIntensityNormal;
        dirLight.bounceIntensity = DirIndirectNormal;
        dirLight.transform.rotation = Quaternion.Euler(DirRotationNormal);

        // для отдельных ламп 3d
        foreach (var l in nightLamps) if (l) l.enabled = false;

        // для карт 3d
        LightmapSettings.lightmaps = System.Array.Empty<LightmapData>();

        //глобал вольюм
        _colorAdj.colorFilter.value = GlobalColorNormal;
    }

    void SetNight()
    {
        dirLight.color = DirColorMadness;
        dirLight.intensity = DirIntensityMadness;
        dirLight.bounceIntensity = DirIndirectMadness;
        dirLight.transform.rotation = Quaternion.Euler(DirRotationMadness);

        // для отдельных ламп 3d
        foreach (var l in nightLamps) if (l) l.enabled = true;
        
        // для карт 3d
        LightmapSettings.lightmapsMode = LightmapsMode.CombinedDirectional; // возможно не надо...

        int n = Mathf.Max(nightColorMaps?.Length ?? 0,
                          nightDirMaps?.Length ?? 0);

        var data = new LightmapData[n];
        for (int i = 0; i < n; i++)
        {
            var d = new LightmapData();
            if (nightColorMaps != null && i < nightColorMaps.Length) d.lightmapColor = nightColorMaps[i];
            if (nightDirMaps != null && i < nightDirMaps.Length) d.lightmapDir = nightDirMaps[i];
            data[i] = d;
        }

        LightmapSettings.lightmaps = data;

        //глобал вольюм
        _colorAdj.colorFilter.value = GlobalColorMadness;

    }

    void DumpProps(Material m)
    {
        var s = m.shader;
        for (int i = 0; i < s.GetPropertyCount(); i++)
        {
            var t = s.GetPropertyType(i);
            if (t == UnityEngine.Rendering.ShaderPropertyType.Color ||
                t == UnityEngine.Rendering.ShaderPropertyType.Vector)
            {
                Debug.Log($"Prop: {s.GetPropertyName(i)} ({t})");
            }
        }
    }
}
