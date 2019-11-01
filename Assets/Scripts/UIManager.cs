using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [Header("Simulation Scene")]
    public Text PopSizeText;
    public Text MutateProbText;
    public Text CurGenerationText;
    public Text BestFitnessText;
    public Text SurvivedCountText;

    [Header("RealWorld Scene")]
    public Text FitnessText;

    public void SimulationUI()
    {
        PopSizeText.text       = $">> {Simulation.BulletNum}";
        MutateProbText.text    = $">> {Population.MUTATEPROB}";
        CurGenerationText.text = $">> {Simulation.curGeneration}";
        BestFitnessText.text   = $">> {Simulation.BestFitnessEver:N5}";
        SurvivedCountText.text = $">> {Simulation.SurvivedCount}";
    }

    public void RealWorldSimulationUI(string sentence) 
    {
        FitnessText.text = sentence;
    }       
}