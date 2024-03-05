using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//FSM
public class FiniteStateMachine<T>
{
    public IState currentState;
    private Dictionary<T, IState> _allStates = new Dictionary<T, IState>();

    public void Update()
    {
        currentState.OnUpdate();
    }

    public void ChangeState(T state)
    {
        if (!_allStates.ContainsKey(state)) return;

        if (currentState != null) currentState.OnExit();
        currentState = _allStates[state];
        currentState.OnStart();
    }
    public void AddState(T key, IState value)
    {
        if (!_allStates.ContainsKey(key)) _allStates.Add(key, value);
        else _allStates[key] = value;
    }
}
