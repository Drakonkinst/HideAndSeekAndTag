/*
 * Name: Wesley Ho
 * ID: 2382489
 * Email: weho@chapman.edu
 * CPSC 236-02
 * Assignment: Final Project - Hide and Seek (and Tag)
 * This is my own work, and I did not cheat on this assignment.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    private const float defaultVolume = 1.0f;
    private const float defaultPitch = 1.0f;

    void Start()
    {
        Instance = this;
    }

    public AudioSource Play(AudioClip clip, Transform emitter)
    {
        return Play(clip, emitter, defaultVolume, defaultPitch);
    }

    public AudioSource Play(AudioClip clip, Transform emitter, float volume)
    {
        return Play(clip, emitter, volume, defaultPitch);
    }

    public AudioSource Play(AudioClip clip, Transform emitter, float volume, float pitch)
    {
        GameObject obj = new GameObject("Audio: " + clip.name);
        obj.transform.position = emitter.position;
        obj.transform.parent = emitter;
        return MakeAudioSource(obj, clip, volume, pitch);
    }

    public AudioSource Play(AudioClip clip, Vector3 point)
    {
        return Play(clip, point, defaultVolume, defaultPitch);
    }

    public AudioSource Play(AudioClip clip, Vector3 point, float volume)
    {
        return Play(clip, point, volume, defaultPitch);
    }

    public AudioSource Play(AudioClip clip, Vector3 point, float volume, float pitch)
    {
        GameObject obj = new GameObject("Audio: " + clip.name);
        obj.transform.position = point;
        return MakeAudioSource(obj, clip, volume, pitch);
    }

    private AudioSource MakeAudioSource(GameObject obj, AudioClip clip, float volume, float pitch)
    {
        AudioSource source = obj.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.Play();
        Destroy(obj, clip.length);
        return source;
    }
}