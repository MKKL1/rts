using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Terrain
{
    //Modified from https://github.com/mdymel/superfastblur
    public class BiomeGuassianBlur
    {
        private List<byte[,]> biomeMap;

        private readonly int _width;
        private readonly int _height;

        private BiomeWeightManager biomeWeightManager;

        private List<Task> tasks;

        private readonly ParallelOptions _pOptions = new ParallelOptions { MaxDegreeOfParallelism = 16 };

        public BiomeGuassianBlur(ref BiomeWeightManager biomeWeightManager)
        {
            //for(byte i = 0; i < biomeWeightManager.biomeCount; i++)
            //{
            //    biomeMap.Add((BiomesType)i, biomeWeightManager.biomeWeightMap[i]);
            //}
            this.biomeWeightManager = biomeWeightManager;
            biomeMap = biomeWeightManager.biomeWeightMap;
            _width = biomeWeightManager.size.x;
            _height = biomeWeightManager.size.y;
        }

        public void Process(int radial)
        {

            //Parallel.Invoke(
            //    () => gaussBlur_4(_alpha, newAlpha, radial),
            //    () => gaussBlur_4(_red, newRed, radial),
            //    () => gaussBlur_4(_green, newGreen, radial),
            //    () => gaussBlur_4(_blue, newBlue, radial));
            List<byte[]> destList = new List<byte[]>(biomeWeightManager.biomeCount);

            for(int i = 0; i < biomeWeightManager.biomeCount; i++)
                tasks.Add(ProcessWeightMap(biomeMap[i], destList[i], radial));

            tasks.ForEach(x => x.Start());
            Task.WaitAll(tasks.ToArray());

            for(int i = 0; i < destList.Count; i++)
            {
                for (int j = 0; j < _width; j++)
                    for (int k = 0; k < _height; k++)
                        biomeMap[i][j,k] = destList[i][j + (k * _width)];
            }

            biomeWeightManager.SetBiomeWeightMap(biomeMap);
            //Parallel.For(0, dest.Length, _pOptions, i =>
            //{
            //    if (newAlpha[i] > 255) newAlpha[i] = 255;
            //    if (newRed[i] > 255) newRed[i] = 255;
            //    if (newGreen[i] > 255) newGreen[i] = 255;
            //    if (newBlue[i] > 255) newBlue[i] = 255;

            //    if (newAlpha[i] < 0) newAlpha[i] = 0;
            //    if (newRed[i] < 0) newRed[i] = 0;
            //    if (newGreen[i] < 0) newGreen[i] = 0;
            //    if (newBlue[i] < 0) newBlue[i] = 0;
            //});
        }

        private Task ProcessWeightMap(byte[,] source, byte[] dest, int radial)
        {
            return new Task(() =>
            {
                byte[] newsource = new byte[_width * _height];
                for (int i = 0; i < _width; i++)
                    for (int j = 0; j < _height; j++)
                        newsource[i + (j * _width)] = source[i, j];
                gaussBlur_4(newsource, dest, radial);
            });
        }

        private void gaussBlur_4(byte[] source, byte[] dest, int r)
        {
            var bxs = boxesForGauss(r, 3);
            boxBlur_4(source, dest, _width, _height, (bxs[0] - 1) / 2);
            boxBlur_4(dest, source, _width, _height, (bxs[1] - 1) / 2);
            boxBlur_4(source, dest, _width, _height, (bxs[2] - 1) / 2);
        }

        private int[] boxesForGauss(int sigma, int n)
        {
            var wIdeal = Mathf.Sqrt((12 * sigma * sigma / n) + 1);
            var wl = (int)Mathf.Floor(wIdeal);
            if (wl % 2 == 0) wl--;
            var wu = wl + 2;

            var mIdeal = (float)(12 * sigma * sigma - n * wl * wl - 4 * n * wl - 3 * n) / (-4 * wl - 4);
            var m = Mathf.Round(mIdeal);

            var sizes = new List<int>();
            for (var i = 0; i < n; i++) sizes.Add(i < m ? wl : wu);
            return sizes.ToArray();
        }

        private void boxBlur_4(byte[] source, byte[] dest, int w, int h, int r)
        {
            for (var i = 0; i < source.Length; i++) dest[i] = source[i];
            boxBlurH_4(dest, source, w, h, r);
            boxBlurT_4(source, dest, w, h, r);
        }

        private void boxBlurH_4(byte[] source, byte[] dest, int w, int h, int r)
        {
            var iar = (float)1 / (r + r + 1);
            Parallel.For(0, h, _pOptions, i =>
            {
                var ti = i * w;
                var li = ti;
                var ri = ti + r;
                var fv = source[ti];
                var lv = source[ti + w - 1];
                var val = (r + 1) * fv;
                for (var j = 0; j < r; j++) val += source[ti + j];
                for (var j = 0; j <= r; j++)
                {
                    val += source[ri++] - fv;
                    dest[ti++] = (byte)Mathf.Round(val * iar);
                }
                for (var j = r + 1; j < w - r; j++)
                {
                    val += source[ri++] - dest[li++];
                    dest[ti++] = (byte)Mathf.Round(val * iar);
                }
                for (var j = w - r; j < w; j++)
                {
                    val += lv - source[li++];
                    dest[ti++] = (byte)Mathf.Round(val * iar);
                }
            });
        }

        private void boxBlurT_4(byte[] source, byte[] dest, int w, int h, int r)
        {
            var iar = (float)1 / (r + r + 1);
            Parallel.For(0, w, _pOptions, i =>
            {
                var ti = i;
                var li = ti;
                var ri = ti + r * w;
                var fv = source[ti];
                var lv = source[ti + w * (h - 1)];
                var val = (r + 1) * fv;
                for (var j = 0; j < r; j++) val += source[ti + j * w];
                for (var j = 0; j <= r; j++)
                {
                    val += source[ri] - fv;
                    dest[ti] = (byte)Mathf.Round(val * iar);
                    ri += w;
                    ti += w;
                }
                for (var j = r + 1; j < h - r; j++)
                {
                    val += source[ri] - source[li];
                    dest[ti] = (byte)Mathf.Round(val * iar);
                    li += w;
                    ri += w;
                    ti += w;
                }
                for (var j = h - r; j < h; j++)
                {
                    val += lv - source[li];
                    dest[ti] = (byte)Mathf.Round(val * iar);
                    li += w;
                    ti += w;
                }
            });
        }
    }
}