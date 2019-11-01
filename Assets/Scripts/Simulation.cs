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
    private UIManager UIManager;
    private Population population;
    private AudioSource audioSource;
    private Animation anim;
    private int bul_ID;
    private float nextFire;
    private float fireRate;


    // variables for character animation
    private bool isActiveUI;
    [SerializeField][Range(0.001f, 0.3f)]
    private float charDisplayInterval = 0.05f;
    private string[] sentences;
    private string wholeSentence;
    private string nowSentence;
    private int nowSentenceNum;
    private int lastUpdateCharNum;
    private float beginTime;
    private float displayLength;


    private void Awake() 
    {
        audioSource = this.GetComponent<AudioSource>();
        anim = this.GetComponent<Animation>();
        UIManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        bul_ID = 0;
        fireRate = 0.3f;
        nextFire = fireRate;

        sentences = new string[Population.LIFESPAN];
        isActiveUI = false;

        population = new Population(Vector3.Angle(Vector3.right, FirePos.transform.right));

        if (SimulationMode == MODE.NormalSimulation) 
        {
            UIManager.SimulationUI();
        }
    }

    private void Update() 
    {
        if (isActiveUI) 
        {
            if (nowSentenceNum == sentences.Length) 
            {
                // visited all sentences, so back to shooting and go to next generation
                isActiveUI = false;
                return;
            }

            if (Time.time > beginTime + displayLength) 
            {
                wholeSentence += nowSentence+"\n";
                SetNextSentence();
            }
            else
            {
                int displayCharNum = (int)(Mathf.Clamp01((Time.time-beginTime) / displayLength) * nowSentence.Length);
                if (displayCharNum != lastUpdateCharNum) 
                {
                    string partialSentence = wholeSentence + nowSentence.Substring(0, displayCharNum);
                    UIManager.RealWorldSimulationUI(partialSentence);
                    lastUpdateCharNum = displayCharNum;
                }
            }
        }
        else 
        {
            if (bul_ID == BulletNum)
            {
                Evolution();
            }

            if (SimulationMode == MODE.NormalSimulation) 
            {
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
            }

            if (Time.time >= nextFire) 
            {
                nextFire = Time.time + fireRate;
                
                if (SimulationMode == MODE.RealWorldSimulation) 
                {
                    anim.Play("GunAnimation");
                }
                audioSource.Play();
                // instantiate bullet
                population.bulletObjects[bul_ID] = Instantiate(Prefab, FirePos.transform.position, Quaternion.Euler(-10f, 0f, 0f)).GetComponent<Bullet>();
                // pass the path which this bullet will follow
                population.bulletObjects[bul_ID].Fire(population.curIndividuals[bul_ID].chrom, bul_ID);

                bul_ID++;
            }
        }
    }

    private void Evolution() 
    {
        population.alternate();
        
        if (SimulationMode == MODE.RealWorldSimulation) 
        {
            Vector3[] bestChrom = population.curIndividuals[0].chrom;

            // create whole sentences which will be displayed in the scene
            for (int i = 0; i < Population.LIFESPAN; i++) 
            {
                // pass chromosome info which is fitness ranking, x, y and z as string
                sentences[i] = $"{i+1}: {bestChrom[i].x}, {bestChrom[i].y}, {bestChrom[i].z}";
            }
            wholeSentence = $"第{curGeneration}世代\n";
            nowSentenceNum = 0;
            SetNextSentence();

            isActiveUI = true;
        }

        // finish if we reach the maximum generation count
        if (++curGeneration >= Population.GENMAX) 
        {
            Debug.Log("Finish Evolution");
            Application.Quit();
        }
        bul_ID = 0;
    }

    private void SetNextSentence() 
    {
        nowSentence = sentences[nowSentenceNum];
        beginTime   = Time.time;
        displayLength  = nowSentence.Length * charDisplayInterval;
        nowSentenceNum++;
        lastUpdateCharNum = 0;
    }
}
