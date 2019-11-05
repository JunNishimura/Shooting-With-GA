using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SampleUI : MonoBehaviour
{
    public Text PopSizeText;
    public Text MutateProbText;
    public Text CurGenerationText;
    public Text BestFitnessText;
    public Text ReachTargetCountText;

    public void DisplaySampleUI(int p_size, float m_prob, int c_gen, float b_fit, int r_cnt)
    {
        PopSizeText.text       = $">> {p_size}";
        MutateProbText.text    = $">> {m_prob}";
        CurGenerationText.text = $">> {c_gen}";
        BestFitnessText.text   = $">> {b_fit:N5}";
        ReachTargetCountText.text = $">> {r_cnt}";
    }
}
