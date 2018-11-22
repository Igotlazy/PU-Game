using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MHA.BattleBehaviours;

public class TakahiroA1Model {

    public CharAbilityModel takaHiroA1Model;
    Attack blargh = new Attack(5, Attack.DamageType.Magical);

    public TakahiroA1Model(LivingCreature givenCreature)
    {
        takaHiroA1Model = new CharAbilityModel(givenCreature)
        {

            abilityName = "Flamethower",
            abilityDescription = "Deals damage in a line.",
            abilitySprite = null,

            energyCost = 2,
            turnCooldown = 1,

            activatableBBehaviourModelList = new List<List<BattleBehaviourModel>>
            {
                new List<BattleBehaviourModel>
                {
                    new BBDealDamageModel
                    {
                        attackToDeal = blargh,

                        auxBehaviourModels = new List<BattleBehaviourModel>
                        {
                            new BBDealDamageModel
                            {
                                attackToDeal = blargh
                            }
                        }
                    },
                    new BBDealDamageModel
                    {
                        attackToDeal = blargh,

                        auxBehaviourModels = new List<BattleBehaviourModel>
                        {
                            new BBDealDamageModel
                            {
                                attackToDeal = blargh
                            }
                        }
                    }                   
                },
                new List<BattleBehaviourModel>
                {
                    new BBDealDamageModel
                    {
                        attackToDeal = blargh,

                        auxBehaviourModels = new List<BattleBehaviourModel>
                        {
                            new BBDealDamageModel
                            {
                                attackToDeal = blargh
                            }
                        }
                    },
                    new BBDealDamageModel
                    {
                        attackToDeal = blargh,
                    }
                }
            }
        };
    }    
}
