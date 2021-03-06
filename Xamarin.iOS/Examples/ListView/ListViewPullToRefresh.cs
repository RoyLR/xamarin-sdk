﻿using System;
using System.Collections;
using System.Collections.Generic;

using Foundation;
using UIKit;
using CoreFoundation;

using TelerikUI;

namespace Examples
{
	public class ListViewPullToRefresh: ExampleViewController
	{
		List<string> data = new List<string> ();
		TKDataSource dataSource = new TKDataSource();
		int newItemsCount = 0;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			dataSource.LoadDataFromJSONResource ("ListViewSampleData", "json", "teams");
			dataSource.GroupItemSourceKey = "items";
			dataSource.FilterWithQuery ("key like 'Marketing'");
			dataSource.GroupWithKey ("key");

			this.UpdateData (3);

			TKListView listView = new TKListView (this.View.Bounds);
			listView.RegisterClassForCell (new ObjCRuntime.Class (typeof(TKListViewCell)), "cell");
			listView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			listView.DataSource = new ListViewDataSource (this);
			listView.Delegate = new ListViewDelegate (this);
			listView.AllowsPullToRefresh = true;
			listView.PullToRefreshTreshold = 70;
			listView.PullToRefreshView.BackgroundColor = UIColor.Blue;
			listView.PullToRefreshView.ActivityIndicator.Color = UIColor.White;
			this.View.AddSubview (listView);
		}

		int UpdateData(int count)
		{
			TKDataSourceGroup group = (TKDataSourceGroup)this.dataSource.Items [0];
			long startIndex = this.data.Count;
			int i = 0;
			for (; i < count; i++) {
				if (i + startIndex >= group.Items.Length) {
					return i;
				}

				Random r = new Random ();
				int points = r.Next (0, 101);
				this.data.Insert (0, string.Format ("{0}: {1} points", group.Items [i + startIndex], points));
			}

			return i;
		}

		bool IsUpdated(NSIndexPath indexPath)
		{
			return indexPath.Row < newItemsCount;
		}

		public class ListViewDataSource: TKListViewDataSource
		{
			ListViewPullToRefresh owner;

			public ListViewDataSource(ListViewPullToRefresh owner)
			{
				this.owner = owner;
			}

			public override int NumberOfItemsInSection (TKListView listView, int section)
			{
				return this.owner.data.Count;
			}

			public override TKListViewCell CellForItem (TKListView listView, NSIndexPath indexPath)
			{
				TKListViewCell cell = listView.DequeueReusableCell ("cell", indexPath) as TKListViewCell;
				bool isUpdated = this.owner.IsUpdated (indexPath);
				cell.TextLabel.Text = this.owner.data [indexPath.Row];
				cell.BackgroundView.BackgroundColor = isUpdated ? new UIColor (1f, 1f, 0f, 0.4f) : UIColor.White;
				if (isUpdated) {
					UIView.Animate (0.5f, () => {
						cell.BackgroundView.Alpha = 1;
					});
				}

				return cell;
			}
		}

		class ListViewDelegate: TKListViewDelegate 
		{
			ListViewPullToRefresh owner;

			public ListViewDelegate(ListViewPullToRefresh owner) 
			{
				this.owner = owner;
			}

			public override void DidPull (TKListView listView, nfloat offset)
			{
				listView.PullToRefreshView.Alpha = (float)Math.Min (offset / listView.PullToRefreshTreshold, 1.0f);
			}

			public override bool ListViewShouldRefreshOnPull (TKListView listView)
			{
				DispatchQueue.DefaultGlobalQueue.DispatchAsync (() => {
					Random r = new Random();
					this.owner.newItemsCount = this.owner.UpdateData(1 + r.Next(0, 4));
					DispatchQueue.MainQueue.DispatchAfter(new DispatchTime(DispatchTime.Now, 2 * 500000000), () => {
						listView.DidRefreshOnPull();
						if (this.owner.newItemsCount < 1) {
							UIAlertView alert = new UIAlertView("Pull to refresh", "No more data available!",null,"Close",null);
							alert.Show();
						}
					});
				});

				return true;
			}
		}
	}
}
