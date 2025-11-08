using System.Collections.Generic;
using UnityEngine;

public enum InputStep { ATK1, ATK2, UP, DOWN, LEFT, RIGHT }
public class SequenceManager : MonoBehaviour
{
    // Define the possible input steps


    // Configuration
    public int sequenceLength = 3;
    public int sequenceComplexity = 2; // Number of different input types to use
    private int inputAcuarcy = 0; 
    private List<InputStep> currentSequence = new List<InputStep>();
    private int currentStepIndex = 0;

    // References (Link these in the Inspector)
    public UISequence sequenceDisplay; // Script to update UI
    // public CombatSystem combatSystem; // Reference to transition turns

    public HERO hero;

    public void StartSequence()
    {
        // 1. Reset state
        currentSequence.Clear();
        currentStepIndex = 0;
        inputAcuarcy = 0;

        // 2. Generate a random sequence
        for (int i = 0; i < sequenceLength; i++)
        {
            // Only include I and J for simplicity first, or use a wider range
            InputStep randomStep = (InputStep)Random.Range(0, sequenceComplexity); // 2 = ATK1, ATK2
            currentSequence.Add(randomStep);
        }

        // 3. Display the sequence and start listening for input
        sequenceDisplay.DisplaySequence(currentSequence);

        /*Debug.Log($"New Sequence For {this.gameObject.name}:");
        foreach (var step in currentSequence)
        {
            Debug.Log(step.ToString());
        }*/
    }

    public void HandlePlayerInput(InputStep inputPressed)
    {
        // Check if the input matches the current required step
        if (currentSequence.Count > 0 && currentStepIndex < currentSequence.Count)
        {
            if (inputPressed == currentSequence[currentStepIndex])
            {
                // Correct Input!
                sequenceDisplay.GoodStep(currentStepIndex);
                inputAcuarcy++;
                Debug.Log("good");
            }else
            {
                sequenceDisplay.BadStep(currentStepIndex);
                Debug.Log("bad");
                // Incorrect Input
            }
                
            currentStepIndex++;
            sequenceDisplay.AdvanceSecuenceDisplay(currentStepIndex); // Highlight the next step
            if (currentStepIndex >= currentSequence.Count)
            {
                // Sequence Complete
                StartCoroutine(sequenceDisplay.FadeOutSecunceDisplay());
                TurnBasedSystem.Instance.SequenceCompleted();

                // Sequence Complete! Evaluate success
                if (inputAcuarcy > 0)
                {
                    //Debug.Log($"Sequence Success! Attack Initiated. Accuracy: {inputAcuarcy}/{currentSequence.Count}");

                    // Execute attack
                    hero.animator.SetTrigger("Attack1");
                    hero.sword.damage = hero.sword.baseDamage * inputAcuarcy/currentSequence.Count;
                        // modify damage based on inputAcuarcy
                }else
                {
                    TurnBasedSystem.Instance.SequenceFailed();
                    //Debug.Log("Sequence fail! Attack miss.");
                }
            }
        }
    }
}