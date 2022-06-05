using System;
using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public Sound[] sounds;

    private void Awake()
    {
        Instance = this;
        
        foreach (var s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.loop = s.loop;
        }
        
        Play("BackgroundMusic");
    }

    public void Play(string soundName)
    {
        var s = Array.Find(sounds, sound => sound.name == soundName);
        s.source.Play();
    }

    public void Volume(string soundName, float volume)
    {
        var s = Array.Find(sounds, sound => sound.name == soundName);
        s.source.volume = volume;
    }
}
