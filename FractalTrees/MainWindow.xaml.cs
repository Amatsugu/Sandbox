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
		private int width = 1920, height = 1080;
		private SKCanvas canvas;
		private Thread thread;
		private bool running = true;
		private SKPaint paint = new SKPaint
		{
			Color = new SKColor(255, 0, 100),
			StrokeWidth = 4,
			IsAntialias = true
		};

		public MainWindow()
		{
			InitializeComponent();
			var multi = 1f;
			height = (int)Math.Floor(multi * height);
			width = (int)Math.Floor(multi * width);
			bitmap = new SKBitmap(width, height, SKImageInfo.PlatformColorType, SKAlphaType.Opaque);
			canvas = new SKCanvas(bitmap);
			canvas.Clear(new SKColor(255 / 10, 0, 10));
			canvas.Flush();
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			running = false;
			thread.Join();
			base.OnClosing(e);
		}

		public void Start()
		{
			var mb = new Metaballs(bitmap, 1);
			while (running)
			{
				mb.Render();
			}
			//var f = new FileStream("day of black sun.png", FileMode.Create);
			//SKImage.FromBitmap(bitmap).Encode(SKEncodedImageFormat.Png, 100).SaveTo(f);
			//f.Flush();
		}

		private void GlInit(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
		{
			var gl = glControl.OpenGL;
			gl.ClearColor((255/10)/255f, 0, (10/255f), 1f);
		}

		private void GlResize(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
		{

		}

		private void GlDraw(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
		{
			var gl = glControl.OpenGL;
			gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT);
			gl.LoadIdentity();

			gl.Begin(OpenGL.GL_QUADS);
			gl.Color((byte)255, (byte)255, (byte)255);
			gl.Vertex(-0.5, -0.5);
			gl.Vertex( 0.5, -0.5);
			gl.Vertex( 0.5,  0.5);
			gl.Vertex(-0.5,  0.5);
			gl.End();

			gl.Flush();
		}
	}
}
