using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerSoldierLostState : IState
{
    private HealerSoldier _npc;
    private FiniteStateMachine<HealerSoldierStates> _fsm;
    public HealerSoldierLostState(FiniteStateMachine<HealerSoldierStates> fsm, HealerSoldier npc)
    {
        _fsm = fsm;
        _npc = npc;
    }
    public void OnStart()
    {
        _npc.CalculateRandomPathFinding();
        _npc.text.text = "?";
    }

    public void OnUpdate()
    {
        _npc.Lost();
    }

    public void OnExit()
    {

    }
}
