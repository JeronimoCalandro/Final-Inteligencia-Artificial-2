using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeSoldierAttackState : IState
{
    private MeleeSoldier _npc;
    private FiniteStateMachine<MeleeSoldierStates> _fsm;
    float attackTime, currentTime;
    int damage;
    public MeleeSoldierAttackState(FiniteStateMachine<MeleeSoldierStates> fsm, MeleeSoldier npc)
    {
        _fsm = fsm;
        _npc = npc;
    }
    public void OnStart()
    {
        damage = Random.Range(2, 3);
        currentTime = Time.time;
        attackTime = 2;
        _npc.text.text = "!";
    }

    public void OnUpdate()
    {
        if (Time.time > currentTime + attackTime)
        {
            if (_npc.enemy.life > 0)
                _npc.enemy.ReceiveDamage(damage);
            currentTime = Time.time;
            damage = Random.Range(2, 3);
        }

        if (!_npc.InFOV(_npc.enemy.gameObject) || _npc.enemy.isDead)
        {
            _npc.enemy = null;
            _fsm.ChangeState(MeleeSoldierStates.Search);
        }

        /*if(_npc.enemy != null)
            _npc.AddForce(_npc.Persuit(_npc.enemy.transform));*/

        _npc.Persuit();
    }

    public void OnExit()
    {

    }
}
