using Godot;
using System;
using System.Collections.Generic;


public partial class Humanoid
{
	public Health health;
    public struct Health
    {
        public Health(float hp, Action whenDead)
        {
            maxHp = hp;
            nowHp = hp;
            this.whenDead = whenDead;
        }

        public Action whenDead;
        public float maxHp, nowHp;
        public bool isDead => nowHp <= 0f;

        public float HealthPercentage => nowHp / maxHp;

        public void GetDamage(float damage)
        {
            nowHp -= damage;
            nowHp = Math.Max(0, nowHp); // 최소 0 이하로 감소 방지

            if (isDead) whenDead();
        }

        public void Heal(float amount)
        {
            nowHp = Math.Min(nowHp + amount, maxHp);
        }

        public void Reset()
        {
            nowHp = maxHp;
        }
    }

}