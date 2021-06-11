using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{

    public float minDist = 1.2f; // center to center for now
    public float maxDist = 1.5f;

    // for debugging
    // these are different for diff cars.
    // maybe bc of the different times the numbers are calculated?
    // one is calculated, then one moves, then another uses the new position for the calc
    public float distAhead = 0;
    public float distBehind = 0;

    public float currentSpeed = 0;
    public float maxSpeed = 2;
    public float minSpeed = 1;

    public Transform front; // for navigation. When there's a rail that can be followed, use that instead
    public Car behind;

    public Rail rail;
    protected int railIndex = 0;
    protected Vector3 targetPoint;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CalculateSpeed();

        if (rail.Ready())
        {
            CalculateTargetPoint();

            transform.forward = (targetPoint - transform.position).normalized;

            transform.position = Vector3.MoveTowards(transform.position, targetPoint, currentSpeed * Time.fixedDeltaTime);

            if (Vector3.Distance(transform.position, targetPoint) < 0.01f)
            {
                railIndex++;
            }
        }
        if(currentSpeed > maxSpeed)
        {
            currentSpeed -= 0.01f; // lose speed over time
            // when being pulled, the car gains extra speed so it's not quite always at max length
            // I want to use that extra speed to get within the dist bounds, but never hit the min dist while being pulled
            // this is the workaround for that
            //currentSpeed = Mathf.Max(maxSpeed, currentSpeed); would keep the speed at 10 instead of 9.999901231. probably doesn't matter
        }
    }

    public void Pull(float accel)
    {
        currentSpeed += accel;
        currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);
    }

    public float GetSpeed()
    {
        return currentSpeed;
    }

    public void SetSpeed(float newSpeed)
    {
        currentSpeed = newSpeed;
    }

    /// <summary>
    /// Set max speeds to be consistent for each car
    /// </summary>
    /// <param name="max"></param>
    public void SetMaxSpeedPropagate(float max)
    {
        maxSpeed = max;
        if(behind)
        {
            behind.SetMaxSpeedPropagate(max);
        }
    }

    protected virtual void CalculateSpeed()
    {
        if (behind)
        {
            float dist = Vector3.Distance(transform.position, behind.transform.position);
            distBehind = dist;

            if (dist > maxDist)
            {
                float carSpeed = behind.GetSpeed();
                if (carSpeed < currentSpeed)
                {
                    //pulling
                    float avgSpeed = (currentSpeed + carSpeed) / 2;
                    behind.SetSpeed(avgSpeed + 1 * Time.fixedDeltaTime); // make it move a bit faster so it catches up distance
                    currentSpeed = avgSpeed;
                }
                // else just started slowing down
            }
            else if (dist < minDist)
            {
                // this is either braking
                // or after braking is done
                float carSpeed = behind.GetSpeed();
                // if my speed is lower than theirs, I am braking. Slow down the back car
                // if my speed is higher, I am acclerating. Let me go and don't give the car any speed
                if (currentSpeed < carSpeed)
                {
                    // brake
                    float avgSpeed = (currentSpeed + carSpeed) / 2;
                    behind.SetSpeed(Mathf.Min(carSpeed, Mathf.Clamp(avgSpeed - 0.2f * Time.fixedDeltaTime, 0, maxSpeed))); // don't give the car any speed, only take
                    currentSpeed = avgSpeed;
                }
                else
                {
                    // do nothing
                    // let me keep my speed
                    // behind car keeps its current speed
                }

            }
        }
        if(front != null)
        {
            distAhead = Vector3.Distance(transform.position, front.position);
        }
    }

    protected void CalculateTargetPoint()
    {
        targetPoint = rail.GetPoint(railIndex);
    }
}
