using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simulation : MonoBehaviour
{
    public static int curGeneration = 1;
    public static int BulletNum = 30;
    public static GameObject[] bul_objects;
    public GameObject FirePos;
    public GameObject Prefab;
    public GameObject TowerTop;

    private Population population;
    private int bul_ID;
    // private float rotTimer;
    private float nextFire;
    private float fireRate;

    private void Awake() 
    {
        bul_objects = new GameObject[BulletNum];
        bul_ID = 0;
        // rotTimer = 0.0f;
        fireRate = 1f;
        nextFire = fireRate;

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
            bul_objects[bul_ID] = Instantiate(Prefab, FirePos.transform.position, Quaternion.Euler(-10f, 0f, 0f)) as GameObject;
            // pass the path which this bullet will follow
            bul_objects[bul_ID].GetComponent<Bullet>().Fire(population.curIndividuals[bul_ID].chrom, bul_ID);

            bul_ID++;
        }
    }

    private void Evolution() 
    {
        curGeneration ++;
        
        if (curGeneration == Population.GENMAX) 
        {
            Debug.Log("ゲーム終了");
            return;
        }


        // population.alternate();

        // 現世代終了時にオブジェクトを削除
        for (int i = 0; i < BulletNum; i++) 
        {
            Destroy(bul_objects[i]);
        }
    }
}
