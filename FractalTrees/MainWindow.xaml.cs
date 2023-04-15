using System;
using System.Collections.Generic;
using System.ComponentModel;
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

using FractalTrees.Fractal;

using SharpGL;
using SkiaSharp;

namespace FractalTrees
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private SKBitmap bitmap;
		private Timer timer;
		private int width = 1920, height = 1080;
		private SKCanvas canvas;
		private Thread thread;
		private bool running = true;
		private Stopwatch time = new Stopwatch();
		private SKPaint paint = new SKPaint
		{
			Color = new SKColor(255, 0, 100),
			StrokeWidth = 4,
			IsAntialias = true
		};

		public MainWindow()
		{
			InitializeComponent();
			height = (int)Math.Floor(Height);
			width = (int)Math.Floor(Width);
			bitmap = new SKBitmap(width, height, SKImageInfo.PlatformColorType, SKAlphaType.Opaque);

			//timer = new Timer((_) => Render(), null, TimeSpan.Zero, TimeSpan.FromSeconds(1 / 10f));
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			running = false;
			thread?.Join();
			base.OnClosing(e);
		}

		public void Render()
		{
			time.Stop();
			var delta = time.Elapsed;
			time.Start();
			var mb = new Metaballs(bitmap, 10);
			Mandlebrot.Render(bitmap);
			//mb.Render(delta.TotalSeconds);
		}

		private void GLRender()
		{

			var gl = glControl.OpenGL;
			gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT);
			gl.LoadIdentity();
			//gl.Begin(OpenGL.GL_QUADS);
			////gl.Color((byte)255, (byte)255, (byte)255);
			////gl.Vertex(-0.5, -0.5);
			////gl.Vertex(0.5, -0.5);
			////gl.Vertex(0.5, 0.5);
			////gl.Vertex(-0.5, 0.5);
			//gl.End();
			Render();
			gl.DrawPixels(width, height, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, bitmap.GetPixels());

			gl.Flush();
		}

		private void glControl_OpenGLDraw(object sender, SharpGL.WPF.OpenGLRoutedEventArgs args)
		{
			GLRender();
		}

		private void glControl_Resized(object sender, SharpGL.WPF.OpenGLRoutedEventArgs args)
		{
			height = (int)Math.Floor(Height);
			width = (int)Math.Floor(Width);
			bitmap = new SKBitmap(width, height, SKImageInfo.PlatformColorType, SKAlphaType.Opaque);
		}

		private void glControl_OpenGLInitialized(object sender, SharpGL.WPF.OpenGLRoutedEventArgs args)
		{
			var gl = glControl.OpenGL;
			gl.ClearColor((255 / 10) / 255f, 0, (10 / 255f), 1f);
		}
	}
}
