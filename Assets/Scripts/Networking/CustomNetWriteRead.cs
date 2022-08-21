using Mirror;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Networking
{
    public static class CustomNetWriteRead
    {
        public static void WriteArray<T>(this NetworkWriter networkWriter, T[,] array) 
        {
            networkWriter.WriteUInt((uint)array.GetLength(0));
            networkWriter.WriteUInt((uint)array.GetLength(1));

            for(int i = 0; i < array.GetLength(0); i++)
                for(int j = 0; j < array.GetLength(1); j++)
                {
                    networkWriter.Write(array[i, j]);
                }
        }

        public static T[,] ReadArray<T>(this NetworkReader networkReader)
        {
            T[,] array = new T[networkReader.ReadUInt(), networkReader.ReadUInt()];

            for (int i = 0; i < array.GetLength(0); i++)
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    array[i,j] = networkReader.Read<T>();
                }

            return array;
        }
    }
}