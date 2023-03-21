using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponStaticParameters_", menuName = "PCG/WeaponStatistic")]
public class WeaponSatisticSO : ScriptableObject
{
    public int bulletsPerMagazine;
    public float shootInterval;
    public float reloadTime;

}
