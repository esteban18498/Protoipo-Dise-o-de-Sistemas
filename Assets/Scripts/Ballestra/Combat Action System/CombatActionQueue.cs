using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatActionQueue
{
    private Queue<ICombatAction> actionQueue = new Queue<ICombatAction>();

    public ICombatAction executingAction;

    private int maxQueueSize = 3;

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
        if (actionQueue.Count > 0)
        {
            executingAction = actionQueue.Dequeue();
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

    public void FillQueueWithTestActions(ICombatAction testAction)
    {
        while (actionQueue.Count < maxQueueSize)
        {
            actionQueue.Enqueue(testAction);
        }
    }

    public void FillQueueWithSteps(NovaCharacterController character)
    {
        while (actionQueue.Count < maxQueueSize)
        {
            actionQueue.Enqueue(new Action_Step(character));
        }
    }
    
}
