using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISequence : MonoBehaviour
{
    public GameObject stepPrefab;

    public Transform SecuenceContainer;

    public IndicatorTarget indicatorTarget;
    private List<InputStep> currentSequence;





    // Update is called once per frame
    void Update()
    {
        if (currentSequence == null)
        {
            TurnOff();
            return;
        }


    }


    public void DisplaySequence(List<InputStep> sequence)
    {
        if (sequence == null || sequence.Count == 0)
        {
            TurnOff();
            return;
        }
        TurnOn();
        currentSequence = new List<InputStep>(sequence);
        currentSequence.Reverse();


        // Clear existing step buttons
        foreach (Transform child in SecuenceContainer)
        {
            if (child.name == "stepPref") continue;
            Destroy(child.gameObject);
        }


        int i = 1;
        // Create new step buttons
        foreach (InputStep step in currentSequence)
        {
            GameObject stepButtonObj = Instantiate(stepPrefab, SecuenceContainer);
            stepButtonObj.name = "step " + i;
            UIStepButton stepButton = stepButtonObj.GetComponent<UIStepButton>();
            stepButton.SetStepType(step);
            stepButton.SetPreview();
            i++;
        }

        //update on end of frame
        StartCoroutine(UpdateAtEndOfFrame());


    }

    public void GoodStep(int index)
    {
        index++;
        if (index - 1 < SecuenceContainer.childCount)
        {
            UIStepButton stepButton = SecuenceContainer.GetChild(SecuenceContainer.childCount - index).GetComponent<UIStepButton>();
            stepButton.SetGood();
        }
    }

    public void BadStep(int index)
    {
        index++;
        if (index - 1 < SecuenceContainer.childCount)
        {
            UIStepButton stepButton = SecuenceContainer.GetChild(SecuenceContainer.childCount - index).GetComponent<UIStepButton>();
            stepButton.SetBad();
        }
    }

    public void AdvanceSecuenceDisplay(int currentStepIndex)
    {

        currentStepIndex++;//avoid zero index, where i hold a prefab
        //Debug.Log($"UISequence: Moving indicator to step index {currentStepIndex}");
        //move indicator to the current step
        int indicatorIndex = currentStepIndex;

        if (currentStepIndex < SecuenceContainer.childCount)
        {
            indicatorTarget.Target = SecuenceContainer.GetChild(SecuenceContainer.childCount - indicatorIndex).gameObject;
            //Debug.Log($"UISequence: Indicator target set to {indicatorTarget.Target.name}");
        }
        else
        {

            indicatorTarget.gameObject.SetActive(false);
        }
    }

    public void TurnOff()
    {
        indicatorTarget.Target = null;
        this.gameObject.SetActive(false);
        indicatorTarget.gameObject.SetActive(false);
    }

    public void TurnOn()
    {
        this.gameObject.SetActive(true);
        indicatorTarget.gameObject.SetActive(true);
    }

    public IEnumerator UpdateAtEndOfFrame()
    {
        yield return new WaitForEndOfFrame();
        AdvanceSecuenceDisplay(0);
    }

    public IEnumerator FadeOutSecunceDisplay()
    {

        yield return new WaitForSeconds(1f);

        TurnOff();
    }
}
