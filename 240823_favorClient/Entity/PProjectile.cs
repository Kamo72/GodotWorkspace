using _favorClient.library.DataType;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _favorClient.Entity
{
    public partial class PProjectile : RigidBody2D
    {
        public PDamage damage;

        public List<GodotObject> collidedList = new();
        public float lifeMax = 3.0f, lifeNow = 3.0f;

        public Enemy owner = null;
        public bool isParryable = false;


        public void SetOwner(Enemy owner, bool isParryable)
        {
            this.owner = owner;
            this.isParryable = isParryable;
        }


        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
        protected virtual void OnHit(Character character) 
        {
            GD.PrintErr("OnHit : " + character.Name);
        }


        public void CheckHit(GodotObject body)
        {
            if (collidedList.Contains(body))
                return;

            if (body is Character character)
            {
                OnHit(character);
                collidedList.Add(body);
            }
        }


        public override void _Ready()
        {
            GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").SetMultiplayerAuthority(1);
        }


        protected Vector2 syncPos;
        protected float syncRot;
        protected Vector2 syncVec;

        public override void _PhysicsProcess(double delta)
        {
            if (GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").GetMultiplayerAuthority() == Multiplayer.GetUniqueId())
            {
                ProcessOnAuthority((float)delta);

                var collision = MoveAndCollide(LinearVelocity);
                if(collision != null)
                    if (collision.GetCollider() != null)
                        CheckHit(collision.GetCollider());

                syncPos = GlobalPosition;
                syncRot = GlobalRotation;
                syncVec = LinearVelocity;
            }
            else
            {
                GlobalPosition = GlobalPosition.Lerp(syncPos, .1f);
                GlobalRotation = Mathf.Lerp(GlobalRotation, syncRot, .1f);
                LinearVelocity = syncVec;
                MoveAndCollide(LinearVelocity);
            }
        }

        public virtual void ProcessOnAuthority(float delta) 
        {
            //TODO
        }


    }
}
