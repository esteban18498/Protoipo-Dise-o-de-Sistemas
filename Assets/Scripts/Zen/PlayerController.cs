using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using ZenUtils;
using ZenCommand;
using ZenCommand.commands;


public class PlayerController_to_deprecrate : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer model;
    private Stats stats;

    //ui
    [SerializeField]
    private Slider slider;

    //state Machine
    public state currentState;

    //movement
    private Vector2 move;
    public float moveSpeed = 10f;
    public float guardSlowdown = 0.25f;

    //Combo
    public List<ComboMod> preComboModChain;
    public List<Combo> comboChain;
    public float performingTime;

    //ZenCommand
    Guard_Class Guard;

    //-------------------------------------------------Gameobject Functions------------------------------------//

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        model = GetComponentInChildren<SpriteRenderer>();
    }

    void Start()
    {
        stats = new Stats();
        stats.maxHealth = 10;
        stats.health = stats.maxHealth;
        stats.maxStamina = 3;
        stats.stamina = stats.maxStamina;

        currentState = state.Idle;

        move = new Vector2(0, 0);

        comboChain = new List<Combo>();
        preComboModChain = new List<ComboMod>();

        Guard = new Guard_Class();

        performingTime = Time.time;

        slider.maxValue = stats.maxStamina;

    }

    void Update()
    {
        rb.velocity = move * moveSpeed;

        switch (currentState)
        {
            case state.Idle:
                if (stats.stamina < stats.maxStamina)
                {
                    stats.stamina += Time.deltaTime;
                }
                break;
            case state.OnGuard:

                rb.velocity *= guardSlowdown;
                stats.stamina -= Time.deltaTime;

                if (stats.stamina <= 0)
                {
                    LowGuard();
                    comboChain = new List<Combo>();
                    preComboModChain = new List<ComboMod>();
                }
                break;
            case state.Performing:
                if (performingTime < Time.time)
                {
                    if (comboChain.Count != 0)
                    {
                        PerformCombo(comboChain[0]);
                        comboChain.RemoveAt(0);
                    }
                    else
                    {
                        currentState = state.Idle;
                    }
                }
                break;
            case state.Vulnerable:
                break;
        }
        slider.value = stats.stamina;
    }

    //--------------------------------------------------Class state transitions-----------------------------------------//


    //NaN-onGuard//
    private void RaiseGuard()
    {
        model.color = Color.red;
        currentState = state.OnGuard;

    }

    //onGuard-Idle//
    private void LowGuard()
    {
        model.color = Color.white;
        currentState = state.Idle;
        Guard.Low();
    }

    //onGuard-Performing//
    private void PerformGuard(){
        Guard.Execute_Commands();
        Guard.Low();
    }

    private void PerformComboChain()
    {
        if (currentState == state.Idle || currentState == state.OnGuard)
        {
            string combostring = "";
            foreach (Combo combo in comboChain)
            {
                string modstring = "";
                foreach (ComboMod mod in combo.mods)
                {
                    modstring += " " + mod.type + '+';
                }
                combostring += " / " + modstring + combo.type + " / ";
            }
            Debug.Log(combostring);
            currentState = state.Performing;
        }
    }
    private void PerformCombo(Combo combo)
    {
        string modstring = "";
        foreach (ComboMod mod in combo.mods)
        {
            modstring += " " + mod.type + '+';
        }
        Debug.Log(modstring + combo.type);
        performingTime = Time.time + 1;
    }

    //--------------------------------------------------Input System-------------------------------------------//

    public void Fire(float input)
    {
        if (currentState == state.Idle && input == 1)
        {
            RaiseGuard();
        }
        if (currentState == state.OnGuard && input < 0.6f)
        { // 0.6f = trigger sensitivity
            PerformGuard();
            LowGuard();
            PerformComboChain();
        }
    }


    public void Move(Vector2 input)
    {
        move = input;
    }
    public void Combo(Combo input)
    {
        input.mods = preComboModChain;
        preComboModChain = new List<ComboMod>();
        if (currentState == state.OnGuard)
        {
            comboChain.Add(input);
        }

    }
    public void ModCombo(ComboMod input)
    {
        if (currentState == state.OnGuard)
        {
            preComboModChain.Add(input);
        }

    }

    public void atack1(){

       Guard.commands.Add(new Hi());



    }
}

