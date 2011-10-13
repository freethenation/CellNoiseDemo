using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Threading;
using System.ComponentModel;

namespace CellNoiseDemo
{
	public class ImprovedBackgroundWorker : BackgroundWorker
	{
		public bool Cancel()
		{
			if (!this.IsBusy) return true;
			if (!this.WorkerSupportsCancellation) return false;
			this.CancelAsync();
			while (this.IsBusy)
			{
				System.Threading.Thread.Sleep(0);
			}
			return !this.IsBusy;
		}
	}

	public class FillBitmapBackgroundWorker : ImprovedBackgroundWorker
	{
		public FillBitmapBackgroundWorker()
		{
			this.WorkerReportsProgress = false;
			this.WorkerSupportsCancellation = true;
		}

		protected override void OnDoWork(DoWorkEventArgs e)
		{
			float width = _writeableBitmap.PixelWidth;
			float height = _writeableBitmap.PixelHeight;
			int index = 0;
			for (int y = 0; y < _writeableBitmap.PixelHeight; y++)
			{
				if (this.CancellationPending)
				{
					e.Cancel = true;
					return;
				}
				for (int x = 0; x < _writeableBitmap.PixelWidth; x++)
				{
					Vector4 color = _fillFunction(new Vector3((float)x / width, (float)y / height, 0f) * _zoom);
					_writeableBitmap.SetPixeli(index++, (byte)(color.W * 255), (byte)(color.X * 255), (byte)(color.Y * 255), (byte)(color.Z * 255));
				}
			}
		}

		public void FillBitmapAsync(float zoom, Func<Vector3, Vector4> fillFunction, WriteableBitmap writeableBitmap)
		{
			if (this.Cancel())
			{
				_zoom = zoom;
				_fillFunction = fillFunction;
				_writeableBitmap = writeableBitmap;
				this.RunWorkerAsync();
			}
		}

		//public void Cancel

		protected override void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e)
		{
			base.OnRunWorkerCompleted(e);
		}
		private float _zoom;
		private Func<Vector3, Vector4> _fillFunction;
		private WriteableBitmap _writeableBitmap;
	}
}
