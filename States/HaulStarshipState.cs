using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaulStarshipState : StarshipState
{

    HaulShipMission mission;
    public bool ReturningHome { get; set; }

    public Vector2 Destination { get; set; }

    public HaulStarshipState(StarshipStateMachine _StateMachine) : base(_StateMachine)
    {
        Name = "HarvestState";
    }
    public override void EnterState()
    {
      
        //get the mission when we enter the state
        mission = (HaulShipMission)Starship.PrimaryMission;
        //determine if the shipment is here in this building
        //  Destination = mission.Shipment.Destination.GetDockOffset();


    }

    private void Launch()
    { 
        StateMachine.ChangeState(StateMachine.LaunchState);
        return;
    }

    public override void LeaveState()
    {
       
    }

    public override void Tick()
    {
        if (mission.Shipment.ShipmentStatus == Shipment.Status.LOST || mission.Shipment.ShipmentStatus == Shipment.Status.UNKNOWN)
        {
            mission.Fail();
            return;
        }

        if (Starship.IsDockedAt(mission.CargoStartLocation.Dock))  //if the starship is docked at the cargo start location
        {
            
            //We are docked at the origin AND the shipment is also at the origin
            if (mission.Shipment.ShipmentStatus == Shipment.Status.AT_ORIGIN)
            {   //load the cargo and go back to default state for next logic
                mission.LoadCargo();  
                StateMachine.ChangeState(StateMachine.WaitingState);
               
            }
            else if(mission.Shipment.ShipmentStatus == Shipment.Status.LOADED)
            {//cargo is loaded AND we are at the origin, we can launch
                mission.Shipment.ShipmentStatus = Shipment.Status.IN_TRANSIT;
                Starship.DestinationDock = mission.DeliveryDestination;
                Launch();
            }
        }
        else if (Starship.IsDockedAt(mission.DeliveryDestination.Dock))   //if the starship is docked at the destination for the cargo
        {
            Debug.Log("HAUL SHIP IS DOCKED AT THE DELIVERY DESTINATION "  + mission.Shipment.Id + " " + mission.Shipment + " SHIP LOCATED AT " + Starship.DockedPort.StructureRef.Name + " [" + Starship.DockedPort.Owner.Name + "]");
           
            if (mission.Shipment.ShipmentStatus == Shipment.Status.UNLOADED)
            {
                mission.ResolveMission();
                StateMachine.ChangeState(StateMachine.WaitingState);
                return;
                //TO:DO  If company is industrial, go home - otherwise, if hauler, we need to look for deals here.
            }
            else if (mission.Shipment.ShipmentStatus == Shipment.Status.IN_TRANSIT)
            {
                mission.UnloadCargo();
                StateMachine.ChangeState(StateMachine.WaitingState);
                return;
            }
            else if(mission.Shipment.ShipmentStatus == Shipment.Status.AT_ORIGIN) // we need to go pick it up.  we are where it needs to end up
            {
                Starship.DestinationDock = mission.CargoStartLocation;
                Launch();
            }
        }
        else if (Starship.IsDocked())  //if the starship is docked somehwere else...
        {   
            Launch();
        }
        else if (!Starship.IsDocked())
        {
            if (mission.Shipment.ShipmentStatus == Shipment.Status.IN_TRANSIT)
            {
                GoToStarshipState go = (GoToStarshipState)StateMachine.GoToState;
                Destination = mission.Shipment.Destination.StructureRef.AbsoluteLocation + mission.Shipment.Destination.GetDockOffset();
                go.Destination = Destination;
                StateMachine.ChangeState(StateMachine.GoToState);
                return;
            }else if(mission.Shipment.ShipmentStatus == Shipment.Status.AT_ORIGIN)
            {
                GoToStarshipState go = (GoToStarshipState)StateMachine.GoToState;
                Destination = mission.Shipment.Origin.StructureRef.AbsoluteLocation + mission.Shipment.Origin.GetDockOffset();
                go.Destination = Destination;
                StateMachine.ChangeState(StateMachine.GoToState);
                return;
            }
        }

    }
}
