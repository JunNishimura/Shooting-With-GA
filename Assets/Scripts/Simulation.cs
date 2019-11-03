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
    public GameObject FitnessTextObject;

    private UIManager UIManager;
    private Population population;
    private AudioSource audioSource;
    private Animation anim;
    private int bornCount; // bornCount follows the number of bullets fired
    private int deadCount; // deadCount follows the number of bullets which stopped running
    private float nextFire;
    [SerializeField][Range(0.1f, 0.5f)] private float fireRate = 0.25f;

    //----- variables for character animation -----//
    [SerializeField][Range(0.001f, 0.3f)] private float charDisplayInterval = 0.05f; // interval to dispaly next character
    private bool isActiveUI; // make this true when displaying UI
    private bool isTextScrollUp; // FitnessTextObject has to move upward to show all texts
    private string[] sentences;
    private string wholeSentence;
    private string nowSentence; // one line sentence
    private int lastCharIndex; // index for nowSentence
    private float beginTime;
    private float displayLength;
    private int fontSize;
    private int maxLinesNumOnScreen; // maximum number of lines which the scene can display

    private void Awake() 
    {
        audioSource = this.GetComponent<AudioSource>();
        anim = this.GetComponent<Animation>();
        UIManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        nextFire = fireRate;
        sentences = new string[BulletNum];
        isActiveUI = false;

        population = new Population(Vector3.Angle(Vector3.right, FirePos.transform.right));

        if (SimulationMode == MODE.NormalSimulation) 
        {
            UIManager.SimulationUI();
        } 
        else 
        {
            fontSize = UIManager.FitnessText.fontSize;
            maxLinesNumOnScreen = (int)(Screen.height / fontSize);
        }

        // shoot the first bullet
        InitFire();
    }

    private void Update() 
    {
        if(isActiveUI) 
        {
            // scroll up when the text is full on the screen
            if (isTextScrollUp) 
            {
                // calculate how much the text should be scrolled per frame
                float scroll = (Time.deltaTime * fontSize) / displayLength; 
                // scroll *= 1.005f; // an adjustment
                FitnessTextObject.GetComponent<RectTransform>().localPosition += Vector3.up * scroll;
            }

            if (Time.time > beginTime + displayLength) 
            {
                // time to fire next bullet
                wholeSentence += nowSentence+"\n";
                UIManager.DisplayFitnessText(wholeSentence);
                isActiveUI = false;

                // time to evolve if we reach the bulletNum
                if (deadCount == BulletNum)
                {
                    Evolution();
                }
            }
            else
            {
                // here is just visiting visiting nowSentence one by one
                int nowCharIndex = (int)(Mathf.Clamp01((Time.time-beginTime) / displayLength) * nowSentence.Length);
                // if index has changed, show it
                if (nowCharIndex != lastCharIndex)
                {
                    string partialSentence = wholeSentence + nowSentence.Substring(0, nowCharIndex);
                    UIManager.DisplayFitnessText(partialSentence);
                    lastCharIndex = nowCharIndex;
                }
            }
        }
        else 
        {
            // when current bullet stops running, fire the next one
            if (population.bulletObjects[deadCount].isStopRunning) 
            {
                // calculate fitness and display on the screen
                population.curIndividuals[deadCount].fitness = population.bulletObjects[deadCount].calculateFitness();
                sentences[deadCount] = $"{deadCount+1}: {population.curIndividuals[deadCount].fitness}";
                SetNextSentence(deadCount);
                deadCount++;
                isActiveUI = true;
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
        // selectionSentences = population.alternate();
        // population
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
        FitnessTextObject.GetComponent<RectTransform>().localPosition = Vector3.zero;
        wholeSentence = $"第{curGeneration}世代\n";
        UIManager.DisplayFitnessText(wholeSentence);
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

    private void SetNextSentence(int nowSentenceNum) 
    {
        if (nowSentenceNum == sentences.Length)
        {
            // visited all sentences, so back to shooting and go to next generation
            isTextScrollUp = false;
            UIManager.DisplayFitnessText(wholeSentence);
            return;
        }
        nowSentence = sentences[nowSentenceNum];
        beginTime   = Time.time;
        displayLength = nowSentence.Length * charDisplayInterval;
        lastCharIndex = 0;

        // When the text is full with sentences, it is time to scroll up text to display the rest
        if (nowSentenceNum >= maxLinesNumOnScreen) 
        {
            isTextScrollUp = true;
        }
    }
}