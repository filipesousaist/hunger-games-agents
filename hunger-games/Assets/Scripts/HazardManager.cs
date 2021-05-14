using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms;

public class HazardManager : MonoBehaviour
{
    private int RADIUS_HAZARD_SPAWN;

    private List<Vector3>[] sections;

    enum Hazard
    {
        FIRE, FOG, RAIN, RADIATION,
    }

    private Hazard[] hazard_order;
    private int[] region_order;
    
    // Start is called before the first frame update
    void Start()
    {
        sections = new List<Vector3>[8];
        
        int i,j;
        
        for (i=-250; i<250; i++)
        {
            for (j = -250; j < 250; j++)
            {
                Vector3 point = new Vector3(i, 1, j);
                sections[GETSection(point)].Add(point);
            }
        }

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
       //TODO
        
        
    }

    public int GETSection(Vector3 coordinates)
    {
        float x = coordinates.x;
        float z = coordinates.z;
        if (Mathf.Abs(x) == Mathf.Abs(z) || (x * x) + (z * z) <= RADIUS_HAZARD_SPAWN)
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
