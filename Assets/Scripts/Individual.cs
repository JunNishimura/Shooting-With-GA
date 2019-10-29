using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Individual
{
    public Vector3[] chrom { get; private set; }
    public float fitness { get; set; }

    public Individual()
    {
        chrom = new Vector3[Population.LIFESPAN];
        for (int i = 0; i < Population.LIFESPAN; i++) 
        {
            // Spherical coordinate system
            // pick one point from the 1/4 size sphere (1/4 is the part which the bullet is heading)
            // reference: https://ja.wikipedia.org/wiki/%E7%90%83%E9%9D%A2%E5%BA%A7%E6%A8%99%E7%B3%BB
            // ATTENTION: In unity, y direction of vector is upward, so Mathf.Cos(theta) is y value.
            float theta = Random.Range(Mathf.PI/6, Mathf.PI/2);
            float phi = Random.Range(Mathf.PI/6, Mathf.PI-Mathf.PI/6);
            chrom[i] = new Vector3(Mathf.Sin(theta) * Mathf.Cos(phi),
                                Mathf.Cos(theta),
                                Mathf.Sin(theta) * Mathf.Sin(phi));
            fitness = 0.0f;
        }
    }

    /// <param name="p1">parent 1</param>
    /// <param name="p2">parent 2</param>
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
                float theta = Random.Range(0f, Mathf.PI/4);
                float phi = Random.Range(Mathf.PI/6, Mathf.PI-Mathf.PI/6);
                chrom[i] = new Vector3(Mathf.Sin(theta) * Mathf.Cos(phi),
                                    Mathf.Cos(theta),
                                    Mathf.Sin(theta) * Mathf.Sin(phi));
            }
        }
    }
}
