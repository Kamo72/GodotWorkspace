using _favorClient.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _favorClient.library.DataType
{
    public struct PDamage
    {
        public float damage;

        public Action<Character> CC;

    }
    public struct BDamage
    {
        public float damage;
        public float stagger;

        public Action<Boss> softCC;
        public Action<Boss> hardCC;
    }
}
