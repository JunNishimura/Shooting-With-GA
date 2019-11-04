using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simulation : MonoBehaviour
{
    public enum MODE 
    {
        NormalSimulation,
        RealWorldSimulation
    }
    public MODE SimulationMode;

    public static int curGeneration = 1;
    public static int BulletNum = 30;
    public static int SurvivedCount = 0;
    public static float BestFitnessEver = Mathf.Infinity;
    public GameObject FirePos;
    public GameObject Prefab;
    public GameObject TowerTop;

    private RealWorldUI realWorldUI;
    private Population population;
    private AudioSource audioSource;
    private Animation anim;
    private int bornCount; // bornCount follows the number of bullets fired
    private int deadCount; // deadCount follows the number of bullets which stopped running
    private float nextFire;
    [SerializeField][Range(0.1f, 0.5f)] private float fireRate = 0.25f;

   
    private void Awake() 
    {
        audioSource = this.GetComponent<AudioSource>();
        anim = this.GetComponent<Animation>();
        realWorldUI = GameObject.Find("Canvas").GetComponent<RealWorldUI>();
        nextFire = fireRate;

        population = new Population(Vector3.Angle(Vector3.right, FirePos.transform.right));

        // shoot the first bullet
        InitFire();
    }

    private void Update() 
    {
        if (realWorldUI.writingNum == BulletNum-1)
        {
            Evolution();
        }

        if (deadCount < BulletNum) 
        {
            // when current bullet stops running, fire the next one
            if (population.bulletObjects[deadCount].isStopRunning) 
            {
                // calculate fitness and display on the screen
                population.curIndividuals[deadCount].fitness = population.bulletObjects[deadCount].calculateFitness();
                string sentence = $"{deadCount+1}: {population.curIndividuals[deadCount].fitness}";
                realWorldUI.SetFitnessSentences(deadCount, sentence);
                deadCount++;
            }
        }
        
        if (Time.time > nextFire && bornCount < BulletNum)
        {
            nextFire  = Time.time + fireRate;
            Fire();
        }
    }

    private void Evolution() 
    {
        population.alternate();
        
        // finish if we reach the maximum generation count
        if (++curGeneration >= Population.GENMAX) 
        {
            Debug.Log("Finish Evolution");
            Application.Quit();
        }
        else 
        {
            // start next generation
            InitFire();
        }
    }

    private void TowerHeadRotate(float nextFire) 
    {
        // make the tower head toward the direction to fire
        // tower rotation on y axis
        Vector3 fireDirection = population.curIndividuals[bornCount-1].chrom[0];
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

    private void InitFire() 
    {
        realWorldUI.InitSetting();
        deadCount = 0;
        bornCount = 0;
        Fire();
    }
    private void Fire() 
    {
        ++bornCount;
        if (SimulationMode == MODE.RealWorldSimulation)
        {
            anim.Play("GunAnimation");
        }
        audioSource.Play();

        Vector3[] nowBulletGenom = population.curIndividuals[bornCount-1].chrom;
        // instantiate bullet
        population.bulletObjects[bornCount-1] = Instantiate(Prefab, FirePos.transform.position, Quaternion.Euler(-10f, 0f, 0f)).GetComponent<Bullet>();
        // pass the path which this bullet will follow
        population.bulletObjects[bornCount-1].Fire(nowBulletGenom, bornCount);
    }
}