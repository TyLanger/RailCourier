using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coupler : MonoBehaviour
{

    public bool frontCouple = true;
    public Car car;

    private void Awake()
    {
        car = GetComponentInParent<Car>();
        if (car)
        {
            if (frontCouple)
            {
                car.frontCoupler = this;
            }
            else
            {
                car.backCoupler = this;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Coupler otherCoupler = other.GetComponent<Coupler>();
        // does this run on both objects? Or just the one with an rbody? both
        if(otherCoupler)
        {
            //Debug.Log("Hit car"); // both say it
            // are we opposites
            if(frontCouple && !otherCoupler.frontCouple)
            {
                if(frontCouple)
                {
                    Couple(otherCoupler.car);
                }
            }
        }
    }

    void Couple(Car frontCar)
    {
        car.Couple(frontCar);
    }
}
