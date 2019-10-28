using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Individual
{
    public Vector3[] chrom { get; private set; }
    public float fitness { get; private set; }

    public Individual()
    {
        chrom = new Vector3[Population.LIFESPAN];
        for (int i = 0; i < Population.LIFESPAN; i++) 
        {
            // 球座標
            // 半径1の球体上の1点をとる。(objectが向いている方向の半分の球体上から)
            // 参照: https://ja.wikipedia.org/wiki/%E7%90%83%E9%9D%A2%E5%BA%A7%E6%A8%99%E7%B3%BB
            float theta = Random.Range(0f, Mathf.PI/2);
            float phi = Random.Range(0, Mathf.PI);
            chrom[i] = new Vector3(Mathf.Sin(theta) * Mathf.Cos(phi),
                                Mathf.Sin(theta) * Mathf.Sin(phi),
                                Mathf.Cos(theta));
            fitness = 0.0f;
        }
    }

    public void calcFitness() 
    {
        // fitness = Vector3.Distance();

        // if (isReachedTarget && LifeSpan - nowLife > 0)  
        // {
        //     fitness /= nowLife;
        // }
    }

    public void Crossover(Individual p1, Individual p2) 
    {
        OnePointCrossover(p1, p2);
    }

    private void OnePointCrossover(Individual p1, Individual p2) 
    {
        int point = Random.Range(0, Simulation.BulletNum-1);
        for (int i = 0; i <= point; i++)
        {
            this.chrom[i] = p1.chrom[i];
        }
        for (int i = point+1; i < Simulation.BulletNum; i++) 
        {
            this.chrom[i] = p2.chrom[i];
        }
    }

    public void Mutate() 
    {
        for (int i = 0; i < Simulation.BulletNum; i++) 
        {
            if (Random.Range(0.0f, 1.0f) < Population.MUTATEPROB) 
            {
                // 球座標
                // 半径1の球体上の1点をとる。(objectが向いている方向の半分の球体上から)
                float theta = Random.Range(0f, Mathf.PI);
                float phi = Random.Range(0f, Mathf.PI);
                chrom[i] = new Vector3(Mathf.Sin(theta) * Mathf.Cos(phi),
                                    Mathf.Sin(theta) * Mathf.Sin(phi),
                                    Mathf.Cos(theta));
            }
        }
    }
}
