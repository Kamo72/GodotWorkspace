using _favorClient.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _favorClient.library.DataType
{
    public struct UserStatus
    {
        //기본 정보
        public string id;
        public string name;
        public int idx;



        //실질적인 데이터
        public CharacterData.Type type;
        public TraitTree traitTree;

        public static UserStatus Parse(string str)
        {
            //parser '\v', '\t';

            return new();
        }

        public override string ToString()
        {




            return base.ToString();
        }

    }

}
