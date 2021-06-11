using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Train : Car
{
    public float acceleration = 1;
    float accel = 0;
    public float brakeAccel = -0.5f;

    // -1 is revers, 0 is stop, 1 is forward, 2 is forward fast
    public int throttle = 0;

    //public Rail rail;
    //int railIndex = 0;
    //Vector3 targetPoint;

    // Start is called before the first frame update
    void Start()
    {
        SetMaxSpeedPropagate(maxSpeed);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            // forward
            accel = acceleration;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            //reverse;
            accel = brakeAccel;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //float dist = Vector3.Distance(transform.position, behind.transform.position);
        currentSpeed += accel * Time.fixedDeltaTime;
        currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);

        CalculateSpeed();
        if (rail.Ready())
        {
            CalculateTargetPoint();

            transform.forward = (targetPoint - transform.position).normalized;
            transform.position = Vector3.MoveTowards(transform.position, targetPoint, Time.fixedDeltaTime * currentSpeed);


            if (Vector3.Distance(transform.position, targetPoint) < 0.01f)
            {
                railIndex++;
            }
        }
    }
}
