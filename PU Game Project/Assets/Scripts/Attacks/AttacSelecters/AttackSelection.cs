using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MHA.BattleBehaviours;

public class AttackSelection : MonoBehaviour{

    public bool hasSentTargets;

    protected bool isTargetBased;
    protected bool isNodeBased;
    protected HeroCharacter receivedCharacter;
    public List<Node> targettedNodes = new List<Node>();
    public List<GameObject> targettedObjects = new List<GameObject>();

    //public List<BattleBehaviourModel> modelsToSendData;

    protected virtual void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            MadeSelection();
        }
    }

    protected void MadeSelection()
    {
        receivedCharacter.UnitAbilityCleanup(); //Calls NodeDisplayCleanup as well.
        /*
        if (isTargetBased)
        {
            
            TargetDataPacket newPacket = new TargetDataPacket(targettedObjects);
            foreach(BattleBehaviourModel currentModel in modelsToSendData)
            {
                currentModel.targets = newPacket;
            }
        }
        else
        {
            TargetDataPacket newPacket = new TargetDataPacket(targettedNodes);
            foreach (BattleBehaviourModel currentModel in modelsToSendData)
            {
                currentModel.targets = newPacket;
            }
        }
        */
    }

    public virtual void NodeDisplayCleanup()
    {
        foreach (Node currentNode in targettedNodes)
        {
            currentNode.IsAttackable = false;
        }

        targettedNodes.Clear();
    }



    public class TargetDataPacket
    {
        public bool isTargetBased;
        public bool isNodeBased;
        public List<Node> targetNodes = new List<Node>();
        public List<GameObject> targetObjects = new List<GameObject>();

        public TargetDataPacket(List<Node> givenNodes)
        {
            foreach(Node currentNode in givenNodes)
            {
                targetNodes.Add(currentNode);
            }

            isNodeBased = true;
        }

        public TargetDataPacket(List<GameObject> givenObjects)
        {
            foreach (GameObject currentObject in givenObjects)
            {
                targetObjects.Add(currentObject);
            }

            isTargetBased = true;
        }
    }

}
