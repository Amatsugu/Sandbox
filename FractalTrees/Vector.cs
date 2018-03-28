using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalTrees
{
	public struct Vector
	{
		public double X { get; set; }
		public double Y { get; set; }
		public double Z { get; set; }

		private static Random rand = new Random();

		public Vector(double x = 0, double y = 0, double z = 0)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public double Distance(double x, double y = 0, double z = 0) => Distance(this, new Vector(x, y, z));
		public double Distance(Vector b) => Distance(this, b);

		public static double Distance(Vector a, Vector b) => 
			Math.Sqrt(Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2) + Math.Pow(b.Z - a.Z, 2));

		public static Vector Random2D()
		{
			return new Vector(rand.NextDouble(), rand.NextDouble());
		}

		public override string ToString()
		{
			return $"{X}, {Y}, {Z}";
		}

		public static Vector operator *(Vector a, double b) => new Vector(a.X * b, a.Y * b, a.Z * b);
		public static Vector operator *(double b, Vector a) => new Vector(a.X * b, a.Y * b, a.Z * b);
		public static Vector operator +(Vector a, Vector b) => new Vector(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
	}
}
