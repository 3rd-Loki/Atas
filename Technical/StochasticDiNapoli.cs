﻿namespace ATAS.Indicators.Technical
{
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;
	using System.Windows.Media;

	using ATAS.Indicators.Technical.Properties;

	[DisplayName("Preferred Stochastic - DiNapoli")]
	public class StochasticDiNapoli : Indicator
	{
		#region Fields

		private readonly EMA _ema = new EMA();

		private readonly ValueDataSeries _fastSeries = new ValueDataSeries(Resources.FastLine);
		private readonly KdFast _kdFast = new KdFast();
		private readonly KdSlow _kdSlow = new KdSlow();
		private readonly ValueDataSeries _slowSeries = new ValueDataSeries(Resources.SlowLine);

		#endregion

		#region Properties

		[Display(ResourceType = typeof(Resources), Name = "PeriodK", GroupName = "ShortPeriod", Order = 100)]
		public int PeriodK
		{
			get => _kdFast.PeriodK;
			set
			{
				if (value <= 0)
					return;

				_kdFast.PeriodK = _kdSlow.PeriodK = value;
				RecalculateValues();
			}
		}

		[Display(ResourceType = typeof(Resources), Name = "PeriodD", GroupName = "ShortPeriod", Order = 110)]
		public int PeriodD
		{
			get => _kdFast.PeriodD;
			set
			{
				if (value <= 0)
					return;

				_kdFast.PeriodD = _kdSlow.PeriodD = _ema.Period = value;
				RecalculateValues();
			}
		}

		[Display(ResourceType = typeof(Resources), Name = "PeriodD", GroupName = "LongPeriod", Order = 110)]
		public int SlowPeriodD
		{
			get => _kdSlow.SlowPeriodD;
			set
			{
				if (value <= 0)
					return;

				_kdSlow.SlowPeriodD = value;
				RecalculateValues();
			}
		}

		#endregion

		#region ctor

		public StochasticDiNapoli()
			: base(true)
		{
			Panel = IndicatorDataProvider.NewPanel;
			_ema.Period = _kdFast.PeriodD;
			Add(_kdFast);
			Add(_kdSlow);

			_fastSeries.Color = Colors.Blue;
			_slowSeries.Color = Colors.Red;

			DataSeries[0] = _fastSeries;
			DataSeries.Add(_slowSeries);
		}

		#endregion

		#region Protected methods

		protected override void OnCalculate(int bar, decimal value)
		{
			if (bar == 0)
				return;

			_fastSeries[bar] = _ema.Calculate(bar, ((ValueDataSeries)_kdFast.DataSeries[0])[bar]);
			var prevSlowD = ((ValueDataSeries)_kdSlow.DataSeries[1])[bar - 1];
			var fastD = ((ValueDataSeries)_kdFast.DataSeries[1])[bar];

			_slowSeries[bar] = prevSlowD + (fastD - prevSlowD) / SlowPeriodD;
		}

		#endregion
	}
}