using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekerSoldierSeekState : IState
{
    private SeekerSoldier _npc;
    private FiniteStateMachine<SeekerSoldierStates> _fsm;
    public SeekerSoldierSeekState(FiniteStateMachine<SeekerSoldierStates> fsm, SeekerSoldier npc)
    {
        _fsm = fsm;
        _npc = npc;
    }
    public void OnStart()
    {

    }

    public void OnUpdate()
    {
        _npc.Seek();

        if (Vector3.Distance(_npc.transform.position, _npc.targetPosition) < 1f)
            _npc.SetNewRandomPosition();

        foreach (var npc in LevelManager.instance.allNpc)
        {
            if (_npc.InFOV(npc.gameObject) && npc.team != _npc.team && npc.life > 0)
            {
                _npc.leaderTransform.GetComponent<Leader>().SetNewRandomPosition(npc.transform.position);
                _npc.flag.transform.position = npc.transform.position;
                _npc.flag.transform.parent = null;
                _npc.flag.SetActive(true);
                _fsm.ChangeState(SeekerSoldierStates.Search);
            }
        }
    }

    public void OnExit()
    {
        _npc.velocity = Vector3.zero;
    }
}
