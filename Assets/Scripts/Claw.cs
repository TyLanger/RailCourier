using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Claw : MonoBehaviour
{
    public Camera mainCam;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // look at mouse
        Ray CameraRay = mainCam.ScreenPointToRay(Input.mousePosition);
        Plane eyePlane = new Plane(Vector3.up, Vector3.zero);

        if (eyePlane.Raycast(CameraRay, out float cameraDist))
        {
            Vector3 lookPoint = CameraRay.GetPoint(cameraDist);
            Vector3 eyeLookPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
            transform.LookAt(eyeLookPoint);
        }

        if (Input.GetButtonDown("Fire1"))
        {

        }
    }

    void FireClaw(Vector3 point)
    {
        // start moving towards the point
        // when you get there, move back
    }
}
