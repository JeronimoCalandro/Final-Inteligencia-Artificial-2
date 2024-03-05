using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderAttackState : IState
{
    private Leader _leader;
    private FiniteStateMachine<LeaderStates> _fsm;
    float attackTime, currentTime;
    int damage;
    public LeaderAttackState(FiniteStateMachine<LeaderStates> fsm, Leader leader)
    {
        _fsm = fsm;
        _leader = leader;
    }
    public void OnStart()
    {
        damage = Random.Range(1, 3);
        currentTime = Time.time;
        attackTime = 2;
        _leader.text.text = "!";
    }

    public void OnUpdate()
    { 
        _leader.Persuit();

        if (Time.time > currentTime + attackTime)
        {
            if(_leader.enemy.life > 0)
                _leader.enemy.ReceiveDamage(damage);
            currentTime = Time.time;
        }

        if (!_leader.InFOV(_leader.enemy.gameObject) || _leader.enemy.isDead)
        {
            _leader.enemy = null;
            _fsm.ChangeState(LeaderStates.Move);
        }

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            _fsm.ChangeState(LeaderStates.Move);
    }

    public void OnExit()
    {
        _leader.velocity = Vector3.zero;
    }
}
