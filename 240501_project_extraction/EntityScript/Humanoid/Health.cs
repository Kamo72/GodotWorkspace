using Godot;
using System;
using System.Collections.Generic;


public partial class Humanoid
{

    public struct Health
    {
        public Health(Action whenDead, float hp = 100)
        {
            nowHp = hp;
            maxHp = hp;
            this.whenDead = whenDead;
        }

        Action whenDead;

        public float maxHp, nowHp;
        public bool isDead => nowHp <= 0f;
        
        public void GetDamage(float damage)
        {
            nowHp -= damage;

            if(isDead) whenDead();
        }

    }

}