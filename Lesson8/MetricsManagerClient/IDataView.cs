using System.Collections.Generic;

namespace MetricsManagerClient
{
	/// <summary>
	/// Интерфейс, через который приложение передает данные диаграммам
	/// </summary>
	public interface IDataView
	{
		void UpdateData(List<double> values);
		void SetCaption(string period);
	}
}
