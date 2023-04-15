using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;

namespace FractalTrees
{
	class Metaballs
	{
		private Blob[] blobs;
		private double width, height;
		private SKBitmap bitmap;
		private double diag;

		public Metaballs(SKBitmap bitmap, int balls)
		{
			blobs = new Blob[balls];
			this.bitmap = bitmap;
			height = bitmap.Height;
			width = bitmap.Width;
			var rng = new Random();
			for (int i = 0; i < balls; i++)
				blobs[i] = new Blob(rng.NextDouble() * width, rng.NextDouble() * height, rng.NextDouble() * 50);
			diag = Math.Sqrt(Math.Pow(width, 2) + Math.Pow(height, 2));
		}

		public void Render(double delta)
		{
			var b = blobs.First();
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					double d = b.Position.Distance(x, y);
					var c = (byte)Math.Min(255, (b.Radius * 1000) / d);
					var col = new SKColor(c, c, c);
					bitmap.SetPixel(x, y, col);
				}
			}
			b.Update(delta);
		}
	}

	class Blob
	{
		public Vector Position { get; set; }
		public Vector Velocity { get; set; }
		public double Radius { get; set; }

		public Blob(Vector position, double radius = 40)
		{
			Position = position;
			Radius = radius;
			Velocity = Vector.Random2D() * .01;
		}

		public void Update(double delta)
		{
			Position += Velocity * delta;
		}

		public Blob(double x, double y, double radius = 40) :this(new Vector(x,y), radius)
		{
			
		}
	}
}
