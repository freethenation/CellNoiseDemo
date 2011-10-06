using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CellNoiseDemo
{
	public partial class MainPage : UserControl
	{
		private CellNoiseDemoDataObject dataObject = new CellNoiseDemoDataObject();
		public MainPage()
		{
			InitializeComponent();
			this.DataContext = dataObject;
		}

		private void butRndSeed_Click(object sender, RoutedEventArgs e)
		{
			dataObject.Seed = (new Random()).Next(10000);
		}

		private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			dataObject.DistanceMetricString = ((sender as ComboBox).SelectedItem as ComboBoxItem).Tag as string;
		}

		private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
		{
			dataObject.CombinationFunctionString = ((sender as ComboBox).SelectedItem as ComboBoxItem).Tag as string;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}
