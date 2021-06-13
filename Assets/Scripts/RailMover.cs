using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailMover : RailPoint
{

    public Transform startPoint;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Jump"))
        {
            // move whatever car in the are.
            MoveCar();
        }
    }

    void MoveCar()
    {
        if(cars.Count > 0)
        {
            // trains can't use this
            if (cars[0].GetType() == typeof(Train))
                return;

            //Debug.Log($"Count: {cars.Count}");
            cars[0].ChangeTracks(startPoint.position, altPoint);
            Exit(cars[0]);

        }
    }

    public override void Enter(Car car)
    {
        base.Enter(car);
        car.Highlight(true);
    }
}
