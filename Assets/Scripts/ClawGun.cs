using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawGun : MonoBehaviour
{
    public Camera mainCam;
    public Claw claw;
    public ICanHoldCrate crateProcessor;

    public float clawFireSpeed = 1;
    public float clawRetractSpeed = 1;

    public float clawRestDist = 1;
    public float clawMaxDist = 5;

    bool shooting = false;
    bool crateHit = false;

    Vector3 eyeLookPoint;

    // Start is called before the first frame update
    void Start()
    {
        claw.OnHitCrate += StopClaw;
        //train = GetComponentInParent<Car>();
        crateProcessor = GetComponentInParent<ICanHoldCrate>();
    }

    public void SetLookPoint(Vector3 lookPoint, bool cheatMode = false)
    {
        // cheatmode lets it track while moving. For the town to use so they don't miss
        if (!shooting || cheatMode)
        {
            transform.LookAt(lookPoint);
            claw.transform.LookAt(lookPoint);
            if (!shooting)
            {
                claw.transform.position = transform.position + transform.forward * clawRestDist;
            }
        }
    }

    public void FireClaw(Vector3 point)
    {
        if (!shooting)
        {
            // start moving towards the point
            // when you get there, move back
            shooting = true;
            StartCoroutine(ClawMove(point));
        }
    }

    void StopClaw()
    {
        crateHit = true;
    }

    public bool IsShooting()
    {
        return shooting;
    }

    IEnumerator ClawMove(Vector3 point)
    {
        crateHit = false;
        float dist = Vector3.Distance(point, claw.transform.position);
        float clawReach = Vector3.Distance(claw.transform.position, transform.position);
        while (dist > 0.1f && clawReach < clawMaxDist && !crateHit)
        {
            claw.transform.position = Vector3.MoveTowards(claw.transform.position, point, clawFireSpeed * Time.fixedDeltaTime);
            dist = Vector3.Distance(point, claw.transform.position);
            clawReach = Vector3.Distance(claw.transform.position, transform.position);
            yield return new WaitForFixedUpdate();
        }

        // hold position for a bit
        yield return new WaitForSeconds(0.2f);

        dist = Vector3.Distance(claw.transform.position, transform.position);
        while(dist > 0.1f)
        {
            claw.transform.position = Vector3.MoveTowards(claw.transform.position, transform.position, clawRetractSpeed * Time.fixedDeltaTime);
            dist = Vector3.Distance(transform.position, claw.transform.position);

            yield return new WaitForFixedUpdate();
        }

        // does the claw have something?
        // claw.GetItem?
        if (claw.HasCrate())
        {
            if (crateProcessor.CanHoldCrate())
            {
                crateProcessor.PlaceCrate(claw.ReleaseCrate());
            }
        }
        shooting = false;
    }
}
