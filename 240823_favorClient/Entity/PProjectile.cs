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
        public BDamage damage;

        public virtual Boss GetCollide() 
        {
            //OverrideNeeded
            return null;
        }






    }
}
