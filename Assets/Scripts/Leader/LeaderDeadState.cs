using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderDeadState : IState
{
    private Leader _leader;
    private FiniteStateMachine<LeaderStates> _fsm;
    public LeaderDeadState(FiniteStateMachine<LeaderStates> fsm, Leader leader)
    {
        _fsm = fsm;
        _leader = leader;
    }
    public void OnStart()
    {
        _leader.text.text = "X";
        _leader.GetComponentInChildren<MeshRenderer>().enabled = false;
        _leader.GetComponentInChildren<BoxCollider>().enabled = false;
        _leader.isDead = true;
        LevelManager.instance.RemoveBoid(_leader);
        LevelManager.instance.RefreshTeamText(_leader.team);
    }

    public void OnUpdate()
    {

    }

    public void OnExit()
    {

    }
}
