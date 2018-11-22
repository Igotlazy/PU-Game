using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MHA.BattleBehaviours
{
    public class AbilityFactory : MonoBehaviour
    {

        public static AbilityFactory instance;

        private void Awake()
        {
            instance = this;
        }


        public CharAbilityController CreateAbilityController(CharAbilityModel givenCharModel)
        {
            CharAbilityController abilityController = new CharAbilityController { charModel = givenCharModel };

            foreach (List<BattleBehaviourModel> currentActiveModelList in givenCharModel.activatableBBehaviourModelList)
            {
                abilityController.charModel.activatableBBehaviourControllers.Add(CreateBehaviourControllerList(currentActiveModelList, givenCharModel));
            }
            foreach (BBActivator currentActivator in givenCharModel.passiveActivators)
            {
                currentActivator.passiveBBehaviourControllerList = (CreateBehaviourControllerList(currentActivator.passiveBBehaviorModelList, givenCharModel));
            }

            return abilityController;
        }

        public List<BattleBehaviourController> CreateBehaviourControllerList(List<BattleBehaviourModel> givenModelList, CharAbilityModel givenCharModel)
        {
            List<BattleBehaviourController> returnList = new List<BattleBehaviourController>();

            foreach (BattleBehaviourModel currentModel in givenModelList)
            {
                BattleBehaviourController behaviourController = FindCorrectBehaviourController(currentModel, givenCharModel);
                returnList.Add(behaviourController);
            }
            return returnList;
        }

        public BattleBehaviourController FindCorrectBehaviourController(BattleBehaviourModel givenBehaviourModel, CharAbilityModel givenCharModel)
        {
            givenBehaviourModel.associatedCharAbilityModel = givenCharModel;

            foreach (BattleBehaviourModel currentAuxModeList in givenBehaviourModel.auxBehaviourModels)
            {
                givenBehaviourModel.auxBehaviourControllers.Add(FindCorrectBehaviourController(currentAuxModeList, givenCharModel));
            }
            return ControllerFinder(givenBehaviourModel);
        }

        public BattleBehaviourController FindCorrectBehaviourController(BattleBehaviourModel givenModel)
        {
            foreach (BattleBehaviourModel currentAuxModeList in givenModel.auxBehaviourModels)
            {
                givenModel.auxBehaviourControllers.Add(FindCorrectBehaviourController(currentAuxModeList));
            }
            return ControllerFinder(givenModel);
        }

        public BattleBehaviourController ControllerFinder(BattleBehaviourModel givenModel)
        {
            foreach (BattleBehaviourModel currentAuxModeListl in givenModel.auxBehaviourModels)
            {
                givenModel.auxBehaviourControllers.Add(ControllerFinder(currentAuxModeListl));
            }

            string givenName = givenModel.identifierString;

            switch (givenName)
            {
                case ("DealDamage"):
                    return new BBDealDamageController((BBDealDamageModel)givenModel);

                case ("GridMove"):
                    return new BBGridMoveController((BBGridMoveModel)givenModel);

                case ("MoveObject"):
                    return new BBMoveObjectController((BBMoveObjectModel)givenModel);

                default:
                    Debug.LogError("Ability Factory Error: Requested BehaviourModel name does not have a respective Controller");
                    return null;
            }
        }
    }
}
