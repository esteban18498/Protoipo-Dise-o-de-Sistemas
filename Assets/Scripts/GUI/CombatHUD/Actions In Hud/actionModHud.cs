using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;


public class actionModHud : MonoBehaviour
{

    public Combat_Action_mod? Mod;

    public bool reversed = false;

    void Start()
    {

    }
    

    public void ConfigImage()
    {
        if (Mod == null)
        {
            Debug.LogWarning("No Mod Set for Action Mod Hud Element");
            enabled = false;
            return;
        }
        switch (Mod)
        {
            case Combat_Action_mod.Up:
                // ui element rotation
                this.transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case Combat_Action_mod.Down:
                this.transform.rotation = Quaternion.Euler(0, 0, 180);
                break;
            case Combat_Action_mod.Back:
                if (!reversed)
                {
                    this.transform.rotation = Quaternion.Euler(0, 0, 90);
                }
                else
                {
                    this.transform.rotation = Quaternion.Euler(0, 0, 270);
                }
                break;
            case Combat_Action_mod.Front:
                if (reversed)
                {
                    this.transform.rotation = Quaternion.Euler(0, 0, 90);
                }
                else
                {
                    this.transform.rotation = Quaternion.Euler(0, 0, 270);
                }
                break;
        }
    }
}
