using UnityEngine;
using UnityEngine.UIElements;

public class swordscript : MonoBehaviour
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
    void OnEnable()
    {
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }

    void FixedUpdate()
    {
        Attackinput();
    }

    public void Attackinput()
    {
        if (controls.Gameplay.Attack.triggered)
        {
            attack(controls.Gameplay.Attack.ReadValue<int>());
        }
    }
    public bool attack(int attacktype)
    {
        if (attacktype == 1)
        {
            Debug.Log("Stabattack");
            anim.SetTrigger("Slash");
        }
        return true;
    }
}
