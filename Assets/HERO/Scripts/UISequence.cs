using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISequence : MonoBehaviour
{

    private List<InputStep> currentSequence;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (currentSequence == null)
        {
            //this.gameObject.SetActive(false);
            return;
        }


    }


    public void DisplaySequence(List<InputStep> sequence)
    {

        currentSequence = sequence;
        this.gameObject.SetActive(true);
                // For testing, print the current sequence
        string sequenceStr = "Current Sequence: ";
        foreach (var step in currentSequence)
        {
            sequenceStr += step.ToString() + " ";
        }
        Debug.Log(sequenceStr);
    }
}
