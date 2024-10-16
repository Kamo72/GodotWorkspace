using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _favorClient.library.DataType
{
    public partial struct CharacterData
    {
        public string weaponCodeMain, weaponCodeSub;

        public static CharacterData Parse(string str)
        {
            CharacterData cStat = new CharacterData();

            try
            {
                List<string> pbv = str.SplitWithSpan(':');
                if (pbv.Count != 3) throw new Exception("UserStatus Parse pbv.Count != 3 : " + str);

                cStat.weaponCodeMain = pbv[0];
                cStat.weaponCodeSub= pbv[1];
            }
            catch (Exception ex)
            {
                GD.PushError(ex);
            }

            return cStat;
        }

        public override string ToString()
        {
            string str = "";
            str += weaponCodeMain + ":";
            str += weaponCodeSub;

            return str;
        }
    }


}
