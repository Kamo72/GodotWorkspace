using System;
using System.Collections.Generic;

public partial class Weapon {

    public struct Status
    {
        public Status(){}

        public float damage = 10f;
        public float muzzleSpeed = 20f;
        public int rpm = 750;
        public int mag = 30;



        public float reloadTime = 2.0f;

    }
}


