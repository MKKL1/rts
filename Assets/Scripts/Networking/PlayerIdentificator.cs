using Mirror;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Networking
{
    public struct PlayerIdentificator : IComparable<PlayerIdentificator>
    {
        public byte id;

        public PlayerIdentificator(byte id)
        {
            this.id = id;
        }

        public bool isServer() { return id == serverID; }
        public bool isPlayer() { return id != serverID; }

        public int CompareTo(PlayerIdentificator obj)
        {
            if (this.id < obj.id)
                return 1;
            else if (this.id > obj.id)
                return -1;
            else
                return 0;
        }

        public static implicit operator PlayerIdentificator(byte newid)
        {
            return new PlayerIdentificator(newid);
        }

        public static byte serverID = 0;
    }

    public static class PlayerIDNetWriteRead
    {
        public static void WritePlayerID(this NetworkWriter networkWriter, PlayerIdentificator value)
        {
            networkWriter.WriteByte(value.id);
        }

        public static PlayerIdentificator ReadPlayerID(this NetworkReader networkReader)
        {
            return new PlayerIdentificator(networkReader.ReadByte());
        }
    }
}