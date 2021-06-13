using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownTollSwitch : RailPoint
{

    bool waitingForCarToLeave = false;

    public override void Enter(Car car)
    {
        base.Enter(car);

        if(!car.hasBehind)
        {
            if (cars.Count > 1)
            {
                // wait until the last car leaves before switching
                Debug.Log("Gonna wait");
                waitingForCarToLeave = true;
            }
            else
            {
                if (car.GetType() != typeof(Train))
                {
                    GoToTown(car);
                }
            }
        }
    }

    public override void Exit(Car car)
    {
        if(cars.Count == 0)
        {
            // switch back to base
            useAlt = false;
        }
        else if(waitingForCarToLeave && cars.Count == 1)
        {
            waitingForCarToLeave = false;
            GoToTown(car);
        }
        base.Exit(car);

    }

    void GoToTown(Car car)
    {
        //Debug.Log("Go to town. Spooooky");
        car.Decouple();
        useAlt = true;
    }
}
