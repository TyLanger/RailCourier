using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailStopper : RailPoint
{
    public bool takeInput = false;
    public bool active = true;

    public RailStopper stopper;
    public DangerZone danger;

    public GameObject space;

    private void Update()
    {
        if (takeInput)
        {
            if (Input.GetButtonDown("Jump"))
            {
                if(space)
                    space.SetActive(false);
                if (cars.Count > 0)
                {
                    InjectCar();
                }
            }
        }
    }

    public override void Enter(Car car)
    {
        if (active)
        {
            if(takeInput)
            {
                if(space)
                    space.SetActive(true);
            }
            car.StopMoving();
        }
        base.Enter(car);
    }

    void InjectCar()
    {
        //cars[0].ResumeMoving();

        // don't fire immediately
        // split the train at the danger zone
        // stop the back half

        // wait until the car currently in the danger zone leaves
        // fire this
        // fire the back half
        // train.split()
        // backHalf.Stop()
        // car.addSpeed(100)
        // backHalf.addSpeed(100)

        if (danger.Split())
        {
            danger.OnAllCarsLeft += CoastClear;
            stopper.SetStopper(true);
        }
        else
        {
            // nothing to split
            CoastClear();
        }
    }

    void CoastClear()
    {
        //Debug.Log("Coast clear");
        danger.OnAllCarsLeft -= CoastClear;

        StartCoroutine(WeaveCars());
    }

    void MergeClear()
    {
        danger.OnAllCarsLeft -= MergeClear;
        //Debug.Log("merge clear");

        stopper.SetCarMoving();
        stopper.SetStopper(false);
    }

    IEnumerator WeaveCars()
    {
        yield return new WaitForSeconds(0.1f);
        SetCarMoving();
        // event system
        danger.OnAllCarsLeft += MergeClear;
        // maybe instead of waiting time, wait for the danger zone to be clear again?

        // time system
        //yield return new WaitForSeconds(0.4f);
        // 0.4s is up before merge clear
        // but I think merge clear works a bit better. More reliable?
        //Debug.Log("0.4s up");
        //stopper.SetCarMoving();
        //stopper.SetStopper(false);
    }

    public void SetCarMoving()
    {
        if (cars.Count > 0)
        {
            cars[0].ResumeMoving(true);
        }
    }

    public void SetStopper(bool active)
    {
       this.active = active;
    }
}
