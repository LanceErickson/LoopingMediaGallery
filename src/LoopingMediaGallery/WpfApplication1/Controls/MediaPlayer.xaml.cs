using LoopingMediaGallery.Interfaces;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
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

			MuteAudio();
		}

		private Timer _durationTimer;
		private UIElement _currentElement;
		private UIElement _queuedElement;
		public event EventHandler MediaEnded;
		private DoubleAnimation _fadeIn = new DoubleAnimation(0d, 1d, TimeSpan.FromSeconds(0.7d));
		private DoubleAnimation _fadeOut = new DoubleAnimation(1d, 0d, TimeSpan.FromSeconds(0.7d));
		private DoubleAnimation _cutIn = new DoubleAnimation(0d, 1d, TimeSpan.FromSeconds(0.0d));
		private DoubleAnimation _cutOut = new DoubleAnimation(1d, 0d, TimeSpan.FromSeconds(0.0d));

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
		
		public bool UseFade
		{
			get { return (bool)GetValue(UseFadeProperty); }
			set { SetValue(UseFadeProperty, value); }
		}
		public static DependencyProperty UseFadeProperty = DependencyProperty.Register("UseFade", typeof(bool), typeof(MediaPlayer), null);
		
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

		public bool Blank
		{
			get { return (bool)GetValue(BlankProperty); }
			set { SetValue(BlankProperty, value); }
		}
		public static DependencyProperty BlankProperty = DependencyProperty.Register("Blank", typeof(bool), typeof(MediaPlayer), new PropertyMetadata(false, (s, o) => BlankChanged(s, o)));

		private static void BlankChanged(DependencyObject s, DependencyPropertyChangedEventArgs o)
			=> (s as MediaPlayer)?.ToggleBlank();

		private void ToggleBlank()
		{
			if (Blank)
			{
				Play = false;
				ToggleElementVisibility(_currentElement, false);
			}
			else
			{
				ToggleElementVisibility(_currentElement, true);
			}
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
			
			SwitchElementsVisibility(_currentElement, _queuedElement);
			if (Play)
			{
				(_queuedElement as MediaElement)?.Play();
				InitializeTimer();
			}

			if (_currentElement != null)
			{
				if (_currentElement is Image)
					(_currentElement as Image).Source = null;
				else
					(_currentElement as MediaElement).Source = null;
			}

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

		private void SwitchElementsVisibility(UIElement currentElement, UIElement queuedElement)
		{
			currentElement?.BeginAnimation(UIElement.OpacityProperty, UseFade ? _fadeOut : _cutOut, HandoffBehavior.Compose);
			queuedElement?.BeginAnimation(UIElement.OpacityProperty, UseFade ? _fadeIn : _cutIn, HandoffBehavior.Compose);
		}

		private void ToggleElementVisibility(UIElement element, bool visible)
			=> element.BeginAnimation(UIElement.OpacityProperty, UseFade ? (visible ? _fadeIn : _fadeOut) : (visible ? _cutIn : _cutOut), HandoffBehavior.Compose);
	}
}
