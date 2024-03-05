using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankSoldierSearhCoinState : IState
{
    private TankSoldier _npc;
    private FiniteStateMachine<TankSoldierStates> _fsm;
    public TankSoldierSearhCoinState(FiniteStateMachine<TankSoldierStates> fsm, TankSoldier npc)
    {
        _fsm = fsm;
        _npc = npc;
    }
    public void OnStart()
    {
        _npc.leaderTransform = _npc.GetNearestCoin().transform;
    }

    public void OnUpdate()
    {
        _npc.SearchCoin();
    }

    public void OnExit()
    {

    }
}
