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

namespace CellNoiseDemo
{
	public class CellNoiseDemoDataObject : INotifyPropertyChanged
	{
		public CellNoiseDemoDataObject()
		{
			PropertyChanged += new PropertyChangedEventHandler(CellNoiseDemoDataObject_PropertyChanged);
		}

		void CellNoiseDemoDataObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private double _zoom = 0;
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
