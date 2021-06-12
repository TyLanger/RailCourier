using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSwitcher : RailPoint
{
    float timeBetweenSwitches = 0.5f;
    float timeOfNextSwitch = 0;

    float timeOfIntendedSwitch = 0;
    float leewayTime = 0.4f;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            if (CanSwitch() && timeOfNextSwitch < Time.time)
            {
                timeOfNextSwitch = Time.time + timeBetweenSwitches;
                timeOfIntendedSwitch = 0; // reset so it doesn't double switch
                Switch();
            }
            else
            {
                timeOfIntendedSwitch = Time.time;
            }
        }
    }

    bool CanSwitch()
    {
        // if the car currently in the zone (zone behind this is next node) has a behind, don't switch
        // things enter a railPoint when they set it to be their target. So the zone of cars is before the point
        // is this the only condition I need?
        bool blocked;
        if(useAlt)
        {
            blocked = altPoint.DoesCarHaveBehind();
        }
        else
        {
            blocked = nextPoint.DoesCarHaveBehind();
        }

        return !blocked;

    }

    public override void Enter(Car car)
    {
        base.Enter(car);

        if (!car.hasBehind)
        {
            // if a car enters and it is the last in the train,
            // check if the player tried to switch in the last fraction of a second
            // if so, switch it now
            if(timeOfIntendedSwitch > 0)
            {
                Debug.Log($"time: {Time.time} < {timeOfIntendedSwitch}+{leewayTime}");
            }
            if (Time.time < timeOfIntendedSwitch + leewayTime)
            {
                Debug.Log("Auto delayed switch");
                timeOfNextSwitch = Time.time + timeBetweenSwitches;
                Switch();
            }
        }

    }
}
