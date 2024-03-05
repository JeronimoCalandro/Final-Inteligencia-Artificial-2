using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekerSoldierSearchState : IState
{
    private SeekerSoldier _npc;
    private FiniteStateMachine<SeekerSoldierStates> _fsm;
    public SeekerSoldierSearchState(FiniteStateMachine<SeekerSoldierStates> fsm, SeekerSoldier npc)
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

        /*foreach (var npc in LevelManager.instance.allNpc)
        {
            if (_npc.InFOV(npc.gameObject) && npc.team != _npc.team && npc.life > 0)
            {
                _npc.enemy = npc;
                _fsm.ChangeState(MeleeSoldierStates.Attack);
            }
        }*/
    }

    public void OnExit()
    {

    }
}
