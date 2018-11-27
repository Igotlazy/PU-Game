using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAbilityPrefabRef : MonoBehaviour {

    public static CharacterAbilityPrefabRef instance;

	void Awake ()
    {
        instance = this;
	}

    public List<GameObject> TakahiroPrefabs = new List<GameObject>();

    public List<GameObject> NodeCollectors = new List<GameObject>();

}
