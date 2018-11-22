using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MHA.BattleBehaviours
{
    public class BBMoveObjectModel : BattleBehaviourModel
    {

        public BBMoveObjectModel()
        {
            identifierString = "MoveObject";
        }

        public bool instantiate;
        public GameObject objectToInstantiate;
        public GameObject objectToMove;
        public Vector3 finalPos;
        public float speed;

    }
}
