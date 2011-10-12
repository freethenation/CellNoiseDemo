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
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Threading;

namespace CellNoiseDemo
{
	public class CellNoiseDemoDataObject : INotifyPropertyChanged
	{
		private FillBitmapBackgroundWorker _worker = new FillBitmapBackgroundWorker();
		public CellNoiseDemoDataObject()
		{
			_worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_worker_RunWorkerCompleted);
			PropertyChanged += new PropertyChangedEventHandler(CellNoiseDemoDataObject_PropertyChanged);
			//CellNoiseDemoDataObject_PropertyChanged(this, null);
		}

		void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (!e.Cancelled && e.Error == null)
			{
				Bitmap.Invalidate();
				PropertyChanged(this, new PropertyChangedEventArgs("Bitmap"));
			}
		}
				
		private Vector4 bitmapFillFunc(Vector4 pos)
		{
			PointColor pc = new PointColor(pos, new Vector4());
			float[] dists = new float[3];

			Func<Vector3, Vector3, float> distanceFunction = null;
			switch (DistanceMetric)
			{
				case DistanceMetrics.Euclidean:
					distanceFunction = WorleyNoise.EuclidianDistanceFunc;
					break;
				case DistanceMetrics.Manhattan:
					distanceFunction = WorleyNoise.ManhattanDistanceFunc;
					break;
				case DistanceMetrics.Chebyshev:
					distanceFunction = WorleyNoise.ChebyshevDistanceFunc;
					break;
			}

			Func<float[], float> combinerFunc = null;
			switch (CombinationFunction)
			{
				case CombinationFunctions.D1:
					combinerFunc = i => i[0];
					break;
				case CombinationFunctions.D2MinusD1:
					combinerFunc = i => i[1] - i[0];
					break;
				case CombinationFunctions.D3MinusD1:
					combinerFunc = i => i[2] - i[0];
					break;
			}

			pc = WorleyNoise.WorleyFunc(pc, Seed, distanceFunction, ref dists, combinerFunc);
			return pc.Color;
		}

		void CellNoiseDemoDataObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			FillBitmap(ref _bitmap, true);
		}

		public void FillBitmap(ref WriteableBitmap bitmap, bool doAsync)
		{
			_worker.FillBitmapAsync((float)Zoom, bitmapFillFunc, _bitmap);
			while (!doAsync && _worker.IsBusy)
			{
				System.Threading.Thread.Sleep(10);
			}
		}

		private WriteableBitmap _bitmap = new WriteableBitmap(200, 200);
		public WriteableBitmap Bitmap
		{
			get { return _bitmap; }
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private double _zoom = 5;
		public double Zoom
		{
			get { return _zoom; }
			set
			{
				if (_zoom != value)
				{
					_zoom = value;
					PropertyChanged(this, new PropertyChangedEventArgs("Zoom"));
				}
			}
		}

		private int _seed = (new System.Random()).Next(10000);
		public int Seed
		{
			get { return _seed; }
			set
			{
				if (_seed != value)
				{
					_seed = value;
					PropertyChanged(this, new PropertyChangedEventArgs("Seed"));
				}
			}
		}

		private DistanceMetrics _distanceMetric = DistanceMetrics.Euclidean;
		public string DistanceMetricString
		{
			get { return DistanceMetric.ToString(); }
			set
			{
				DistanceMetrics metricValue = (DistanceMetrics)Enum.Parse(typeof(DistanceMetrics), value, true);
				DistanceMetric = metricValue;
			}
		}
		public DistanceMetrics DistanceMetric
		{
			get { return _distanceMetric; }
			set
			{
				if (_distanceMetric != value)
				{
					_distanceMetric = value;
					PropertyChanged(this, new PropertyChangedEventArgs("DistanceMetricString"));
					PropertyChanged(this, new PropertyChangedEventArgs("DistanceMetric"));
				}
			}
		}

		private CombinationFunctions _combinationFunctions = CombinationFunctions.D1;
		public string CombinationFunctionString
		{
			get { return DistanceMetric.ToString(); }
			set
			{
				CombinationFunctions cfValue = (CombinationFunctions)Enum.Parse(typeof(CombinationFunctions), value, true);
				CombinationFunction = cfValue;
			}
		}
		public CombinationFunctions CombinationFunction
		{
			get { return _combinationFunctions; }
			set
			{
				if (_combinationFunctions != value)
				{
					_combinationFunctions = value;
					PropertyChanged(this, new PropertyChangedEventArgs("CombinationFunctionString"));
					PropertyChanged(this, new PropertyChangedEventArgs("CombinationFunction"));
				}
			}
		}
	}

	public enum CombinationFunctions
	{
		D1,
		D2MinusD1,
		D3MinusD1
	}

	public enum DistanceMetrics
	{
		Euclidean,
		Manhattan,
		Chebyshev
	}
}
