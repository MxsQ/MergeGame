using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [SerializeField] AudioClip LoseCilp;
    [SerializeField] AudioClip VictoryClip;
    [SerializeField] AudioClip BGNormalClip;
    [SerializeField] AudioClip BGFightClip;

    [SerializeField] AudioSource BGSource;
    [SerializeField] AudioSource AttackSource;
    [SerializeField] AudioSource ShootSource;
    [SerializeField] AudioSource LevelEndSource;
    [SerializeField] AudioSource EvilDieSource;

    private static AudioManager _instance;
    public static AudioManager Instance { get { return _instance; } }

    void Update()
    {
        _instance = this;
        GameManagers.OnGameFaild += () => { PlayLevelEndAudio(LoseCilp); };
        GameManagers.OnGameWin += () => { PlayLevelEndAudio(VictoryClip); };
        GameManagers.OnGameEnd += () => { PlayNormalBGAudio(); };
        GameManagers.OnGameStart += () => { PlayFightBGAudio(); };
    }

    public void PlayAttack()
    {

        if (!AttackSource.isPlaying)
        {
            AttackSource.Play();
        }

    }

    public void PlayShoot()
    {
        if (!ShootSource.isPlaying)
        {
            ShootSource.Play();
        }
    }

    private void PlayLevelEndAudio(AudioClip clip)
    {
        LevelEndSource.clip = clip;
        LevelEndSource.Play();

        if (BGSource.isPlaying)
        {
            BGSource.Pause();
        }
    }

    private void PlayFightBGAudio()
    {
        if (BGSource.isPlaying)
        {
            BGSource.Pause();
        }

        BGSource.clip = BGFightClip;
        BGSource.Play();
    }

    private void PlayNormalBGAudio()
    {
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
        EvilDieSource.Play();
    }

}
