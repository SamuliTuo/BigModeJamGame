using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;

public enum audios
{
    None = 0,
    BGM_CRASH, BGM_ZONE1, BGM_URBAN, BGM_ZONE2, BGM_BOSS1, BGN_BOSS_ZONE,
    PAUSE, BUTTON_CLICK,
    WALRUS_SLIDE, WALRUS_SQUASH, WALRUS_DIE,
    MELON
}
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    [SerializeField] private GameObject audioSourceObj = null;
    [SerializeField] private float maxVolume = 0;
    [SerializeField] private float minVolume = -10;
    [SerializeField] private AudioMixer mixer = null;
    [SerializeField] private AudioMixerSnapshot snapshot_paused, snapshot_unpaused;
    [Space(20)]
    [SerializeField] private AudioSource bgm_player_1 = null;
    [SerializeField] private AudioSource bgm_player_2 = null;
    [Space(20)]
    public AudioClipPacket bgm_crash = null;
    public AudioClipPacket bgm_zone_1 = null;
    public AudioClipPacket bgm_urban = null;
    public AudioClipPacket bgm_zone_2 = null;
    public AudioClipPacket bgm_boss_1 = null;
    public AudioClipPacket bgm_boss_zone = null;
    [Space(20)]
    [SerializeField] private AudioClipPacket pauseButton = null;
    [SerializeField] private AudioClipPacket buttonClick = null;
    [SerializeField] private AudioClipPacket walrus_slide = null;
    [SerializeField] private AudioClipPacket walrus_squash = null;
    [SerializeField] private AudioClipPacket walrus_die = null;
    [SerializeField] private AudioClipPacket melons = null;

    private Dictionary<audios, AudioClipPacket> audioLibrary = new Dictionary<audios, AudioClipPacket>();
    private float volume_SFX;
    private float volume_BGM;
    
    private IObjectPool<GameObject> audioSourcePool;
    private GameObject clone;

    void Awake()
    { 
        if (instance != null && instance != this) Destroy(this);
        else instance = this;
        audioSourcePool = new ObjectPool<GameObject>(CreateAudioSource, OnTakeAudioSourceFromPool, OnReturnAudioSourceToPool);
        PopulateAudioLibrary();
    }
    void Start()
    {
        //ChangeBGM(bgm_crash.clip, 0);
    }

    public void ChangeSFXVolume(float value)
    {
        volume_SFX = Mathf.Lerp(minVolume, maxVolume, value);
    }
    public void changeBGMVolume(float value)
    {
        volume_BGM = Mathf.Lerp(minVolume, maxVolume, value);
    }

    public void PlayClip(audios clip, Vector3 position)
    {
        if (clip == audios.None)
            return;

        print(clip);
        AudioClipPacket audioClipPack;
        audioLibrary.TryGetValue(clip, out audioClipPack);
        if (audioClipPack == null)
            return;

        clone = audioSourcePool.Get();
        print(clone);
        clone.GetComponent<AudioSourceObj>().InitAudioSourceObj(audioClipPack, position);
        clone = null;
    }

    public void Pause()
    {
        bgm_player_1.Pause();
        bgm_player_2.Pause();
        snapshot_paused.TransitionTo(0);
    }
    public void UnPause()
    {
        snapshot_unpaused.TransitionTo(0);
        mixer.SetFloat("SFX", volume_SFX);
        mixer.SetFloat("BGM", volume_BGM);
        bgm_player_1.UnPause();
        bgm_player_2.UnPause();
    }
    
    private void PopulateAudioLibrary()
    {
        audioLibrary.Clear();
        audioLibrary.Add(audios.PAUSE, pauseButton);
        audioLibrary.Add(audios.BUTTON_CLICK, buttonClick);
        audioLibrary.Add(audios.WALRUS_SLIDE, walrus_slide);
        audioLibrary.Add(audios.WALRUS_SQUASH, walrus_squash);
        audioLibrary.Add(audios.WALRUS_DIE, walrus_die);
        audioLibrary.Add(audios.MELON, melons);
    }

    // ===== BGM PLAYER =====
    int currentDisctPlayer = 0; // 1 or 2
    public void ChangeBGM(AudioClipPacket bgm, float changeSpeed)
    {
        if (currentDisctPlayer == 0)
        {
            currentDisctPlayer = 1;
            bgm_player_1.clip = bgm.clip[0];
            bgm_player_1.Play();
        }
        else if (currentDisctPlayer == 1)
        {
            currentDisctPlayer = 2;
            bgm_player_2.clip = bgm.clip[0];
            bgm_player_2.volume = 0;
            bgm_player_2.Play();
            StartCoroutine(ChangeVolumeOverTime(bgm_player_1, 0, changeSpeed));
            StartCoroutine(ChangeVolumeOverTime(bgm_player_2, bgm.volume, changeSpeed));
        }
        else if (currentDisctPlayer == 2)
        {
            currentDisctPlayer = 1;
            bgm_player_1.clip = bgm.clip[0];
            StartCoroutine(ChangeVolumeOverTime(bgm_player_1, bgm.volume, changeSpeed));
            StartCoroutine(ChangeVolumeOverTime(bgm_player_2, 0, changeSpeed));
        }
    }
    public void SkipBGMTo(float time)
    {
        if (currentDisctPlayer == 1)
        {
            bgm_player_1.time = bgm_player_1.clip.length * time;
        }
    }

    IEnumerator ChangeVolumeOverTime(AudioSource source, float target, float time)
    {
        float startVol = source.volume;
        float t = 0;

        if (target == 1)
            source.volume = 0;

        while (t < time)
        {
            source.volume = Mathf.Lerp(startVol, target, t / time);
            t += Time.deltaTime;
            yield return null;
        }
        if (target == 0) 
            source.Stop();
    }


    // ===== Audiosource pools =====
    public void ClearPools()
    {
        audioSourcePool.Clear();
    }
    GameObject CreateAudioSource()
    {
        var instance = Instantiate(audioSourceObj) as GameObject;
        instance.GetComponent<AudioSourceObj>().SetPool(audioSourcePool);
        return instance;
    }
    void OnTakeAudioSourceFromPool(GameObject audi)
    {
        audi.SetActive(true);
    }
    void OnReturnAudioSourceToPool(GameObject audi)
    {
        audi.SetActive(false);
    }
}
