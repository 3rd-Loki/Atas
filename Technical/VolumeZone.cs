﻿namespace ATAS.Indicators.Technical
{
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;
	using System.Windows.Media;

	using ATAS.Indicators.Technical.Properties;

	using OFT.Attributes;

	[DisplayName("Volume Zone Oscillator")]
	[FeatureId("NotReady")]
	[HelpLink("https://support.atas.net/ru/knowledge-bases/2/articles/45332-volume-zone-oscillator")]
	public class VolumeZone : Indicator
	{
		#region Fields

		private readonly EMA _emaTv = new();
		private readonly EMA _emaVp = new();
		private readonly LineSeries _overboughtLine1 = new(Resources.Overbought1);
		private readonly LineSeries _overboughtLine2 = new(Resources.Overbought2);
		private readonly LineSeries _overboughtLine3 = new(Resources.Overbought3);
		private readonly LineSeries _oversoldLine1 = new(Resources.Oversold1);
		private readonly LineSeries _oversoldLine2 = new(Resources.Oversold2);
		private readonly LineSeries _oversoldLine3 = new(Resources.Oversold3);

		private readonly ValueDataSeries _renderSeries = new(Resources.Visualization);

		#endregion

		#region Properties

		[Display(ResourceType = typeof(Resources), Name = "Period", GroupName = "Settings", Order = 100)]
		public int Period
		{
			get => _emaVp.Period;
			set
			{
				if (value <= 0)
					return;

				_emaVp.Period = _emaTv.Period = value;
				RecalculateValues();
			}
		}

		#endregion

		#region ctor

		public VolumeZone()
			: base(true)
		{
			Panel = IndicatorDataProvider.NewPanel;

			_overboughtLine1.Value = 50;
			_overboughtLine1.Color = Colors.LawnGreen;
			_overboughtLine2.Value = 75;
			_overboughtLine2.Color = Colors.LimeGreen;
			_overboughtLine3.Value = 90;
			_overboughtLine3.Color = Colors.DarkGreen;

			_oversoldLine1.Value = -50;
			_oversoldLine1.Color = Colors.IndianRed;
			_oversoldLine2.Value = -75;
			_oversoldLine2.Color = Colors.Red;
			_oversoldLine3.Value = -90;
			_oversoldLine3.Color = Colors.DarkRed;

			LineSeries.Add(_overboughtLine1);
			LineSeries.Add(_overboughtLine2);
			LineSeries.Add(_overboughtLine3);
			LineSeries.Add(_oversoldLine1);
			LineSeries.Add(_oversoldLine2);
			LineSeries.Add(_oversoldLine3);
			DataSeries[0] = _renderSeries;
		}

		#endregion

		#region Protected methods

		protected override void OnCalculate(int bar, decimal value)
		{
			var candle = GetCandle(bar);

			if (bar == 0)
			{
				_emaVp.Calculate(bar, candle.Volume);
				_emaTv.Calculate(bar, candle.Volume);
				return;
			}

			var prevCandle = GetCandle(bar - 1);

			var rVolume = candle.Close > prevCandle.Close ? candle.Volume : -candle.Volume;

			_emaVp.Calculate(bar, rVolume);
			_emaTv.Calculate(bar, candle.Volume);

			if (_emaTv[bar] != 0)
				_renderSeries[bar] = _emaVp[bar] / _emaTv[bar] * 100;
			else
				_renderSeries[bar] = _renderSeries[bar - 1];
		}

		#endregion
	}
}