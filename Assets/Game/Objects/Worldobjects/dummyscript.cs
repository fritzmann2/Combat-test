using Unity.Netcode;
using UnityEngine;

public class dummyscript : BaseEntety
{
    public GameObject hpbarfiller;
    override public void Awake()
    {
        base.Awake();
        maxHealth = 20;
        health.Value = maxHealth;
    }
    public override void OnHealthChanged(float previousValue, float newValue)
    {
        base.OnHealthChanged(previousValue, newValue);
        if (hpbarfiller != null)
        {
            hpbarfiller.transform.localScale = new Vector3 (newValue / maxHealth, 1f, 1f);
        }
    }
}
