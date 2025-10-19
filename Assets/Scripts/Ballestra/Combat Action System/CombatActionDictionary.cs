using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class CombatActionDictionary
{
    public Dictionary<ListKey<Combat_Action_mod>, ICombatAction> MovesDictionary;
    public Dictionary<ListKey<Combat_Action_mod>, ICombatAction> AttacksDictionary;
    public Dictionary<ListKey<Combat_Action_mod>, ICombatAction> BlocksDictionary;
    public Dictionary<ListKey<Combat_Action_mod>, ICombatAction> UtilsDictionary;

    public ICombatAction defaultAction;

    public CombatActionDictionary(NovaCharacterController character)
    {
        defaultAction = new Action_Unknown(character); // default action

        //create core actions
        #region Moves
        List<ICombatAction> CoreMoves = new List<ICombatAction>()
        {
            new Action_Advance(character),
        };

        MovesDictionary = new Dictionary<ListKey<Combat_Action_mod>, ICombatAction>();

        foreach (var action in CoreMoves)
        {
            MovesDictionary[action.mods] = action;
        }
        #endregion


        #region Attacks
        List<ICombatAction> CoreAttacks = new List<ICombatAction>()
        {
            new Action_Attack(character),
            new Action_superAttack(character)
        };
        AttacksDictionary = new Dictionary<ListKey<Combat_Action_mod>, ICombatAction>();
        foreach (var action in CoreAttacks)
        {
            AttacksDictionary[action.mods] = action;
        }
        #endregion

        #region Blocks
        List<ICombatAction> CoreBlocks = new List<ICombatAction>()
        {
            new Action_Block(character),
            // add block actions here
        };
        BlocksDictionary = new Dictionary<ListKey<Combat_Action_mod>, ICombatAction>();
        foreach (var action in CoreBlocks)
        {
            BlocksDictionary[action.mods] = action;
        }
        #endregion

        #region Utils
        List<ICombatAction> CoreUtils = new List<ICombatAction>()
        {
            new Action_Step(character),
        };
        UtilsDictionary = new Dictionary<ListKey<Combat_Action_mod>, ICombatAction>();
        foreach (var action in CoreUtils)
        {
            UtilsDictionary[action.mods] = action;
        }
        #endregion


    }


    public ICombatAction GetCombatAction(ListKey<Combat_Action_mod> mods, Combat_Action_Type type)
    {
        ICombatAction action = null;

        if (type == Combat_Action_Type.Attack)
        {
            AttacksDictionary.TryGetValue(mods, out action);
        }
        else if (type == Combat_Action_Type.Block)
        {
            BlocksDictionary.TryGetValue(mods, out action);
        }
        else if (type == Combat_Action_Type.Utils)
        {
            UtilsDictionary.TryGetValue(mods, out action);
        }
        else // Move
        {
            MovesDictionary.TryGetValue(mods, out action);
        }

        if (action == null)
        {
            action = defaultAction;
        }

        return action;
    }
}

