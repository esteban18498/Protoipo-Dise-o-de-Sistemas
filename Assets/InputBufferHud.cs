using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputBufferHud : MonoBehaviour
{
    public Player player;



    [Header("internal References")]
    [SerializeField] private actionTypeHud actionTypeHudElement;
    [SerializeField] private GameObject arrowModsContainer;
    [SerializeField] private actionModHud arrowModprefab;


    void Start()
    {
        if (player == null)
        {
            this.enabled = false;
            return;
        }

        actionTypeHudElement.gameObject.SetActive(false);


        player.OnModsUpdated += UpdateModsQueue;
    }

    public void UpdateModsQueue()
    {
        if (arrowModsContainer != null && arrowModprefab != null)
        {
            foreach (Transform child in arrowModsContainer.transform)
            {
                if (child == arrowModprefab.transform) continue;
                Destroy(child.gameObject);
            }

            int counter = 0;

            //list reverse mods

            List<Combat_Action_mod> reversed = new List<Combat_Action_mod>(player.Mods.ToList());
            reversed.Reverse();



            foreach (Combat_Action_mod mod in reversed)
            {
                counter++;
                actionModHud modHudInstance = Instantiate(arrowModprefab, arrowModsContainer.transform);
                modHudInstance.Mod = mod;
                modHudInstance.ConfigImage();
                modHudInstance.setOpacity(1 - counter * 0.1f);
                modHudInstance.gameObject.SetActive(true);

            }
        }
    }

}
