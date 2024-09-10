

using _240823_favorServer.library.DataType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

public struct Packet
{
    public Packet(Flag flag, params object[] values)
    {
        this.flag = flag;
        value = values;
    }

    public Flag flag;
    public object[] value;

    char[] parsers = new char[4] { '\n', '\f', '\v', '\t' };


    public override string ToString()
    {
        string str = "";

        str += ((int)flag).ToString() + "\n";

        switch (flag)
        {
            case Flag.PING:
                {
                    DateTime dt = (DateTime)value[0];
                    str += dt.ToString("yyyy-MM-dd HH:mm:ss.fff");
                }
                break;
            case Flag.PONG:
                {
                    DateTime dt = (DateTime)value[0];
                    str += dt.ToString("yyyy-MM-dd HH:mm:ss.fff");
                }
                break;

            case Flag.UNDEFINED:
                break;

            case Flag.NET_ERROR:
                str += value[0].ToString();
                break;
            case Flag.NET_CRASH:
                str += value[0].ToString();
                break;


            case Flag.ACCOUNT_SIGNUP:
                str += value[0].ToString() + "\f";
                str += value[1].ToString() + "\f";
                str += value[2].ToString() + "\f";
                break;
            case Flag.ACCOUNT_SIGNUP_CALLBACK:
                str += value[0].ToString() + "\f";
                str += value[1].ToString() + "\f";
                break;

            case Flag.ACCOUNT_SIGNIN:
                str += value[0].ToString() + "\f";
                str += value[1].ToString() + "\f";
                break;
            case Flag.ACCOUNT_SIGNIN_CALLBACK:
                str += value[0].ToString() + "\f";
                str += value[1].ToString();
                break;

            case Flag.ACCOUNT_SIGNOUT:
                break;
            case Flag.ACCOUNT_SIGNOUT_CALLBACK:
                str += value[0].ToString() + "\f";
                str += value[1].ToString() + "\f";
                break;


            case Flag.ROOM_LIST:
                {
                    List<(int, string, bool, int, int)> list = value[0] as List<(int, string, bool, int, int)>;

                    foreach (var item in list)
                    {
                        str += item.Item1.ToString() + "\t";
                        str += item.Item2.ToString() + "\t";
                        str += item.Item3.ToString() + "\t";
                        str += item.Item4.ToString() + "\t";
                        str += item.Item5.ToString() + "\t";

                        str += "\v";
                    }
                }
                break;
            case Flag.ROOM_LIST_REQUEST:
                break;

            case Flag.ROOM_HOST:
                {
                    str += value[0].ToString() + "\f";
                    str += value[1].ToString() + "\f";
                    str += value[1].ToString() + "\f";
                }
                break;
            case Flag.ROOM_HOST_CALLBACK:
                {
                    str += value[0].ToString() + "\f";
                    str += value[1].ToString() + "\f";
                }
                break;

            case Flag.ROOM_JOIN:
                str += value[0].ToString() + "\f";
                str += value[1].ToString() + "\f";
                break;
            case Flag.ROOM_JOIN_CALLBACK:
                str += value[0].ToString() + "\f";
                str += value[1].ToString() + "\f";
                break;

            case Flag.ROOM_USERS:
                {
                    List<(int, string, string)> list = value[0] as List<(int, string, string)>;

                    foreach (var item in list)
                    {
                        str += item.Item1.ToString() + "\t";
                        str += item.Item2.ToString() + "\t";
                        str += item.Item3.ToString() + "\t";

                        str += "\v";
                    }
                }
                break;

            case Flag.ROOM_CHAT_SEND:
                str += value[1].ToString() + "\f";
                break;
            case Flag.ROOM_CHAT_RECV:
                str += value[0].ToString() + "\f";
                str += value[1].ToString() + "\f";
                break;

            case Flag.ROOM_EXIT:
                break;
            case Flag.ROOM_EXIT_CALLBACK:
                str += value[0].ToString() + "\f";
                str += value[1].ToString() + "\f";
                break;

            case Flag.ROOM_READY_SEND:
                str += value[0].ToString() + "\f";
                break;
            case Flag.ROOM_READY_RECV:
                str += value[0].ToString() + "\f";
                str += value[1].ToString() + "\f";
                break;

            case Flag.ROOM_STATUS_SEND:
                {
                    str += value[0].ToString() + "\f";
                }
                break;
            case Flag.ROOM_STATUS_RECV:
                {
                    str += value[0].ToString() + "\f";
                    str += value[1].ToString() + "\f";
                }
                break;

            case Flag.ROOM_COUNTDOWN:
                {
                    str += value[0].ToString() + "\f";
                }
                break;
            case Flag.ROOM_START:
                {
                    str += value[0].ToString() + "\f";
                }
                break;

            case Flag.ROOM_RPC_SEND:
                {
                    str += value[0].ToString() + "\f";
                    str += value[1].ToString() + "\f";
                }
                break;
            case Flag.ROOM_RPC_RECV:
                {
                    str += value[0].ToString() + "\f";
                    str += value[1].ToString() + "\f";
                }
                break;

            case Flag.DEBUG_FAST_LOGIN:
                {
                }
                break;
            case Flag.DEBUG_FAST_LOGIN_CALLBACK:
                {
                    str += value[0].ToString() + "\f";
                    str += value[1].ToString() + "\f";
                }
                break;
            case Flag.DEBUG_FAST_JOIN:
                {
                }
                break;
            case Flag.DEBUG_FAST_JOIN_CALLBACK:
                {
                    str += value[0].ToString() + "\f";
                }
                break;
        }

        return str;
    }
    public static Packet FromString(string str)
    {
        int flagInt;

        //플래그 분리
        List<String> p = str.SplitWithSpan('\n');
        string flagStr = p[0];
        string values = p[1];

        //if (str.Length < 3) throw new Exception("패킷 파싱 오류 : 파싱 가능한 최소 길이보다 짧습니다. - " + str);

        bool res = int.TryParse(flagStr, out flagInt);

        if (res == false) throw new Exception("패킷 파싱 오류 : 플래그 파싱에 실패했습니다. - " + str);

        Packet packet = new Packet();

        //플래그 분기
        Flag flag = (Flag)int.Parse(flagStr);
        List<string> sp = values.SplitWithSpan('\f');
        switch (flag)
        {
            case Flag.PING:
                {
                    DateTime dt = DateTime.ParseExact(sp[0], "yyyy-MM-dd HH:mm:ss.fff", null);
                    packet = new Packet(flag, dt);
                }
                break;
            case Flag.PONG:
                {
                    DateTime dt = DateTime.ParseExact(sp[0], "yyyy-MM-dd HH:mm:ss.fff", null);
                    packet = new Packet(flag, dt);
                }
                break;

            case Flag.UNDEFINED:
                packet = new Packet(flag);
                break;

            case Flag.NET_ERROR:
                packet = new Packet(flag, values);
                break;
            case Flag.NET_CRASH:
                packet = new Packet(flag, values);
                break;


            case Flag.ACCOUNT_SIGNUP:
                {
                    string id = sp[0];
                    string nickname = sp[1];
                    string pw = sp[2];

                    packet = new Packet(flag, id, nickname, pw);
                }
                break;
            case Flag.ACCOUNT_SIGNUP_CALLBACK:
                {
                    bool isSucceed = bool.Parse(sp[0]);
                    string msg = sp[1];

                    packet = new Packet(flag, isSucceed, msg);
                }
                break;

            case Flag.ACCOUNT_SIGNIN:
                {
                    string id = sp[0];
                    string pw = sp[1];

                    packet = new Packet(flag, id, pw);
                }
                break;
            case Flag.ACCOUNT_SIGNIN_CALLBACK:
                {
                    bool isSucceed = bool.Parse(sp[0]);
                    string msg = sp[1];

                    packet = new Packet(flag, isSucceed, msg);
                }
                break;

            case Flag.ACCOUNT_SIGNOUT:
                {
                    packet = new Packet(flag);
                }
                break;
            case Flag.ACCOUNT_SIGNOUT_CALLBACK:
                {
                    bool isSucceed = bool.Parse(sp[0]);
                    string msg = sp[1];

                    packet = new Packet(flag, isSucceed, msg);
                }
                break;

            case Flag.ROOM_LIST:
                {
                    List<(int, string, bool, int, int)> roomList = new List<(int, string, bool, int, int)>();

                    List<string> roomStrList = sp[0].SplitWithSpan('\v');
                    for (int i = 0; i < roomStrList.Count - 1; i++)
                    {
                        List<string> ssp = roomStrList[i].SplitWithSpan('\t');

                        int idx = int.Parse(ssp[0]);
                        string name = ssp[1];
                        bool isPw = bool.Parse(ssp[2]);
                        int state = int.Parse(ssp[3]);
                        int playerCount = int.Parse(ssp[4]);

                        roomList.Add((idx, name, isPw, state, playerCount));
                    }

                    packet = new Packet(flag, roomList);
                }
                break;
            case Flag.ROOM_LIST_REQUEST:
                {
                    packet = new Packet(flag);
                }
                break;

            case Flag.ROOM_HOST:
                {
                    string roomname = sp[0];
                    bool isPw = bool.Parse(sp[1]);
                    string pw = sp[2];

                    packet = new Packet(flag, roomname, isPw, pw);
                }
                break;
            case Flag.ROOM_HOST_CALLBACK:
                {
                    bool isSucceed = bool.Parse(sp[0]);
                    string msg = sp[1];

                    packet = new Packet(flag, isSucceed, msg);
                }
                break;

            case Flag.ROOM_JOIN:
                {
                    string roomname = sp[0];
                    string pw = sp[1];

                    packet = new Packet(flag, roomname, pw);
                }
                break;
            case Flag.ROOM_JOIN_CALLBACK:
                {
                    bool isSucceed = bool.Parse(sp[0]);
                    string msg = sp[1];

                    packet = new Packet(flag, isSucceed, msg);
                }
                break;

            case Flag.ROOM_USERS:
                {
                    List<(int, string, string)> userList = new List<(int, string, string)>();

                    List<string> roomStrList = sp[0].SplitWithSpan('\v');


                    for (int i = 0; i < roomStrList.Count - 1; i++)
                    {
                        List<string> ssp = roomStrList[i].SplitWithSpan('\t');

                        int idx = int.Parse(ssp[0]);
                        string id = ssp[1];
                        string nickname = ssp[2];

                        userList.Add((idx, id, nickname));
                    }
                    packet = new Packet(flag, userList);
                }
                break;

            case Flag.ROOM_CHAT_SEND:
                {
                    string msg = sp[0];

                    packet = new Packet(flag, msg);
                }
                break;
            case Flag.ROOM_CHAT_RECV:
                {
                    string sender = sp[0];
                    string msg = sp[1];

                    packet = new Packet(flag, sender, msg);
                }
                break;

            case Flag.ROOM_EXIT:
                packet = new Packet(flag);
                break;
            case Flag.ROOM_EXIT_CALLBACK:
                {
                    string sender = sp[0];
                    string msg = sp[1];

                    packet = new Packet(flag, sender, msg);
                }
                break;

            case Flag.ROOM_READY_SEND:
                {
                    bool isReady = bool.Parse(sp[0]);

                    packet = new Packet(flag, isReady);
                }
                break;
            case Flag.ROOM_READY_RECV:
                {
                    int userIdx = int.Parse(sp[0]);
                    bool isReady = bool.Parse(sp[1]);

                    packet = new Packet(flag, userIdx, isReady);
                }
                break;


            case Flag.ROOM_COUNTDOWN:
                {
                    int count = int.Parse(sp[0]);

                    packet = new Packet(flag, count);
                }
                break;
            case Flag.ROOM_START:
                {
                    int hostIdx = int.Parse(sp[0]);
                    packet = new Packet(flag, hostIdx);
                }
                break;

            case Flag.ROOM_STATUS_RECV:
                {
                    UserStatus status = UserStatus.Parse(sp[0]);

                    packet = new Packet(flag, status);
                }
                break;
            case Flag.ROOM_STATUS_SEND:
                {
                    int idx = int.Parse(sp[0]);
                    UserStatus status = UserStatus.Parse(sp[1]);

                    packet = new Packet(flag, idx, status);
                }
                break;
            case Flag.ROOM_RPC_SEND:
                {
                    string ip = sp[0];
                    int port = int.Parse(sp[1]);

                    packet = new Packet(flag, ip, port);
                }
                break;
            case Flag.ROOM_RPC_RECV:
                {
                    string ip = sp[0];
                    int port = int.Parse(sp[1]);

                    packet = new Packet(flag, ip, port);
                }
                break;

            case Flag.DEBUG_FAST_LOGIN:
                {
                    packet = new Packet(flag);
                }
                break;
            case Flag.DEBUG_FAST_LOGIN_CALLBACK:
                {

                    string id = sp[0];
                    string name = sp[1];

                    packet = new Packet(flag, id, name);
                }
                break;

            case Flag.DEBUG_FAST_JOIN:
                {
                    packet = new Packet(flag);
                }
                break;
            case Flag.DEBUG_FAST_JOIN_CALLBACK:
                {
                    string roomName = sp[0];

                    packet = new Packet(flag, roomName);
                }
                break;
        }

        return packet;
    }





    public enum Flag
    {
        /// <summary>
        /// 서버, 클라 양쪽이 호출 가능한 패킷입니다.
        /// 호출 시, PONG으로 돌려받습니다. 연결 대상이 살아있는지 확인하는데 주로 쓰입니다. 시간값을 담고 있습니다.
        /// </summary>
        PING,
        /// <summary>
        /// PING 패킷의 응답 패킷입니다. 연결 대상이 살아있음을 알립니다. 시간값을 담고 있습니다.
        /// </summary>
        PONG,

        /// <summary>
        /// 정의되지 않은 패킷입니다.
        /// </summary>
        UNDEFINED,

        /// <summary>
        /// 네트워크 관련 오류 패킷입니다. 연결 관련 문제를 담고 있습니다.
        /// </summary>
        NET_ERROR,
        /// <summary>
        /// 네트워크가 더는 지속될 수 없음을 알립니다. 이를 발송하면 대상 소켓으로 강제 해제 당합니다.
        /// </summary>
        NET_CRASH,


        /// <summary>
        /// 회원가입을 위한 요청 패킷입니다. 아이디, 닉네임, 비밀번호를 담고 있고,
        /// ACCOUNT_SIGNUP_CALLBACK을 반환합니다.
        /// </summary>
        ACCOUNT_SIGNUP,
        /// <summary>
        /// 회원가입의 응답 패킷입니다. 성공 여부와 메세지를 반환합니다.
        /// </summary>
        ACCOUNT_SIGNUP_CALLBACK,

        /// <summary>
        /// 로그인을 위한 요청 패킷입니다. 아이디, 비밀번호를 담고 있고,
        /// ACCOUNT_SIGNIN_CALLBACK을 반환합니다.
        /// </summary>
        ACCOUNT_SIGNIN,
        /// <summary>
        /// 로그인의 응답 패킷입니다. 성공 여부와 메세지를 반환합니다.
        /// </summary>
        ACCOUNT_SIGNIN_CALLBACK,

        /// <summary>
        /// 로그아웃을 위한 요청 패킷입니다. 아이디를 담고 있고,
        /// ACCOUNT_SIGNOUT_CALLBACK을 반환합니다.
        /// </summary>
        ACCOUNT_SIGNOUT,
        /// <summary>
        /// 로그아웃의 응답 패킷입니다. 성공 여부와 메세지를 반환합니다.
        /// </summary>
        ACCOUNT_SIGNOUT_CALLBACK,


        /// <summary>
        /// 서버가 현재 존재하는 룸 리스트를 전달합니다.
        /// 서버 측에서 적절한 클라이언트들에게 자동으로 전달합니다.
        /// (방 인덱스, 방 이름, 비밀번호 여부, 현재 상태, 현재 인원)
        /// </summary>
        ROOM_LIST,
        /// <summary>
        /// 클라가 현재 존재하는 룸 리스트를 강제로 요청합니다. ROOM_LIST로 응답 받습니다.
        /// </summary>
        ROOM_LIST_REQUEST,

        /// <summary>
        /// 클라이언트가 새로운 룸을 생성하기 위한 패킷입니다. 방이름, 비밀번호 여부, 비밀번호를 전달합니다.
        /// ROOM_HOST_CALLBACK으로 응답받습니다.
        /// </summary>
        ROOM_HOST,
        /// <summary>
        /// ROOM_HOST의 응답 패킷입니다. 생성 성공 여부를 응답받습니다.
        /// </summary>
        ROOM_HOST_CALLBACK,

        /// <summary>
        /// 클라이언트가 룸에 참가하기 위한 패킷입니다. 방이름, 비밀번호를 전달합니다.
        /// ROOM_JOIN_CALLBACK으로 응답받습니다.
        /// </summary>
        ROOM_JOIN,
        /// <summary>
        /// 서버가 ROOM_JOIN에 응답하기 위한 패킷입니다. 방에 입장하는데 성공했음을 전달합니다.
        /// </summary>
        ROOM_JOIN_CALLBACK,

        /// <summary>
        /// 서버가 룸 내에 존재하는 유저들을 전달합니다. 클라이언트가 룸 내에 존재하지만
        /// 충분한 시간 내에 해당 패킷을 받지 못할 경우, 스스로 룸리스트로 돌아갑니다.
        /// 사용자의 최소 정보는 인덱스, 아이디와 닉네임로 구성됩니다.
        /// </summary>
        ROOM_USERS,

        /// <summary>
        /// 클라이언트가 룸 내에 존재하는 모든 이들에게 채팅을 전송합니다. 전달 내용만 있어도 됩니다.
        /// </summary>
        ROOM_CHAT_SEND,
        /// <summary>
        /// 클라이언트가 채팅을 전달 받습니다. 전송자의 닉네임, 전달 내용이 있습니다.
        /// </summary>
        ROOM_CHAT_RECV,

        /// <summary>
        /// 클라이언트가 룸에서 퇴장하기 위한 패킷.
        /// ROOM_EXIT_CALLBACK으로 응답받습니다.
        /// </summary>
        ROOM_EXIT,
        /// <summary>
        /// 서버가 ROOM_EXIT에 응답하기 위한 패킷입니다. 방에서 퇴장하는데 성공했음을 전달합니다.
        /// </summary>
        ROOM_EXIT_CALLBACK,

        /// <summary>
        /// 클라이언트가 게임을 진행할 준비가 됐음의 여부를 전달합니다.
        /// 이를 수령한 서버는 ROOM_READY_RECV을 다른 클라들에게 전달합니다.
        /// </summary>
        ROOM_READY_SEND,
        /// <summary>
        /// 서버가 ROOM_READY_SEND를 수령하면 전달합니다. 이용자 인덱스와 준비 여부로 이뤄져 있습니다.
        /// </summary>
        ROOM_READY_RECV,

        /// <summary>
        /// 클라이언트가 방안에서 스스로의 상태 변화를 얻을 경우, 서버에 이를 보내 다른 클라이언트에서
        /// 자신의 상태를 초기화 할 수 있도록 요청합니다. 캐릭터의 정보를 담고 있습니다.
        /// </summary>
        ROOM_STATUS_SEND,
        /// <summary>
        /// 서버가 ROOM_STATUS_SEND을 수령할 경우, 다른 사용자들에게 보내는 정보 패킷입니다. 사용자 인덱스와 캐릭터의 정보를 담고 있습니다.
        /// </summary>
        ROOM_STATUS_RECV,

        /// <summary>
        /// 모든 플레이어가 레디를 했다면, 서버가 카운트 다운을 시작합니다. 10초까지 샙니다. 단순 int 값을 반환합니다. 카운트다운이 취소되면 -1을 반환합니다.
        /// </summary>
        ROOM_COUNTDOWN,
        /// <summary>
        /// 게임의 시작을 알립니다. 모든 참가자들을 인게임으로 유도합니다. 호스트 유저의 인덱스를 포함하며,
        /// 해당 호스트의 RPC 연결을 시작한 뒤 ROOM_RPC_SEND를 유발합니다.
        /// </summary>
        ROOM_START,
        /// <summary>
        /// ROOM_START를 받은 호스트 유저가 RPC 호스팅을 시작함을 알립니다. IP문자열과 port 정수값을 반환합니다.
        /// 해당 정보는 ROOM_RPC_RECV을 통해 다른 참가자들에게 전달됩니다.
        /// </summary>
        ROOM_RPC_SEND,
        /// <summary>
        /// 호스트 유저의 RPC 호스팅이 시작됐음을 다른 참여자들에게 알립니다. IP문자열과 port 정수값을 반환합니다.
        /// 이 이후로는 RPC 연결을 통해 정보교환이 진행됩니다.
        /// </summary>
        ROOM_RPC_RECV,

        /// <summary>
        /// 클라측 디버그용 패킷입니다. 어떤 값도 저장하지 않으며 DEBUG_FAST_LOGIN_CALLBACK을 유발합니다.
        /// </summary>
        DEBUG_FAST_LOGIN,
        /// <summary>
        /// 서버측 디버그용 패킷입니다. DEBUG_FAST_LOGIN 요청에 대해 임의의 디버그용 계정의 아이디와 닉네임을 반환합니다.
        /// </summary>
        DEBUG_FAST_LOGIN_CALLBACK,
        /// <summary>
        /// 클라측 디버그용 패킷입니다. 어떤 값도 저장하지 않으며 DEBUG_FAST_JOIN_CALLBACK 유발합니다.
        /// </summary>
        DEBUG_FAST_JOIN,
        /// <summary>
        /// 서버측 디버그용 패킷입니다. DEBUG_FAST_JOIN 요청에 대해 임의의 디버그용 방의 이름을 반환합니다.
        /// </summary>
        DEBUG_FAST_JOIN_CALLBACK,
    }
}


