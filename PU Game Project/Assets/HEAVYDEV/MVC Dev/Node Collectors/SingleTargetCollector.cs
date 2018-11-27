using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SingleTargetCollector : MonoBehaviour
{

    public int numOfSelections;
    public List<GameObject> selectedObjects = new List<GameObject>();
    public GameObject selectionIndicator;

    void Start()
    {

    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GatherClick();
        }
    }

    private void GatherClick()
    {
        RaycastHit hitInfo = new RaycastHit();
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 100f, CombatUtils.clickLayerMask);

        if (!EventSystem.current.IsPointerOverGameObject()) //Makes sure it doesn't interact with UI
        {
            if (hit && (hitInfo.transform.gameObject.CompareTag("Champion") && hitInfo.collider.gameObject.activeInHierarchy))
            {
                if (!selectedObjects.Contains(hitInfo.collider.gameObject))
                {
                    selectedObjects.Add(hitInfo.collider.gameObject);
                    Unit unitScript = hitInfo.collider.gameObject.GetComponent<Unit>();
                    Instantiate(selectionIndicator, unitScript.centerPoint.transform.position, Quaternion.identity);
                }                
            }
        }
    }
}
