using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public partial class Humanoid
{
    public Society society;
    public partial class Society
    {
        public Humanoid master;
        public Society(Humanoid master, string facCode)
        {
            this.master = master;
            faction = Faction.Get(facCode);
        }

        public Faction faction;

        float GetFriendship(Humanoid other)
        {
            if (other.society.faction == null)
                return 0f;

            if (faction == other.society.faction)
                return 999f;

            float friendship = Faction.FriendShipGet(faction.key, other.society.faction.key);
            
            return friendship;
        }

        public Stance GetStance(Humanoid other)
        {
            float friendship = GetFriendship(other);
            if (friendship < -50f)
                return Stance.HOSTILE;
            else if (friendship < -10f)
                return Stance.WARNING;
            else if (friendship < 40f)
                return Stance.NUETRAL;
            else if (friendship < 70f)
                return Stance.FRIENDLY;
            else
                return Stance.ALLY;
        }

        public enum Stance 
        {
            HOSTILE,    //적대 - 발견 즉시 선공
            WARNING,    //경계 - 먼저 발견해도 경고하며, 무시시 선공
            NUETRAL,    //중립 - 무시. 피격 시 공격
            FRIENDLY,   //우호 - 우호적. 가진 물건을 나눠주거나 교전 중 소극적인 도움을 줌. 약간의 오사를 용인
            ALLY,       //동맹 - 완전 동맹. 교전 중 적극적인 도움을 줌. 오사를 용인
        }
    }
}
