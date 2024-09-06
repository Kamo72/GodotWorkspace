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

        public PDamage damage;

        public virtual List<Character> GetCollide()
        {
            //OverrideNeeded

            return new();
        }


    }
}
