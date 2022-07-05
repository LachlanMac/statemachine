using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingStarshipState : StarshipState
{
    //This is the primary transition class for the AI
    public WaitingStarshipState(StarshipStateMachine _StateMachine) : base (_StateMachine)
    {
        Name = "WaitingState";

    }
    public override void EnterState()
    {   
        //Debug.Log("Entered Waiting State: Waiting for next order");
    }

    public override void LeaveState()
    {
       
    }

    public override void Tick()
    {

        if (Starship.PrimaryMission is IdleShipMission)
        {
           //we should probably force into IdleState?
        }
        else if (Starship.PrimaryMission is HarvestShipMission hMission)
        {
            if (Starship.InHangar && !hMission.DoneHarvest)
                StateMachine.ChangeState(StateMachine.LaunchState);
            else
            {//we can enter our harvest state
                if (Starship.PrimaryMission.Complete)
                {
                    Starship.PrimaryMission = Starship.IdleMission;
                    StateMachine.ChangeState(StateMachine.IdleState);
                }
                else if (Starship.PrimaryMission is IdleShipMission)
                    StateMachine.ChangeState(StateMachine.IdleState);
                else
                    StateMachine.ChangeState(StateMachine.HarvestState);
            }
        }else if(Starship.PrimaryMission is HaulShipMission hMisision)
        {
            if (Starship.PrimaryMission.Complete)
            {
                Starship.PrimaryMission = Starship.IdleMission;
                StateMachine.ChangeState(StateMachine.IdleState);
            }
            else if (Starship.PrimaryMission is IdleShipMission)
                StateMachine.ChangeState(StateMachine.IdleState);
            else
                StateMachine.ChangeState(StateMachine.HaulState);
        }

    }
}
