using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;

namespace FractalTrees.Fractal
{
	class Mandlebrot
	{
		public static void Render(SKBitmap bitmap, float scale = 2.5f, SKPoint pos = default)
		{
			var height = bitmap.Height;
			var width = bitmap.Width;
			if (pos == default)
			{
				pos.X = -.5f;
				pos.Y = (1f - ((float)height / width)) * scale;
			}
			var iterations = 100;
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					var a = MathUtils.Map(x, 0, width, -scale + pos.X, scale + pos.X);
					var b = MathUtils.Map(y, 0, width, -scale + pos.Y, scale + pos.Y);
					var n = 0;
					var ca = a;
					var cb = b;
					//ca = Map(ca, -100, 100, -1, 1);
					//cb = Map(cb, -100, 100, -1, 1);
					for (n = 0; n < iterations; n++)
					{
						var aa = a * a - b * b;
						var bb = 2 * a * b;

						a = aa + ca;
						b = bb + cb;

						if (Math.Abs(a + b) > 16)
						{
							break;
						}
					}

					var bright = MathUtils.Map(n, 0, iterations, 0, 1);
					bright = Math.Sqrt(bright);
					if (iterations == n)
						bright = 0;
					bitmap.SetPixel(x, y, new SKColor((byte)(255 * bright), (byte)(0 * bright), (byte)(100 * bright)));
				}
			};
		}
	}
}
