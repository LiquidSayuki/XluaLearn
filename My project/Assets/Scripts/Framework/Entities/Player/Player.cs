using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Player : Character
{
    public Vector2 inputVec;
    private Vector2 mousePos;

    private float minDamageTakeInterval = 0.8f;
    private float timer;

    public GameObject[] guns;
    private AbstractGun currentGun;
    private int gunNum;

    private new SpriteRenderer renderer;
    private Animator animator;

    Rigidbody2D rb;

    // public event UnityAction onFire = delegate { };
    // public event UnityAction onStopFire = delegate { };

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        GameObject go = Instantiate(guns[0], this.transform);
        currentGun = go.GetComponent<AbstractGun>();

        GameManager.Instance.RegisterPlayer(this);

        timer = minDamageTakeInterval;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        inputVec = context.ReadValue<Vector2>();
    }

    void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        renderer.flipX = mousePos.x < transform.position.x;

        if (timer >= 0)
        {
            this.timer -= Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        Vector2 nextVec = inputVec * (speed * Time.fixedDeltaTime);
        rb.MovePosition(rb.position + nextVec);
    }

    void LateUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            this.currentGun.Fire();
        }

        animator.SetFloat("Speed", inputVec.magnitude);
    }

    #region TakingDamage
    public override void OnDamageTake(float damage)
    {
        // ÎÞµÐÖ¡
        if (this.timer <= 0)
        {
            base.OnDamageTake(damage);
            UIManager.Instance.UpdateHealth(this.currentHealth);
            timer = minDamageTakeInterval;
        }
    }
    public override void Die()
    {
        base.Die();
        animator.SetTrigger("Dead");
    }


    #endregion

    #region Fire(Abandoned)

    /* public void OnFire(InputAction.CallbackContext context)
     {
         if (context.phase == InputActionPhase.Performed)
         {
             //onFire.Invoke();
             Fire();
         }
         if (context.phase == InputActionPhase.Canceled)
         {
             //onStopFire.Invoke();
             StopFire();
         }
     }


     private void Fire()
     {
         currentGun.Fire();
     }

     private void StopFire()
     {
         currentGun.StopFire();
     }*/


    #endregion
}
