﻿namespace ATAS.Indicators.Technical
{
	using System;
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;
	using System.Windows.Media;

	using OFT.Attributes;
	using OFT.Localization;

	[DisplayName("Study Angle")]
	[FeatureId("NotReady")]
	[HelpLink("https://support.atas.net/ru/knowledge-bases/2/articles/45344-study-angle")]
	public class Angle : Indicator
	{
		#region Fields

		private readonly ValueDataSeries _renderSeries = new(Strings.Visualization);
		private int _period;

		#endregion

		#region Properties

		[Display(ResourceType = typeof(Strings), Name = "Period", GroupName = "Settings", Order = 100)]
		public int Period
		{
			get => _period;
			set
			{
				if (value <= 0)
					return;

				_period = value;
				RecalculateValues();
			}
		}

		#endregion

		#region ctor

		public Angle()
		{
			Panel = IndicatorDataProvider.NewPanel;
			_period = 10;
			LineSeries.Add(new LineSeries(Strings.ZeroValue) { Color = Colors.Gray, Value = 0, Width = 2 });
			DataSeries[0] = _renderSeries;
		}

		#endregion

		#region Protected methods

		protected override void OnCalculate(int bar, decimal value)
		{
			if (bar == 0)
				_renderSeries.Clear();

			if (bar < _period)
				return;

			_renderSeries[bar] = (decimal)(Math.Atan((double)
				((value - (decimal)SourceDataSeries[bar - _period]) / _period)) * 180 / Math.PI);
		}

		#endregion
	}
}