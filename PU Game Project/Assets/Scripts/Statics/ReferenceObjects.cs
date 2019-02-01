//Provides easy references to all Units to other scripts.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public static class ReferenceObjects {

    //public static readonly string generalPathToAbilitySO = "Assets/Resouces/CharAbilityData/";

    public static List<GameObject> UnitList = new List<GameObject>();
    public static List<GameObject> HeroList = new List<GameObject>();
    public static List<GameObject> VillainList = new List<GameObject>();
    public static List<GameObject> VigilanteList = new List<GameObject>();


    //public static List<BattleBuff> BuffList = new List<BattleBuff>();


    public static void AddToPlayerList(GameObject receivedObject)
    {
        Unit unitScript = receivedObject.GetComponent<Unit>();
        if(unitScript.team == Unit.Teams.Hero)
        {
            HeroList.Add(receivedObject);
        }
        else if (unitScript.team == Unit.Teams.Villain)
        {
            VillainList.Add(receivedObject);
        }
        else if (unitScript.team == Unit.Teams.Vigilante)
        {
            VigilanteList.Add(receivedObject);
        }

        UnitList.Add(receivedObject);
    }

    public static void RemovePlayerFromLists(GameObject removeObject)
    {
        if (UnitList.Contains(removeObject))
        {
            UnitList.Remove(removeObject);
        }
        if (HeroList.Contains(removeObject))
        {
            HeroList.Remove(removeObject);
        }
        if (VillainList.Contains(removeObject))
        {
            VillainList.Remove(removeObject);
        }
        if (VigilanteList.Contains(removeObject))
        {
            VigilanteList.Remove(removeObject);
        }
    }

    /*public static void AddToBuffList(BattleBuff receivedBuff)
    {

    }
    */



}
