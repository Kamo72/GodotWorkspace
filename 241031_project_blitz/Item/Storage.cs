
using System;
using System.Collections.Generic;
using Godot;

public class Storage
{
    public struct StorageNode
    {
        public Item item;
        public Vector2I pos;
        public bool isRotated;

        public override string ToString()
        {
            return $"{item.status.name} {pos} {(isRotated ? "rotated" : "")}";
        }
    }

    public Vector2I size { get; set; } // 저장공간 크기
    List<Type> whiteList; //화이트리스트 whiteList.Count() == 0? 전체 허용
    public List<StorageNode> itemList = new List<StorageNode>();    //저장 노드 리스트

    public Storage(Vector2I size, List<Type> whiteList = null)
    {
        this.size = size;
        this.whiteList = whiteList ?? new List<Type>();
    }

    #region [멤버 함수]
    //stackable에 따른 별도의 처리를 생각해두지 않았당...
    //아예 저장 가능, 저장 불가 외에 일부만 저장 가능이라는 경우가 하나 더 생김...
    //내일의 내가 알아서 하자


    //허용된 유형인가?
    public bool IsWhiteList(Item item)
    {
        if (whiteList.Count == 0) { return true; }

        foreach (Type type in whiteList)
        {
            if (type.IsAssignableFrom(item.GetType()))
            {
                return true;
            }
        }

        return false;
    }

    //임의 위치에 아이템을 저장 가능
    public bool IsAbleToInsert(Item item)
    {
        Vector2I itemSize = item.status.size;

        // 모든 가능한 위치에서 시도
        for (int i = 0; i <= size.X - itemSize.X; i++)
        {
            for (int j = 0; j <= size.Y - itemSize.Y; j++)
            {
                StorageNode newNode = new StorageNode
                {
                    item = item,
                    pos = new Vector2I(i, j),
                    isRotated = false // 일단은 회전 안 시키도록 설정
                };

                // 회전을 고려한 newNode를 만들어서 겹치지 않으면 반환
                if (!IsOverlapped(newNode))
                {
                    return true;
                }

                // 회전해서 시도
                newNode.isRotated = true;
                if (!IsOverlapped(newNode))
                {
                    return true;
                }
            }
        }

        return false; // 아무 공간도 찾지 못한 경우
    }

    //특정 위치에 아이템을 저장 가능. - StorageNode가 겹쳐도 StorageNode.item과 item이 같다면 무시
    public bool IsAbleToInsert(Item item, Vector2I pos, bool isRotated)
    {
        StorageNode newNode = new StorageNode
        {
            item = item,
            pos = pos,
            isRotated = isRotated
        };

        return !IsOverlapped(newNode);
    }

    //입력된 노드에 따라 아이템을 저장.
    public bool Insert(StorageNode newNode)
    {
        if (IsWhiteList(newNode.item) == false)
            return false;

        if (IsOverlapped(newNode))
            return false;

        if (newNode.item.onStorage != null)
            newNode.item.onStorage.RemoveItem(newNode.item);

        itemList.Add(newNode);
        newNode.item.onStorage = this;

        return true;
    }

    //자동으로 아이템이 들어갈 수 있는 공간을 찾아 반환
    public StorageNode? GetPosInsert(Item item)
    {
        // 모든 가능한 위치에서 시도
        for (int i = 0; i <= size.Y; i++)
        {
            for (int j = 0; j <= size.X; j++)
            {
                StorageNode newNode = new StorageNode
                {
                    item = item,
                    pos = new Vector2I(j, i),
                    isRotated = false // 일단은 회전 안 시키도록 설정
                };

                // 회전을 고려한 newNode를 만들어서 겹치지 않으면 반환
                if (!IsOverlapped(newNode))
                {
                    return newNode;
                }

                // 회전해서 시도
                newNode.isRotated = true;
                if (!IsOverlapped(newNode))
                {
                    return newNode;
                }
            }
        }

        return null; // 아무 공간도 찾지 못한 경우
    }

    //내부에 pos 위치에 아이템이 있는 확인 후 반환
    public StorageNode? GetPosTo(Vector2I pos)
    {
        foreach (var node in itemList)
        {
            Vector2I itemSize = node.isRotated ? new Vector2I(node.item.status.size.Y, node.item.status.size.X) : node.item.status.size;

            // pos가 해당 아이템의 범위 내에 있는지 확인
            if (pos.X >= node.pos.X && pos.X < node.pos.X + itemSize.X &&
                pos.Y >= node.pos.Y && pos.Y < node.pos.Y + itemSize.Y)
            {
                return node;
            }
        }

        return null; // 해당 위치에 아무 아이템도 없는 경우
    }

    //인벤토리 내부의 Item을 찾아 제거.
    public bool RemoveItem(Item item)
    {
        foreach (var node in itemList)
        {
            if (node.item == item)
            {
                itemList.Remove(node);
                item.onStorage = null;

                return true;
            }
        }
        return false;
    }

    private bool IsOverlapped(StorageNode newNode)
    {
        Vector2I newOneSize = newNode.isRotated ? new Vector2I(newNode.item.status.size.Y, newNode.item.status.size.X) : newNode.item.status.size;

        // Storage 범위를 벗어나는지 검사
        if (newNode.pos.X < 0 || newNode.pos.Y < 0 || newNode.pos.X + newOneSize.X > size.X || newNode.pos.Y + newOneSize.Y > size.Y)
        {
            return true; // 범위를 벗어남
        }

        foreach (var node in itemList)
        {
            if (CheckOverlap(node, newNode))
            {
                return true;
            }
        }
        return false;
    }
    private bool CheckOverlap(StorageNode node1, StorageNode node2)
    {
        Vector2I newOneSize = node1.isRotated ? new Vector2I(node1.item.status.size.Y, node1.item.status.size.X) : node1.item.status.size;
        Vector2I oldOneSize = node2.isRotated ? new Vector2I(node2.item.status.size.Y, node2.item.status.size.X) : node2.item.status.size;

        if (node1.pos.X < node2.pos.X + oldOneSize.X &&
            node1.pos.X + newOneSize.X > node2.pos.X &&
            node1.pos.Y < node2.pos.Y + oldOneSize.Y &&
            node1.pos.Y + newOneSize.Y > node2.pos.Y)
        {
            return true; // Overlapping
        }
        return false; // Not overlapping
    }

    #endregion
}
