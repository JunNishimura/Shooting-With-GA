using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private float prevCurGeneration;
    public Text PopSizeText;
    public Text MutateProbText;
    public Text CurGenerationText;
    public Text BestFitnessText;
    public Text SurvivedCountText;

    private void Awake()
    {
        UIUpdate();
        prevCurGeneration = Simulation.curGeneration;
    }

    private void Update() 
    {
        // an increment of curGeneration is a flag to change the UI
        if (prevCurGeneration != Simulation.curGeneration) 
        {
            UIUpdate();
            prevCurGeneration = Simulation.curGeneration;
        }
    }

    private void UIUpdate() 
    {
        PopSizeText.text       = $">> {Simulation.BulletNum}";
        MutateProbText.text    = $">> {Population.MUTATEPROB}";
        CurGenerationText.text = $">> {Simulation.curGeneration}";
        BestFitnessText.text   = $">> {Simulation.BestFitnessEver:N5}";
        SurvivedCountText.text = $">> {Simulation.SurvivedCount}";
    }
}