using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Claw : MonoBehaviour
{
    public event Action OnHitCrate;

    public Transform crateRestSpot;
    Crate currentCrate;
    bool holdingCrate = false;


    public bool HasCrate()
    {
        return holdingCrate;
    }

    public Crate ReleaseCrate()
    {
        holdingCrate = false;
        return currentCrate;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!holdingCrate)
        {
            if (other.CompareTag("Crate"))
            {
                other.transform.position = crateRestSpot.position;
                other.transform.rotation = crateRestSpot.rotation;
                other.transform.parent = transform;

                currentCrate = other.GetComponent<Crate>();
                currentCrate.ActivateTrigger(false);
                holdingCrate = true;

                OnHitCrate?.Invoke();
            }
        }
    }
}
