using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankSoldierCollectingState : IState
{
    private TankSoldier _npc;
    private FiniteStateMachine<TankSoldierStates> _fsm;
    public TankSoldierCollectingState(FiniteStateMachine<TankSoldierStates> fsm, TankSoldier npc)
    {
        _fsm = fsm;
        _npc = npc;
    }
    public void OnStart()
    {
        
    }

    public void OnUpdate()
    {
        _npc.Collecting();
    }

    public void OnExit()
    {
        LevelManager.instance.RemoveCoin(_npc.currentCoin);
        _npc.coinsCollected++;
        _npc.coinText.text = (_npc.coinsCollected + " / 9");
        if (_npc.coinsCollected >= 9)
            LevelManager.instance.FinishGame(_npc.team);
    }
}
