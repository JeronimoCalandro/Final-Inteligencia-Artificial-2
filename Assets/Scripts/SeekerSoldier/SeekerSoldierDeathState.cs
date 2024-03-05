using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekerSoldierDeathState : IState
{
    private SeekerSoldier _npc;
    private FiniteStateMachine<SeekerSoldierStates> _fsm;
    public SeekerSoldierDeathState(FiniteStateMachine<SeekerSoldierStates> fsm, SeekerSoldier npc)
    {
        _fsm = fsm;
        _npc = npc;
    }
    public void OnStart()
    {
        _npc.text.text = "X";
        _npc.GetComponentInChildren<MeshRenderer>().enabled = false;
        _npc.GetComponentInChildren<BoxCollider>().enabled = false;
        _npc.isDead = true;
        LevelManager.instance.RemoveBoid(_npc);
    }

    public void OnUpdate()
    {

    }

    public void OnExit()
    {

    }
}
