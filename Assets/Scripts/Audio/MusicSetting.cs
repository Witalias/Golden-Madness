using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicSetting : MonoBehaviour
{
    [SerializeField] private Slider musicVolume;
    public static MusicSetting Instanse;
    //public delegate void VolumeChange(float x);
    //public event VolumeChange onVolumeChanged;

    public Slider MusicVolume { get => musicVolume; }

    private void Awake()
    {
        if (Instanse == null)
            Instanse = this;
        else if (Instanse == this)
            Destroy(gameObject);
        musicVolume.value = 0.2f;
    }

    private void Update()
    {
        musicVolume.gameObject.transform.position = Player.Instanse.transform.position - new Vector3(0, 1, 0);
        //gameVolume.onValueChanged.AddListener(x => onVolumeChanged?.Invoke(x));

    }
}
