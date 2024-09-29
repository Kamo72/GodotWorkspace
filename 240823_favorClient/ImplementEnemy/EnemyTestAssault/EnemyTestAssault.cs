using _favorClient.Entity;
using _favorClient.library.DataType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _favorClient.EntityImplemented
{
    public partial class EnemyTestAssault : Enemy
    {
        public override void _Ready()
        {
            type = EnemyData.Type.TEST_ASSAULT;
            base._Ready();
        }

        protected override void ProcessOnAuthority(float delta)
        {

        }
    }
}
