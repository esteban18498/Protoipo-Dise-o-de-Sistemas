using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;



public enum Combat_state {
 free_move = 0,
 freez = 1,
 perfom = 2
}

public class CombatManager : MonoBehaviour
{
    static public CombatManager Instance;


    public float freezDuration = 3.0f;
    public float perfomDuration = 3.0f;


    public Image time_bar;
    public float timer_start = 0.0f;

    public void Awake()
    {
        Instance = this;
    }


    public Combat_state state = Combat_state.free_move;
    [SerializeField] private TextMeshProUGUI state_text;


    [SerializeField] private NovaCharacterController Character1; // left
    [SerializeField] private NovaCharacterController Character2; // right

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case Combat_state.free_move:
                state_text.text = "free move";
                time_bar.fillAmount = 0.0f;
                break;
            case Combat_state.freez:
                state_text.text = "freez";
                time_bar.fillAmount = 1-(Time.time - timer_start) / freezDuration;
                break;
            case Combat_state.perfom:
                state_text.text = "performing";
                time_bar.fillAmount = 1-(Time.time -timer_start) / perfomDuration;
                break;
        }
    }

    public void EnterFreezState()
    {
        if (state != Combat_state.free_move) return;

        state = Combat_state.freez;

        Character1.EnterFreezState();
        Character2.EnterFreezState();

        StartCoroutine(EndFreez());
        timer_start = Time.time;
    }

    public IEnumerator EndFreez()
    {
        yield return new WaitForSeconds(freezDuration);
        state = Combat_state.perfom;

        Character1.EnterPerformState();
        Character2.EnterPerformState();


        StartCoroutine(EndPerform());
        timer_start = Time.time;
    }

    public IEnumerator EndPerform()
    {
        yield return new WaitForSeconds(perfomDuration);
        state = Combat_state.free_move;

        Character1.EnterFreeMoveState();
        Character2.EnterFreeMoveState();



    }





}
