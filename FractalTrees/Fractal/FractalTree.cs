using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;

namespace FractalTrees.Fractal
{
	class FractalTree
	{
		private float branchDecay;
		private float angleDecay;
		private SKPaint paint;
		private SKCanvas canvas;
		private double height, width;

		public FractalTree(SKBitmap bitmap, float branchDecay, float angleDecay)
		{
			height = bitmap.Height;
			width = bitmap.Width;
			this.branchDecay = branchDecay;
			this.angleDecay = angleDecay;
			canvas = new SKCanvas(bitmap);
			paint = new SKPaint
			{
				Color = new SKColor(255, 0, 100),
				StrokeWidth = 4,
				IsAntialias = true
			};
		}

		public FractalTree Branch(float length, SKPoint p1 = default, float angle = default)
		{
			if (p1.X < 0 || p1.Y < 0)
				return this;
			if (p1.X > width || p1.Y > height)
				return this;
			var m = MathUtils.Map(length, 0, height / 2, 0, 1);
			paint.Color = new SKColor((byte)(255 * m), (byte)(255 * (1 - m)), (byte)(0 * m));
			paint.StrokeWidth = (float)MathUtils.Map(length, 0, height, 0.001, 4);
			var p2 = new SKPoint(-length * (float)Math.Sin(angle), -length * (float)Math.Cos(angle)) + p1;
			canvas.DrawLine(p1, p2, paint);
			if (length > 10)
			{
				Branch(length * branchDecay, p2, angle + angleDecay);
				Branch(length * branchDecay, p2, angle - angleDecay);
			}
			return this;
		}

		public void Render()
		{
			canvas.Flush();
		}


	}
}
