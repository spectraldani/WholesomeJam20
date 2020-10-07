using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace AudioKid {
    public class SoundManager : MonoBehaviour {

        protected AudioSource source;

        protected virtual bool Validate() {
            if (source != null) {
                return true;
            }
            Debug.LogError("No Audio Sources found for SoundManager!");
            return false;
        }

        protected virtual void Init() {
            source = GetComponentInChildren<AudioSource>();
            Validate();
        }

        private void Start() {
            Init();
        }
    }

    [Serializable]
    public class Sound {
        public AudioClip clip;
        [Range(0, 1)]
        public float volume = 1.0f;
        private AudioSource source;

        public void Init(AudioSource s) {
            source = s;
            if (clip != null) {
            } else {
                Debug.LogError("The clip for a Sound has not been set.");
            }
        }
        public void Play() {
            source.PlayOneShot(clip, volume);
        }
        public void Stop() {
            source.Stop();
        }
    }

    //public enum LoopPattern { None, AA, ABB, ABBC }
}


