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

        public bool isParryable;

        public Action<Character> CC;

        public readonly static Action<Character> softStagger = c => c.stunDur = 0.2f;
        public readonly static Action<Character> middleStagger = c => c.stunDur = 0.6f;
        public readonly static Action<Character> hardStagger = c => c.stunDur = 1.2f;
    }
    public struct BDamage
    {
        public float damage;
        public float stagger;

        public Action<Boss> softCC;
        public Action<Boss> hardCC;
    }
}
