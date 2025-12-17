using Unity.Netcode;
using UnityEngine;

public class dummyscript : BaseEntety
{

    override public void Awake()
    {
        base.Awake();
        maxHealth = 20;
        health.Value = maxHealth;
        
    }
}
