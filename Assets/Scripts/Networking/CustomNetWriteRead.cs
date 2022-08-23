using Assets.Scripts.TerrainScripts;
using Assets.Scripts.TerrainScripts.Details;
using Assets.Scripts.TerrainScripts.Generation;
using Mirror;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Networking
{
    public static class CustomNetWriteRead
    {
        /// <summary>
        /// Write array with header (size of array in uint)
        /// </summary>
        public static void WriteArrayH<T>(this NetworkWriter networkWriter, T[,] array) 
        {
            networkWriter.WriteUInt((uint)array.GetLength(0));
            networkWriter.WriteUInt((uint)array.GetLength(1));

            for(int i = 0; i < array.GetLength(0); i++)
                for(int j = 0; j < array.GetLength(1); j++)
                {
                    networkWriter.Write(array[i, j]);
                }
        }

        /// <summary>
        /// Read array with header (size of array in uint)
        /// </summary>
        public static T[,] ReadArrayH<T>(this NetworkReader networkReader)
        {
            T[,] array = new T[networkReader.ReadUInt(), networkReader.ReadUInt()];

            for (int i = 0; i < array.GetLength(0); i++)
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    array[i,j] = networkReader.Read<T>();
                }

            return array;
        }

        public static void WriteArray<T>(this NetworkWriter networkWriter, T[,] array)
        {
            //Checking if weaver has given writer before iterating
            if (Writer<T>.write == null)
            {
                Debug.LogError("No writer found for " + typeof(T));
                return;
            }

            for (int i = 0; i < array.GetLength(0); i++)
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    networkWriter.Write(array[i, j]);
                }
        }


        public static T[,] ReadArray<T>(this NetworkReader networkReader, uint sizex, uint sizey)
        {
            //Checking if weaver has given reader before iterating
            if (Reader<T>.read == null)
            {
                Debug.LogError("No reader found for " + typeof(T));
                return null;
            }

            T[,] array = new T[sizex, sizey];

            for (int i = 0; i < array.GetLength(0); i++)
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    array[i, j] = networkReader.Read<T>();
                }

            return array;
        }
    }
}