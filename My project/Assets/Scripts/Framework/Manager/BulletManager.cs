using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BulletManager : MonoBehaviour
{

    public ObjectPool<Bullet> bulletPool;
    private AbstractGun currentGun;

    void Start()
    {
        currentGun = GetComponent<AbstractGun>();

        // 4functions (OnCreate OnGet OnRelease OnDestroy) 1bool (OnCheck) 2int (defaultCapacity maxSize)
        bulletPool = new ObjectPool<Bullet> (CreateBullet,OnTakeBulletFromPool,OnReturnBulletToPool,OnDestroyBullet, true, 100, 300);
    }
        
    private Bullet CreateBullet()
    {
        // spawn the bullet
        Bullet bullet = Instantiate(currentGun.bulletPrefab, currentGun.muzzlePos.position, Quaternion.identity, GameManager.Instance.transform);
        // bind the bullet to the pool
        bullet.SetPool(bulletPool);

        return bullet;
    }

    private void OnTakeBulletFromPool(Bullet bullet)
    {
        // set transform and rotataion
        bullet.transform.position = currentGun.muzzlePos.position;

        //set active
        bullet.gameObject.SetActive(true);
    }

    private void OnReturnBulletToPool(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);
    }

    private void OnDestroyBullet(Bullet bullet)
    {
        Destroy(bullet.gameObject);
    }
}
