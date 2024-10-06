using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _241003_blitzServer.library.DataType
{
    public struct UserStatus
    {
        public UserStatus(string id, string name, int idx, int rpcId, CharacterData.Type type)
        {
            this.id = id;
            this.name = name;
            this.idx = idx;
            this.rpcId = rpcId;
            this.type = type;
            traitTree = new TraitTree();
        }

        //기본 정보
        public string id;
        public string name;
        public int idx;
        public int rpcId;

        //실질적인 데이터
        public CharacterData.Type type;
        public TraitTree traitTree;

        public static UserStatus Parse(string str)
        {
            UserStatus ustat = new UserStatus();

            try
            {
                List<string> pbv = str.SplitWithSpan('^'), pbt;
                if (pbv.Count != 3) throw new Exception("UserStatus Parse pbv.Count != 3 : " + str);

                pbt = pbv[0].SplitWithSpan('~');
                if (pbt.Count != 5) throw new Exception("UserStatus Parse [0]pbt.Count != 5 : " + str);

                ustat.id = pbt[0];
                ustat.name = pbt[1];
                ustat.idx = int.Parse(pbt[2]);
                ustat.rpcId = int.Parse(pbt[3]);

                pbt = pbv[1].SplitWithSpan('~');
                ustat.type = (CharacterData.Type)int.Parse(pbt[0]);

                pbt = pbv[2].SplitWithSpan('~');
                foreach (string sp in pbt)
                    if (sp != "" && sp != "기본 노드")
                        ustat.traitTree.TakeTraitByName(sp);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }


            return ustat;
        }

        public override string ToString()
        {
            string str = "";

            str += id + "~";
            str += name + "~";
            str += idx + "~";
            str += rpcId + "~";
            str += "^";

            str += ((int)type).ToString();
            str += "^";

            if (traitTree.traitsList != null)
                foreach (var trait in traitTree.traitsList)
                    str += trait + "~";

            return str;
        }

    }

}
