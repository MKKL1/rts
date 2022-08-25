using Assets.Scripts.TerrainScripts.Details;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.TerrainScripts
{

    public class MainGrid : GridBase<MainGridChunk>
    {
        public MainGrid(int chunkDataSizeX, int chunkDataSizeY, Vector2 terrainSize)
            : base(
                  chunkDataSizeX, 
                  chunkDataSizeY, 
                  terrainSize.x / chunkDataSizeX,
                  terrainSize.y / chunkDataSizeY) { }

        //public Texture2D GetWalkable()
        //{
        //    Texture2D texture = new Texture2D(chunkDataSize.x, chunkDataSize.y, TextureFormat.RGBA32, false);
        //    for(int i = 0; i < chunkDataSize.x; i++)
        //        for(int j = 0; j < chunkDataSize.y; j++)
        //        {
        //            if ([i, j])
        //                texture.SetPixel(i, j, Color.white);
        //            else texture.SetPixel(i, j, Color.black);
        //        }
        //    texture.Apply();
        //    return texture;
        //}
        

    }
}