using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattleEventControllable{

    void BattleEventRun();

    void BattleEventPause();

    void BattleEventResume();

    void BattleEventCancel();

    void BattleEventFinish();
}
