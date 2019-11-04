using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RealWorldUI : MonoBehaviour
{
    public Text TerminalText;
    public GameObject textObject;
    public int displayedIndex { get; private set; } // the last index of array(fitnessSentences) which was displayed

    [SerializeField][Range(0.001f, 0.3f)] private float charDisplayInterval = 0.05f; // interval to dispaly next character
    private string[] fitnessSentences;
    private string wholeSentence;
    private string nowSentence;
    private int setIndex; // the last index of array(fitnessSentences) which was set
    private float beginTime;
    private float displayLength;
    private int lastCharIndex;
    private bool isTextScrollUp;
    private bool isWriting;
    private int fontSize;
    private int maxLinesNumOnScreen; // maximum number of lines which the scene can display

    private void Awake() 
    {
        fitnessSentences = new string[Simulation.BulletNum];
        fontSize = TerminalText.fontSize;
        maxLinesNumOnScreen = (int)(Screen.height / fontSize);
    }

    private void Update() 
    {
        if (isWriting && displayedIndex < setIndex) 
        {
            // scroll up when the text is full on the screen
            if (isTextScrollUp) 
            {
                // calculate how much the text should be scrolled per frame
                float scroll = (Time.deltaTime * fontSize) / displayLength; 
                // scroll *= 1.005f; // an adjustment
                textObject.GetComponent<RectTransform>().localPosition += Vector3.up * scroll;
            }

            if (Time.time > beginTime + displayLength)
            {
                wholeSentence += nowSentence + "\n";
                DisplayTerminalText(wholeSentence);
                displayedIndex++;

                // Go on next sentence if it's already set. If not, just wait next set.
                if (displayedIndex < setIndex) 
                {
                    SetNextSentence(fitnessSentences, displayedIndex+1);
                }
                else 
                {
                    isWriting = false;
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
                    DisplayTerminalText(partialSentence);
                    lastCharIndex = nowCharIndex;
                }
            }
        }
    }

    private void DisplayTerminalText(string sentence) 
    {
        TerminalText.text = sentence;
    }

    // set fitness from Simulation.cs 
    // this is called when the bullet stops moving and calculates its fitness
    public void SetFitnessSentences(int index, string sentence) 
    {
        this.setIndex = index;
        this.fitnessSentences[setIndex] = sentence;
        // if not writing now, let's write.
        if (! isWriting)
        {
            SetNextSentence(fitnessSentences, displayedIndex+1);
        }
    }

    private void SetNextSentence(string[] sentences, int index) 
    {
        nowSentence = sentences[index];
        beginTime   = Time.time;
        displayLength = nowSentence.Length * charDisplayInterval;
        lastCharIndex = 0;
        isWriting = true;

        // When the text is full with sentences, it is time to scroll up text to display the rest
        if (displayedIndex >= maxLinesNumOnScreen) 
        {
            isTextScrollUp = true;
        }
    }

    public void InitSetting(int d_index, int s_index, string initialMessage) 
    {
        textObject.GetComponent<RectTransform>().localPosition = Vector3.zero;
        isTextScrollUp  = false;
        isWriting       = false;
        displayedIndex  = d_index;
        setIndex        = s_index;
        wholeSentence   = initialMessage;
        DisplayTerminalText(wholeSentence);
    }
}
