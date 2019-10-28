using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Individualの実体
public class Bullet : MonoBehaviour
{
    public Vector3 finalPos;

    private Vector3[] path;
    private Rigidbody rb;
    private Vector3 acceleration;
    private Vector3 velocity;
    private int uniqueID;
    private float speed;
    private float y_diff;
    private int nowLife;
    private bool isReachedTarget;
    private bool isStopRunning;

    private void Awake()
    {
        path = new Vector3[Population.LIFESPAN];
        rb = this.GetComponent<Rigidbody>();

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
        // this.gameObject.SetActive(false); // 画面から消す
    }

    public void Fire(Vector3[] _path, int _id) 
    {
        _path.CopyTo(this.path, 0); // passing value, not reference
        this.uniqueID = _id;
    }
}
