﻿namespace ATAS.Indicators.Technical
{
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;

	using ATAS.Indicators.Technical.Properties;

	[DisplayName("Balance of Power")]
	public class BOP : Indicator
	{
		#region Fields

		private readonly ValueDataSeries _bop = new ValueDataSeries("BOP");
		private readonly ValueDataSeries _renderSeries = new ValueDataSeries(Resources.Visualization);
		private readonly SMA _sma = new SMA();

		#endregion

		#region Properties

		[Display(ResourceType = typeof(Resources), Name = "Period", GroupName = "Settings", Order = 100)]
		public int Period
		{
			get => _sma.Period;
			set
			{
				if (value <= 0)
					return;

				_sma.Period = value;
				RecalculateValues();
			}
		}

		#endregion

		#region ctor

		public BOP()
			: base(true)
		{
			Panel = IndicatorDataProvider.NewPanel;
			_sma.Period = 14;
			DataSeries[0] = _renderSeries;
		}

		#endregion

		#region Protected methods

		protected override void OnCalculate(int bar, decimal value)
		{
			var candle = GetCandle(bar);

			var highLow = candle.High - candle.Low;

			if (highLow == 0)
				_bop[bar] = 0;
			else
				_bop[bar] = (candle.Close - candle.Open) / highLow;

			_renderSeries[bar] = _sma.Calculate(bar, _bop[bar]);
		}

		#endregion
	}
}