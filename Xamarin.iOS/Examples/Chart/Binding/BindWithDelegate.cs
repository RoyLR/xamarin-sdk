﻿using System;
using System.Collections.Generic;

using Foundation;
using UIKit;

using TelerikUI;

namespace Examples
{
	public class BindingWithDelegate: ExampleViewController
	{
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			TKChart chart = new TKChart (this.ExampleBounds);
			chart.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			chart.DataSource = new ChartDataSource ();
			chart.Legend.Hidden = false;
			chart.Legend.Style.Position = TKChartLegendPosition.Top;
			chart.Legend.Container.Stack.Orientation = TKStackLayoutOrientation.Horizontal;
			this.View.AddSubview (chart);
		}

		class ChartDataSource: TKChartDataSource
		{
			Random r = new Random();

			public override nuint NumberOfSeries (TKChart chart)
			{
				return 3;
			}

			public override TKChartSeries GetSeries (TKChart chart, nuint index)
			{
				TKChartSeries series = null;

				if (index == 2) {
					series = new TKChartSplineSeries ();
				} else {
					series = new TKChartLineSeries ();
				}

				series.SelectionMode = TKChartSeriesSelectionMode.Series;
				series.Style.PointShape = new TKPredefinedShape (TKShapeType.Circle, new System.Drawing.SizeF (10, 10));
				series.Title = string.Format ("Series: {0}", index + 1);
				return series;
			}

			public override nuint PointsInSeries (TKChart chart, nuint seriesIndex)
			{
				return 10;
			}

			public override TKChartData GetPoint (TKChart chart, nuint dataIndex, nuint seriesIndex)
			{
				TKChartDataPoint point = new TKChartDataPoint ();
				point.DataXValue = new NSNumber (dataIndex);
				point.DataYValue = new NSNumber(r.Next (100));
				return point;
			}
		}
	}
}

