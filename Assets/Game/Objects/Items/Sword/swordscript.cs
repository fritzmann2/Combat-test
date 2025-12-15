using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class swordscript : NetworkBehaviour
{

    private GameControls controls;
    private BoxCollider2D bx;
    private Animator anim;

    void Awake()
    {
        controls = new GameControls();
        bx = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            controls.Enable();
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner)
        {
            controls.Disable();
        }
    }

    void FixedUpdate()
    {
        if (!IsOwner) return;
        Attackinput();
    }

    public void Attackinput()
    {
        if (controls.Gameplay.Attack.triggered)
        {
            int attackValue = (int)controls.Gameplay.Attack.ReadValue<float>(); 
            
            if (attackValue > 0)
            {
                RequestAttackServerRpc(attackValue);
            }
        }
    }

    [ServerRpc]
    private void RequestAttackServerRpc(int attackType)
    {
        PlayAttackClientRpc(attackType);
    }

    [ClientRpc]
    private void PlayAttackClientRpc(int attackType)
    {
        if (attackType == 1)
        {
            Debug.Log("Attack 2: Slice");
            anim.SetTrigger("Slice");
        }
        else if (attackType == 2)
        {
            Debug.Log("Attack 1: Stab");
            anim.SetTrigger("Stab"); 
        }
        else if (attackType == 3)
        {
            Debug.Log("Attack 3: Charge");
        }
    }
}
