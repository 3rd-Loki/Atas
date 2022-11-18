﻿namespace ATAS.Indicators.Technical;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Media;

using ATAS.Indicators.Technical.Properties;

using OFT.Attributes;

[DisplayName("Color Bar HH/LL")]
[Category("Technical")]
[FeatureId("NotApproved")]
public class ColorBarHighLow : Indicator
{
	#region Fields

	private Color _averageColor = Colors.Orange;
	private Color _highColor = Colors.Aqua;
	private Color _lowColor = Colors.DarkMagenta;

	private PaintbarsDataSeries _renderSeries = new("PaintBars")
	{
		IsHidden = true
	};

	#endregion

	#region Properties

	[Display(ResourceType = typeof(Resources), Name = "Average", GroupName = "Color", Order = 100)]
	public Color AverageColor
	{
		get => _averageColor;
		set
		{
			_averageColor = value;
			RecalculateValues();
		}
	}

	[Display(ResourceType = typeof(Resources), Name = "Highest", GroupName = "Color", Order = 100)]
	public Color HighColor
	{
		get => _highColor;
		set
		{
			_highColor = value;
			RecalculateValues();
		}
	}

	[Display(ResourceType = typeof(Resources), Name = "Lowest", GroupName = "Color", Order = 100)]
	public Color LowColor
	{
		get => _lowColor;
		set
		{
			_lowColor = value;
			RecalculateValues();
		}
	}

	#endregion

	#region ctor

	public ColorBarHighLow()
		: base(true)
	{
		DenyToChangePanel = true;
        DataSeries[0] = _renderSeries;
	}

	#endregion

	#region Protected methods

	protected override void OnRecalculate()
	{
		Clear();
	}

	protected override void OnCalculate(int bar, decimal value)
	{
		if (bar == 0)
			return;

		var candle = GetCandle(bar);
		var prevCandle = GetCandle(bar - 1);

		if (candle.High == prevCandle.High && candle.Low == prevCandle.Low)
			return;

		if (candle.High > prevCandle.High && candle.Low < prevCandle.Low)
		{
			_renderSeries[bar] = AverageColor;
			return;
		}

		if (candle.High > prevCandle.High && candle.Low >= prevCandle.Low)
		{
			_renderSeries[bar] = HighColor;
			return;
		}

		if (candle.High <= prevCandle.High && candle.Low < prevCandle.Low)
			_renderSeries[bar] = LowColor;
	}

	#endregion
}