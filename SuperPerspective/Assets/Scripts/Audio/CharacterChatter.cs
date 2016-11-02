using UnityEngine;
using System.Collections;

public class CharacterChatter : MonoBehaviour {

    public AudioSource chatterSFX;
    float volume, pitch, volVariation = 0.01f, pitchVariation = 0.1f;

    //float timer, maxTimer = 0.1f;

	// Use this for initialization
	void Start () {
        chatterSFX = GetComponent<AudioSource>();
        if (chatterSFX != null)
        {
            volume = chatterSFX.volume;
            pitch = chatterSFX.pitch;
        }
        //timer = maxTimer;
    }
	
	// Update is called once per frame
	void Update () {
        /*if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            Chatter();
            timer = maxTimer;
        }*/
	}

    public void Chatter()
    {
        chatterSFX.volume = volume + Random.Range(-volVariation, volVariation);
        chatterSFX.pitch = pitch + Random.Range(-pitchVariation, pitchVariation);
        chatterSFX.Play();
    }

}
