using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityFactory : MonoBehaviour {

    public static AbilityFactory instance;

    private void Awake()
    {
        instance = this;
    }


    public CharAbilityController CreateAbilityController(CharAbilityModel givenCharModel)
    {
        CharAbilityController abilityController = new CharAbilityController {associatedModel = givenCharModel};

        foreach(BattleBehaviourModel currentBehaviourModel in givenCharModel.bBehaviourList)
        {
            abilityController.charBehaviourControllers.Add(CreateBehaviourController(currentBehaviourModel));            
        }

        return abilityController;
    }

    public BattleBehaviourController CreateBehaviourController(BattleBehaviourModel givenBehaviourModel)
    {
        BattleBehaviourController behaviourController = FindCorrectBehaviourController(givenBehaviourModel);
        return behaviourController;
    }

    private BattleBehaviourController FindCorrectBehaviourController(BattleBehaviourModel givenModel)
    {
        string givenName = givenModel.identifierString;

        switch (givenName)
        {
            case ("DealDamage"):
                return new BBDealDamageController((BBDealDamageModel) givenModel);

            case ("GridMove"):
                return new BBGridMoveController((BBGridMoveModel) givenModel);


            default:
                Debug.LogAssertion("Ability Factory Error: Requested Behaviour name does not have a respective Controller");
                return null;
        }
    }
}
