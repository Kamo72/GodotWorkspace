using _favorClient.library.DataType;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _favorClient.Entity
{
    public partial class BProjectile : RigidBody2D
    {
        public BDamage damage;

        public List<GodotObject> collidedList = new();
        public float lifeMax = 0.3f, lifeNow = 0.3f, warnMax = 1f, warnNow = 1f;


        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
        protected virtual void OnHit(Boss boss)
        {
            //TODO
        }

        public void CheckHit(GodotObject body)
        {
            if (collidedList.Contains(body))
                return;

            if (body is Boss boss)
            {
                OnHit(boss);
                collidedList.Add(body);
            }
        }

        public override void _Ready()
        {
            GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").SetMultiplayerAuthority(1);
        }

        protected Vector2 syncPos;
        protected float syncRot;

        public override void _PhysicsProcess(double delta)
        {
            if (GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").GetMultiplayerAuthority() == Multiplayer.GetUniqueId())
            {
                ProcessOnAuthority();

                var collision = MoveAndCollide(LinearVelocity);
                if (collision.GetCollider() != null)
                    CheckHit(collision.GetCollider());

                syncPos = GlobalPosition;
                syncRot = GlobalRotation;
            }
            else
            {
                GlobalPosition = GlobalPosition.Lerp(syncPos, .1f);
                GlobalRotation = Mathf.Lerp(GlobalRotation, syncRot, .1f);
            }
        }

        public virtual void ProcessOnAuthority()
        {
            //TODO
        }

        public override void _Draw()
        {
            DrawCircle(GlobalPosition, 1f, Colors.Red);

            base._Draw();
        }
    }
}
