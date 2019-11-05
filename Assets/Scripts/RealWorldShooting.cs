using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealWorldShooting : Simulation
{
    private RealWorldUI realWorldUI;
    private Animation anim;
    private int deadCount; // deadCount follows the number of bullets which stopped running

   
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
        if (realWorldUI.displayedIndex == BulletNum-1)
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
            anim.Play("GunAnimation");
            Fire();
        }
    }

    protected override void Evolution()
    {
        if (! realWorldUI.isSelectionMode) 
        {
            string[] sentences = population.alternate();
            realWorldUI.SetSelectionSentences(sentences);
        }
        else 
        {
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
    }

    // initial fire is special because there are some required settings.
    protected override void InitFire() 
    {
        realWorldUI.InitSetting(-1, -1, $"Genration {Simulation.curGeneration}\n");
        deadCount = 0;
        bornCount = 0;
        anim.Play("GunAnimation");
        Fire();
    }
}