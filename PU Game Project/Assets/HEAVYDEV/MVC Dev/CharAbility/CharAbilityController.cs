using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharAbilityController {

    public CharAbilityModel charModel;
    public CharAbilityView charView;

    public void AbilityCast(int abilityCastIndex)
    {
        if (abilityCastIndex <= charModel.activatableBBehaviourControllers.Count)
        {
            ResolutionManager.instance.LoadBattleBehaviour(charModel.activatableBBehaviourControllers[abilityCastIndex]);
        }
    }
}
