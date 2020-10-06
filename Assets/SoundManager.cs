using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class SoundManager : MonoBehaviour
{
    [Serializable]
    public class Sound {
        public AudioClip audioClip;
        [Range(0, 1)]
        public float volume = 1.0f;
        public AudioSource audioSource;


        public void Play() {
            audioSource.clip = audioClip;
            audioSource.volume = volume;
            audioSource.Play();
        }
    }


    public Sound[] sounds;
    
    public void Play(int index) {
        sounds[index].Play();
    }

    private void Start() {
        
    }
}

