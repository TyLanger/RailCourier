using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
    public CrateType crateType;

    private void OnTriggerEnter(Collider other)
    {
        // did the claw grab me?
        Debug.Log("Something touched me");
        if(other.CompareTag("Claw"))
        {
            // claw touched me
            // I should have the claw do this...
            // it will know where to place the crate
            transform.position = other.transform.position;
            transform.parent = other.transform;
        }
    }

    public void ActivateTrigger(bool active)
    {
        GetComponent<BoxCollider>().enabled = active;
    }
}

public enum CrateType { Wood, Coal, Iron, Fish, Wheat, Bricks};
