using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vayfrem.models.structs
{
    public class Vector2
    {
        public double X { get; set; } 
        public double Y { get; set; }

        public Vector2 (double x, double y) {  X = x; Y = y; }

        public Vector2 (Vector2 vec2)
        {
            X = vec2.X;
            Y = vec2.Y;
        }

        public static bool operator == (Vector2 v1, Vector2 v2)
        {
            if (v1.X == v2.X && v1.Y == v2.Y)
            {
                return true;
            }
            return false;
        }

        public static bool operator != (Vector2 v1, Vector2 v2)
        {
            if (v1.X == v2.X && v1.Y == v2.Y)
            {
                return false;
            }
            return true;
        }

        public static Vector2 operator - (Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static Vector2 operator +(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static Vector2 operator /(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static Vector2 Normalize(Vector2 v)
        {
            float length = (float)Math.Sqrt(v.X * v.X + v.Y * v.Y);

            if (length > 0)
                return new Vector2(v.X / length, v.Y / length);
            else
                return new Vector2(0,0); // Sıfır vektörü normalize edildiğinde sıfır vektörü döndürülür
        }

        public static double Dot(Vector2 v1, Vector2 v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y;
        }

        public double Length()
        {
            return Math.Sqrt((X * X) + (Y * Y));
        }
    }
}
