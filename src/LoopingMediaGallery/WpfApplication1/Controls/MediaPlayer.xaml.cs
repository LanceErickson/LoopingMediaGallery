using LoopingMediaGallery.Interfaces;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace LoopingMediaGallery.Controls
{
	/// <summary>
	/// Interaction logic for MediaPlayer.xaml
	/// </summary>
	public partial class MediaPlayer : UserControl
	{
		public MediaPlayer()
		{
			InitializeComponent();
		}

		private Timer _durationTimer;
		private UIElement _currentElement;
		private UIElement _queuedElement;
		public event EventHandler MediaEnded;


		public bool Play
		{
			get { return (bool)GetValue(PlayProperty); }
			set { SetValue(PlayProperty, value); }
		}
		public static DependencyProperty PlayProperty = DependencyProperty.Register("Play", typeof(bool), typeof(MediaPlayer), new PropertyMetadata(false, (s,o) => PlayChanged(s, o)));

		private static void PlayChanged(DependencyObject s, DependencyPropertyChangedEventArgs o)
			=> (s as MediaPlayer)?.TogglePlay();

		private void TogglePlay()
		{
			if (Play)
			{
				(_currentElement as MediaElement)?.Play();
				InitializeTimer();
			}
			else
			{
				(_currentElement as MediaElement)?.Stop();
				DisposeTimer();
			}
		}

		public bool Mute
		{
			get { return (bool)GetValue(MuteProperty); }
			set { SetValue(MuteProperty, value); }
		}
		public static DependencyProperty MuteProperty =
			DependencyProperty.Register("Mute", typeof(bool), typeof(MediaPlayer), new PropertyMetadata(true, (s, o) => MuteChanged(s, o)));

		private static void MuteChanged(DependencyObject s, DependencyPropertyChangedEventArgs o)
			=> (s as MediaPlayer)?.MuteAudio();

		private void MuteAudio()
		{
			videoOne.IsMuted = Mute;
			videoTwo.IsMuted = Mute;
		}

		public IMediaObject Source
		{
			get { return (IMediaObject)GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}
		public static DependencyProperty SourceProperty = 
			DependencyProperty.Register("Source", typeof(IMediaObject), typeof(MediaPlayer), new PropertyMetadata(null, (s, o) => SourceChanged(s, o)));

		private static void SourceChanged(DependencyObject s, DependencyPropertyChangedEventArgs o)
			=>	(s as MediaPlayer)?.Update();
		
		private void Update()
		{
			if (Source == null) return;

			if (Source.Type == MediaType.Image)
				SetupImage(Source);
			else
				SetupVideo(Source);

			StartPlaying();
		}

		private void StartPlaying()
		{
			DisposeTimer();

			(_currentElement as MediaElement)?.Stop();

			_queuedElement.Visibility = Visibility.Visible;
			if (Play)
			{
				(_queuedElement as MediaElement)?.Play();
				InitializeTimer();
			}

			if (_currentElement != null)
				_currentElement.Visibility = Visibility.Collapsed;

			_currentElement = _queuedElement;
			_queuedElement = null;
		}

		private void DisposeTimer()
		{
			if (_durationTimer != null)
			{
				_durationTimer.Dispose();
				_durationTimer = null;
			}
		}

		private void InitializeTimer()
		{
			_durationTimer = new Timer((s) => Dispatcher.BeginInvoke(new Action(() => MediaEnded?.Invoke(this, new EventArgs()))), new AutoResetEvent(false), (int)Source.Duration.TotalMilliseconds, (int)Source.Duration.TotalMilliseconds);
		}

		private void SetupVideo(IMediaObject media)
		{
			if (_currentElement == videoOne)
				_queuedElement = videoTwo;
			else
				_queuedElement = videoOne;

			(_queuedElement as MediaElement).Source = Source.Source;
		}
			
		private void SetupImage(IMediaObject media)
		{
			if (_currentElement == imageOne)
				_queuedElement = imageTwo;
			else
				_queuedElement = imageOne;

			var newSource = new BitmapImage();
			newSource.BeginInit();
			newSource.UriSource = media.Source;
			newSource.CacheOption = BitmapCacheOption.OnLoad;
			newSource.EndInit();
			newSource.Freeze();
			(_queuedElement as Image).Source = newSource;
		}
	}
}
