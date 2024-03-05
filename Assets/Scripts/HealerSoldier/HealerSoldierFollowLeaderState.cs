using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HealerSoldierFollowLeaderState : IState
{
    private HealerSoldier _npc;
    private FiniteStateMachine<HealerSoldierStates> _fsm;
    public HealerSoldierFollowLeaderState(FiniteStateMachine<HealerSoldierStates> fsm, HealerSoldier npc)
    {
        _fsm = fsm;
        _npc = npc;
    }
    public void OnStart()
    {
        _npc.leaderTransform = _npc.originalLeaderTransform;
        _npc.text.text = "";
    }

    public void OnUpdate()
    {
        _npc.Follow(false);

        if (LevelManager.instance.allNpc.Where(x => x != null)
            .Where(x => x.team == _npc.team)
            .Any(x => x.life < x.maxLife && x.life > 0))
        {
            _fsm.ChangeState(HealerSoldierStates.Heal);
        }
    }

    public void OnExit()
    {

    }
}
