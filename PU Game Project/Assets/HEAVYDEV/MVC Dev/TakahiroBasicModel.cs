using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MHA.BattleBehaviours;

public class TakahiroBasicModel {

    public CharAbilityModel takaHiroBasicModel;

    public Attack blargh = new Attack(5, Attack.DamageType.Magical);
    public Vector3 dynamicTarget;
    public GameObject objectToIns = CharacterAbilityPrefabRef.instance.TakahiroPrefabs[0];
    public GameObject objectToHit;

    public TakahiroBasicModel(LivingCreature givenCreature)
    {
        takaHiroBasicModel = new CharAbilityModel(givenCreature)
        {

            abilityName = "Fire Ball",
            abilityDescription = "Deals damage to a single target.",
            abilitySprite = null,

            energyCost = 2,
            turnCooldown = 0,

            activatableBBehaviourModelList = new List<List<BattleBehaviourModel>>
            {
                new List<BattleBehaviourModel>
                {
                    new BBMoveObjectModel
                    {
                        instantiate = true,
                        speed = 5f,
                        finalPos = dynamicTarget,
                        objectToInstantiate = objectToIns,

                        auxBehaviourModels = new List<BattleBehaviourModel>
                        {
                            new BBDealDamageModel
                            {
                                targetObject = objectToHit,
                                attackToDeal = blargh
                            }
                        }
                    }
                }
            }
        };
    }
}
