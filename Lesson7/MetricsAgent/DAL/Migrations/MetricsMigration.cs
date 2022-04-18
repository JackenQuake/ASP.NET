using FluentMigrator;

namespace MetricsAgent.DAL.Migrations
{
	/// <summary>
	/// Создание и удаление таблиц базы данных на основе FluentMigratior
	/// </summary>
	[Migration(1)]
	public class MetricsMigration : Migration
	{
		/// <summary>
		/// Создание таблиц базы данных
		/// </summary>
		public override void Up()
		{
			Create.Table("cpumetrics")
				.WithColumn("Id").AsInt64().PrimaryKey().Identity()
				.WithColumn("Value").AsInt32()
				.WithColumn("Time").AsInt64();
			Create.Table("dotnetmetrics")
				.WithColumn("Id").AsInt64().PrimaryKey().Identity()
				.WithColumn("Value").AsInt32()
				.WithColumn("Time").AsInt64();
			Create.Table("hddmetrics")
				.WithColumn("Id").AsInt64().PrimaryKey().Identity()
				.WithColumn("Value").AsInt32()
				.WithColumn("Time").AsInt64();
			Create.Table("networkmetrics")
				.WithColumn("Id").AsInt64().PrimaryKey().Identity()
				.WithColumn("Value").AsInt32()
				.WithColumn("Time").AsInt64();
			Create.Table("rammetrics")
				.WithColumn("Id").AsInt64().PrimaryKey().Identity()
				.WithColumn("Value").AsInt32()
				.WithColumn("Time").AsInt64();
		}

		/// <summary>
		/// Удаление таблиц базы данных
		/// </summary>
		public override void Down()
		{
			Delete.Table("cpumetrics");
			Delete.Table("dotnetmetrics");
			Delete.Table("hddmetrics");
			Delete.Table("networkmetrics");
			Delete.Table("rammetrics");
		}
	}
}
