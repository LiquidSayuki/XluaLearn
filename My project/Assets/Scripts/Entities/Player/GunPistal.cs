using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GunPistal : AbstractGun
{
    public override void Fire()
    {
        if (this.bulletsInMagazine == 0)
        {
            Reload();
        }
        if (this.timer <= 0)
        {
            Bullet bullet = bulletManager.bulletPool.Get();
            bullet.Move(this.direction);
            this.timer = this.shootInterval;
            this.bulletsInMagazine -= 1;
        }
            
    }

    public void Reload()
    {
        this.bulletsInMagazine = this.bulletsPerMagazine;
    }

    /*    public override void StopFire()
        {
            StopCoroutine(nameof(Shooting));
        }



        IEnumerator Shooting()
        {
            WaitForSeconds reload = new WaitForSeconds(1.5f);
            while (true)
            {
                if(this.bulletsInMagazine == 0)
                {
                    Reload();
                    yield return reload;
                }
                if(this.timer <=0)
                {
                    Instantiate(bulletPrefab, muzzlePos.position, Quaternion.identity);
                    this.timer = this.shootInterval;
                    this.bulletsInMagazine -= 1;
                }
            }
        }*/
}
