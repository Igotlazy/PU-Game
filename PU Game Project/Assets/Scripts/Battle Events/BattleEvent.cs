//Defines some Event to be resolved by the EffectResolutionTree on TurnManager. Any action (Attacks, Movements, Item use etc.) is derived from this class. 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MHA.BattleBehaviours
{
    [System.Serializable]
    public class BattleEvent
    {

        public MonoBehaviour bEventMonoBehaviour;
        public List<BattleBehaviourController> behavioursToProcess = new List<BattleBehaviourController>();
        int controllerTracker = 0;

        public bool isMovementBased;

        private bool isFinished; //Defines whether the Event has completed.
        public bool IsFinished
        {
            get
            {
                return isFinished;
            }
            set
            {
                isFinished = value;
            }
        }
        private bool isDirty; //Defines whether the Event has been visited before.
        public bool IsDirty
        {
            get
            {
                return isDirty;
            }
            set
            {
                isDirty = value;
            }
        }
        private bool isPaused; //Defines whether the Event is currently paused and not running. 
        public bool IsPaused
        {
            get
            {
                return isPaused;
            }
            set
            {
                isPaused = value;
            }
        }

        //Gets reference to the TurnManager monobehavior. Need a monobehavior to call Coroutines. If either TurnManager or the GameObject that holds it is destroyed, this system won't work.
        public BattleEvent(BattleBehaviourController givenBehaviour)
        {
            behavioursToProcess.Add(givenBehaviour);

            this.bEventMonoBehaviour = ResolutionManager.instance;
        }

        public BattleEvent(List<BattleBehaviourController> givenBehaviours)
        {
            foreach (BattleBehaviourController currentController in givenBehaviours)
            {
                behavioursToProcess.Add(currentController);
            }
            this.bEventMonoBehaviour = ResolutionManager.instance;
        }


        /*
        public virtual void BattleEventRun()
        {
            IsDirty = true;
            bEventMonoBehaviour.StartCoroutine(BattleEventProceed());
        }


        public virtual void BattleEventPause()
        {
            IsPaused = true;
        }  


        public virtual void BattleEventResume()
        {
            IsPaused = false;
        }


        public virtual void BattleEventCancel()
        {

        }

        public virtual void BattleEventFinish()
        {
            IsFinished = true;
        }
        */

        public IEnumerator ALLOWINTERRUPT(float InSeconds)
        {
            if (InSeconds < 0) { InSeconds = 0; }

            if (InSeconds == 0)
            {
                yield return null;
            }
            else
            {
                yield return new WaitForSeconds(InSeconds);
            }

            if (IsPaused)
            {
                if (WaitUntilInstance == null)
                {
                    WaitUntilInstance = new WaitUntil(() => !IsPaused);
                }

                yield return WaitUntilInstance;
            }
        }
        WaitUntil WaitUntilInstance = null;
    }
}
