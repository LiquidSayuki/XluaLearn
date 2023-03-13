using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractGun : MonoBehaviour
{
    public int bulletsPerMagazine;
    protected int bulletsInMagazine;

    public float shootInterval;
    protected float timer;

    public float reloadTime;

    public Bullet bulletPrefab;

    public Transform muzzlePos;
    protected Vector2 mousePos;
    protected Vector2 direction;

    //private SpriteRenderer spriteRenderer;
    private float scaleY;

    protected BulletManager bulletManager;

    private void Awake()
    {
        //spriteRenderer = GetComponent<SpriteRenderer>();
        bulletManager = GetComponent<BulletManager>();
        scaleY = transform.localScale.y;
        bulletsInMagazine = bulletsPerMagazine;
    }

    void Update()
    {
        // 旋转枪械朝向鼠标
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        direction = (mousePos - new Vector2(transform.position.x, transform.position.y)).normalized;
        if (mousePos.x < this.transform.position.x)
        {
            //spriteRenderer.flipX = true;
            transform.localScale = new Vector3(1, -scaleY, 1);
            transform.right = direction;
        }
        else
        {
            //spriteRenderer.flipX = false;
            transform.localScale = new Vector3(1, scaleY, 1);
            transform.right = direction;
        }

        // 计算射击时间
        if (timer >= 0)
        {
            timer -= Time.deltaTime;
        }
    }

    public virtual void Fire(){}

    public virtual void StopFire(){ }




}
