using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientSoundManager : MonoBehaviour
{
    public static AmbientSoundManager Instance;

    public List<AudioClip> dangerNoises;
    public List<AudioClip> ambientNoises;

    public bool playingMusic = false;

    /// <summary>
    /// Play a sound with a volume and reverb
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="volume"></param>
    /// <param name="reverb"></param>
    public static void PlaySound(AudioClip clip, float volume, bool reverb = false)
    {
        Instance.PlaySoundInstanced(clip, volume, reverb);
    }

    /// <summary>
    /// Play a sound with a volume and reverb
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="volume"></param>
    /// <param name="reverb"></param>
    /// <returns></returns>
    public static bool PlayMusic(AudioClip clip, float volume, bool reverb = false)
    {
        if (Instance.playingMusic) return false;
        Instance.playingMusic = true;
        Instance.PlaySoundInstanced(clip, volume, reverb);
        TimerComponent tc = Instance.gameObject.AddComponent<TimerComponent>();
        tc.duration = clip.length;
        tc.OnCompleteAction += () =>
        {
            Instance.playingMusic = false;
        };

        return true;
    }

    /// <summary>
    /// Play a sound with a volume and reverb
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="volume"></param>
    /// <param name="reverb"></param>
    public void PlaySoundInstanced(AudioClip clip, float volume, bool reverb = false)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.dopplerLevel = 0;
        audioSource.loop = false;
        audioSource.spatialBlend = 0;
        audioSource.bypassReverbZones = reverb;
        audioSource.volume = volume;
        audioSource.Play();
        TimerComponent tc = audioSource.gameObject.AddComponent<TimerComponent>();
        tc.duration = clip.length;
        tc.OnCompleteAction += () =>
        {
            Destroy(audioSource);
        };
    }

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
