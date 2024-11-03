using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

public struct WeaponStatus
{
    public int rpm;           // 분당 발사 속도
    public float damage;      // 피해량
    public float muzzleSpeed; // 탄속
    public int magSize;
    public float reloadSpeed;

    public WeaponStatus(int rpm, float damage, float muzzleSpeed, int magSize, float reloadSpeed)
    {
        this.rpm = rpm;
        this.damage = damage;
        this.muzzleSpeed = muzzleSpeed;
        this.magSize = magSize;
        this.reloadSpeed = reloadSpeed;
    }
}
