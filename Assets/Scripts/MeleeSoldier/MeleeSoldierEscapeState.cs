using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeSoldierEscapeState : IState
{
    private MeleeSoldier _npc;
    private FiniteStateMachine<MeleeSoldierStates> _fsm;
    public MeleeSoldierEscapeState(FiniteStateMachine<MeleeSoldierStates> fsm, MeleeSoldier npc)
    {
        _fsm = fsm;
        _npc = npc;
    }
    public void OnStart()
    {
        _npc.enemy = null;
        _npc.CalculatePathFindingToEscape();
        _npc.text.text = "O";
    }

    public void OnUpdate()
    {
        _npc.Escape();

        if (_npc.ReturnToCombat())
            _fsm.ChangeState(MeleeSoldierStates.Follow);
    }

    public void OnExit()
    {

    }

}
