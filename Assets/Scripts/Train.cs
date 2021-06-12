using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Train : Car
{
    public event Action<int> OnThrottleChanged;

    public float maxAccel = 2;
    public float acceleration = 1;
    public float brakeAccel = 0f;
    public float reverseAccel = -0.5f;
    
    float accel = 0;

    public float throttle1Speed = 2;
    public float throttle2Speed = 3;
    public float reverseSpeed = -1;

    // -1 is revers, 0 is stop, 1 is forward, 2 is forward fast
    public int throttle = 0;

    //public Rail rail;
    //int railIndex = 0;
    //Vector3 targetPoint;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        SetMaxSpeedPropagate(throttle2Speed);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            // forward
            //accel = acceleration;
            //throttle = Mathf.Min(throttle + 1, 2);
            AdjustThrottle(1);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            //reverse;
            //accel = brakeAccel;
            //throttle = Mathf.Max(throttle - 1, -1);
            AdjustThrottle(-1);
        }
        if(Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("Jetison back");
            JetisonLast();
        }

    }

    // Update is called once per frame
    protected override void FixedUpdate()
    {
        //float dist = Vector3.Distance(transform.position, behind.transform.position);
        currentSpeed += GetAccel() * Time.fixedDeltaTime;
        currentSpeed = GetCurrentSpeedClamped(); // Mathf.Clamp(currentSpeed, 0, GetMaxSpeed());

        base.FixedUpdate();

        /*
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
        */
    }

    float GetAccel()
    {
        switch(throttle)
        {
            case -1:
                return reverseAccel;
            case 0:
                return brakeAccel;
            case 1:
                if(currentSpeed > throttle1Speed)
                {
                    // going faster than I want to be going; brake
                    return brakeAccel;
                }
                return acceleration;
            case 2:
                return maxAccel;
        }
        return 0;
    }

    float GetCurrentSpeedClamped()
    {
        // clamps based on current throttle
        float current = currentSpeed;

        switch(throttle)
        {
            case -1:
                current = Mathf.Max(current, reverseSpeed);
                break;
            case 0:
                current = Mathf.Max(current, 0);
                break;
            case 1:
                current = Mathf.Min(current, throttle2Speed); // don't just immediately set it to this speed
                break;
            case 2:
                current = Mathf.Min(current, throttle2Speed);
                break;
        }

        return current;
    }

    void AdjustThrottle(int increment)
    {
        throttle = Mathf.Clamp(throttle + increment, -1, 2);
        OnThrottleChanged?.Invoke(throttle);
    }
}
