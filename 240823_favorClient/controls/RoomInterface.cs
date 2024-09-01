using Godot;
using System;
using System.Collections.Generic;
using _favorClient.library;
using _favorClient.controls;

public partial class RoomInterface : UserInterface
{
    [Export]
    ItemList roomList;

    [Export]
    Button joinBtn;
    [Export]
    Button hostBtn;
    [Export]
    Button signoutBtn;

    [Export]
    TextEdit roomNameTxt;
    [Export]
    TextEdit roomPlayerCountTxt;
    [Export]
    TextEdit roomIsPublicTxt;
    [Export]
    TextEdit roomStateTxt;


    List<(int idx, string name, bool isPw, int state, int userCount)> roomDataList = new();

    List<(int idx, string name, bool isPw, int state, int userCount)> toAddList = new();
    List<(int idx, int userCount)> toRefreshList = new();
    List<int> toDelList = new();



    Action requestDisposer;
    public override void _Ready()
    {

        roomList.ItemSelected += e => {
            RefreshRoomList((int)e);
        };

        joinBtn.Pressed += () => {

            joinBtn.Disabled = true;
            int idx = roomList.GetSelectedItems()[0];
            MainClient.instance.Send(new Packet(Packet.Flag.ROOM_JOIN, roomDataList[idx].idx, ""));

            requestDisposer = MainClient.instance.AddPacketListener(Packet.Flag.ROOM_JOIN_CALLBACK, packet =>
            {
                joinBtn.SetDeferred(Button.PropertyName.Disabled, false);

                //ROOM_JOIN_CALLBACK
                bool isSucceed = (bool)packet.value[0];
                if (isSucceed)
                {
                    GD.Print("참여 성공 : " + packet.value[1].ToString());
                    CallDeferred("ShowAcceptDialog", "참여 성공", packet.value[1].ToString(), "확인");

                    //로그인 성공에 따른 처리
                    CallDeferred("ControlExchange", "InroomInterface", "res://controls/InroomInterface.tscn");
                    InroomInterface.roomName = roomDataList[idx].name;
                }
                else
                {
                    GD.Print("참여 실패 : " + packet.value[1].ToString());
                    CallDeferred("ShowAcceptDialog", "참여 실패", packet.value[1].ToString(), "확인");
                }

                requestDisposer();
            });
        };

        hostBtn.Pressed += () => {

            ControlExchange("HostInterface", "res://controls/HostInterface.tscn");
        };

        signoutBtn.Pressed += () => {
            Packet packet = new Packet(Packet.Flag.ACCOUNT_SIGNOUT);
            MainClient.instance.Send(packet);

            requestDisposer = MainClient.instance.AddPacketListener(Packet.Flag.ACCOUNT_SIGNOUT_CALLBACK, packet =>
            {
                joinBtn.SetDeferred(Button.PropertyName.Disabled, false);

                //ACCOUNT_SIGNOUT_CALLBACK
                bool isSucceed = (bool)packet.value[0];
                if (isSucceed)
                {
                    GD.Print("로그아웃 성공 : " + packet.value[1].ToString());

                    //로그아웃 성공에 따른 처리
                    CallDeferred("ControlExchange", "LoginInterface", "res://controls/LoginInterface.tscn");
                }
                else
                {
                    GD.Print("로그아웃 실패 : " + packet.value[1].ToString());
                    CallDeferred("ShowAcceptDialog", "로그아웃 실패", packet.value[1].ToString(), "확인");
                    //로그인 성공에 따른 처리
                    CallDeferred("ControlExchange", "LoginInterface", "res://controls/LoginInterface.tscn");
                }

                requestDisposer();
            });
        };

        MainClient.instance.AddPacketListener(Packet.Flag.ROOM_LIST, packet => {

            List<(int idx, string name, bool isPw, int state, int userCount)> newDataList =
                (List<(int idx, string name, bool isPw, int state, int userCount)>)packet.value[0];

            toDelList.Clear();
            foreach (var item in roomDataList)
                if (newDataList.Contains(item) == false)
                    toDelList.Add(item.idx);

            toRefreshList.Clear();
            foreach (var item in roomDataList)
            {
                var t = newDataList.Find(i => i.idx == item.idx);

                if (t.idx == item.idx)
                    if(t.userCount != item.userCount)
                        toRefreshList.Add((t.idx, t.userCount));
            }

            toAddList.Clear();
            foreach (var item in newDataList)
                if (roomDataList.Contains(item) == false)
                    toAddList.Add(item);


        });
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (toDelList.Count == 0 && toAddList.Count == 0) return;

        foreach (var idxToDel in toDelList)
        {
            var item = roomDataList.Find(i => i.idx == idxToDel);
            int idxInList = roomDataList.IndexOf(item);
            if (roomList.IsAnythingSelected())
                if (roomList.GetSelectedItems()[0] == idxInList)
                {
                    roomIsPublicTxt.Text = "";
                    roomNameTxt.Text = "";
                    roomPlayerCountTxt.Text = "";
                    roomStateTxt.Text = "";
                    joinBtn.Disabled = true;
                }

            roomList.RemoveItem(idxInList);
            roomDataList.Remove(item);
        }
        toDelList.Clear();

        foreach (var item in toAddList)
        {
            roomList.AddItem($"{item.name} ({item.userCount}/4)");
            roomDataList.Add(item);
        }
        toAddList.Clear();


        foreach (var item in toRefreshList)
        {
            //roomDataList.ForEach(e =>
            //    GD.Print($"data is {e.idx}/{e.userCount}")
            //);
            //GD.Print($"item is {item.idx}/{item.userCount}");

            var tItem = roomDataList.Find(i => {
                GD.Print("i.idx == item.idx : " + (i.idx == item.idx) + " /i.idx : " + i.idx + " - item.idx : " + item.idx);
                return i.idx == item.idx; });

            //GD.Print("roomDataList.IndexOf(tItem) : " + roomDataList.IndexOf(tItem));

            int oriIdx = roomDataList.IndexOf(tItem);

            tItem.userCount = item.userCount;

            roomList.SetItemText(oriIdx, $"{tItem.name} ({tItem.userCount}/4)");

            roomList.Select(oriIdx);
            RefreshRoomList(oriIdx);
        }
        toRefreshList.Clear();


    }

    void RefreshRoomList(int e)
    {
        roomIsPublicTxt.Text = roomDataList[e].isPw ? "비공개방" : "공개방";
        roomNameTxt.Text = roomDataList[e].name;
        roomPlayerCountTxt.Text = roomDataList[e].userCount.ToString();
        roomStateTxt.Text = roomDataList[e].state.ToString();

        joinBtn.Disabled = roomDataList[e].userCount == 4 ? true : false;
    }

}