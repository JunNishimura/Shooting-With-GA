using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Simulation : MonoBehaviour
{
    public static int curGeneration = 1;
    public static int BulletNum = 30;
    public static int reachTargetCount = 0;
    public static float BestFitnessEver = Mathf.Infinity;

    public GameObject FirePos;
    public GameObject Prefab;

    protected Population population;

    protected AudioSource audioSource;
    protected float nextFire;
    [SerializeField][Range(0.1f, 1.0f)] protected float fireRate = 0.25f;
    protected int bornCount; // bornCount follows the number of bullets fired


    protected abstract void Evolution();

    protected virtual void InitFire()
    {
        bornCount = 0;
        Fire();
    }

    protected void Fire() 
    {
        nextFire  = Time.time + fireRate;
        audioSource.Play();
        Vector3[] nowBulletGenom = population.curIndividuals[bornCount].chrom;
        // instantiate bullet
        population.bulletObjects[bornCount] = Instantiate(Prefab, FirePos.transform.position, Quaternion.Euler(-10f, 0f, 0f)).GetComponent<Bullet>();
        // pass the path which this bullet will follow
        population.bulletObjects[bornCount].Fire(nowBulletGenom, bornCount);
        ++bornCount;
    }
}