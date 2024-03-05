using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MeleeSoldierSearchLeaderState : IState
{
    private MeleeSoldier _npc;
    private FiniteStateMachine<MeleeSoldierStates> _fsm;
    public MeleeSoldierSearchLeaderState(FiniteStateMachine<MeleeSoldierStates> fsm, MeleeSoldier npc)
    {
        _fsm = fsm;
        _npc = npc;
    }
    public void OnStart()
    {
        _npc.CalculatePathFindingToLeader();
        _npc.text.text = "?";
    }

    public void OnUpdate()
    {
        _npc.Search();

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
