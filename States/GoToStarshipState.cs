using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToStarshipState : StarshipState
{


    public Vector2 Destination { get; set; }
    public List<StarSystem> WarpPath { get; set; }
    public enum DestinationType { PLANET, STATION, SPACE, RALLY}
    public DestinationType DType { get; set; }

    public GoToStarshipState(StarshipStateMachine _StateMachine) :base(_StateMachine)
    {
        Name = "GotoState";
    }
    public override void EnterState()
    {
        if (Universe.GetSystemByCoordinates(Destination) == Universe.GetSystemByCoordinates(Starship.Controller.transform.position))
        {   //we are in the same state, so clear the WarpPath
            WarpPath = new List<StarSystem>();
        }
        else
        {  //We need to calculate our warp path
            WarpPath = DataLoader.Instance.Universe.CalculateWarpPath(
                Universe.GetSystemByCoordinates(Starship.Controller.transform.position),
                Universe.GetSystemByCoordinates(Destination), (int)Starship.Template.WarpRange);
            WarpPath.Remove(WarpPath[0]); //Remove the first path, because that is the system we are in.
        }

    }
    public override void LeaveState() {
        Destination = Vector2.zero;
        WarpPath = null;
    }
    public override void Tick()
    {
        if(WarpPath.Count == 0)
        {//we just need to move
            if (Starship.Controller.Warping) //we are on our last warp...wait...
                return;
            else
                if(Starship.Controller.Destination == Vector2.zero)
                    Starship.Controller.Destination = Destination;

            if (!Starship.Controller.InTransit)
            {   //we have stopped, we need to determine if we're at our destination
                if (Vector2.Distance(Destination, Starship.Controller.transform.position) < 0.008f)
                {   //we have arrived so we can leave this state
                    if (Starship.ReturningHome)
                    {
                        DockStarshipState ds = (DockStarshipState)StateMachine.DockState;
                        Building home = Starship.PrimaryMission.ReturnLocation;
                        if (home == null)
                            home = Starship.HomeBase;
                        ds.SetDockLocation(home.StructureRef, home.Dock);
                        StateMachine.ChangeState(ds);
                        Starship.ReturningHome = false; //we have returned home
                    }else if(Starship.DestinationDock != null)
                    {//we have an actual destination dock, and by all accounts, we are here.
                        DockStarshipState ds = (DockStarshipState)StateMachine.DockState;
                        ds.SetDockLocation(Starship.DestinationDock.StructureRef, Starship.DestinationDock.Dock);
                        Starship.DestinationDock = null;
                        StateMachine.ChangeState(ds);
             
                    }
                    else
                        StateMachine.ChangeState(StateMachine.WaitingState);
                }
                else
                {
                    Starship.Controller.Direction = Destination;
                    Starship.Controller.InTransit = true;
                }
            }
        }
        else //there is a warp path with at least one more node
        {    //We are warping, don't do anything
            if (Starship.Controller.Warping || Starship.Controller.InWarpSequence)
                return;
            else
            {   //we are not warping, but we have a warp path.  Figure out the next destination
                if (Universe.GetSystemByCoordinates(Destination) == Universe.GetSystemByCoordinates(Starship.Controller.transform.position))
                {  //we are in the same system, so remove the last warp path.
                    WarpPath.Remove(WarpPath[0]);
                    return;
                }
                else
                {
                    Vector2 Target = WarpPath[0].Controller.GetClosestHyperSpaceNode(Starship.Controller.transform.position);
                    WarpPath.Remove(WarpPath[0]);
                    Starship.Controller.StartWarp(Target);
                }
            }
        }
    }
}
