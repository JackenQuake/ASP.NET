using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MetricsManagerClient
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly ManagerClient client;
		private readonly List<int> agents;
		public ObservableCollection<ComboBoxItem> cbItems { get; set; }

		public MainWindow()
		{
			InitializeComponent();
			DataContext = this;
			client = new ManagerClient("https://localhost:5000");
			agents = client.GetAgentList();
			cbItems = new ObservableCollection<ComboBoxItem>();
			foreach (int agent in agents) cbItems.Add(new ComboBoxItem { Content = $"Агент {agent}" });
			agentsList.SelectedIndex = 0;
		}

		private async void UpdateClick(object sender, RoutedEventArgs e)
		{
			string period = ((TextBlock)periodsList.SelectedItem).Text;
			CPUChart.SetCaption(period);
			RAMChart.SetCaption(period);
			HDDChart.SetCaption(period);
			NetworkChart.SetCaption(period);
			DotNetChart.SetCaption(period);
			long from, to;
			to = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
			switch (periodsList.SelectedIndex)
			{
				case 0: from = to   - 600; break;  // 10 min
				case 1: from = to  - 3600; break;  // 60 min
				case 2: from = to - 21600; break;  // 6 hours
				case 3: from = to - 86400; break;  // 24 hours
				default: from = to - 43200; break;
			}
			int agent = agents[agentsList.SelectedIndex];
			try
			{
				Task[] tasks = new Task[5];
				tasks[0] = client.UpdateDataView(CPUChart, agent, "cpu", from, to);
				tasks[1] = client.UpdateDataView(RAMChart, agent, "ram", from, to);
				tasks[2] = client.UpdateDataView(HDDChart, agent, "hdd", from, to);
				tasks[3] = client.UpdateDataView(NetworkChart, agent, "network", from, to);
				tasks[4] = client.UpdateDataView(DotNetChart, agent, "dotnet", from, to);
				await Task.WhenAll(tasks);
			} catch (Exception) { }
		}
	}
}
