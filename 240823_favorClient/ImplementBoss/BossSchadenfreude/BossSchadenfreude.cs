using _favorClient.library.DataType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _favorClient.Entity.Boss_Test
{
    public partial class BossSchadenfreude : Boss
    {
        public BossSchadenfreude() 
        {
            type = BossData.Type.SCHADENFREUDE;
        }

    }
}
