using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattleBehaviour {

    IEnumerator RunBehaviour();

    void CancelBehaviour();

    void FinishBehaviour();

    void AddToBehaviourList();

    void RemoveFromBehaviourList();

};
