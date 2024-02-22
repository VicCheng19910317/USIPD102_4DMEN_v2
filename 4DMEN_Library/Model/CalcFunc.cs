using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4DMEN_Library.Model
{
    public class Point3D
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
    }
    public static class CalcFunc
    {
        /// <summary>
        /// 計算基準面
        /// </summary>
        /// <param name="real_pos">設定x,y位置(PLC位置)</param>
        /// <param name="est_hei">量測高度結果</param>
        /// <param name="base_param">基準面四個參數(a,b,c,d)</param>
        /// <returns>平面公式</returns>
        public static string CalBasePlane(List<EstimatePosition> real_pos, List<float> est_hei,out List<float> base_param)
        {
            string result = "";
            base_param = new List<float>();
            if(real_pos.Count < 3 || est_hei.Count < 3)
            {
                result = "ALARM";
                return result;
            }
            List<Point3D> tmp = new List<Point3D>()
            {
                new Point3D{ X = real_pos[0].X / 100.0f, Y = real_pos[0].Y / 100.0f, Z = est_hei[0]},
                new Point3D{ X = real_pos[1].X / 100.0f, Y = real_pos[1].Y / 100.0f, Z = est_hei[1]},
                new Point3D{ X = real_pos[2].X / 100.0f, Y = real_pos[2].Y / 100.0f, Z = est_hei[2]}
            };
            base_param.Add((float)Math.Round((tmp[1].Y - tmp[0].Y) * (tmp[2].Z - tmp[0].Z) - (tmp[1].Z - tmp[0].Z) * (tmp[2].Y - tmp[1].Y), 2)); //a
            base_param.Add((float)Math.Round((tmp[1].Z - tmp[0].Z) * (tmp[2].X - tmp[0].X) - (tmp[1].X - tmp[0].X) * (tmp[2].Z - tmp[1].Z), 2)); //b
            base_param.Add((float)Math.Round((tmp[1].X - tmp[0].X) * (tmp[2].Y - tmp[0].Y) - (tmp[1].Y - tmp[0].Y) * (tmp[2].X - tmp[1].X), 2)); //c
            base_param.Add((float)Math.Round(0 - (base_param[0] * tmp[0].X + base_param[1] * tmp[0].Y + base_param[2] * tmp[0].Z), 2)); //d
            result = $"{base_param[0]}X + {base_param[1]}Y + {base_param[2]}Z = {-base_param[3]}";
            return result;
        }
        /// <summary>
        /// 計算平面距離
        /// </summary>
        /// <param name="real_pos">設定x,y位置(PLC位置)</param>
        /// <param name="est_hei">量測高度結果</param>
        /// <param name="base_param">基準面四個參數(a,b,c,d)</param>
        /// <returns>計算結果</returns>
        public static List<float> CalDist(List<EstimatePosition> real_pos, List<float> est_hei, List<float> base_param)
        {
            List<float> result = new List<float>();
            if (real_pos.Count < 12 || est_hei.Count < 12 || base_param.Count < 4 || est_hei.Contains(float.NaN))
                return result;
            for(int i = 3; i < real_pos.Count; i++)
            {
                var pos = new Point3D
                {
                    X = real_pos[i].X / 100.0f,
                    Y = real_pos[i].Y / 100.0f,
                    Z = est_hei[i],
                };
                result.Add((float)Math.Round((base_param[0] * pos.X + base_param[1] * pos.Y + base_param[2] * pos.Z + base_param[3]) / Math.Sqrt(Math.Pow(base_param[0], 2) + Math.Pow(base_param[1], 2) + Math.Pow(base_param[2], 2)), 2));
            }
            return result;
        }
        /// <summary>
        /// 計算平整度
        /// </summary>
        /// <param name="cal_dist">平面距離</param>
        /// <returns>計算結果</returns>
        public static List<float> CalFlatness(List<float> cal_dist)
        {
            List<float> result = new List<float>();
            if (cal_dist.Count < 9 || cal_dist.Contains(float.NaN))
                return result;
            var tmp = new List<float> { cal_dist[0], cal_dist[1], cal_dist[8] };
            result.Add((float)Math.Round(Math.Abs(tmp.Max() - tmp.Min()), 2));
            tmp = new List<float> { cal_dist[2], cal_dist[3], cal_dist[7] };
            result.Add((float)Math.Round(Math.Abs(tmp.Max() - tmp.Min()), 2));
            tmp = new List<float> { cal_dist[4], cal_dist[5], cal_dist[6] };
            result.Add((float)Math.Round(Math.Abs(tmp.Max() - tmp.Min()), 2));
            return result;
        }
    }
}
