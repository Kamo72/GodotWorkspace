using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

public struct WeaponStatuss
{
    public int rpm;           // 분당 발사 속도
    public float damage;      // 피해량
    public float muzzleSpeed; // 탄속
    public int magSize;
    public float reloadTime;
    public float muzzleDistance; //총기 길이
    public string resPath;
    public float moa;
    public CaliberType caliberType;
    public List<string> attachableMagCodes;
    public int pellits;

    public WeaponStatuss(
        int rpm, float damage, float muzzleSpeed, int magSize, float reloadTime,
        string resPath, float muzzleDistance, float moa, CaliberType caliberType,
        List<string> attachableMagCodes, int pellits = 1)
    {
        this.rpm = rpm;
        this.damage = damage;
        this.muzzleSpeed = muzzleSpeed;
        this.magSize = magSize;
        this.reloadTime = reloadTime;
        this.resPath = resPath;
        this.muzzleDistance = muzzleDistance;
        this.caliberType = caliberType;
        this.moa = moa;
        this.attachableMagCodes = attachableMagCodes;
        this.pellits = pellits;
    }
}
