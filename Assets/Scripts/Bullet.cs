using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public bool isReachedTarget { get; private set;}
    private AudioSource audioSource;
    private Rigidbody rb;
    private Vector3[] path;
    private Vector3 finalPos;
    private Vector3 acceleration;
    private Vector3 velocity;
    private GameObject explosion;
    private GameObject target;
    private int uniqueID;
    private int nowLife;
    private float speed;
    private float y_diff;
    private bool isStopRunning;

    private void Awake()
    {
        audioSource = this.GetComponent<AudioSource>();
        rb = this.GetComponent<Rigidbody>();
        path = new Vector3[Population.LIFESPAN];
        explosion = this.transform.Find("explosion").gameObject;
        explosion.SetActive(false);
        target = GameObject.FindWithTag("Target");

        speed = 0.5f;
        nowLife = 0;
        isReachedTarget = false;
        isStopRunning = false;
    }

    private void FixedUpdate()
    {
        if (isStopRunning) return;

        acceleration = path[nowLife];
        velocity += acceleration;
        rb.MovePosition(rb.position + velocity * speed * Time.fixedDeltaTime);

        if (++nowLife == Population.LIFESPAN) 
        {
            StopRunning();
        }
    }

    private void OnCollisionEnter(Collision collision) 
    {
        if (collision.gameObject.tag == "Target") 
        {
            isReachedTarget = true;
        }
        StopRunning();
    }

    private void Update() 
    {
        if (isStopRunning) return;

        y_diff = this.transform.forward.y - velocity.y;
        transform.rotation = Quaternion.LookRotation(velocity + new Vector3(0f, y_diff, 0f), this.transform.up);
    }

    private void StopRunning() 
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
        finalPos = this.transform.position;
        isStopRunning = true;
        if (! isReachedTarget)
        {
            StartCoroutine("Explode");
        }
    }

    // calculate fitness based on the distance to the target
    public float calculateFitness() 
    {
        // fitness is the distance between final position and target position
        float fitness = Vector3.Distance(finalPos, target.transform.position);

        // The faster the bullet reaches the target, the higher the fitness is
        int remainingLife = Population.LIFESPAN - nowLife;
        if (isReachedTarget && remainingLife > 0) 
        {
            fitness /= remainingLife;
        }

        return fitness;
    }

    /// <param name="path">the path which the bullet will follow</param>
    public void Fire(Vector3[] path, int _id) 
    {
        path.CopyTo(this.path, 0); // passing value, not reference
        this.uniqueID = _id;
    }

    public void DestroyMyself() 
    {
        Destroy(this.gameObject);
    }

    private IEnumerator Explode() 
    {
        explosion.SetActive(true);
        audioSource.Play();
        yield return new WaitForSeconds(1f);
        this.gameObject.SetActive(false);
    }
}
