using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownProcessor : MonoBehaviour, ICanHoldCrate
{
    public TownProcessNode processNode;
    public ClawGun clawGun;

    List<Crate> crateList;

    bool clawHasCrate = false;
    bool grabbingCrates = false;

    // Start is called before the first frame update
    void Start()
    {
        crateList = new List<Crate>();
        processNode.OnCarEntered += CarEntered;
        clawGun.claw.OnHitCrate += CrateHit;
    }


    void CarEntered(Car car)
    {

        if (car.crates != null && car.crates.Length > 0)
        {
            // update the list of crates
            car.ActivateLocalCrates();
            crateList.AddRange(car.GetLocalCrates());
            // only start firing if you haven't started yet
            if (!grabbingCrates)
            {
                StartCoroutine(GrabCrateCR());
            }
        }
    }

    IEnumerator GrabCrateCR()
    {
        grabbingCrates = true;
        yield return null;
        // will this work if the list is updated while the coroutine is running?
        while(crateList.Count > 0)
        {
            Debug.Log($"Count: {crateList.Count}");
            Vector3 cratePos = crateList[0].transform.position;

            // what if the crate is too far away?
            // could mean that it left the area
            // or it's somehow not in range?
            float dist = Vector3.Distance(cratePos, transform.position);
            if (dist > clawGun.clawMaxDist)
            {
                crateList.RemoveAt(0);
                Debug.Log($"Crate out of range {dist}");
                continue;
            }

            clawGun.SetLookPoint(cratePos, true);
            clawGun.FireClaw(cratePos);

            // do i need has crate?
            // !clawHasCrate && 
            while (clawGun.IsShooting())
            {
                // keep aiming
                clawGun.SetLookPoint(cratePos, true);
                yield return null;
            }

            // claw has shot and come back
            crateList.RemoveAt(0);
        }

        // send the car away
        processNode.PushCarAway();
        grabbingCrates = false;
    }

    void CrateHit()
    {
        clawHasCrate = true;
    }

    public bool CanHoldCrate()
    {
        return true;
    }

    public void PlaceCrate(Crate c)
    {
        Debug.Log($"Delivered a crate of {c.crateType}");
        Destroy(c);
    }
}
