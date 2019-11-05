using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public Text text;
    private void Awake() 
    {
        Debug.Log("Screen Height: " + Screen.height);
        Debug.Log("Font size: " + text.fontSize);
        Debug.Log("Number of rows: " + (int)(Screen.height / text.fontSize));
    }
}
