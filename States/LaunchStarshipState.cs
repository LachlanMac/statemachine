using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchStarshipState : StarshipState
{

    public LaunchStarshipState(StarshipStateMachine _StateMachine) : base(_StateMachine)
    {
        Name = "LaunchState";
    }
    public override void EnterState()
    {

    }

    public override void LeaveState()
    {
       
    }

    public override void Tick()
    {

        if (Starship.DockedPort.Dock.InUse && !Starship.Controller.Launching)
        {
          //If the starships dock is in use and this ship isn't the one that is launching, we wait.
        }
        else
        {
            if (Starship.Controller.Launching)
            {   //we are still launching, make the dock be occupied...
               
            }
            else
            {
                //launc the ship
                Starship.Controller.Undock();
            }
        }
        if (Starship.Controller.Launched)
        {//we launched
            //continue with mission
           
            Starship.DockedPort.Dock.InUse = false;
            Starship.InHangar = false;
            Starship.DockedPort = null;
            StateMachine.ChangeState(StateMachine.WaitingState);

        }
    }
}
