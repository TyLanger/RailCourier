using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawGun : MonoBehaviour
{
    public Camera mainCam;
    public Transform claw;
    public float clawFireSpeed = 1;
    public float clawRetractSpeed = 1;

    public float clawRestDist = 1;
    public float clawMaxDist = 5;

    bool shooting = false;

    Vector3 eyeLookPoint;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!shooting)
        {
            // look at mouse
            Ray CameraRay = mainCam.ScreenPointToRay(Input.mousePosition);
            Plane eyePlane = new Plane(Vector3.up, Vector3.zero);


            if (eyePlane.Raycast(CameraRay, out float cameraDist))
            {
                Vector3 lookPoint = CameraRay.GetPoint(cameraDist);
                eyeLookPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
                transform.LookAt(eyeLookPoint);
                claw.position = transform.position + transform.forward * clawRestDist;
            }

            if (Input.GetButtonDown("Fire1"))
            {
                FireClaw(eyeLookPoint);
            }
        }
    }

    void FireClaw(Vector3 point)
    {
        // start moving towards the point
        // when you get there, move back
        shooting = true;
        StartCoroutine(ClawMove(point));
    }

    IEnumerator ClawMove(Vector3 point)
    {
        float dist = Vector3.Distance(point, claw.position);
        float clawReach = Vector3.Distance(claw.position, transform.position);
        while (dist > 0.1f && clawReach < clawMaxDist)
        {
            claw.position = Vector3.MoveTowards(claw.position, point, clawFireSpeed * Time.fixedDeltaTime);
            dist = Vector3.Distance(point, claw.position);
            clawReach = Vector3.Distance(claw.position, transform.position);
            yield return new WaitForFixedUpdate();
        }

        // hold position for a bit
        yield return new WaitForSeconds(0.2f);

        dist = Vector3.Distance(claw.position, transform.position);
        while(dist > 0.1f)
        {
            claw.position = Vector3.MoveTowards(claw.position, transform.position, clawRetractSpeed * Time.fixedDeltaTime);
            dist = Vector3.Distance(transform.position, claw.position);

            yield return new WaitForFixedUpdate();
        }

        // does the claw have something?
        // claw.GetItem?

        shooting = false;
    }
}
