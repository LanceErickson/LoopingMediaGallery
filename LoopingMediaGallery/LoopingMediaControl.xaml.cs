using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Windows.Media;

namespace LoopingMediaGallery
{
	enum MediaType { Video, Image };

    /// <summary>
    /// Interaction logic for LoopingMediaControl.xaml
    /// </summary>
    public partial class LoopingMediaControl : UserControl, IDisposable, INotifyPropertyChanged
    {
        public delegate void VideoFinished(object sender, EventArgs e);
        public VideoFinished OnVideoFinished;

		public delegate void MediaFailure(object sender, EventArgs e);
		public MediaFailure OnMediaFailure;

		private bool _isVideoPlaying = false;
		public bool IsVideoPlaying { get { return _isVideoPlaying; } }

		private UIElement _activeElement;
		private DoubleAnimation _fadeIn = new DoubleAnimation(0d, 1d, TimeSpan.FromSeconds(0.7d));
		private DoubleAnimation _fadeOut = new DoubleAnimation(1d, 0d, TimeSpan.FromSeconds(0.7d));

		public event PropertyChangedEventHandler PropertyChanged;
		public void SendPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		private bool _isMuted = false;
		public bool IsMuted
		{
			get
			{
				return _isMuted;
			}
			set
			{
				if (_isMuted == value)
					return;
				_isMuted = value;
				SendPropertyChanged("IsMuted");
			}
		}

		public LoopingMediaControl()
        {
            InitializeComponent();
			this.DataContext = this;

            mediaElement1.LoadedBehavior = MediaState.Manual;
            mediaElement1.UnloadedBehavior = MediaState.Pause;
            mediaElement1.MediaEnded += VideoHasFinished;
			
			mediaElement2.LoadedBehavior = MediaState.Manual;
            mediaElement2.UnloadedBehavior = MediaState.Pause;
			mediaElement2.MediaEnded += VideoHasFinished;
		}

		// https://blogs.msdn.microsoft.com/jaimer/2009/07/03/rendertargetbitmap-tips/
		public RenderTargetBitmap RenderPreviewBitmap()
		{
			if (!(_activeElement != null && (_activeElement as FrameworkElement).ActualHeight > 0 && (_activeElement as FrameworkElement).ActualWidth > 0))
				return null;
			
			Rect bounds = VisualTreeHelper.GetDescendantBounds((_activeElement as FrameworkElement));
			RenderTargetBitmap rtb = new RenderTargetBitmap((int)bounds.Width,
															(int)bounds.Height,
															96,
															96,
															PixelFormats.Pbgra32);
			DrawingVisual dv = new DrawingVisual();
			using (DrawingContext ctx = dv.RenderOpen())
			{
				VisualBrush vb = new VisualBrush((_activeElement as FrameworkElement));
				ctx.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
			}
			rtb.Render(dv);
			return rtb;
		}

		private void VideoHasFinished(object sender, RoutedEventArgs e)
        {
            _isVideoPlaying = false;
            _activeElement.BeginAnimation(UIElement.OpacityProperty, _fadeOut, HandoffBehavior.Compose);
			if (OnVideoFinished != null)
				OnVideoFinished.Invoke(this, new EventArgs());
        }

        public void ShowImage(string source)
		{ 
            Dispatcher.BeginInvoke(new Action(() => ShowMedia(source, MediaType.Image)));
        }

        public void ShowVideo(string source, bool preview = false)
		{ 
			Dispatcher.BeginInvoke(new Action(() => ShowMedia(source, MediaType.Video)));
        }

		private void ShowMedia(string source, MediaType type)
		{
			if (_activeElement is MediaElement && _isVideoPlaying)
			{
				OnVideoFinished = null;
				((MediaElement)_activeElement).Stop();
				((MediaElement)_activeElement).Source = null;
                VideoHasFinished(this, new RoutedEventArgs());
			}

			// New logic for displaying media
			if (type == MediaType.Image)
			{
				var oldElement = _activeElement;
				_activeElement = oldElement == image1 ? image2 : image1;

				try
				{
					((Image)_activeElement).Source = new BitmapImage(new Uri(source));
				}
				catch
				{
					if (OnMediaFailure != null)
						OnMediaFailure.Invoke(this, new EventArgs());
				}

				_activeElement.BeginAnimation(UIElement.OpacityProperty, _fadeIn, HandoffBehavior.Compose);
				if (oldElement != null)
					oldElement.BeginAnimation(UIElement.OpacityProperty, _fadeOut, HandoffBehavior.Compose);
			}
			else
			{
				var oldElement = _activeElement;
				_activeElement = oldElement == mediaElement1 ? mediaElement2 : mediaElement1;

				try
				{
					((MediaElement)_activeElement).Source = new Uri(source);
					((MediaElement)_activeElement).Play();
				}
				catch
				{
					if (OnMediaFailure != null)
						OnMediaFailure.Invoke(this, new EventArgs());
				}

				if (oldElement is Image)
					oldElement.BeginAnimation(OpacityProperty, _fadeOut, HandoffBehavior.Compose);
				_activeElement.BeginAnimation(OpacityProperty, _fadeIn, HandoffBehavior.Compose);

				_isVideoPlaying = true;
			}
		}

		public void Clear()
		{
			if(_activeElement != null)
				_activeElement.BeginAnimation(OpacityProperty, _fadeOut, HandoffBehavior.Compose);
		}

        public void Dispose()
        {
			if (_isVideoPlaying && _activeElement != null && _activeElement is MediaElement)
			{
				((MediaElement)_activeElement).Stop();
				((MediaElement)_activeElement).Source = null;
            }
        }
    }
}
