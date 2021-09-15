﻿namespace ATAS.Indicators.Technical
{
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;
	using System.Windows.Media;

	using OFT.Attributes;
	using OFT.Localization;

	[DisplayName("Cumulative Adjusted Value")]
	[HelpLink("https://support.atas.net/ru/knowledge-bases/2/articles/45492-cumulative-adjusted-value")]
	public class CAV : Indicator
	{
		#region Fields

		private readonly EMA _ema = new();

		private readonly ValueDataSeries _renderSeries = new(Strings.Visualization);

		#endregion

		#region Properties

		[Display(ResourceType = typeof(Strings), Name = "Period", GroupName = "Settings", Order = 100)]
		public int Period
		{
			get => _ema.Period;
			set
			{
				if (value <= 0)
					return;

				_ema.Period = value;
				RecalculateValues();
			}
		}

		#endregion

		#region ctor

		public CAV()
		{
			Panel = IndicatorDataProvider.NewPanel;
			LineSeries.Add(new LineSeries(Strings.ZeroValue) { Color = Colors.Gray, Value = 0 });
			_ema.Period = 10;
			DataSeries[0] = _renderSeries;
		}

		#endregion

		#region Protected methods

		protected override void OnCalculate(int bar, decimal value)
		{
			var adjVal = value - _ema.Calculate(bar, value);

			if (bar == 0)
			{
				_renderSeries[bar] = adjVal;
				return;
			}

			_renderSeries[bar] = _renderSeries[bar - 1] + adjVal;
		}

		#endregion
	}
}