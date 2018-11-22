using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MHA.BattleBehaviours
{
    public interface IBattleBehaviour
    {

        void RunBehaviour();

        void CancelBehaviour();

        void FinishBehaviour();

        void RemoveFromBehaviourList();

    };
}
