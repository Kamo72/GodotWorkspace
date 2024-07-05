using System;
using System.Collections.Generic;

public partial class Weapon {

    public struct WeaponStatus
    {
        public WeaponStatus(){}

        public float damage = 10f;  //value of bullet damage
        public float muzzleSpeed = 100f;    //initial speed of bullet
        public float friction = 0.21f;  //friction of bullet speed
        public int rpm = 750;   //round per min
        public int mag = 30;    //round per mag

        public float swapTime = 1.0f;   //duration of swapping weapon
        public float reloadTime = 2.0f; //duration of reloading mag 

    }
}


