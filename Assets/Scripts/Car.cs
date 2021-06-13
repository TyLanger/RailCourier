using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour, ICanHoldCrate
{

    public float minDist = 1.2f; // center to center for now
    public float maxDist = 1.5f;
    public float breakDist = 2.3f;

    // for debugging
    // these are different for diff cars.
    // maybe bc of the different times the numbers are calculated?
    // one is calculated, then one moves, then another uses the new position for the calc
    public float distAhead = 0;
    public float distBehind = 0;

    public float currentSpeed = 0;
    public float maxSpeed = 2;
    public float minSpeed = 1;

    public Car front; // for navigation. When there's a rail that can be followed, use that instead
    public Car behind;
    public bool hasFront = false;
    public bool hasBehind = false;

    public Coupler frontCoupler;
    public Coupler backCoupler;

    protected Vector3 targetPoint;
    public RailPoint nextPoint;
    bool beingShoved = false;
    bool stopped = false;

    public GameObject ejectIndicator;
    public GameObject insertIndicator;

    public int crateSlots = 2;
    public Transform[] cratePoints;
    public Crate[] crates;
    // how to tell how many are filled and which ones?
    // some struct?

    // Start is called before the first frame update
    protected virtual void Start()
    {
        if(front != null)
        {
            hasFront = true;
        }
        if(behind != null)
        {
            hasBehind = true;
        }

        Highlight(false);
        DangerHighlight(false);
    }

    // Update is called once per frame
    protected virtual void FixedUpdate()
    {
        CalculateSpeed();

        if (!beingShoved)
        {
            CalculateTargetPoint();

            transform.forward = (targetPoint - transform.position).normalized;

            if (!stopped)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPoint, currentSpeed * Time.fixedDeltaTime);
            }
            if (Vector3.Distance(transform.position, targetPoint) < 0.01f)
            {
                nextPoint = nextPoint.GetNext(this);
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
        currentSpeed = Mathf.Max(0, currentSpeed - 0.1f*Time.fixedDeltaTime);

        if (hasFront && front != null)
        {
            if (Vector3.Distance(transform.position, front.transform.position) > breakDist)
            {
                // break off car in front
                // already have a method to split the behind car off. Using that
                Debug.Log($"{gameObject.name} broke from distance to {front.name}");
                front.SplitBack();
            }
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

    public void AddCurrentSpeedPropagate(float boost)
    {
        currentSpeed += boost;
        if(behind && hasBehind)
        {
            behind.AddCurrentSpeedPropagate(boost);
        }
    }

    public bool CanHoldCrate()
    {
        for (int i = 0; i < crates.Length; i++)
        {
            if (crates[i] == null)
            {
                return true;
            }
        }
        if(hasBehind && behind != null)
        {
            return behind.CanHoldCrate();
        }
        return false;
    }

    public void PlaceCrate(Crate c)
    {
        for (int i = 0; i < crates.Length; i++)
        {
            if (crates[i] == null)
            {
                c.transform.position = cratePoints[i].position;
                c.transform.parent = cratePoints[i];
                c.transform.rotation = cratePoints[i].rotation;
                crates[i] = c;
                c.ActivateTrigger(false);
                return;
            }
        }
        if (hasBehind && behind != null)
        {
            behind.PlaceCrate(c);
        }
    }

    public List<Crate> GetAllCrates(List<Crate> current)
    {
        if(crates != null && crates.Length > 0)
        {
            for (int i = 0; i < crates.Length; i++)
            {
                if (crates[i] != null)
                {
                    current.Add(crates[i]);
                }
            }
        }
        if(hasBehind && behind)
        {
            return behind.GetAllCrates(current);
        }
        return current;
    }

    public List<Crate> GetLocalCrates()
    {
        List<Crate> local = new List<Crate>();
        if (crates != null && crates.Length > 0)
        {
            for (int i = 0; i < crates.Length; i++)
            {
                if (crates[i] != null)
                {
                    local.Add(crates[i]);
                }
            }
        }
        return local;
    }

    public void ActivateLocalCrates()
    {
        if (crates != null && crates.Length > 0)
        {
            for (int i = 0; i < crates.Length; i++)
            {
                if (crates[i] != null)
                {
                    crates[i].ActivateTrigger(true);
                }
            }
        }
    }

    protected virtual void CalculateSpeed()
    {
        if (hasBehind && behind)
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
                    if (stopped)
                    {
                        behind.SetSpeed(0);
                    }
                    else
                    {
                        // brake
                        float avgSpeed = (currentSpeed + carSpeed) / 2;
                        behind.SetSpeed(Mathf.Min(carSpeed, Mathf.Clamp(avgSpeed - 0.2f * Time.fixedDeltaTime, 0, maxSpeed))); // don't give the car any speed, only take
                        currentSpeed = avgSpeed;
                    }
                }
                    // else
                    // do nothing
                    // let me keep my speed
                    // behind car keeps its current speed
                

            }
        }
        if(hasFront && front != null)
        {
            distAhead = Vector3.Distance(transform.position, front.transform.position);
        }
    }

    protected void CalculateTargetPoint()
    {
        targetPoint = nextPoint.transform.position;
    }

    protected void JetisonLast()
    {
        if(hasBehind && behind)
        {
            behind.JetisonLast();
        }
        else
        {
            Decouple();
        }
    }

    public void Couple(Car frontCar)
    {
        frontCar.AttachToBack(this);
        front = frontCar;
        hasFront = true;
        frontCoupler.gameObject.SetActive(false);
    }

    public void AttachToBack(Car behindCar)
    {
        behind = behindCar;
        hasBehind = true;
        backCoupler.gameObject.SetActive(false);
    }

    void Decouple()
    {
        front.DetachFromBack();
        hasFront = false;
        if(hasBehind)
        {
            hasBehind = false;
            behind.DetachFromFront();
        }
        StartCoroutine(ActivateCouplers());
    }

    IEnumerator ActivateCouplers()
    {
        // wait a bit so they don't just immediately reattach
        yield return new WaitForSeconds(3);
        frontCoupler.gameObject.SetActive(true);
        backCoupler.gameObject.SetActive(true);
    }

    public void DetachFromBack()
    {
        hasBehind = false;
        backCoupler.gameObject.SetActive(true);
    }

    public void DetachFromFront()
    {
        hasFront = false;
        frontCoupler.gameObject.SetActive(true);
    }

    public void DetachFromFrontWait()
    {
        StartCoroutine(DetachFromFrontCR());
    }

    IEnumerator DetachFromFrontCR()
    {
        yield return new WaitForSeconds(0.5f);

        DetachFromFront();
    }

    public void SplitBack()
    {
        if (hasBehind)
        {
            behind.DetachFromFrontWait();
        }
        DetachFromBack();
    }

    public void ChangeTracks(Vector3 oldPoint, RailPoint newPoint)
    {
        Decouple();
        nextPoint = newPoint;
        //Debug.Break();
        StartCoroutine(Shove(oldPoint, newPoint.transform.position));
    }

    IEnumerator Shove(Vector3 pivotPoint, Vector3 newPoint)
    {
        Vector3 ogForward = transform.forward;
        Vector3 newForward = (newPoint - pivotPoint).normalized;

        float distAlong = Vector3.Distance(pivotPoint, transform.position);

        beingShoved = true;

        for (int i = 0; i < 11; i++)
        {
            transform.forward = Vector3.Lerp(ogForward, newForward, (float)i / 10);
            transform.position = pivotPoint + Vector3.Lerp(ogForward * distAlong, newForward * distAlong, (float)i / 10);
            //Debug.Log($"{i} P: {transform.position} F: {transform.forward}");

            yield return null;
        }
        beingShoved = false;
        currentSpeed += 2;
        yield return new WaitForSeconds(0.3f);
        //Debug.Log($"Behind: {behind.name} speed: {behind.currentSpeed}");
        //behind.currentSpeed += 4; // need 4 to get the back 2 to attach to the front
        // might not work with 3 back cars...
        // should I boost all back cars by 2?
        behind.AddCurrentSpeedPropagate(2);
    }

    public void StopMoving()
    {
        stopped = true;
        currentSpeed = 0;
    }

    public void ResumeMoving(bool push = false)
    {
        stopped = false;
        if(push)
        {
            //Debug.Log($"Speed before push: {currentSpeed}");
            //currentSpeed += 4;
            AddCurrentSpeedPropagate(4);
        }
    }

    public void Highlight(bool active)
    {
        if (ejectIndicator)
        {
            ejectIndicator.SetActive(active);
        }
    }

    public void DangerHighlight(bool active)
    {
        insertIndicator.SetActive(active);
    }

    
}
