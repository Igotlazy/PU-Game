using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharAbilityController {

    public CharAbilityModel associatedModel;
    public List<BattleBehaviourController> charBehaviourControllers = new List<BattleBehaviourController>();

    public void AbilityCast()
    {
        BattleEvent battleEventToSend = new BattleEvent(charBehaviourControllers);
        foreach(BattleBehaviourController currentControl in charBehaviourControllers)
        {
            currentControl.attachedBattleEvent = battleEventToSend;
        }

        TurnManager.instance.EventResolutionReceiver(battleEventToSend);
    }
}
