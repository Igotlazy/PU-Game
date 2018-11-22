using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MHA.BattleBehaviours
{
    public abstract class BattleAnimation
    {

        protected MonoBehaviour mono = ResolutionManager.instance;
        private bool animFinished;
        protected bool AnimFinished
        {
            get
            {
                return animFinished;
            }
            set
            {
                animFinished = value;
                if (value == true)
                {
                    ResolutionManager.instance.NextQueueAnimation();
                }
            }
        }

        protected void LoadBattleAnimation()
        {
            ResolutionManager.instance.QueueAnimation(this);
        }

        public void PlayBattleAnimation()
        {
            PlayBattleAnimationImpl();
        }
        protected abstract void PlayBattleAnimationImpl();
    }

}
