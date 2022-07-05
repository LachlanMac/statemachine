using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DockStarshipState : StarshipState
{


    private bool IsWaitingToDock = true;
    public Structure Structure { get; set; }
    public Dock Dock { get; set; }
    private Vector2 destination;
    public DockStarshipState(StarshipStateMachine _StateMachine) : base(_StateMachine)
    {
        Name = "DockState";
    }

    public void SetDockLocation(Structure _Station, Dock _Dock)
    {
        Dock = _Dock;
        Structure = _Station;
    }

    public override void EnterState()
    {
        IsWaitingToDock = true; //we need to put ourselves in queue
    }

    public override void LeaveState()
    {
        IsWaitingToDock = false;
        Dock.InUse = false;
    }

    private void GetClearance()
    {
        if (!Dock.InUse)
        {
            IsWaitingToDock = false;
            Dock.InUse = true;
            destination = Structure.AbsoluteLocation;
            Starship.Controller.StartDock(destination);
        }
        //wait for clearance
    }

    public override void Tick()
    {

        if (IsWaitingToDock)
        {
            GetClearance();
            return;
        }
        else
        {   //keep in this loop until we are docked
            if (Starship.Controller.Docked)
            {
                Starship.Dock(Dock);
                StateMachine.ChangeState(StateMachine.WaitingState);
            }
           
        }

        

    }
}
