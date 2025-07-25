using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class SoundSlider : MonoBehaviour
{
    [SerializeField] private string sliderName;
    [SerializeField] private string exposedParameter;
    [SerializeField] private AudioMixerGroup mixer;

    private TextMeshProUGUI sliderText;
    private Slider slider;
    private float volume;
    private string saveKey => $"volume_{exposedParameter}";
    
    private void Awake()
    {
        slider = GetComponentInChildren<Slider>();
        sliderText = GetComponentInChildren<TextMeshProUGUI>();
    }
    
    private void Start()
    {
        slider.onValueChanged.AddListener(HandleVolumeChanged);
        volume = PlayerPrefs.GetFloat(saveKey, 50f);
        slider.value = volume;

        if (sliderText != null)
            sliderText.text = sliderName;
        
        HandleVolumeChanged(volume);
    }
    
    private void HandleVolumeChanged(float value)
    {
        volume = value;

        if (mixer != null && mixer.audioMixer != null)
        {
            mixer.audioMixer.SetFloat(exposedParameter, NormalizedValueToDb(volume));
        }
    }

    private float NormalizedValueToDb(float vol)
    {
        if (vol <= 0.0001f)
            return -80f;

        return Mathf.Log10(vol) * 20f;
    }
    
    private void OnDisable()
    {
        PlayerPrefs.SetFloat(saveKey, volume);
    }
}