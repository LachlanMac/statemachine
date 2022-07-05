using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarshipStateMachine {
    public Universe Universe { get; set; }
    public Starship Starship { get; set; }
    public StarshipState CurrentState, WaitingState, LaunchState, HarvestState, GoToState, DockState, IdleState, HaulState;
    public bool Debug { get; set; }

    public StarshipStateMachine(Starship _Starship)
    {

        Debug = false;
        Starship = _Starship;
        Universe = DataLoader.Instance.Universe;
        WaitingState = new WaitingStarshipState(this);
        LaunchState = new LaunchStarshipState(this);
        HarvestState = new HarvestStarshipState(this);
        GoToState = new GoToStarshipState(this);
        DockState = new DockStarshipState(this);
        IdleState = new IdleStarshipState(this);
        HaulState = new HaulStarshipState(this);
        CurrentState = WaitingState;



    }

    public void Tick()
    {
        CurrentState.Tick();
    }

    public void ChangeState(StarshipState State)
    {


        CurrentState.LeaveState();
        CurrentState.Print("Leaving State");
        CurrentState = State;

        if (Debug)
            CurrentState.DebugStateAI();
        CurrentState.Print("Entering State");
        State.EnterState();
       
    }


}