using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerSoldierSearchState : IState
{
    private HealerSoldier _npc;
    private FiniteStateMachine<HealerSoldierStates> _fsm;
    public HealerSoldierSearchState(FiniteStateMachine<HealerSoldierStates> fsm, HealerSoldier npc)
    {
        _fsm = fsm;
        _npc = npc;
    }
    public void OnStart()
    {
        _npc.CalculatePathFindingToLeader();
        _npc.text.text = "+";
    }

    public void OnUpdate()
    {
        _npc.Search();
    }

    public void OnExit()
    {

    }
}
