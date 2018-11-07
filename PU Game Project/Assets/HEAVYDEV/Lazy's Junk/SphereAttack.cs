using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MHA.DebugGame
{
    public class SphereAttack : MonoBehaviour
    {
        public bool isFreeSelection;
        public List<Node> nodeList = new List<Node>();
        RaycastHit hitInfo = new RaycastHit();
        public LayerMask clickLayerMask;
        bool hit;
        Vector3 currentPosition;
        public Transform pointer;

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 100f, clickLayerMask);

            if (hit)
            {
                if (isFreeSelection)
                {
                    pointer.transform.position = new Vector3(hitInfo.point.x, 0.5f, hitInfo.point.z);
                }
                else
                {
                    currentPosition = GridGen.instance.NodeFromWorldPoint(hitInfo.point).worldPosition;
                    pointer.transform.position = new Vector3(currentPosition.x, 0.5f, currentPosition.z);
                }


                this.transform.LookAt(pointer);
            }
        }

        private void OnTriggerEnter(Collider enteringCollider)
        {
            Node enteringNode = GridGen.instance.NodeFromWorldPoint(enteringCollider.gameObject.transform.position);
            enteringNode.IsAttackable = true;
            nodeList.Add(enteringNode);
        }

        private void OnTriggerExit(Collider exitingCollider)
        {
            Node exitingNode = GridGen.instance.NodeFromWorldPoint(exitingCollider.gameObject.transform.position);
            if (nodeList.Contains(exitingNode))
            {
                exitingNode.IsAttackable = false;
                nodeList.Remove(exitingNode);
            }
        }

        public void Cleanup()
        {
            foreach(Node currentNode in nodeList)
            {
                currentNode.IsAttackable = false;
            }

            nodeList.Clear();
        }
    }
}
