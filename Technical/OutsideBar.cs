﻿namespace ATAS.Indicators.Technical
{
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;
	using System.Windows.Media;

	using ATAS.Indicators.Technical.Properties;

	using OFT.Attributes;

	[DisplayName("Outside Bar")]
	[HelpLink("https://support.atas.net/ru/knowledge-bases/2/articles/45345-outside-bar")]
	public class OutsideBar : Indicator
	{
		#region Fields

		private readonly ValueDataSeries _renderSeries = new(Resources.Visualization);
		private bool _includeEqual;

		#endregion

		#region Properties

		[Display(ResourceType = typeof(Resources), Name = "IncludeEqualHighLow", GroupName = "Settings", Order = 100)]
		public bool IncludeEqual
		{
			get => _includeEqual;
			set
			{
				_includeEqual = value;
				RecalculateValues();
			}
		}

		#endregion

		#region ctor

		public OutsideBar()
			: base(true)
		{
			_renderSeries.VisualType = VisualMode.Dots;
			_renderSeries.Width = 3;
			_renderSeries.Color = Colors.Blue;

			DataSeries[0] = _renderSeries;
		}

		#endregion

		#region Protected methods

		protected override void OnCalculate(int bar, decimal value)
		{
			if (bar == 0)
			{
				_renderSeries.Clear();
				return;
			}

			var candle = GetCandle(bar);
			var prevCandle = GetCandle(bar - 1);

			switch (_includeEqual)
			{
				case false when candle.High > prevCandle.High && candle.Low < prevCandle.Low:
					_renderSeries[bar] = candle.High;
					return;
				case true when candle.High >= prevCandle.High && candle.Low <= prevCandle.Low:
					_renderSeries[bar] = candle.High;
					break;
			}
		}

		#endregion
	}
}