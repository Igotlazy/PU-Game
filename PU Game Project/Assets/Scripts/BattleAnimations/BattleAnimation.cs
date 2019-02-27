using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MHA.BattleAnimations
{
    public abstract class BattleAnimation
    {
        public object source;

        public BattleAnimation(object _source)
        {
            source = _source;
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
