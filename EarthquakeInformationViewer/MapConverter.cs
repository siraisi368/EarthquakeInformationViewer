using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EarthquakeInformationViewer
{
    public static class MapConverter
    {
        const double L = 85.05112878;

        /// <summary>
        /// 地理座標からピクセル座標に変換する
        /// </summary>
        /// <param name="Latlon">地理座標</param>
        /// <param name="zoom">ズームレベル</param>
        public static PointDouble LatlonToPixel(Latlon latlon, double zoom)
        {
            var x = Math.Pow(2, zoom + 7) * ((latlon.Latitude / 180) + 1);

            var y = (Math.Pow(2, zoom + 7) / Math.PI) *

                    ((-1 * (atanh(Math.Sin((Math.PI / 180) * latlon.Longitude)))) +

                    (atanh(Math.Sin((Math.PI / 180) * L))));

            return new PointDouble(x, y);
        }

        /// <summary>
        /// ピクセル座標から地理座標に変換する
        /// </summary>
        public static Latlon PixelToLatlon(PointDouble pd, double zoom)
        {
            var lon = 180 * (pd.X / (Math.Pow(2, zoom + 7)) - 1);

            var lat = (180 / Math.PI) *
                      (Math.Asin(Math.Tanh(
                          ((-1 * ((Math.PI / Math.Pow(2, zoom + 7)) * pd.Y)) +
                          (atanh(Math.Sin((Math.PI / 180) * L))))
                      )));

            return new Latlon(lat, lon);
        }

        /// <summary>
        /// 逆双曲線正接
        /// </summary>
        public static double atanh(double x)
        {
            return (1d / 2d * Math.Log((1 + x) / (1 - x), Math.E));
        }

        public class Latlon
        {
            /// <summary>
            /// Lat:緯度, Lon:経度
            /// </summary>
            public double Latitude, Longitude;

            public Latlon(double lat, double lon)
            {
                this.Latitude = lat;
                this.Longitude = lon;
            }
        }

        public class PointDouble
        {
            public double X, Y;
            public PointDouble(double x, double y)
            {
                this.X = x;
                this.Y = y;
            }

            public PointDouble Diff(PointDouble x)
            {
                return new PointDouble(this.X - x.X, this.Y - x.Y);
            }
            public PointDouble DiffAbs(PointDouble x)
            {
                return new PointDouble(Math.Abs(this.X - x.X), Math.Abs(this.Y - x.Y));
            }
        }
    }
}
