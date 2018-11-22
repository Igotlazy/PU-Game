//Provides easy references to all Units to other scripts.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public static class ReferenceObjects {

    //public static readonly string generalPathToAbilitySO = "Assets/Resouces/CharAbilityData/";

    public static List<GameObject> UnitList = new List<GameObject>();
    public static List<GameObject> UnitAllyList = new List<GameObject>();
    public static List<GameObject> UnitEnemyList = new List<GameObject>();

    //public static List<BattleBuff> BuffList = new List<BattleBuff>();


    public static void AddToPlayerList(GameObject receivedObject)
    {
        if(receivedObject.GetComponent<Unit>().teamValue == 0)
        {
            UnitAllyList.Add(receivedObject);
        }
        else
        {
            UnitEnemyList.Add(receivedObject);
        }

        UnitList.Add(receivedObject);
    }

    /*public static void AddToBuffList(BattleBuff receivedBuff)
    {

    }
    */



}
