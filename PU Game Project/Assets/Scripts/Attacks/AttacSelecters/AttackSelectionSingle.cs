using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSelectionSingle : AttackSelection {

    List<Node> allNodes = new List<Node>();

	void Start ()
    {
        allNodes = CombatUtils.BasicAttackSelect(receivedCharacter.gameObject, 3.75f);
    }
	
	// Update is called once per frame
	protected override void Update ()
    {
        if (Input.GetMouseButtonDown(1))
        {
            AttackClick();
        }
	}

    public override void Initialize(HeroCharacter _receivedCharacter)
    {
        base.Initialize(_receivedCharacter);
    }

    public override void NodeDisplayCleanup()
    {
        foreach (Node currentNode in allNodes)
        {
            currentNode.IsAttackable = false;
        }
        allNodes.Clear();
    }

    private void AttackClick()
    {
        RaycastHit hitInfo = new RaycastHit();
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 100f, CombatUtils.clickLayerMask);

        if (hit && hitInfo.transform.gameObject.CompareTag("Champion") && hitInfo.collider.gameObject.activeInHierarchy)
        {
            Unit targetUnitScript = hitInfo.collider.gameObject.GetComponent<Unit>();

            if (targetUnitScript.currentNode.IsAttackable)
            {
                targettedNodes = new List<Node> { targetUnitScript.currentNode };
                CombatUtils.AttackHitCalculation(receivedCharacter.gameObject, hitInfo.collider.gameObject); //[TESTING FOR % CHECK.]
                MadeSelection();
            }
        }
    }
    
}
