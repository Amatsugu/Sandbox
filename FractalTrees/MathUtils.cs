using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FractalTrees
{
	public class MathUtils
	{
		private MathUtils()
		{

		}

		public static double Map(double value, double min, double max, double a, double b) => Lerp(a, b, (value - min) / (max - min));

		public static double Lerp(double a, double b, double time) => a + (b - a) * time;
	}
}
