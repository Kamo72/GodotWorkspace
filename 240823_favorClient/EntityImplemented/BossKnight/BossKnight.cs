using _favorClient.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _favorClient.EntityImplemented.BossKnight
{
    public partial class BossKnight : Boss
    {
        public BossKnight()
        {
            type = library.DataType.BossData.Type.KNIGHT;
        }

        public override void LoadRoom()
        {

        }

        protected override void ProcessOnAuthority()
        {

        }

    }
}
