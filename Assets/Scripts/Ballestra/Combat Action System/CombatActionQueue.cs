using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatActionQueue
{
    private Queue<ICombatAction> actionQueue = new Queue<ICombatAction>();

    public ICombatAction executingAction;

    private int maxQueueSize = 3;
    private NovaCharacterController character;
    private ISpendable CharacterResource;//stamina


    public CombatActionQueue(NovaCharacterController _character)
    {
        character = _character;
        CharacterResource = character.GetComponent<ISpendable>();
    }

    public void EnqueueAction(ICombatAction action)
    {
        if (actionQueue.Count >= maxQueueSize)
        {
            Debug.Log("Action queue is full. Cannot enqueue more actions.");
            return;
        }
        actionQueue.Enqueue(action);
    }

    public void ExecuteNextAction()
    {
        if (HasActions())
        {
            ICombatAction nextAction = actionQueue.Dequeue();

            if(CharacterResource.TrySpend(nextAction.staminaCost)) //stamina
            {
                executingAction = nextAction;
            }
            else
            {
                executingAction = new Action_Step(character);
            }

            executingAction.Execute();
        }
        else
        {
            executingAction = null;
            Debug.Log("No actions in the queue to execute.");
        }
    }

    public void ClearQueue()
    {
        actionQueue.Clear();
    }

    public bool HasActions()
    {
        return actionQueue.Count > 0;
    }

    public int GetQueueCount()
    {
        return actionQueue.Count;
    }

    public void FillQueueWithSteps()
    {
        while (actionQueue.Count < maxQueueSize)
        {
            actionQueue.Enqueue(new Action_Step(character));
        }
    }
    
}
