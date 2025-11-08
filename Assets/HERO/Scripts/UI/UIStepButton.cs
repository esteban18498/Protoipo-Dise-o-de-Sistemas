using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStepButton : MonoBehaviour
{

    public InputStep? stepType; //{ ATK1, ATK2, UP, DOWN, LEFT, RIGHT }
    public Texture atk1;
    public Texture atk2;
    public Texture up;
    public Texture down;
    public Texture left;
    public Texture right;

    public Texture badEntry;

    public void SetStepType(InputStep step)
    {

        this.gameObject.SetActive(true);
        stepType = step;
        switch (step)
        {
            case InputStep.ATK1:
                GetComponent<RawImage>().texture = atk1;
                break;
            case InputStep.ATK2:
                GetComponent<RawImage>().texture = atk2;
                break;
            case InputStep.UP:
                GetComponent<RawImage>().texture = up;
                break;
            case InputStep.DOWN:
                GetComponent<RawImage>().texture = down;
                break;
            case InputStep.LEFT:
                GetComponent<RawImage>().texture = left;
                break;
            case InputStep.RIGHT:
                GetComponent<RawImage>().texture = right;
                break;
            default:
                GetComponent<RawImage>().texture = badEntry;
                break;
        }
    }

    public void SetPreview()
    {
        GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0.5f);
    }

    public void SetGood()
    {
        GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
    }
    
        public void SetBad()
    {
        GetComponent<RawImage>().texture = badEntry;
        GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
    }

}
