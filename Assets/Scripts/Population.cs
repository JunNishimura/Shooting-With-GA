using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population
{
    public static int LIFESPAN = 120;
    public static int GENMAX = 100; // 世代数
    public static float MUTATEPROB = 0.2f; // 突然変異率
    public Bullet[] bulletObjects;
    public Individual[] curIndividuals;
    private Individual[] nextIndividuals;
    private float[] trFit;
    private int elite = 0; 
    private int superElite = 0; // elite which reached the target at last game

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
    // public void alternate() 
    // {
    //     // evaluate the previous generation
    //     Evaluate();

    //     // 前世代にTargetに到達した数を現世代のエリート数とする
    //     elite = 2;
    //     superElite = 0;
    //     for (int i = 0; i < Simulation.BulletNum; i++) 
    //     {
    //         if (curIndividuals[i].isReachedTarget) superElite++;
    //     }
    //     elite = Mathf.Max(elite+superElite, 5); // elite上限5
        
    //     // エリートは無条件に保存する
    //     for (int i = 0; i < elite; i++)
    //     {
    //         for (int j = 0; j < LIFESPAN; j++)
    //         {
    //             nextIndividuals[i].chrom[j] = curIndividuals[i].chrom[j];
    //         }
    //     }

    //     float totalFitness = 0;
    //     float worstFitness = curIndividuals[Simulation.BulletNum-1].fitness; // ソート後、配列最後尾に最悪適応度が格納されている
    //     for (int i = 0; i < Simulation.BulletNum; i++) 
    //     {
    //         trFit[i] = (worstFitness - curIndividuals[i].fitness + 0.001f) / worstFitness;
    //         trFit[i] = Mathf.Pow(trFit[i], 4.0f);
    //         totalFitness += trFit[i];
    //     }

    //     // 交叉
    //     for (int i = elite; i < Simulation.BulletNum; i++) 
    //     {
    //         int parent = rouletteSelection(totalFitness);
    //         int r = Random.Range(0, 3);
    //         if (r == 0) 
    //         {
    //             nextIndividuals[i].prevFitness = trFit[i] * trFit[parent];
    //             nextIndividuals[i].Crossover(curIndividuals[i], curIndividuals[parent]);
    //         }
    //         else if (r == 1)
    //         {
    //             nextIndividuals[i].prevFitness = trFit[i] * trFit[parent];
    //             nextIndividuals[i].Crossover(curIndividuals[parent], curIndividuals[i]);
    //         }
    //         else 
    //         {
    //             int anotherParent = rouletteSelection(totalFitness);
    //             nextIndividuals[i].prevFitness = trFit[anotherParent] * trFit[parent];
    //             nextIndividuals[i].Crossover(curIndividuals[parent], curIndividuals[anotherParent]);
    //         }
    //     }

    //     // 突然変異
    //     for (int i = superElite; i < Simulation.BulletNum; i++) 
    //     {
    //         nextIndividuals[i].Mutate();
    //     }

    //     // 次世代に受け継ぐ
    //     Individual[] tmp = new Individual[Simulation.BulletNum];
    //     curIndividuals.CopyTo(tmp, 0);
    //     nextIndividuals.CopyTo(curIndividuals, 0);
    //     tmp.CopyTo(nextIndividuals, 0);
    // }

    // ルーレット選択
    private int rouletteSelection(float totalFitness) 
    {
        float rand = Random.Range(0.0f, 1.0f);
        int rank = 1;
        for (; rank <= Simulation.BulletNum; rank++) 
        {
            float prob = trFit[rank-1] / totalFitness;
            if (rand <= prob) break;
            rand -= prob;
        }
        return rank-1;
    }

    // 確率に基づくランキング選択
    private int rankingSelection() 
    {
        int rn = Simulation.BulletNum;
        int denom = rn * (rn + 1) / 2;
        float r = Random.Range(0.0f, 1.0f);

        int rank = 1;
        for (; rank <= rn; rank++) 
        {
            // 個体群は適応度に基づいて昇順にソートされているから、ランキングが高い（小さい）方が選ばれやすい
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
        Debug.Log($"Generation {Simulation.curGeneration-1}: best fitness is {this.curIndividuals[0].fitness}");
    }

    // クイックソート
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
