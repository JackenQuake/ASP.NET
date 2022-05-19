using LiveCharts;
using LiveCharts.Wpf;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace MetricsManagerClient
{
	/// <summary>
	/// Логика взаимодействия для ChartDotNet.xaml
	/// </summary>
	public partial class ChartDotNet : UserControl, INotifyPropertyChanged, IDataView
	{
		public SeriesCollection ColumnSeriesValues { get; set; }
		public event PropertyChangedEventHandler PropertyChanged;

		public ChartDotNet()
		{
			InitializeComponent();
			ColumnSeriesValues = new SeriesCollection {
				new ColumnSeries { Values = new ChartValues<double> { 0 } }
			};
			DataContext = this;
		}

		public void UpdateData(List<double> values)
		{
			double x = 0;

			ColumnSeriesValues[0].Values.Clear();
			foreach (double v in values)
			{
				ColumnSeriesValues[0].Values.Add(v);
				x += v;
			}
			if (values.Count > 0) x /= values.Count;
			AverageTextBlock.Text = x.ToString();
		}

		public void SetCaption(string period)
		{
			AverageCaption.Text = $"За последние {period} среднее значение";
		}

		protected virtual void OnPropertyChanged(string propertyName = null)
		{
			var handler = PropertyChanged;

			if (handler != null) handler(this, new
			PropertyChangedEventArgs(propertyName));
		}

		private void UpdateOnСlick(object sender, RoutedEventArgs e)
		{
			TimePowerChart.Update(true);
		}
	}
}
