using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ComboInputBridge : MonoBehaviour
{
    [Header("UI Target")]
    [SerializeField] private PlayerComboUIController comboUI;

    [Header("Phase Source (CombatManager)")]
    [SerializeField] private MonoBehaviour phaseSourceBehaviour; // arrastrar CombatManager
    private ICombatPhaseSource phaseSource;

    [Header("Player Action References (Player1 or Player2)")]
    public InputActionReference directional;  // Vector2
    public InputActionReference moveAction;   // -> ActA (J / South) -> Player1.Move
    public InputActionReference attackAction; // -> ActB (K / West) -> Player1.Attack
    public InputActionReference blockAction;  // -> ActC (L / East) -> Player1.Block
    public InputActionReference extraAction;  // -> ActD (I / Y) opcional -> Player1.Utility

    [Header("Combat (para validar combo)")]
    [SerializeField] private NovaCharacterController character; // del jugador
    private CombatActionDictionary actionDict;


    [SerializeField, Range(0.1f, 0.9f)] private float pressThreshold = 0.5f;

    private Vector2 _lastDir;


    // guardamos los handlers para desuscribir correctamente
    private readonly Dictionary<InputAction, Action<InputAction.CallbackContext>> _handlers =
        new Dictionary<InputAction, Action<InputAction.CallbackContext>>();


    private void Awake()
    {
        if (!comboUI) comboUI = GetComponent<PlayerComboUIController>() ?? GetComponentInChildren<PlayerComboUIController>(true);
        phaseSource = phaseSourceBehaviour as ICombatPhaseSource;

        if (character && actionDict == null)
            actionDict = new CombatActionDictionary(character);
    }
    private bool IsOnGuard() => phaseSource == null || phaseSource.Current == CombatPhase.OnGuard;

    private void OnEnable()
    {
        // Direccional
        TryBind(directional, OnDirectionalChanged, "directional");

        // Acciones → commit
        //TryBind(moveAction, _ => Commit(ActKey.A, Combat_Action_Type.Move), "moveAction");
        //TryBind(attackAction, _ => Commit(ActKey.B, Combat_Action_Type.Attack), "attackAction");
        //TryBind(blockAction, _ => Commit(ActKey.C, Combat_Action_Type.Block), "blockAction");
        //if (extraAction) TryBind(extraAction, _ => Commit(ActKey.D, Combat_Action_Type.Utils), "extraAction");
    }

    private void OnDisable()
    {
        foreach (var kv in _handlers)
        {
            var action = kv.Key;
            var handler = kv.Value;
            if (action != null)
            {
                action.performed -= handler;
                action.canceled -= handler;
                if (action.enabled) action.Disable();
            }
        }
        _handlers.Clear();
    }

    private void TryBind(InputActionReference aref,
                         Action<InputAction.CallbackContext> callback,
                         string fieldName)
    {
        if (aref == null)
        {
            Debug.LogWarning($"[{name}] ComboInputBridge: '{fieldName}' no asignado.");
            return;
        }

        var action = aref.action;
        if (action == null)
        {
            Debug.LogError($"[{name}] ComboInputBridge: '{fieldName}.action' es null. "
                         + $"¿Arrastraste la acción correcta desde el .inputactions (p.ej. Player1/Move)?");
            return;
        }

        // Evita doble binding si OnEnable se llama más de una vez
        if (_handlers.ContainsKey(action)) return;

        _handlers[action] = callback;

        // Direccional necesita también escuchar canceled
        if (fieldName == "directional")
        {
            action.performed += callback;
            action.canceled += callback;
        }
        else
        {
            action.performed += callback;
        }

        if (!action.enabled) action.Enable();
    }
    private void Commit(ActKey actKey, Combat_Action_Type typeHint)
    {
        if (!IsOnGuard()) return;

        // 1) mapear direcciones del buffer → mods (Up/Front/Down/Back) según facing del character
        var snap = comboUI.Buffer.Snapshot();
        var modsList = new List<Combat_Action_mod>(3);
        MapDirToMods(snap.d1, modsList);
        MapDirToMods(snap.d2, modsList);
        MapDirToMods(snap.d3, modsList);

        var modsKey = new ListKey<Combat_Action_mod>(modsList);
        bool success = false;

        if (actionDict != null)
        {
            var action = actionDict.GetCombatAction(modsKey, typeHint);
            // éxito si NO es la default (Unknown)
            success = !(action is Action_Unknown);
        }
        else
        {
            // si no hay diccionario aún, consideramos ok para probar UI
            success = true;
        }

        comboUI.PressAction(actKey, success);
    }

    private void MapDirToMods(DirKey dir, List<Combat_Action_mod> outList)
    {
        if (dir == DirKey.None) return;


        bool facingRight = true;
        if (character != null)
            facingRight = character.transform.localScale.x > 0f; // x > 0 => mira a la derecha

        switch (dir)
        {
            case DirKey.Up: outList.Add(Combat_Action_mod.Up); break;
            case DirKey.Down: outList.Add(Combat_Action_mod.Down); break;
            case DirKey.Right: outList.Add(facingRight ? Combat_Action_mod.Front : Combat_Action_mod.Back); break;
            case DirKey.Left: outList.Add(facingRight ? Combat_Action_mod.Back : Combat_Action_mod.Front); break;
        }
    }


    private void OnDirectionalChanged(InputAction.CallbackContext ctx)
    {
        var v = ctx.ReadValue<Vector2>();

        if (_lastDir.x <= pressThreshold && v.x > pressThreshold) comboUI.PushDirection(DirKey.Right);
        if (_lastDir.x >= -pressThreshold && v.x < -pressThreshold) comboUI.PushDirection(DirKey.Left);
        if (_lastDir.y <= pressThreshold && v.y > pressThreshold) comboUI.PushDirection(DirKey.Up);
        if (_lastDir.y >= -pressThreshold && v.y < -pressThreshold) comboUI.PushDirection(DirKey.Down);

        _lastDir = v;
    }
}
