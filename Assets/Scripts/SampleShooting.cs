using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleShooting : Simulation
{
    public GameObject TowerTop;
    private SampleUI sampleUI;
    private void Awake() 
    {
        audioSource = this.GetComponent<AudioSource>();
        sampleUI = GameObject.Find("Canvas").GetComponent<SampleUI>();
        nextFire = fireRate;
    
        population = new Population(Vector3.Angle(Vector3.right, FirePos.transform.right));
    }

    private void Update() 
    {
        if (bornCount == BulletNum-1)
        {
            Evolution();
            return;
        }
        else 
        {
            TowerHeading();
            if (Time.time >= nextFire) 
            {
                Fire();
            }
        }
    }

    protected override void Evolution() 
    {
        population.alternate();

        Debug.Log("Say HI");
        
        if (++curGeneration >= Population.GENMAX) 
        {
            Debug.Log("Finish Evolution");
            Application.Quit();
        }
        sampleUI.DisplaySampleUI(BulletNum, Population.MUTATEPROB, curGeneration, BestFitnessEver, reachTargetCount);
        InitFire();
    }

    private void TowerHeading() 
    {
        // make the tower head toward the direction to fire
        // tower rotation on y axis
        Vector3 fireDirection = population.curIndividuals[bornCount].chrom[0];
        Quaternion towerHeading = Quaternion.LookRotation(fireDirection);
        towerHeading.x = 0f;
        towerHeading.z = 0f;
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, towerHeading, 1 - (nextFire-Time.time));
        // tower top rotation on x axis
        Quaternion towerTopHeading = Quaternion.LookRotation(fireDirection);
        towerHeading.y = 0f;
        towerHeading.z = 0f;
        TowerTop.transform.rotation = Quaternion.Lerp(TowerTop.transform.rotation, towerTopHeading, 1 - (nextFire-Time.time));
    }
}
