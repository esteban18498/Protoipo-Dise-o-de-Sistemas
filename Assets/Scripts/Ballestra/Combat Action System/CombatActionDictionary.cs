using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class CombatActionDictionary
{
    public Dictionary<ListKey<Combat_Action_mod>, ICombatAction> MovesDictionary;
    public Dictionary<ListKey<Combat_Action_mod>, ICombatAction> AttacksDictionary;
    public Dictionary<ListKey<Combat_Action_mod>, ICombatAction> BlocksDictionary;
    public Dictionary<ListKey<Combat_Action_mod>, ICombatAction> UtilsDictionary;

    public CombatActionDictionary(NovaCharacterController character)
    {
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

}

