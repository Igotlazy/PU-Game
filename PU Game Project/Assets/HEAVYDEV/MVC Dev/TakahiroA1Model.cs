using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakahiroA1Model {

    public CharAbilityModel takaHiroA1Model = new CharAbilityModel
    {
        
        abilityName = "Flamethower",
        abilityDescription = "Deals damage in a line",
        abilitySprite = null,

        energyCost = 2,
        turnCooldown = 1,

        bBehaviourList = new List<BattleBehaviourModel>
        {
            new BBDealDamageModel
            {
                attackToDeal = new Attack(5f, Attack.DamageType.Physical)
            }
        }
    };
}
