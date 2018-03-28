using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SkiaSharp;

namespace FractalTrees
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private SKBitmap bitmap;
		private float branchAngle = (float)Math.PI / 8f;
		private float branchDecay = 0.8f;
		private int width = 1920, height = 1080;
		private SKCanvas canvas;
		private bool useValue = true;
		private double _ca, _cb;
		private SKPaint paint = new SKPaint
		{
			Color = new SKColor(255, 0, 100),
			StrokeWidth = 4,
			IsAntialias = true
		};

		public MainWindow()
		{
			InitializeComponent();
			var multi = 0.5f;
			height = (int)Math.Floor(multi * height);
			width = (int)Math.Floor(multi * width);
			bitmap = new SKBitmap(width, height, SKImageInfo.PlatformColorType, SKAlphaType.Opaque);
			canvas = new SKCanvas(bitmap);
			canvas.Clear(new SKColor(255 / 10, 0, 10));
			canvas.Flush();
			Thread t = new Thread(Start);
			t.Start();
		}

		public void Start()
		{
			while(true)
			{
				Mandlebrot();
				UpdateImage();
			}
			/*var f = new FileStream("mandlebrot.png", FileMode.Create);
			SKImage.FromBitmap(bitmap).Encode(SKEncodedImageFormat.Png, 100).SaveTo(f);
			f.Flush();*/
		}

		void Mandlebrot(float scale = 2.5f, SKPoint pos = default)
		{
			if(pos == default)
			{
				pos.X = -.5f;
				pos.Y = (1f - ((float)height / width)) * scale;
			}
			var iterations = 100;
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					var a = Map(x, 0, width, -scale + pos.X, scale + pos.X);
					var b = Map(y, 0, width, -scale + pos.Y, scale + pos.Y);
					var n = 0;
					var ca = a;
					var cb = b;
					if (useValue)
					{
						ca = 0;// System.Windows.Forms.Control.MousePosition.X;
						cb = .8f;//System.Windows.Forms.Control.MousePosition.Y;
					}
					for (n = 0; n < iterations; n++)
					{
						var aa = a * a - b * b;
						var bb = 2 * a * b;

						a = aa + ca;
						b = bb + cb;

						if(Math.Abs(a + b) > 16)
						{
							break;
						}
					}

					var bright = Map(n, 0, iterations, 0, 1);
					bright = Math.Sqrt(bright);
					if (iterations == n)
						bright = 0;
					bitmap.SetPixel(x, y, new SKColor((byte)(255 * bright), 0, (byte)(100 * bright)));
				}
			};
		}

		double Map(double value, double min, double max, double a, double b) => Lerp(a, b, (value - min) / (max - min));

		double Lerp(double a, double b, double time) => a + (b - a) * time;

		void Branch(float length, SKPoint p1 = default, float angle = default)
		{
			paint.Color = new SKColor((byte)length, (byte)(255-length), (byte)angle);
			var p2 = new SKPoint(-length * (float)Math.Sin(angle), -length * (float)Math.Cos(angle)) + p1;
			//canvas.DrawLine(p1, p2, paint);
			if (length > 10)
			{
				Branch(length * branchDecay, p2, angle + branchAngle);
				Branch(length * branchDecay, p2, angle - branchAngle);
			}
		}

		void UpdateImage()
		{
			Canvas.Dispatcher.Invoke(() =>
			{
				var src = new BitmapImage();
				src.BeginInit();
				src.StreamSource = SKImage.FromBitmap(bitmap).Encode(SKEncodedImageFormat.Png, 100).AsStream();
				src.CacheOption = BitmapCacheOption.None;
				src.EndInit();
				Canvas.Source = src;
			});
		}
	}
}
