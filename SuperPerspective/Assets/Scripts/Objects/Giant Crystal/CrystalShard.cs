using UnityEngine;
using System.Collections;

public class CrystalShard : MonoBehaviour
{

    public float targetScale = 0.0001f;//What we should be scaling to.
    public float minimumSize = .01f;//The smallest our model can get, in contrast to the target.
    public float shrinkSpeed = 1f;//How fast the shards shrink.

    public ParticleSystem ps;//ParticleSystem to be created for each individual shard.
    private ParticleSystem myPS;//Holds our own instantiated version of the ps Particle System

    public GameObject convergencePoint;//All of the shards should eventually converge to this point.
    public float convergeAddMultiplier = .3f;//How fast the shards could converge back together.
    private float convergeSpeed = 0;

    private Rigidbody rb;
    public float velocityDamper = .2f;

    public float holdTimer = 1f;
    public bool shouldConverge = false;

    // Use this for initialization
    void Start()
    {
        transform.parent = null;
        myPS = GameObject.Instantiate<ParticleSystem>(ps);
        myPS.transform.position = transform.position;
        myPS.transform.parent = transform;

        rb=this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.localScale != new Vector3(targetScale, targetScale, targetScale))
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(targetScale, targetScale, targetScale), Time.deltaTime * shrinkSpeed);
            if (transform.localScale.x < minimumSize)
            {
                if (holdTimer > 0)
                {
                    holdTimer -= Time.deltaTime;
                }
                else
                {
                    shouldConverge = true;
                }
                if (transform.position != convergencePoint.transform.position && shouldConverge)
                {
                    transform.position = Vector3.MoveTowards(transform.position, convergencePoint.transform.position, ((convergeSpeed+ convergeAddMultiplier) * Time.deltaTime));
                }
            }
        }
        
        rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(0, 0, 0), velocityDamper);
    }
}