using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip shoot;
    public AudioClip hit;

    public void Play(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
