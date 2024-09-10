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

            List<string> pbv = str.SplitWithSpan('\v'), pbt;

            pbt = pbv[0].SplitWithSpan('\t');
            ustat.id = pbt[0];
            ustat.name = pbt[1];
            ustat.idx = int.Parse(pbt[2]);
            ustat.rpcId = int.Parse(pbt[3]);

            pbt = pbv[1].SplitWithSpan('\t');
            ustat.type = (CharacterData.Type)int.Parse(pbt[0]);

            pbt = pbv[2].SplitWithSpan('\t');
            foreach (string sp in pbt)
                if(sp != "" && sp != "기본 노드")
                    ustat.traitTree.TakeTraitByName(sp);

            return ustat;
        }

        public override string ToString()
        {
            string str = "";

            str += id + "\t";
            str += name + "\t";
            str += idx + "\t";
            str += rpcId + "\t";
            str += "\v";

            str += ((int)type).ToString();
            str += "\v";

            if(traitTree.traitsList != null)
                foreach(var trait in traitTree.traitsList)
                    str += trait + "\t";

            return str;
        }

    }

}
