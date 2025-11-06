using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : NovaCharacterController
{


    new void Awake()
    {
        if (CurrentSpot == null)
        {
            throw new System.Exception("CurrentSpot is not set. Please assign a SpotController in the inspector.");
        }
        else
        {
            anchor.position = CurrentSpot.transform.position;
            arena = CurrentSpot.GetComponentInParent<ArenaController>();
        }

        if (RigCharacter == null)
        {
            throw new System.Exception("rigCharacter is not set. Please assign a RigCharacter in the inspector.");
        }
        else
        {
            RigCharacter.speed = speed;
        }

        actionQueue = new CombatActionQueue(this);
        CombatActionDictionary = new CombatActionDictionary(this);
    }
    public void Start()
    {

        StartCoroutine(Step_Corrutine());
    }


    IEnumerator Step_Corrutine()
    {
        actionQueue.EnqueueAction(new Action_Step(this));

        yield return new WaitForSeconds(1);
        actionQueue.ExecuteNextAction();

        StartCoroutine(Step_Corrutine());
    }

}
