using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour {
    public AudioClip Intro;
    public AudioClip Loop;
    private AudioSource audioSource;

    // Start is called before the first frame update
    IEnumerator Start() {
        audioSource = GetComponent<AudioSource>();

        audioSource.clip = Intro;
        audioSource.loop = false;
        audioSource.Play();
        yield return new WaitForSecondsRealtime(Intro.length);
        audioSource.clip = Loop;
        audioSource.loop = true;
        audioSource.Play();
    }
}
