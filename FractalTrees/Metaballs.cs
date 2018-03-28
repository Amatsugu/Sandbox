using System;
using System.Collections.Generic;
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
			//blobs = new Blob[balls];
			this.bitmap = bitmap;
			height = bitmap.Height;
			width = bitmap.Width;
			diag = Math.Sqrt(Math.Pow(width, 2) + Math.Pow(height, 2));
			blobs = new Blob[] { new Blob(100, 100) };
		}

		public void Render()
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
			b.Update();
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
			Velocity = Vector.Random2D() * 20;
		}

		public void Update()
		{
			Position = Position + Velocity;
		}

		public Blob(double x, double y, double radius = 40) :this(new Vector(x,y), radius)
		{
			
		}
	}
}
