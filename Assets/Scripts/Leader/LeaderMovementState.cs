using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderMovementState : IState
{
    private Leader _leader;
    private FiniteStateMachine<LeaderStates> _fsm;
    public LeaderMovementState(FiniteStateMachine<LeaderStates> fsm, Leader leader)
    {
        _fsm = fsm;
        _leader = leader;
    }
    public void OnStart()
    {
        _leader.text.text = "";
        //_leader.SetNewPosition();
    }

    public void OnUpdate()
    {
        _leader.Movement();

        if (Vector3.Distance(_leader.transform.position, _leader.targetPosition) < 0.1f)
            _leader.SetNewRandomPosition(Vector3.zero);

        foreach (var npc in LevelManager.instance.allNpc)
        {
            if (_leader.InFOV(npc.gameObject) && npc.team != _leader.team && npc.life > 0)
            {
                _leader.enemy = npc;
                _fsm.ChangeState(LeaderStates.Attack);
            }
        }
    }

    public void OnExit()
    {

    }
}
