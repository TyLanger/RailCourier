using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TownProcessor : MonoBehaviour, ICanHoldCrate
{
    public TownProcessNode processNode;
    public ClawGun clawGun;

    List<Crate> crateList;

    bool clawHasCrate = false;
    bool grabbingCrates = false;

    public float timeBetweenRequests = 10;
    float timeOfNextRequest = 0;

    List<CrateType> requests;
    int maxRequests = 6;
    int currentRequests = 0;
    // if you hit 7 requests, you lose?
    int correctDeliveries = 0;
    // win: get both to 10 correct?

    List<CrateType> standby;
    int standbySize = 2;

    public Image[] images;
    public Sprite[] sprites;

    public Image[] standbyUI;
    public GameObject standbyText;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var item in images)
        {
            item.gameObject.SetActive(false);
        }

        requests = new List<CrateType>();
        standby = new List<CrateType>();
        crateList = new List<Crate>();
        processNode.OnCarEntered += CarEntered;
        clawGun.claw.OnHitCrate += CrateHit;
    }

    void Update()
    {
        if(timeOfNextRequest < Time.time)
        {
            timeOfNextRequest = Time.time + timeBetweenRequests;
            GenerateRequest();
        }
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
            //Debug.Log($"Count: {crateList.Count}");
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
        //Debug.Log($"Delivered a crate of {c.crateType}");

        FulfillRequest(c.crateType);

        Destroy(c.gameObject);
    }

    IEnumerator CheckStandby(CrateType c)
    {
        yield return new WaitForSeconds(1);
        if(standby.Contains(c))
        {
            //correctDeliveries++;
            //currentRequests--;
            //requests.Remove(c);
            standby.Remove(c);

            Debug.Log($"Found a match in standby for {c}");

            for (int i = 0; i < standbyUI.Length; i++)
            {
                if(standbyUI[i].gameObject.activeSelf)
                {
                    if(standbyUI[i].sprite == sprites[(int)c])
                    {
                        standbyUI[i].gameObject.SetActive(false);
                    }
                }
            }

            if(standby.Count == 0)
            {
                standbyText.SetActive(false);
            }
            // play special standby VFX

            FulfillRequest(c);
        }
    }

    void FulfillRequest(CrateType c)
    {
        if(requests.Contains(c))
        {
            Debug.Log($"{gameObject.name} actually wanted {c}. Good job");

            for (int i = 0; i < images.Length; i++)
            {
                // not active. Not our guy
                if (!images[i].gameObject.activeSelf)
                    continue;
                // if the sprite matches, deactivate it
                if(images[i].sprite == sprites[(int)c])
                {
                    images[i].gameObject.SetActive(false);
                    break;
                }
            }

            correctDeliveries++;
            currentRequests--;
            requests.Remove(c);
        }
        else
        {
            //Debug.Log($"No one even likes {c}. Idiot");

            // add item to standby
            if (standby.Count < standbySize)
            {
                for (int i = 0; i < standbyUI.Length; i++)
                {
                    if (standbyUI[i].gameObject.activeSelf)
                        continue;
                    standbyUI[i].sprite = sprites[(int)c];
                    standbyUI[i].gameObject.SetActive(true);
                    break;
                }

                standby.Add(c);
                standbyText.SetActive(true);
                Debug.Log($"Added {c} to standby");
            }
        }
    }

    void GenerateRequest()
    {
        CrateType newRequest = (CrateType)Random.Range(0, System.Enum.GetNames(typeof(CrateType)).Length);
        //Debug.Log($"{gameObject.name} wants {newRequest}");

        for (int i = 0; i < images.Length; i++)
        {
            if(images[i].gameObject.activeSelf)
            {
                continue;
            }
            // find the first inactive one
            // give it the right sprite
            images[i].sprite = sprites[(int)newRequest];
            images[i].gameObject.SetActive(true);
            break;
        }
        
        requests.Add(newRequest);
        currentRequests++;

        StartCoroutine(CheckStandby(newRequest));

        if(currentRequests > maxRequests)
        {
            Debug.Log("You lose");
        }
    }
}
