using _favorClient.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _favorClient.library.DataType
{
    public enum GroggyType
    {
        NONE,
        SOFT,
        HARD
    }

    public struct PDamage
    {
        public float damage;
        public bool isParryable;

        public GroggyType groggyType;
        public Action<Character> CC;

        //이거 지우는게?
        public readonly static Action<Character> softStagger = c => c.stunDur = 0.2f;
        public readonly static Action<Character> middleStagger = c => c.stunDur = 0.6f;
        public readonly static Action<Character> hardStagger = c => c.stunDur = 1.2f;
    }
    public struct EDamage
    {
        public float damage;
        public float stagger;

        public GroggyType groggyType;
        public Action<Enemy> CC;


        //이거 지우는게?
        public readonly static Action<Enemy> softCC;
        public readonly static Action<Enemy> hardCC;
    }
}
