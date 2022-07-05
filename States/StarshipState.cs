using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StarshipState {


    protected string Name;
    protected bool DebugAI = false;
    protected StarshipStateMachine StateMachine;
    protected Starship Starship;
    protected Universe Universe;
    public StarshipState(StarshipStateMachine _StateMachine)
    {
        StateMachine = _StateMachine;
        //Helper references
        Starship = _StateMachine.Starship;
        Universe = _StateMachine.Universe;
    }
    public abstract void EnterState();
    public abstract void LeaveState();
    public abstract void Tick();

    public void DebugStateAI()
    {
        DebugAI = true;
    }

    public void Print(string text)
    {
        if (DebugAI)
            Debug.Log("AI DEBUG:["+Name+"] " + text);
    }


}