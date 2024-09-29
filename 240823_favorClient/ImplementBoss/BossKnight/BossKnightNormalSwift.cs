using _favorClient.Entity;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _favorClient.EntityImplemented.BossKnight
{
    public partial class BossKnightNormalSwift : PProjectile
    {
        Vector2 oriPos;
        float oriRot;

        public override void _Ready()
        {
            lifeMax = lifeNow = 0.2f;

            damage = new()
            {
                damage = 10,
                isParryable = true,
                CC = e => { },
                groggyType = library.DataType.GroggyType.SOFT,
            };

            oriPos = GlobalPosition;
            oriRot = GlobalRotation;

            base._Ready();
        }

        public override void ProcessOnAuthority(float delta)
        {
            lifeNow -= delta;

            GlobalPosition = oriPos;
            GlobalRotation = oriRot;

            LinearVelocity = Vector2.Zero;
            AngularVelocity = 0f;

            if (lifeNow < 0) QueueFree();
        }   
    
    }
}
