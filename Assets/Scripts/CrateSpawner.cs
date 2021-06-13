using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateSpawner : MonoBehaviour
{
    public float timeBetweenSpawns = 2;
    float timeOfNextSpawn = 0;

    public Crate cratePrefab;
    public CrateType typeToSpawn;
    public bool random = false;

    Transform lastSpawned;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > timeOfNextSpawn)
        {
            timeOfNextSpawn = Time.time + timeBetweenSpawns;
            if (lastSpawned == null || Vector3.Distance(lastSpawned.position, transform.position) > 0.1f)
            {
                SpawnCrate();
            }
        }
    }


    void SpawnCrate()
    {
        Crate c = Instantiate(cratePrefab, transform.position, transform.rotation);
        if(random)
        {
            typeToSpawn = (CrateType)Random.Range(0, System.Enum.GetNames(typeof(CrateType)).Length);
        }
        c.SetType(typeToSpawn);

        lastSpawned = c.transform;
    }
}
