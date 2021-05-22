using UnityEngine;

public class Fire : Hazard
{
    public int ENERGY_LOSS;

    public float BURN_BERRIES_CHANCE;

    private float timer;

    protected override void Harm(Agent agent)
    {
        agent.LoseEnergy(ENERGY_LOSS);
    }

    protected override void OnUpdate()
    {
        if (active)
        {

            timer += Time.deltaTime;
            if (timer >= PERIOD)
            {
                timer -= PERIOD;
                BurnBerries();
            }
        }
    }

    private void BurnBerries()
    {
        foreach (Bush bush in FindObjectsOfType<Bush>())
        {
            if (hazardManager.GetRegion(bush.transform.position) == index && Random.Range(0f, 1f) <= BURN_BERRIES_CHANCE)
            {
                if (bush.hasBerries)
                    bush.ChangeBushType();
                else
                    bush.timer = 0;
            }
        }
    }
}

