using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LoopingMediaGallery.Controls
{
	//http://jobijoy.blogspot.ca/2008/08/wpf-custom-controls-marquee-control.html
	//http://stackoverflow.com/questions/15323163/wpf-marquee-text-animation
	public class TickerTextControl : ContentControl
	{
		public IEnumerable<string> ItemsSource
		{
			get { return (IEnumerable<string>)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}
		public static readonly DependencyProperty ItemsSourceProperty =
			DependencyProperty.Register("ItemsSource", typeof(IEnumerable<string>), typeof(TickerTextControl), new PropertyMetadata(Enumerable.Empty<string>()));

        public Visibility TextVisibility
        {
            get { return (Visibility)GetValue(TextVisibilityProperty); }
            set { SetValue(TextVisibilityProperty, value); }
        }
        public static readonly DependencyProperty TextVisibilityProperty =
            DependencyProperty.Register("TextVisibility", typeof(Visibility), typeof(TickerTextControl), new PropertyMetadata(Visibility.Hidden));

        public bool Run
		{
			get { return (bool)GetValue(RunProperty); }
			set { SetValue(RunProperty, value); }
		}
		public static readonly DependencyProperty RunProperty =
			DependencyProperty.Register("Run", typeof(bool), typeof(TickerTextControl), new PropertyMetadata(false, (s,o) => (s as TickerTextControl)?.RunChanged()));

        private void RunChanged()
            => TextVisibility = Run ? Visibility.Visible : Visibility.Collapsed;

		

		//private void Animate()
		//{
		//	if (IsLoaded)
		//	{
		//		_doubleAnimation.From = this.ActualWidth;
		//		_doubleAnimation.To = -_contentPresenter.ActualWidth;

		//		_doubleAnimation.RepeatBehavior = RepeatBehavior.Forever;
		//		_doubleAnimation.Duration = Duration;
		//		Storyboard.SetTargetProperty(_doubleAnimation, new PropertyPath("(Canvas.Left)"));
		//		_storyBoard.Children.Add(_doubleAnimation);

		//		_storyBoard.Begin(_contentPresenter, true);
		//	}
		//}

	}
}
