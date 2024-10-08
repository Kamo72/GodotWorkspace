using _favorClient.Entity;
using _favorClient.library.DataType;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Godot.TextServer;

namespace _favorClient.Entity
{
    public partial class Projectile : RigidBody2D
    {
        public Damage damage;

        public List<GodotObject> collidedList = new();
        public (float now, float max) life = (4.0f, 4.0f);
        public float speed {
            set { LinearVelocity = Vector2.FromAngle(GlobalRotation) * value; }
        }


        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
        protected virtual void OnHit(Humanoid humanoid)
        {
            GD.PushWarning("OnHit : " + humanoid.Name);
        }

        public void CheckHit(GodotObject body)
        {
            if (collidedList.Contains(body))
                return;

            if (body is Humanoid humanoid)
            {
                Rpc("OnHit", humanoid);
                collidedList.Add(body);
            }
        }

        protected Vector2 syncPos;
        protected float syncRot;


        public override void _Ready()
        {
            GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").SetMultiplayerAuthority(1);
        }

        public override void _PhysicsProcess(double delta)
        {
            if (GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").GetMultiplayerAuthority() == Multiplayer.GetUniqueId())
            {
                ProcessOnAuthority((float)delta);

                var collision = MoveAndCollide(LinearVelocity);
                if(collision != null)
                if(collision.GetCollider() != null)
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

        public virtual void ProcessOnAuthority(float delta)
        {
            //TODO
        }
    }
}


