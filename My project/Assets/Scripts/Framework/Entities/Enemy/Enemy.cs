using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Enemy : Character
{
    public float damage;
    bool isLive = true;

    Rigidbody2D self;
    Rigidbody2D target;

    SpriteRenderer spriter;
    Animator animator;

    private ObjectPool<Enemy> _pool;

    void Start()
    {
        self = GetComponent<Rigidbody2D>();
        target = GameManager.Instance.Player.GetComponent<Rigidbody2D>();

        spriter = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void LateUpdate()
    {
            spriter.flipX = target.position.x - self.position.x < 0;
    }

    void FixedUpdate()
    {
        if (!isLive)
        {
            return;
        }
        Vector2 dirVec = target.position - self.position;
        Vector2 nexVec = dirVec.normalized * (speed * Time.fixedDeltaTime);
        //self.MovePosition(self.position + nexVec);
        self.velocity = nexVec;
    }

    #region Difficulty
    public void InitEnemy(int difficulty)
    {
        this.currentHealth = this.maxHealth * (difficulty/5f);
        this.damage = this.damage * (difficulty/5f);
    }
    #endregion

    #region DealDamage
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Player>().OnDamageTake(this.damage);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Player>().OnDamageTake(this.damage);
        }
    }
    #endregion

    #region TakeDamage
    public override void OnDamageTake(float damage)
    {
        currentHealth -= damage;
        if (currentHealth > 0)
        {
            animator.SetTrigger("Hit");
        }
        else
        {
            Die();
        }
    }

    public override void Die()
    {
        base.Die();
        animator.SetBool("Dead", true);
    }

    public void Release()
    {
        _pool.Release(this);
    }
    #endregion

    #region Pool
    public void SetPool(ObjectPool<Enemy> pool)
    {
        _pool = pool;
    }
    #endregion
}
