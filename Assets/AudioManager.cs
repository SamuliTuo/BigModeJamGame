using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.VirtualTexturing;
using static Unity.Burst.Intrinsics.Arm;

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

    [SerializeField] private AudioMixerSnapshot snapshot_paused, snapshot_unpaused;
    [Space(20)]
    [SerializeField] private AudioSource bgm_player_1 = null;
    [SerializeField] private AudioSource bgm_player_2 = null;
    [Space(20)]
    public AudioClip bgm_crash = null;
    public AudioClip bgm_zone_1 = null;
    public AudioClip bgm_urban = null;
    public AudioClip bgm_zone_2 = null;
    public AudioClip bgm_boss_1 = null;
    public AudioClip bgm_boss_zone = null;
    [Space(20)]
    [SerializeField] private AudioSource pauseButton = null;
    [SerializeField] private AudioSource buttonClick = null;
    [SerializeField] private AudioSource walrus_slide = null;
    [SerializeField] private AudioSource walrus_squash = null;
    [SerializeField] private AudioSource walrus_die = null;
    [SerializeField] private AudioSource melon = null;
    [SerializeField] private List<AudioClip> melons = null;

    private Dictionary<audios, AudioSource> audioLibrary = new Dictionary<audios, AudioSource>();
    private float volume_SFX;
    private float volume_BGM;


    void Awake()
    { // make singleton
        if (instance != null && instance != this) Destroy(this);
        else instance = this;
    }
    void Start()
    {
        ChangeBGM(bgm_crash, 0);
        PopulateAudioLibrary();
    }

    private void Update()
    {
    }

    public void PlayClip(audios clip, Vector3 position)
    {
        print(clip);
        if (clip == audios.MELON)
        {
            PlayMelon();
        }
        else if (clip != audios.None)
        {
            AudioSource audioSource;
            audioLibrary.TryGetValue(clip, out audioSource);
            audioSource.transform.position = position;
            audioSource.Play();
        }
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
    }

    void PlayMelon()
    {
        var r = Random.Range(0, melons.Count);
        melon.clip = melons[r];
        melon.Play();
    }

    int currentDisctPlayer = 0; // 1 or 2
    public void ChangeBGM(AudioClip bgm, float changeSpeed)
    {
        print("change music");
        if (currentDisctPlayer == 0)
        {
            print("1");
            currentDisctPlayer = 1;
            bgm_player_1.clip = bgm;
            bgm_player_1.Play();
        }
        else if (currentDisctPlayer == 1)
        {
            print("2");
            currentDisctPlayer = 2;
            bgm_player_2.clip = bgm;
            bgm_player_2.volume = 0;
            bgm_player_2.Play();
            StartCoroutine(ChangeVolumeOverTime(bgm_player_1, 0, changeSpeed));
            StartCoroutine(ChangeVolumeOverTime(bgm_player_2, 1, changeSpeed));
        }
        else if (currentDisctPlayer == 2)
        {
            print("3");
            currentDisctPlayer = 1;
            bgm_player_1.clip = bgm;
            StartCoroutine(ChangeVolumeOverTime(bgm_player_1, 1, changeSpeed));
            StartCoroutine(ChangeVolumeOverTime(bgm_player_2, 0, changeSpeed));
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
}
