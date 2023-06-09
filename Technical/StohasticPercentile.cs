﻿namespace ATAS.Indicators.Technical
{
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;
	using System.Linq;

	using ATAS.Indicators.Technical.Properties;

	using OFT.Attributes;

	[DisplayName("Stochastic - Percentile")]
	[HelpLink("https://support.atas.net/ru/knowledge-bases/2/articles/45493-stochastic-percentile")]
	public class StohasticPercentile : Indicator
	{
		#region Fields

		private readonly ValueDataSeries _renderSeries = new(Resources.Visualization);

		private readonly SMA _sma = new();
		private readonly List<decimal> _values = new();
		private int _lastBar;
		private int _period;

		#endregion

		#region Properties

		[Display(ResourceType = typeof(Resources), Name = "Period", GroupName = "Settings", Order = 100)]
		public int Period
		{
			get => _period;
			set
			{
				if (value <= 1)
					return;

				_period = value;
				RecalculateValues();
			}
		}

		[Display(ResourceType = typeof(Resources), Name = "SMA", GroupName = "Settings", Order = 110)]
		public int SmaPeriod
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

		public StohasticPercentile()
		{
			Panel = IndicatorDataProvider.NewPanel;
			_sma.Period = _period = 10;
			_lastBar = -1;
			DataSeries[0] = _renderSeries;
		}

		#endregion

		#region Protected methods

		protected override void OnCalculate(int bar, decimal value)
		{
			if (bar == 0)
			{
				_values.Clear();
				_renderSeries.Clear();
			}

			if (_values.Count > _period)
				_values.RemoveAt(0);

			if (bar == _lastBar)
				_values.RemoveAt(_values.Count - 1);

			_values.Add(value);

			var rankedValues = _values.OrderBy(x => x).ToList();

			var sp = 100m * rankedValues.IndexOf(value) / (_period - 1);

			_renderSeries[bar] = _sma.Calculate(bar, sp);

			_lastBar = bar;
		}

		#endregion
	}
}