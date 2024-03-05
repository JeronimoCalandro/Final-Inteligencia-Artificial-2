using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderEscapeState : IState
{
    private Leader _leader;
    private FiniteStateMachine<LeaderStates> _fsm;
    public LeaderEscapeState(FiniteStateMachine<LeaderStates> fsm, Leader leader)
    {
        _fsm = fsm;
        _leader = leader;
    }
    public void OnStart()
    {
        _leader.enemy = null;
        _leader.CalculatePathFindingToEscape();
        _leader.text.text = "O";
    }

    public void OnUpdate()
    {
        _leader.Escape();

        if (_leader.ReturnToCombat())
            _fsm.ChangeState(LeaderStates.Move);
    }

    public void OnExit()
    {

    }
}
