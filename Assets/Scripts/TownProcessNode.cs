using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TownProcessNode : RailPoint
{
    public event Action<Car> OnCarEntered;

    // when a car(not train) enters, slow it down
    // stop at the end. Maybe the slowdown and stop are diff nodes?
    // maybe I sould set the speed to something like 1
    // then let it coast to a specific spot
    // speedLimitedZone??? speedSetterZone? slows it down if going to fast, speeds up if going too slow
    // fire an event that says soemthing is here
    // Wait till the crates are taken off
    // move it along


    public override void Enter(Car car)
    {
        base.Enter(car);
        // if it goes negative, it will get clamped to 0 eventually.
        // will have 1 frame where it moves backwards...
        car.AddCurrentSpeedPropagate(-1);
        if(OnCarEntered != null)
        {
            OnCarEntered?.Invoke(car);

        }
        else
        {
            // do this when the towns don't exist yet
            PushCarAway();
        }
    }

    public void PushCarAway()
    {
        if(cars != null && cars.Count > 0)
        {
            cars[0].AddCurrentSpeedPropagate(4);
        }
    }
}
