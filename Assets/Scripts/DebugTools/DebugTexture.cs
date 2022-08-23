using Assets.Scripts.TerrainScripts.Details;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.DebugTools
{
    public class DebugTexture
    {
        public int xS;
        public int yS;
        public Texture2D texture;
        private Color[,] colors;
        public DebugTexture(int x, int y)
        {
            xS = x;
            yS = y;
            colors = new Color[x, y];
            texture = new Texture2D(x, y, TextureFormat.RGB24, false);
        }

        public void SetFromArray(byte[,] bytes)
        {
            for(int i = 0; i < xS; i++)
                for(int j = 0; j < yS; j++)
                {
                    float c = (float)bytes[i, j] / 255f;
                    colors[i, j] =  new Color(c, c, c);
                }
        }

        public void SetFromArray(bool[,] bools)
        {
            for (int i = 0; i < xS; i++)
                for (int j = 0; j < yS; j++)
                {
                    float c = bools[i, j] ? 1f : 0f;
                    colors[i, j] = new Color(c, c, c);
                }
        }

        public void SetFromArray(TerrainResourceNode[,] resource)
        {
            for (int i = 0; i < xS; i++)
                for (int j = 0; j < yS; j++)
                {
                    Color c = Color.white;
                    switch(resource[i,j].prefabsList)
                    {
                        case ResourcePrefabsList.NONE: c = Color.white; break;
                        case ResourcePrefabsList.TREE: c = Color.green; break;
                        case ResourcePrefabsList.GOLD: c = Color.yellow; break;
                        case ResourcePrefabsList.ROCK: c = Color.gray; break;
                    }
                    colors[i, j] = c;
                }
        }

        public void SetFromArray(float[,] val)
        {
            for (int i = 0; i < xS; i++)
                for (int j = 0; j < yS; j++)
                {
                    float c = val[i, j];
                    colors[i, j] = new Color(c, c, c);
                }
        }

        public void SetPixel(int x, int y, Color color)
        {
            colors[x, y] = color;
        }

        public void SetPixel(int x, int y, float value)
        {
            colors[x, y] = new Color(value, value, value);
        }

        public void Apply()
        {
            for (int i = 0; i < xS; i++)
                for (int j = 0; j < yS; j++)
                {
                    texture.SetPixel(i, j, colors[i, j]);
                }
            texture.Apply();
        }

        public void SaveToPath(string path, string name)
        {
            Apply();
            byte[] bytes = texture.EncodeToPNG();
            var dirPath = Application.dataPath + "/" + path;
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            string fpath = dirPath + name + ".png";
            File.WriteAllBytes(fpath, bytes);
            Debug.Log($"Saving debug texture to {fpath}");
        }
    }
}