using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public Camera mainCam;
    public ClawGun gun;
    Vector3 eyeLookPoint;

    public ParticleSystem smoke;
    public ParticleSystem[] sparks;
    bool sparksOn = false;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        StartCoroutine(AdjustSmoke(2));
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

        Ray CameraRay = mainCam.ScreenPointToRay(Input.mousePosition);
        Plane eyePlane = new Plane(Vector3.up, Vector3.zero);

        if (eyePlane.Raycast(CameraRay, out float cameraDist))
        {
            Vector3 lookPoint = CameraRay.GetPoint(cameraDist);
            eyeLookPoint = new Vector3(lookPoint.x, gun.transform.position.y, lookPoint.z);

            /*transform.LookAt(eyeLookPoint);
            claw.transform.LookAt(eyeLookPoint);
            claw.transform.position = transform.position + transform.forward * clawRestDist;*/
            gun.SetLookPoint(eyeLookPoint);
        }

        if (Input.GetButtonDown("Fire1"))
        {
            if (CanHoldCrate())
            {
                gun.FireClaw(eyeLookPoint);
            }
        }

       

        CheckSparks();
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

    IEnumerator AdjustSmoke(float newRate, float sustainTime = 1)
    {
        ParticleSystem.EmissionModule emission = smoke.emission;
        emission.rateOverTime = new ParticleSystem.MinMaxCurve(newRate);

        yield return new WaitForSeconds(sustainTime);

        //ParticleSystem.EmissionModule emission2 = smoke.emission;
        emission.rateOverTime = new ParticleSystem.MinMaxCurve((throttle*2) +1);
    }

    void StartSparks()
    {
        // sparks on
        if (!sparksOn)
        {
            for (int i = 0; i < sparks.Length; i++)
            {
                sparks[i].Play();
            }
            sparksOn = true;
        }
    }

    void StopSparks()
    {
        if (sparksOn)
        {
            for (int i = 0; i < sparks.Length; i++)
            {
                sparks[i].Stop();
            }
            sparksOn = false;
        }
    }

    void CheckSparks()
    {
        if(sparksOn)
        {
            switch(throttle)
            {
                case 0:
                    if(currentSpeed < 0.2f)
                    {
                        StopSparks();
                    }
                    break;

                case 1:
                    if(currentSpeed < (throttle1Speed + 0.1f))
                    {
                        StopSparks();
                    }
                    break;
                case 2:
                    StopSparks();
                    break;
            }
        }

    }

    void AdjustThrottle(int increment)
    {
        float smokeRate = 1;
        float sustainTime = 0;

        if (throttle == 0 && increment == 1)
        {

            smokeRate = 10;
            sustainTime = 2;
        }
        else if(throttle == 1 && increment == 1)
        {

            smokeRate = 20;
            sustainTime = 2;
        }

        StartCoroutine(AdjustSmoke(smokeRate, sustainTime));

        if(increment < 0)
        {
            StartSparks();
        }

        throttle = Mathf.Clamp(throttle + increment, 0, 2);
        OnThrottleChanged?.Invoke(throttle);
    }
}
