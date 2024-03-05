using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeSoldierDeadState : IState
{
    private MeleeSoldier _npc;
    private FiniteStateMachine<MeleeSoldierStates> _fsm;
    public MeleeSoldierDeadState(FiniteStateMachine<MeleeSoldierStates> fsm, MeleeSoldier npc)
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
