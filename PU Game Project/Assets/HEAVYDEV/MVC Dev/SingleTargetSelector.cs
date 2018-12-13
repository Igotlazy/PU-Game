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

    void Start()
    {

    }


    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.B))
        {
            GatherClick();
        }
    }

    private void GatherClick()
    {
        Debug.Log("Click click");
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 100f, CombatUtils.gameEntityMask);

        if (!EventSystem.current.IsPointerOverGameObject()) //Makes sure it doesn't interact with UI
        {
            if (hit && (hitInfo.transform.gameObject.CompareTag("Champion")))
            {
                Debug.Log("Click click Click");
                if (selectedObject !=  hitInfo.collider.gameObject)
                {
                    Debug.Log("Click click Click Click");
                    if (spawnedIndicator != null)
                    {
                        Destroy(spawnedIndicator);
                    }

                    selectedObject = hitInfo.collider.gameObject;
                    Unit unitScript = hitInfo.collider.gameObject.GetComponent<Unit>();
                    spawnedIndicator = Instantiate(selectionIndicator, unitScript.centerPoint.transform.position, Quaternion.identity);
                }                
            }
        }
    }

    public override void MadeSelectionImpl()
    {
        collectedNodes.Add(selectedObject.GetComponent<Unit>().currentNode);
    }
}
