using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeSoldierFollowLeaderState : IState
{
    private MeleeSoldier _npc;
    private FiniteStateMachine<MeleeSoldierStates> _fsm;
    public MeleeSoldierFollowLeaderState(FiniteStateMachine<MeleeSoldierStates> fsm, MeleeSoldier npc)
    {
        _fsm = fsm;
        _npc = npc;
    }
    public void OnStart()
    {
        _npc.text.text = "";
    }

    public void OnUpdate()
    {
        _npc.Follow();

        foreach (var npc in LevelManager.instance.allNpc)
        {
            if (_npc.InFOV(npc.gameObject) && npc.team != _npc.team && npc.life > 0)
            {
                _npc.enemy = npc;
                _fsm.ChangeState(MeleeSoldierStates.Attack);
            }
        }
    }

    public void OnExit()
    {
    
    }
}
