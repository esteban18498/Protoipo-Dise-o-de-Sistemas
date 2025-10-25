using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;


public class actionTypeHud : MonoBehaviour
{

    public Combat_Action_Type type;

    public UnityEngine.UI.Image img;

    public Color atackColor;
    public Color blockColor;
    public Color moveColor;
    public Color utilsColor;

    void Start()

    {
        if (type == Combat_Action_Type.Attack)
        {
            img.color = atackColor;
        }
        else if (type == Combat_Action_Type.Block)
        {
            img.color = blockColor;
        }
        else if (type == Combat_Action_Type.Utils)
        {
            img.color = utilsColor;
        }
        else if (type == Combat_Action_Type.Move)
        {
            img.color = moveColor;

        }
        else if (type == null)
        {
            gameObject.SetActive(false);
            Debug.LogError("actionTypeHud: Unknown Combat_Action_Type.");
        }
    }
    

}
