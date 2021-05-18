using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms;

public class HazardManager : MonoBehaviour
{
    public int RADIUS_HAZARD_SPAWN;

    private List<Vector3>[] regions;

    private int timeslot = 0;
    private float timer = 0;
    public float DURATION;

    enum Hazard
    {
        FIRE, FOG, RAIN, RADIATION,
    }

    public GameObject fire;
    public GameObject fog;
    public GameObject rain;
    public GameObject radiation;

    private Dictionary<Hazard, GameObject> hazard_correspondence;
    private Hazard[] hazard_order;
    private int[] region_order;
    private List<GameObject> curr_hazards = new List<GameObject>();
    
    // Start is called before the first frame update
    void Start()
    {
        regions = new List<Vector3>[9];
        for (int index = 0; index < regions.Length; index++)
        {
            regions[index] = new List<Vector3>();
        } 
        hazard_correspondence = new Dictionary<Hazard, GameObject>()
        {
            {Hazard.FIRE, fire},
            {Hazard.FOG, fog},
            {Hazard.RAIN, rain},
            {Hazard.RADIATION, radiation},
        };
        
        int i,j;
        
        for (i=-250; i<250; i++)
        {
            for (j = -250; j < 250; j++)
            {
                Vector3 point = new Vector3(i, 1, j);
                regions[GETSection(point)].Add(point);
            }
        }
        hazard_order = new Hazard[8];
        int[] indexes = Utils.ShuffledArray(8);
        Hazard[] hazards = {Hazard.FOG, Hazard.FIRE, Hazard.RAIN, Hazard.RADIATION, Hazard.FOG, Hazard.FIRE, Hazard.RAIN, Hazard.RADIATION};
        for (i = 0; i < 8; i++)
        { 
            hazard_order[indexes[i]] = hazards[i];
        }
        region_order = Utils.ShuffledArray(8);
        for (i = 0; i < 8; i++)
        { 
            region_order[i] ++;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= DURATION)
        {
            for (int i = 0; i < curr_hazards.Count; i++)
            {
                Destroy(curr_hazards[i]);
            }

            curr_hazards.Clear();
            List<Vector3> new_region = regions[region_order[timeslot]];
            GameObject new_hazard = hazard_correspondence[hazard_order[timeslot]];
            foreach (Vector3 position in new_region)
            {
                if (Random.Range(0, 100) == 0)
                {
                    GameObject curr_hazard = Instantiate(new_hazard);
                    curr_hazard.transform.position = position;
                    curr_hazards.Add(curr_hazard);
                }
            }

            timer = 0;
            timeslot = (timeslot + 1) % 8;
        }
    }

    public int GETSection(Vector3 coordinates)
    {
        float x = coordinates.x;
        float z = coordinates.z;
        if (Mathf.Abs(x) == Mathf.Abs(z) || ((x * x) + (z * z) <= RADIUS_HAZARD_SPAWN*RADIUS_HAZARD_SPAWN))
        {
            return 0;
        }

        if (x < z && 0 < x && x < 250 && 0 < z && z < 250)
        {
            return 1;
        }

        if (x > z && 0 < x && x < 250 && 0 < z && z < 250)
        {
            return 2;
        }

        if (-x > z && 0 < x && x < 250 && -250 < z && z < 0)
        {
            return 3;
        }

        if (-x < z && 0 < x && x < 250 && -250 < z && z < 0)
        {
            return 4;
        }

        if (x < z && -250 < x && x < 0 && -250 < z && z < 0)
        {
            return 5;
        }

        if (x > z && -250 < x && x < 0 && -250 < z && z < 0)
        {
            return 6;
        }

        if (-x > z && -250 < x && x < 0 && 0 < z && z < 250)
        {
            return 7;
        }

        if (-x < z && -250 < x && x < 0 && 0 < z && z < 250)
        {
            return 8;
        }

        return 0;
    }
}
