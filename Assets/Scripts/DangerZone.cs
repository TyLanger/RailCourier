using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DangerZone : RailPoint
{

    // if you're in this, you're past it, but not to the next point yet
    public event Action OnAllCarsLeft;

    public override void Enter(Car car)
    {
        base.Enter(car);
        car.DangerHighlight(true);
    }

    public override void Exit(Car car)
    {
        base.Exit(car);
        if(cars.Count <= 0)
        {
            OnAllCarsLeft?.Invoke();
        }
        car.DangerHighlight(false);
    }

    public bool Split()
    {
        //Debug.Log($"Count at split: {cars.Count}");
        //Debug.Break();
        if (cars.Count > 0)
        {
            cars[cars.Count-1].SplitBack();
            return true;
        }
        return false;
    }

}
