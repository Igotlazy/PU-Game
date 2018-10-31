using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MHA.DebugGame
{
    public class SphereAttack : MonoBehaviour
    {

        public List<Node> nodeList = new List<Node>();

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider enteringCollider)
        {
            Node enteringNode = GridGen.instance.NodeFromWorldPoint(enteringCollider.gameObject.transform.position);
            enteringNode.IsAttackable = true;
            nodeList.Add(enteringNode);
            Debug.Log("Entered");
        }

        private void OnTriggerExit(Collider exitingCollider)
        {
            Node exitingNode = GridGen.instance.NodeFromWorldPoint(exitingCollider.gameObject.transform.position);
            if (nodeList.Contains(exitingNode))
            {
                exitingNode.IsAttackable = false;
                nodeList.Add(exitingNode);
            }
        }
    }
}
