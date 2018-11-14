using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BBGridMoveModel : BattleBehaviourModel {

    public BBGridMoveModel()
    {
        identifierString = "GridMove";
    }

    public float speed;
    public GameObject moveTarget;
    public Vector3[] path = new Vector3[] { };
}
