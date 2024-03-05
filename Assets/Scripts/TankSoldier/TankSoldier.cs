using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
public enum TankSoldierStates
{
    Collecting,
    SearchLeader,
    SearchCoin,
    Dead,
}
public class TankSoldier : Agent<TankSoldierStates>
{
    public GameObject currentCoin;
    public Transform originalLeaderTransform;
    public int coinsCollected;
    public TMP_Text coinText;
    public override void Start()
    {
        base.Start();

        originalLeaderTransform = leaderTransform;
        fsm.AddState(TankSoldierStates.SearchCoin, new TankSoldierSearhCoinState(fsm, this));
        fsm.AddState(TankSoldierStates.Collecting, new TankSoldierCollectingState(fsm, this));
        fsm.AddState(TankSoldierStates.Dead, new TankSoldierDeadState(fsm, this));

        fsm.ChangeState(TankSoldierStates.SearchCoin);
    }

    public override void Update()
    {
        base.Update();

        velocity.y = 0;
        transform.position += velocity * Time.deltaTime;
        if (velocity != Vector3.zero)
            bodyTransform.forward = velocity;

        fsm.Update();
    }

    public void Collecting()
    {
        if (Vector3.Distance(leaderTransform.position, transform.position) > viewRadius / 4 || leaderTransform.gameObject == null)
        {
            //AddForce(Arrive(leader.position));
            AddForce(Seek(leaderTransform.position));
        }
        else
            fsm.ChangeState(TankSoldierStates.SearchCoin);
    }

    public void SearchCoin()
    {
        if (!InLineOfSight(transform.position, leaderTransform.position))
        {
            if (pathToFollow.Count > 0)
                FollowPath();
            else
                CalculatePathFindingToLeader();
        }
        else
            fsm.ChangeState(TankSoldierStates.Collecting);
    }

    public GameObject GetNearestCoin()
    {
        return currentCoin = LevelManager.instance.allCoins.OrderBy(x => Vector3.Distance(transform.position, x.transform.position))
                                             .First();
    }

    public override void ReceiveDamage(int damage)
    {
        base.ReceiveDamage(damage);
        if (life <= 0)
            fsm.ChangeState(TankSoldierStates.Dead);
    }
}
