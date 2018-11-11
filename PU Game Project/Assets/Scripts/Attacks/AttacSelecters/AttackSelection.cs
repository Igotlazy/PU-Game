using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSelection : MonoBehaviour{

    protected HeroCharacter receivedCharacter;
    public List<Node> targettedNodes = new List<Node>();

    protected virtual void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            MadeSelection();
        }
    }

    public virtual void Initialize(HeroCharacter _receivedCharacter)
    {
        receivedCharacter = _receivedCharacter;
    }

    protected void MadeSelection()
    {
        receivedCharacter.AttackRequester(targettedNodes);
        receivedCharacter.UnitAbilityCleanup(); //Calls NodeDisplayCleanup as well.
    }

    public virtual void NodeDisplayCleanup()
    {
        foreach (Node currentNode in targettedNodes)
        {
            currentNode.IsAttackable = false;
        }

        targettedNodes.Clear();
    }

}
