using System;
using System.Windows;
using LoopingMediaGallery.Interfaces;
using System.Collections.Generic;

namespace LoopingMediaGallery.Objects
{
	public class PresentViewOnSecondScreenHandler : IPresentOnSecondScreenHandler
	{
		public void PresentationView(Window view)
		{
			if (view == null) throw new ArgumentNullException(nameof(view));

			var screens = new List<System.Windows.Forms.Screen>(System.Windows.Forms.Screen.AllScreens);

			System.Windows.Forms.Screen screen;
			if (screens.Count > 1)
				screen = screens[1];
			else
				screen = screens[0];

			System.Drawing.Rectangle r2 = screen.WorkingArea;
			view.Top = r2.Top;
			view.Left = r2.Left;

			view.WindowStyle = WindowStyle.None;
			view.WindowState = WindowState.Maximized;
		}
	}
}
