using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MHA.BattleBehaviours
{
    public abstract class BattleAnimation
    {
        public BattleAnimation()
        {
            LoadBattleAnimation();
        }

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
                if (animFinished == true)
                {
                    Debug.LogWarning("Anim Ping Sent");
                    ResolutionManager.instance.NextQueuedAnimation();
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
