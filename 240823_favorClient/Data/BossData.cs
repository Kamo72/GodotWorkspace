using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _favorClient.Data
{
    public abstract class BossData
    {
        public string name;
        public Status status;

        public string easyDes;
        public Description diffDes;

        public struct Status
        {
            public float hpMax;     //최대 체력
            public float staggerMax; //최대 스태거

            public float speed;     //기본 이동속도
        }
    }
}
