using UnityEngine;
using System.Collections;

public class CrystalExplosionTrigger : ActiveInteractable {

    public bool shouldDissolveShield = false;

    private Renderer dissolveRenderer;
    private Renderer wholeRenderer;
    public GameObject dissolveShield;

    public float dissolveAmount = 0;
    public float dissolveSpeed = .05f;

    [Tooltip("Set this to skip everything and activate the FairyFollow script.")]
    public bool skipAll;

    private LevelGeometry lg;

    //Used when the character decides to select "Yes."

        //Charges up a light then when it hits max, swaps from the whole Crystal to the broke Crystal, which then proceeds to explode everywhere.
        public bool shouldExplode = false;
        public Light chargeLight;
        public GameObject wholeCrystal;
        public GameObject brokeCrystal;

        //These variables control how long the light shines up before it explodes
        public float chargeAmount;
        public float chargeMax;
        public float chargeSpeed;

        public GenericDissolver[] dissArr;

    //Conversation variables
    public startTextEvent startEvent;//Used to activate the TextBoxScript that plays the conversation.
    public textBoxScript textbox;
    public bool conversationEnded;//Used to show that the player has ended the conversation

    public bool allDone = false;

    //Variables for post-deletion cleanup
    public Collider[] myColliders;

    //Used to delay explosion to match SFX
    float explosionCountdown = 0;


	// Use this for initialization
	void Start () {
        
        myColliders = GetComponentsInChildren<Collider>();
        StartSetup();
        textbox = GameObject.FindObjectOfType<textBoxScript>();
        dissolveRenderer = dissolveShield.GetComponent<Renderer>();
        wholeRenderer = wholeCrystal.GetComponent<Renderer>();
        chargeLight.intensity = 0;
        lg = GameObject.FindObjectOfType<LevelGeometry>();

        //Debug.Log("IntroCutsceneFinished = " + PlayerPrefs.GetString("IntroCutsceneFinished"));
        if (skipAll || PlayerPrefs.GetString("IntroCutsceneFinished") == "true")
        {
           InteractableSpirit spirit = brokeCrystal.GetComponentInChildren<InteractableSpirit>();
            spirit.gameObject.transform.SetParent(null);
            spirit.enabled = true;
            spirit.GetComponent<FairyFollow>().shouldFollow = true;
            spirit.gameObject.GetComponent<Collider>().enabled = false;
            spirit.enabled = false;
            gameObject.SetActive(false);
        }

	}
	
    override public void Triggered()
    {
        if (conversationEnded == false)
        {
            startEvent.eventTrigger();
        }
        
    }

	// Update is called once per frame
	void Update () {
        if (!allDone)
        {
            eventCheck();
        }
        if (explosionCountdown > 0)
        {
            explosionCountdown -= Time.deltaTime;
            if (explosionCountdown <= 0)
            {
                shouldDissolveShield = true;
            }
        }
	}

    void eventCheck()
    {
        if(conversationEnded)
        {
            GameStateManager.instance.ShiftSpecialCase();
            isInteractable = false;
            this.setNotMarkerVisibility(false);
            conversationEnded = false;
            explosionCountdown = 1.3f;
            GetComponent<AudioSource>().Play();
        }
        if (shouldDissolveShield)//Start Dissolving
        {

            //If we aren't done yet
            if (dissolveAmount <= 1)
            {
                //Keep going
                dissolveRenderer.material.SetFloat("_SliceAmount", dissolveAmount);
                dissolveAmount += dissolveSpeed;
                wholeRenderer.material.SetFloat("_DetailRange", 0);
                wholeRenderer.material.SetFloat("_EmissionAmount", 1);
            }
            else
            {
                //We're done and we don't need this thing any more.
                Destroy(dissolveShield.gameObject);
                shouldExplode = true;
            }

            //If we're done dissolving and we should start exploding, let's start exploding.
            if (shouldExplode)
            {

                if (chargeAmount < chargeMax)
                {
                    if (chargeAmount > (chargeMax / 2))
                    {
                        wholeRenderer.material.SetFloat("_DetailRange", 2);
                        wholeRenderer.material.SetFloat("_EmissionAmount", (chargeMax - chargeAmount));
                    }
                    chargeAmount += chargeSpeed;
                    chargeLight.intensity = chargeAmount;
                }
                else
                {
                    wholeCrystal.SetActive(false);
                    brokeCrystal.SetActive(true);

                    foreach (GenericDissolver i in dissArr)
                    {
                        if (i != null)
                        { 
                            i.shouldDissolveObject = true;
                            i.gameObject.transform.SetParent(null);
                        }
                    }
                    this.setNotMarkerVisibility(false);

                    for (int i = 0; i < this.myColliders.Length; i++)
                    {
                        if (!(this.myColliders[i] == null))
                        {
                            this.myColliders[i].enabled = false;
                        }
                        allDone = true;
                        //Debug.Log("All done");
                        //PlayerPrefs.SetString("IntroCutsceneFinished", "true");
                    }
                    //Is this unreachable code? What's the deal? -Nick
                    allDone = true;
                    lg.enabled = false;
                    //this.enabled = false;
                    //Destroy(this);
                }
            }
        }
    }
}
