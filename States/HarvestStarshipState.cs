using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestStarshipState : StarshipState
{

    private int HarvestTimer = 0;
    private int UnloadTimer = 0;
    HarvestShipMission mission;
    public Vector2 Destination { get; set; }
    public HarvestStarshipState(StarshipStateMachine _StateMachine) : base(_StateMachine)
    {
        Name = "HarvestState";
    }

    public override void EnterState()
    {

       
        mission = (HarvestShipMission) Starship.PrimaryMission;
        Destination = mission.Destination.AbsoluteLocation;
        
        //check first if we are at our destination... if we are not, we need to go to our destination
        if(Vector2.Distance(Destination, Starship.Controller.transform.position) > 4f  &&  !mission.DoneHarvest)
        {//we are not here yet?
            GoToStarshipState go = (GoToStarshipState)StateMachine.GoToState;
            if(mission.Destination is Planet)
            {
                Destination = new Vector2(Destination.x + Random.Range(-5, 5), Destination.y + Random.Range(-5, 5));
            }
            else if(mission.Destination is Asteroid)
            {
                Destination = new Vector2(Destination.x + Random.Range(-2, 2), Destination.y + Random.Range(-2, 2));
            }
           
            go.Destination = Destination;
            StateMachine.ChangeState(go);
        }
    }

    public override void LeaveState()
    {
        HarvestTimer = 0;
        UnloadTimer = 0;
    }
    public override void Tick()
    {
        if (mission.DoneHarvest) //if the mission tracker is 100% complet
        {
            if (Starship.IsDockedAt(Starship.PrimaryMission.ReturnLocation.Dock))   //if the starship is docked at the return location
            {
                Print("We are Docked and Unloading");
                //we are both finished the mission and we are home, lets unload while we can
                UnloadTimer++;
                if(UnloadTimer >= 60) //TO:DO make this an actual variable.  But for now, unlaod one package of cargo
                {
                    int resInCargo = Starship.GetInStorage(mission.TargetResource);
                    if (resInCargo >= CargoPackage.CARGO_SIZE)//we have one shipment, lets unload it
                    {
                        Starship.DockedPort.AddToStorage(mission.TargetResource, CargoPackage.CARGO_SIZE);
                        Starship.RemoveFromStorage(mission.TargetResource, CargoPackage.CARGO_SIZE);
                    }else if(resInCargo > 0)
                    {
                        Starship.DockedPort.AddToStorage(mission.TargetResource, resInCargo);
                        Starship.RemoveFromStorage(mission.TargetResource, resInCargo);
                    }
                    else // it is 0, we are done..resolve mission
                    {
                        Destination = Vector2.zero;
                        mission.ResolveMission();
                        StateMachine.ChangeState(StateMachine.WaitingState);
                    }

                    UnloadTimer = 0;
                }
            }
            else
            {   //go home becaues we are finished
                GoToStarshipState go = (GoToStarshipState)StateMachine.GoToState;
                Destination = Starship.PrimaryMission.ReturnLocation.StructureRef.AbsoluteLocation + Starship.PrimaryMission.ReturnLocation.GetDockOffset();
                go.Destination = Destination;
                Starship.ReturningHome = true;
                StateMachine.ChangeState(StateMachine.GoToState);
            }
        }else if(Vector2.Distance(Starship.Controller.transform.position, Destination) < 20f)
        {
            if(HarvestTimer >= HarvestShipMission.FramesPerHarvest)
            {
                mission.Harvest();
                HarvestTimer = 0;
            }
            HarvestTimer++;
        }
        else
        {
          

        }
    }

}
