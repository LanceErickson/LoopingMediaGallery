using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LoopingMediaGallery
{
    /// <summary>
    /// Interaction logic for LoopingMediaControl.xaml
    /// </summary>
    public partial class LoopingMediaControl : UserControl, IDisposable
    {
        public List<Uri> Source { get; set; }
        public int ImageDuration { get; set; }
        public delegate void VideoFinished(object sender, EventArgs e);
        public VideoFinished OnVideoFinished;
        private bool _isVideoPlaying = false;
		public bool IsVideoPlaying { get { return _isVideoPlaying; } }
        private MediaElement _activeMediaElement;
        private Image _activeImage;

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
            _activeMediaElement.BeginAnimation(UIElement.OpacityProperty, new DoubleAnimation(1d, 0d, TimeSpan.FromSeconds(1d)), HandoffBehavior.SnapshotAndReplace);
			if (OnVideoFinished != null)
				OnVideoFinished.Invoke(this, new EventArgs());
        }

        public void ShowImage(string source)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (_activeMediaElement != null)
                    _activeMediaElement.Stop();

                Image _inactiveImage;
                if (_activeImage == null || _activeImage == image2)
                {
                    _activeImage = image1;
                    _inactiveImage = image2;
				}
                else
                {
                    _activeImage = image2;
                    _inactiveImage = image1;
				}
                _activeImage.Source = new BitmapImage(new Uri(source));

				_activeImage.BeginAnimation(UIElement.OpacityProperty, new DoubleAnimation(0d, 1d, TimeSpan.FromSeconds(1d)), HandoffBehavior.Compose);
				_inactiveImage.BeginAnimation(UIElement.OpacityProperty, new DoubleAnimation(1d, 0d, TimeSpan.FromSeconds(1d)), HandoffBehavior.Compose);
            }));
        }

        public void ShowVideo(string source, bool preview = false)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if(_activeMediaElement != null)
                    _activeMediaElement.Stop();

                if(_activeImage != null && _activeImage.Opacity > 0)
                    _activeImage.BeginAnimation(UIElement.OpacityProperty, new DoubleAnimation(1d, 0d, TimeSpan.FromSeconds(1d)));

                MediaElement _inactiveMediaElement;
                if (_activeMediaElement == null || _activeMediaElement == mediaElement2)
                {
                    _activeMediaElement = mediaElement1;
                    _inactiveMediaElement = mediaElement2;
                }
                else
                {
                    _activeMediaElement = mediaElement2;
                    _inactiveMediaElement = mediaElement1;
                }

                _activeMediaElement.Source = new Uri(source);

                _activeMediaElement.BeginAnimation(UIElement.OpacityProperty, new DoubleAnimation(0d, 1d, TimeSpan.FromSeconds(1d)), HandoffBehavior.SnapshotAndReplace);

				if (!preview)
					_activeMediaElement.Play();     

                _isVideoPlaying = true;
            }));
        }

        public void Dispose()
        {
            if (_isVideoPlaying && _activeMediaElement != null)
                _activeMediaElement.Stop();
        }
    }
}
