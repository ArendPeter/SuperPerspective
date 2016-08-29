using UnityEngine;
using System.Collections;

public class StepManager : MonoBehaviour {

	//suppress warnings
	#pragma warning disable 414

	//init vars

	AudioClip[] grassSteps, snowSteps, rockSteps, sandSteps, clothes, curClips;
	AudioSource source;
    public AudioSource clothesSound;

    bool sand;

    float stepTimer;

	// Use this for initialization
	void Start () {
		grassSteps = new AudioClip[4];
		grassSteps [0] = Resources.Load ("Sound/SFX/Player/Steps/Grass1")  as AudioClip;
		grassSteps [1] = Resources.Load ("Sound/SFX/Player/Steps/Grass2")  as AudioClip;
		grassSteps [2] = Resources.Load ("Sound/SFX/Player/Steps/Grass3")  as AudioClip;
		grassSteps [3] = Resources.Load ("Sound/SFX/Player/Steps/Grass4")  as AudioClip;

        snowSteps = new AudioClip[4];
        snowSteps[0] = Resources.Load("Sound/SFX/Player/Steps/Snow1") as AudioClip;
        snowSteps[1] = Resources.Load("Sound/SFX/Player/Steps/Snow2") as AudioClip;
        snowSteps[2] = Resources.Load("Sound/SFX/Player/Steps/Snow3") as AudioClip;
        snowSteps[3] = Resources.Load("Sound/SFX/Player/Steps/Snow4") as AudioClip;

        sandSteps = new AudioClip[3];
        sandSteps[0] = Resources.Load("Sound/SFX/Player/Steps/Sand1") as AudioClip;
        sandSteps[1] = Resources.Load("Sound/SFX/Player/Steps/Sand2") as AudioClip;
        sandSteps[2] = Resources.Load("Sound/SFX/Player/Steps/Sand3") as AudioClip;

        rockSteps = new AudioClip[4];
        rockSteps[0] = Resources.Load("Sound/SFX/Player/Steps/Hard1") as AudioClip;
        rockSteps[1] = Resources.Load("Sound/SFX/Player/Steps/Hard2") as AudioClip;
        rockSteps[2] = Resources.Load("Sound/SFX/Player/Steps/Hard3") as AudioClip;
        rockSteps[3] = Resources.Load("Sound/SFX/Player/Steps/Hard4") as AudioClip;

        clothes = new AudioClip[4];
        clothes[0] = Resources.Load("Sound/SFX/Player/Steps/Clothes1") as AudioClip;
        clothes[1] = Resources.Load("Sound/SFX/Player/Steps/Clothes2") as AudioClip;
        clothes[2] = Resources.Load("Sound/SFX/Player/Steps/Clothes3") as AudioClip;
        clothes[3] = Resources.Load("Sound/SFX/Player/Steps/Clothes4") as AudioClip;

        curClips = grassSteps;

        source = gameObject.GetComponent<AudioSource> ();
        clothesSound = GameObject.Find("ClothesSounds").GetComponent<AudioSource>();
        sand = false;

    }
	
	// Update is called once per frame
	void Update () {

	}

    public void updateStepType(Collider type)
    {
        //Debug.Log("Col");
        if (type.gameObject.layer == LayerMask.NameToLayer("Grass"))
        {
            curClips = grassSteps;
            sand = false;
            //Debug.Log("grass");
        }

        else if (type.gameObject.layer == LayerMask.NameToLayer("Snow"))
        {
            curClips = snowSteps;
            sand = false;
            //Debug.Log("snow");
        }

        else if (type.gameObject.layer == LayerMask.NameToLayer("Sand"))
        {
            curClips = sandSteps;
            sand = true;
            //Debug.Log("snow");
        }

        else
        {
            curClips = rockSteps;
            sand = false;
            //Debug.Log("rock");
        }
        //Debug.Log(curClips[0].name);
    }

    public void Step()
    {
        source.clip = curClips[Random.Range(0, curClips.Length)];
        //Debug.Log(source.gameObject.name);
        source.pitch = Random.Range(0.95f, 1.05f);
        if(sand)
            source.volume = 0.05f;
        else
            source.volume = 0.1f;
        source.Play();

        ClothesRustle();
    }

    void ClothesRustle()
    {
        if (clothesSound != null)
        {
            clothesSound.clip = clothes[Random.Range(0, 4)];
            //Debug.Log(clothesSound.clip.name);
            clothesSound.pitch = Random.Range(1.95f, 2.05f);
            clothesSound.volume = 0.15f;
            clothesSound.Play();
        }
    }
}
