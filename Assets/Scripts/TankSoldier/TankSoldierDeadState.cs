using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankSoldierDeadState : IState
{
    private TankSoldier _npc;
    private FiniteStateMachine<TankSoldierStates> _fsm;
    public TankSoldierDeadState(FiniteStateMachine<TankSoldierStates> fsm, TankSoldier npc)
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
        _npc.velocity = Vector3.zero;
    }

    public void OnUpdate()
    {

    }

    public void OnExit()
    {

    }
}
