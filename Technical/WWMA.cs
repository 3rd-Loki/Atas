﻿namespace ATAS.Indicators.Technical
{
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;

	using ATAS.Indicators.Technical.Properties;

	using OFT.Attributes;

	[DisplayName("Welles Wilders Moving Average")]
	[FeatureId("NotReady")]
	[HelpLink("https://support.atas.net/ru/knowledge-bases/2/articles/45291-welles-wilders-moving-average")]
	public class WWMA : Indicator
	{
		#region Fields

		private readonly ValueDataSeries _renderSeries = new(Resources.Visualization);

		private readonly SZMA _szma = new();

		#endregion

		#region Properties

		[Display(ResourceType = typeof(Resources), Name = "Period", GroupName = "Settings", Order = 100)]
		public int Period
		{
			get => _szma.Period;
			set
			{
				if (value <= 0)
					return;

				_szma.Period = value;
				RecalculateValues();
			}
		}

		#endregion

		#region ctor

		public WWMA()
		{
			_szma.Period = 10;
			DataSeries[0] = _renderSeries;
		}

		#endregion

		#region Protected methods

		protected override void OnCalculate(int bar, decimal value)
		{
			_szma.Calculate(bar, value);

			if (bar == 0)
			{
				_renderSeries[bar] = value;
				return;
			}

			if (_renderSeries[bar - 1] == 0)
				_renderSeries[bar] = _szma[bar];
			else
				_renderSeries[bar] = _renderSeries[bar - 1] + (value - _renderSeries[bar - 1]) / Period;
		}

		#endregion
	}
}