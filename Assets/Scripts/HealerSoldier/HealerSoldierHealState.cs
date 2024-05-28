using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HealerSoldierHealState : IState
{
    private HealerSoldier _npc;
    private FiniteStateMachine<HealerSoldierStates> _fsm;
    float healTime, currentTime;
    int healing;
    public HealerSoldierHealState(FiniteStateMachine<HealerSoldierStates> fsm, HealerSoldier npc)
    {
        _fsm = fsm;
        _npc = npc;
    }
    public void OnStart()
    {
        healing = 1;
        currentTime = Time.time;
        healTime = 3;

        _npc.text.text = "+";

        GetAgentToHeal();
    }

    public void OnUpdate()
    {
        if (_npc.agentToHeal == null) return;

        _npc.Follow(true);

        if (_npc.agentToHeal.isDead || _npc.agentToHeal.life == _npc.agentToHeal.maxLife)
            _fsm.ChangeState(HealerSoldierStates.Follow);

        if (Time.time > currentTime + healTime)
        {
            if (_npc.agentToHeal.life < _npc.agentToHeal.maxLife)
            {

                if(Vector3.Distance(_npc.transform.position, _npc.agentToHeal.transform.position) < 2)
                    _npc.agentToHeal.ReceiveHealing(healing);
            }
            currentTime = Time.time;
        }
    }

    public void OnExit()
    {

    }

    void GetAgentToHeal()
    {
        _npc.agentToHeal = LevelManager.instance.allNpc.Where(x => x != null)
            .Where(x => x.team == _npc.team)
            .Where(x => x.life < x.maxLife)
            .Where(x => x != _npc)
            .Where(x => !x.isDead)
            .OrderBy(x => x.life)
            .FirstOrDefault();

        if(_npc.agentToHeal != null)
            _npc.leaderTransform = _npc.agentToHeal.transform;
    }
}
