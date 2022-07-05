using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleStarshipState : StarshipState
{
    
    public IdleStarshipState(StarshipStateMachine _StateMachine) : base(_StateMachine)
    {
        Name = "IdleState";
    }

    public override void EnterState()
    {
       
    }

    public override void LeaveState()
    {
      
    }

    public override void Tick()
    {

        if(Starship.PrimaryMission is IdleShipMission)
        {
           //wait becaues we are in an idle mission
        }
        else
        {
            StateMachine.ChangeState(StateMachine.WaitingState);
        }

    }
}
