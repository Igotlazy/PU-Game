using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SingleTargetSelector : AttackSelection
{

    public int numOfSelections;
    public GameObject selectedObject;
    public GameObject selectionIndicator;
    public GameObject spawnedIndicator;
    RaycastHit hitInfo;

    List<Collider> colliders = new List<Collider>();



    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.B))
        {
            GatherClick();
        }
    }

    protected override void InitializeImpl(int selectorIndex)
    {
        
    }

    private void GatherClick()
    {
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 100f, CombatUtils.gameEntityMask);

        if (!EventSystem.current.IsPointerOverGameObject()) //Makes sure it doesn't interact with UI
        {
            if (hit && (hitInfo.transform.gameObject.CompareTag("Champion")))
            {
                if (selectedObject !=  hitInfo.collider.gameObject)
                {
                    if (spawnedIndicator != null)
                    {
                        Destroy(spawnedIndicator);
                    }

                    selectedObject = hitInfo.collider.gameObject;
                    Unit unitScript = selectedObject.GetComponent<Unit>();
                    LivingCreature livingScript = selectedObject.GetComponent<LivingCreature>();

                    livingScript.healthBar.DisplayHitChance(CombatUtils.AttackHitCalculation(givenAbility.associatedCreature.gameObject, selectedObject));


                    spawnedIndicator = Instantiate(selectionIndicator, unitScript.centerPoint.transform.position, Quaternion.identity);
                }                
            }
        }
    }

    protected override void MadeSelectionImpl()
    {
        collectedNodes.Add(selectedObject.GetComponent<Unit>().currentNode);
        if(spawnedIndicator != null)
        {
            Destroy(spawnedIndicator);
        }
        if(selectedObject != null)
        {
            selectedObject.GetComponent<LivingCreature>().healthBar.HideHitChance();
        }
    }
    protected override void CancelSelectionImpl()
    {
        if (spawnedIndicator != null)
        {
            Destroy(spawnedIndicator);
        }
        if (selectedObject != null)
        {
            selectedObject.GetComponent<LivingCreature>().healthBar.HideHitChance();
        }
    }


}
