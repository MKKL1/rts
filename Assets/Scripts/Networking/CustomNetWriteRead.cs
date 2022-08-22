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

        public static void WriteTerrainGenResult(this NetworkWriter networkWriter, TerrainGeneratorResult value)
        {
            networkWriter.WriteVector2Int(value.mainGridSize);
            networkWriter.WriteVector2(value.terrainSize);
            networkWriter.WriteVector2Int(value.terrainGridSize);
            networkWriter.WriteArray(value.biomeGrid);
            networkWriter.WriteArray(value.heightMap);
            networkWriter.WriteArray(value.resourceMap);
        }

        public static TerrainGeneratorResult ReadTerrainGenResult(this NetworkReader networkReader)
        {
            TerrainGeneratorResult value = new TerrainGeneratorResult();
            value.mainGridSize = networkReader.ReadVector2Int();
            value.terrainSize = networkReader.ReadVector2();
            value.terrainGridSize = networkReader.ReadVector2Int();
            value.biomeGrid = networkReader.ReadArray<BiomeType>((uint)value.terrainGridSize.x, (uint)value.terrainGridSize.y);
            value.heightMap = networkReader.ReadArray<byte>((uint)value.terrainGridSize.x, (uint)value.terrainGridSize.y);
            value.resourceMap = networkReader.ReadArray<TerrainResourceNode>((uint)value.terrainGridSize.x, (uint)value.terrainGridSize.y);
            return value;
        }

        //TODO for some reason enum has to be treated like custom type
        public static void WriteBiomeType(this NetworkWriter networkWriter, BiomeType value)
        {
            networkWriter.WriteByte((byte)value);
        }

        public static BiomeType ReadBiomeType(this NetworkReader networkReader)
        {
            return (BiomeType)networkReader.ReadByte();
        }
    }
}