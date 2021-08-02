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

public class LoopingAudio : MonoBehaviour
{
    public string Name;
    public AudioClip[] AudioTracks;
    public float Volume = 1.0f;
    public float Pitch = 1.0f;
    public bool NoLog = false;

    private AudioSource currentTrack = null;
    private int currentIndex = -1;
    private Transform myTransform;

    public void Start()
    {
        if(AudioTracks == null || AudioTracks.Length == 0)
        {
            Debug.LogWarning("No tracks found!");
            return;
        }

        myTransform = transform;
        GameObject obj = new GameObject("Looping Audio: " + Name);
        obj.transform.position = myTransform.position;
        obj.transform.parent = myTransform;
        currentTrack = obj.AddComponent<AudioSource>();
        currentTrack.volume = Volume;
        currentTrack.pitch = Pitch;

        PlayNextTrack();
    }

    public void PlayNextTrack()
    {
        AudioClip clip = ChooseNewTrack();

        if(!NoLog)
        {
            Debug.Log("Playing " + clip.name);
        }

        currentTrack.clip = clip;
        currentTrack.Play();
        Invoke("PlayNextTrack", clip.length / Pitch);
    }

    public void SetVolume(float volume)
    {
        Volume = volume;
        currentTrack.volume = volume;
    }

    public void SetPitch(float pitch)
    {
        Pitch = pitch;
        currentTrack.pitch = pitch;
        
    }

    private AudioClip ChooseNewTrack()
    {
        if(AudioTracks.Length <= 1)
        {
            return AudioTracks[0];
        }

        int newIndex;
        do
        {
            newIndex = Random.Range(0, AudioTracks.Length);
        }
        while (newIndex == currentIndex);

        return AudioTracks[newIndex];
    }
}
