using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
    public float speed;
    public float damage;

    private Rigidbody2D rb;
    private ObjectPool<Bullet> _pool;

    public float destroyTime;
    private Coroutine DestroyBulletAfterTime;

    // 这里不用awake,子弹生成时rb绑定不到,报错
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        DestroyBulletAfterTime = StartCoroutine(DeactiveAfterTime());
    }

    public void Move(Vector2 direction)
    {
        rb.velocity = direction * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
             collision.gameObject.GetComponent<Enemy>().OnDamageTake(this.damage);
            _pool.Release(this);
        }

    }

    public void SetPool(ObjectPool<Bullet> pool)
    {
        _pool = pool;
    }

    private IEnumerator DeactiveAfterTime()
    {
        float elapsedTime = 0f;
        while (elapsedTime < destroyTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _pool.Release(this);
    }
}
