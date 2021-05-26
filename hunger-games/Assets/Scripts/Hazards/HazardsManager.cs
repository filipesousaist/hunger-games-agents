using System.Collections.Generic;
using UnityEngine;
using static Const;

public class HazardsManager : MonoBehaviour
{
    public int RADIUS_HAZARD_SPAWN;

    private int timeslot = 0;
    private float timer = 0;

    public Hazard fire;
    public Hazard fog;
    public Hazard rain;
    public Hazard radiation;

    private Hazard[] hazards;
    
    // Start is called before the first frame update
    void Start()
    {
        List<Vector3>[] regions = GetRegions();

        hazards = new Hazard[NUM_REGIONS];
        int[] indexes = Utils.ShuffledArray(NUM_REGIONS);
        Hazard[] unsorted_hazards = { fog, fire, rain, radiation, fog, fire, rain, radiation };
        int[] region_order = Utils.ShuffledArray(1, NUM_REGIONS + 1);

        for (int i = 0; i < NUM_REGIONS; i ++)
        {
            Hazard newHazard = hazards[indexes[i]] = Instantiate(unsorted_hazards[i]);
            int r = region_order[i];
            newHazard.SetRegion(r, regions[r]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= hazards[timeslot].DURATION)
        {
            timer = 0;

            hazards[timeslot].Stop();
            timeslot = (timeslot + 1) % NUM_REGIONS;
            hazards[timeslot].Begin();
        }
    }

    public List<Vector3>[] GetRegions()
    {
        List<Vector3>[] regions = new List<Vector3>[NUM_REGIONS + 1];
        for (int i = 0; i < NUM_REGIONS + 1; i ++)
            regions[i] = new List<Vector3>();

        for (int x = -249; x <= 249; x ++)
            for (int z = -249; z <= 249; z ++)
            {
                Vector3 point = new Vector3(x, 1, z);
                regions[GetRegion(point)].Add(point);
            }
        return regions;
    }

    public int GetRegion(Vector3 coordinates)
    {
        float x = coordinates.x;
        float z = coordinates.z;
        if (Mathf.Abs(x) != Mathf.Abs(z) && (x * x + z * z > RADIUS_HAZARD_SPAWN * RADIUS_HAZARD_SPAWN))
        {
            if (z > x && z > -x)
            {
                if (x < 0)
                    return 1;
                return 2;
            }
            if (z < x && z > -x)
            {
                if (z > 0)
                    return 3;
                return 4;
            }
            if (z < x && z < -x)
            {
                if (x > 0)
                    return 5;
                return 6;
            }
            if (z > x && z < -x)
            {
                if (z < 0)
                    return 7;
                return 8;
            }
        }
        return 0;
    }

    public int GetTimeslot()
    {
        return timeslot;
    }
}
