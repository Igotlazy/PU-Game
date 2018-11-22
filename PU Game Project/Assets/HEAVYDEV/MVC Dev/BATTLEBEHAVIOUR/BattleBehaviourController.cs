using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MHA.BattleBehaviours
{
    public abstract class BattleBehaviourController : IBattleBehaviour
    {

        public BattleBehaviourModel battleBehaviourModel;
        protected List<Action> RunBehaviourImplList = new List<Action>();
        public BattleEvent attachedBattleEvent;
        protected bool isFinished;
        protected int invokeIndex = 0;


        public BattleBehaviourController(BattleBehaviourModel givenModel)
        {
            battleBehaviourModel = givenModel;
        }


        public void RunBehaviour()
        {
            if (invokeIndex < RunBehaviourImplList.Count)
            {
                RunBehaviourImplList[invokeIndex]();
                invokeIndex++;

                if (invokeIndex >= RunBehaviourImplList.Count)
                {
                    FinishBehaviour();
                }
            }
        }

        public void QueueAnimation()
        {

        }


        public void CancelBehaviour()
        {
            if (!isFinished)
            {
                CancelBehaviourImpl();
                RemoveFromBehaviourList();
                invokeIndex = 0;
            }
        }
        protected abstract void CancelBehaviourImpl();


        public void FinishBehaviour()
        {
            isFinished = true;
            FinishBehaviourImpl();
            RemoveFromBehaviourList();
            invokeIndex = 0;
            RunAuxControllers();
        }
        protected abstract void FinishBehaviourImpl();


        public void RunAuxControllers()
        {
            foreach (BattleBehaviourController currentController in battleBehaviourModel.auxBehaviourControllers)
            {
                currentController.attachedBattleEvent = this.attachedBattleEvent;
                ResolutionManager.instance.LoadBattleBehaviour(currentController);
            }

            ResolutionManager.instance.LoadBattleBehaviour(battleBehaviourModel.auxBehaviourControllers);
        }

        public void RemoveFromBehaviourList()
        {
            if (ResolutionManager.instance.resolvingBehaviours.Contains(this))
            {
                ResolutionManager.instance.resolvingBehaviours.Remove(this);
            }
        }
    }
}



