using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public Color normalColor;
    public Color activeHitColor;
    public Color blockColor;


    public void SetNormalColor()
    {
        this.GetComponentInChildren<SpriteRenderer>().color = normalColor;
    }

    public void SetActiveHitColor()
    {
        this.GetComponentInChildren<SpriteRenderer>().color = activeHitColor;
    }

    public void SetBlockColor()
    {
        this.GetComponentInChildren<SpriteRenderer>().color = blockColor;
    }
}
