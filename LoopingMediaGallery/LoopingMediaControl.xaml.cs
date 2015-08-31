using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace LoopingMediaGallery
{
	enum MediaType { Video, Image };

    /// <summary>
    /// Interaction logic for LoopingMediaControl.xaml
    /// </summary>
    public partial class LoopingMediaControl : UserControl, IDisposable
    {
        public delegate void VideoFinished(object sender, EventArgs e);
        public VideoFinished OnVideoFinished;

		private bool _isVideoPlaying = false;
		public bool IsVideoPlaying { get { return _isVideoPlaying; } }

		private UIElement _activeElement;
		private DoubleAnimation _fadeIn = new DoubleAnimation(0d, 1d, TimeSpan.FromSeconds(0.7d));
		private DoubleAnimation _fadeOut = new DoubleAnimation(1d, 0d, TimeSpan.FromSeconds(0.7d));


		public LoopingMediaControl()
        {
            InitializeComponent();

            mediaElement1.LoadedBehavior = MediaState.Manual;
            mediaElement1.UnloadedBehavior = MediaState.Pause;
            mediaElement1.MediaEnded += VideoHasFinished;

            mediaElement2.LoadedBehavior = MediaState.Manual;
            mediaElement2.UnloadedBehavior = MediaState.Pause;
            mediaElement2.MediaEnded += VideoHasFinished;
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
				VideoHasFinished(this, new RoutedEventArgs());
			}

			// New logic for displaying media
			if (type == MediaType.Image)
			{
				var oldElement = _activeElement;
				_activeElement = oldElement == image1 ? image2 : image1;

				((Image)_activeElement).Source = new BitmapImage(new Uri(source));

				_activeElement.BeginAnimation(UIElement.OpacityProperty, _fadeIn, HandoffBehavior.Compose);
				if (oldElement != null)
					oldElement.BeginAnimation(UIElement.OpacityProperty, _fadeOut, HandoffBehavior.Compose);
			}
			else
			{
				var oldElement = _activeElement;
				_activeElement = oldElement == mediaElement1 ? mediaElement2 : mediaElement1;

				((MediaElement)_activeElement).Source = new Uri(source);

				((MediaElement)_activeElement).Play();

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
                ((MediaElement)_activeElement).Stop();
        }
    }
}
