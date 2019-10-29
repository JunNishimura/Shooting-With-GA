using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population
{
    public static int LIFESPAN = 120;
    public static int GENMAX = 100; // maximum number of generation
    public static float MUTATEPROB = 0.2f; // probabilty which the mutation happens
    public Bullet[] bulletObjects;
    public Individual[] curIndividuals;
    private Individual[] nextIndividuals;
    private float[] trFit; // traversed fitness. 
    private float totalTrFitness;
    private int elite = 0; 

    public Population()
    {
        bulletObjects   = new Bullet[Simulation.BulletNum];
        curIndividuals  = new Individual[Simulation.BulletNum];
        nextIndividuals = new Individual[Simulation.BulletNum];
        trFit = new float[Simulation.BulletNum];

        for (int i = 0; i < Simulation.BulletNum; i++)
        {
            curIndividuals[i] = new Individual();
            curIndividuals.CopyTo(nextIndividuals, 0);
        }
    }

    // alternate generation
    public void alternate() 
    {
        // evaluate the previous generation
        Evaluate();

        elite = 2; // minimum number of elites is 2
        Simulation.SurvivedCount = 0;
        for (int i = 0; i < Simulation.BulletNum; i++) 
        {
            if (bulletObjects[i].isReachedTarget) Simulation.SurvivedCount++;
        }
        elite = Mathf.Max(elite+Simulation.SurvivedCount, 5); // maximum number of elites is 5
        
        // keep elites for the next generation
        for (int i = 0; i < elite; i++)
        {
            for (int j = 0; j < LIFESPAN; j++)
            {
                nextIndividuals[i].chrom[j] = curIndividuals[i].chrom[j];
            }
        }

        // calculate traversed fitness for each object
        totalTrFitness = 0;
        float worstFitness = curIndividuals[Simulation.BulletNum-1].fitness; // after sorting, worst fitness is stored at the tail of array
        for (int i = 0; i < Simulation.BulletNum; i++) 
        {
            // add 0.001f to avoid the case which trFit[i] is 0 
            // leave a bit possibility being chosen even for the worst fitness object
            trFit[i] = (worstFitness - curIndividuals[i].fitness + 0.001f) / worstFitness;
            trFit[i] = Mathf.Pow(trFit[i], 4.0f); // this helps that better fitness is more likely being picked by selection
            totalTrFitness += trFit[i];
        }

        // crossover
        for (int i = elite; i < Simulation.BulletNum; i++) 
        {
            // here we use roulette selection
            int parent = rouletteSelection();
            // have some variation for selecting parents for the crossover
            int r = Random.Range(0, 3);
            switch (r)
            {
                case 0:
                    nextIndividuals[i].Crossover(curIndividuals[i], curIndividuals[parent]);
                    break;
                case 1:
                    nextIndividuals[i].Crossover(curIndividuals[parent], curIndividuals[i]);
                    break;
                default:
                    int anotherParent = rouletteSelection();
                    nextIndividuals[i].Crossover(curIndividuals[parent], curIndividuals[anotherParent]);
                    break;
            }
        }

        // mutation
        for (int i = Simulation.SurvivedCount; i < Simulation.BulletNum; i++) 
        {
            nextIndividuals[i].Mutate();
        }

        // bring the next generation to the current generation
        Individual[] tmp = new Individual[Simulation.BulletNum];
        curIndividuals.CopyTo(tmp, 0);
        nextIndividuals.CopyTo(curIndividuals, 0);
        tmp.CopyTo(nextIndividuals, 0);

        // destroy all bullet objects
        for (int i = 0; i < Simulation.BulletNum; i++) 
        {
            bulletObjects[i].DestroyMyself();
        }
    }

    // roulette selection
    private int rouletteSelection() 
    {
        float rand = Random.Range(0.0f, 1.0f);
        int rank = 1;
        for (; rank <= Simulation.BulletNum; rank++) 
        {
            float prob = trFit[rank-1] / totalTrFitness;
            if (rand <= prob) break;
            rand -= prob;
        }
        return rank-1;
    }

    // ranking selection based on the probability
    private int rankingSelection() 
    {
        int rn = Simulation.BulletNum;
        int denom = rn * (rn + 1) / 2;
        float r = Random.Range(0.0f, 1.0f);

        int rank = 1;
        for (; rank <= rn; rank++) 
        {
            // The higher the rank is, the more it's likely picked as parent
            float prob = (float)(rn - (rank-1)) / denom;
            if (r <= prob) break;
            r -= prob;
        }
        return rank-1;
    }

    // Evaluate the trial for the previous generation
    private void Evaluate()
    {
        for (int i = 0; i < Simulation.BulletNum; i++) 
        {
            curIndividuals[i].fitness = bulletObjects[i].calculateFitness();
        }
        // sort curIndividuals based on the fitness calculated above
        quickSort(0, Simulation.BulletNum-1);
        float currentBestFitness = this.curIndividuals[0].fitness;
        if (currentBestFitness < Simulation.BestFitnessEver)
        {
            Simulation.BestFitnessEver = currentBestFitness;
        }
        Debug.Log($"Generation {Simulation.curGeneration}: best fitness is {currentBestFitness}");
    }

    // quick sort
    private void quickSort(int lb, int ub) 
    {
        if (lb < ub) 
        {
            float pivot = curIndividuals[(int)((lb + ub)/2)].fitness;
            int i = lb;
            int j = ub;
            while (i <= j) 
            {
                while (this.curIndividuals[i].fitness < pivot) 
                {
                    i++;
                }
                while (this.curIndividuals[j].fitness > pivot) 
                {
                    j--;
                }
                // swap
                if (i <= j) 
                {
                    var tmp = curIndividuals[i];
                    curIndividuals[i] = curIndividuals[j];
                    curIndividuals[j] = tmp;
                    i++;
                    j--;
                }
            }
            quickSort(lb, j);
            quickSort(i, ub);
        }
    }
}
