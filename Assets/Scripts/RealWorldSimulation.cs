using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealWorldSimulation : MonoBehaviour
{
    public static int curGeneration = 1;
    public static int BulletNum = 30;
    public static int SurvivedCount = 0;
    public static float BestFitnessEver = Mathf.Infinity;
    public GameObject FirePos;
    public GameObject Prefab;

    private Population population;
    private int bul_ID;
    private float nextFire;
    private float fireRate;

    private void Awake() 
    {
        bul_ID = 0;
        fireRate = 1f;
        nextFire = fireRate;

        population = new Population(Vector3.Angle(Vector3.right, FirePos.transform.right));
    }

    private void Update() 
    {
        if (bul_ID == BulletNum)
        {
            Evolution();
        }

        if (Time.time >= nextFire) 
        {
            nextFire = Time.time + fireRate;
            
            // instantiate bullet
            population.bulletObjects[bul_ID] = Instantiate(Prefab, FirePos.transform.position, Quaternion.Euler(-10f, 0f, 0f)).GetComponent<Bullet>();
            // pass the path which this bullet will follow
            population.bulletObjects[bul_ID].Fire(population.curIndividuals[bul_ID].chrom, bul_ID);

            bul_ID++;
        }
    }

    private void Evolution() 
    {
        population.alternate();
        
        if (++curGeneration >= Population.GENMAX) 
        {
            Debug.Log("Finish Evolution");
            Application.Quit();
        }
        bul_ID = 0;
    }
}
