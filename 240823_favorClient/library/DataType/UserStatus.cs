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
        public CharacterData.Type type;

        public static UserStatus Parse(string str)
        {
            return new();
        }

        public override string ToString()
        {
            return base.ToString();
        }

    }

}
