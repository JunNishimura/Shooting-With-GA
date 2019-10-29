﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simulation : MonoBehaviour
{
    public static int curGeneration = 1;
    public static int BulletNum = 30;
    public static int SurvivedCount = 0;
    public static float BestFitnessEver = Mathf.Infinity;
    public static GameObject Target;
    public GameObject FirePos;
    public GameObject Prefab;
    public GameObject TowerTop;

    private Population population;
    private int bul_ID;
    private float nextFire;
    private float fireRate;

    private void Awake() 
    {
        bul_ID = 0;
        fireRate = 1f;
        nextFire = fireRate;
        Target = GameObject.FindWithTag("Target");

        population = new Population();
    }

    private void Update() 
    {
        if (bul_ID == BulletNum)
        {
            Evolution();
        }

        // make the tower head toward the direction to fire
        // tower rotation on y axis
        Vector3 fireDirection = population.curIndividuals[bul_ID].chrom[0];
        Quaternion towerHeading = Quaternion.LookRotation(fireDirection);
        towerHeading.x = 0f;
        towerHeading.z = 0f;
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, towerHeading, 1 - (nextFire-Time.time));
        // tower top rotation on x axis
        Quaternion towerTopHeading = Quaternion.LookRotation(fireDirection);
        towerHeading.y = 0f;
        towerHeading.z = 0f;
        TowerTop.transform.rotation = Quaternion.Lerp(TowerTop.transform.rotation, towerTopHeading, 1 - (nextFire-Time.time));

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
