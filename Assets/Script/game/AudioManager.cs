using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] public float MasterVolumePercent = 1f;
    [SerializeField] public float MaxMusicPercent = 1;
    [SerializeField] public float SfxVolumePercent = .8f;

    [Header("War")]
    [SerializeField] AudioClip ShootClip;
    [SerializeField] AudioClip AttackClip;
    [SerializeField] AudioClip BGNormalClip;

    [Header("Clear")]
    [SerializeField] AudioClip LoseCilp;
    [SerializeField] AudioClip VictoryClip;
    [SerializeField] AudioClip GainCoinsClip;
    [SerializeField] AudioClip ButtonClickClip;
    [SerializeField] AudioSource LevelEndSource;

    [Header("BackGround")]
    [SerializeField] AudioClip BGFightClip;
    [SerializeField] AudioClip EvilDeathClip;

    [SerializeField] AudioSource BGSource;
    [SerializeField] AudioSource WheelSource;


    private static AudioManager _instance;
    public static AudioManager Instance { get { return _instance; } }

    void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        GameManagers.OnGameFaild += () => { PlayLevelEndAudio(LoseCilp); };
        GameManagers.OnGameWin += () => { PlayLevelEndAudio(VictoryClip); };
        GameManagers.OnGameEnd += () => { PlayNormalBGAudio(); };
        GameManagers.OnGameStart += () => { PlayFightBGAudio(); };
    }



    public void PlayAttack()
    {
        AudioSource.PlayClipAtPoint(AttackClip, transform.position, MasterVolumePercent * SfxVolumePercent);
    }

    public void PlayShoot()
    {
        AudioSource.PlayClipAtPoint(ShootClip, transform.position, MasterVolumePercent * SfxVolumePercent);
    }

    private void PlayLevelEndAudio(AudioClip clip)
    {
        if (LevelEndSource.isPlaying)
        {
            LevelEndSource.Pause();
        }
        LevelEndSource.clip = clip;
        LevelEndSource.Play();
        LevelEndSource.volume = MasterVolumePercent * MaxMusicPercent;

        BGSource.Stop();
    }

    public void PlayFightBGAudio()
    {
        BGSource.volume = MasterVolumePercent * MaxMusicPercent;
        if (BGSource.isPlaying)
        {
            BGSource.Pause();
        }

        BGSource.clip = BGFightClip;
        BGSource.Play();
    }

    public void PlayNormalBGAudio()
    {
        BGSource.volume = MasterVolumePercent * MaxMusicPercent;
        if (BGSource.isPlaying)
        {
            BGSource.Pause();
        }

        BGSource.clip = BGNormalClip;
        BGSource.Play();
    }

    public void PlayEvilDie()
    {
        Debug.Log("die play");
        AudioSource.PlayClipAtPoint(EvilDeathClip, transform.position, MasterVolumePercent * SfxVolumePercent);
    }

    public void PlayClick()
    {
        AudioSource.PlayClipAtPoint(ButtonClickClip, transform.position, MasterVolumePercent * SfxVolumePercent * 2f);
    }

    public void PlayGetCoins()
    {
        AudioSource.PlayClipAtPoint(GainCoinsClip, transform.position, MasterVolumePercent * SfxVolumePercent);
    }

    public void PlayWheel()
    {
        WheelSource.volume = MasterVolumePercent * MaxMusicPercent / 1.5f;
        WheelSource.Play();
    }

    public void StopWheel()
    {
        WheelSource.Stop();
    }

}
