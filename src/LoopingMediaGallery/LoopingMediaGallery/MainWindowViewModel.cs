using LoopingMediaGallery.Controllers;
using LoopingMediaGallery.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace LoopingMediaGallery
{
	public class MainWindowViewModel : INotifyPropertyChanged
	{
		private readonly ISettingsProvider _settingsProvider;
		private readonly ISaveSettings _settingsSaver;
		private readonly IIntervalTimer _previewTimer;
		private readonly IGetViewPreview _viewPreviewProvider;
		private readonly IPresentOnSecondScreenHandler _presentViewHandler;

		private readonly MediaController _mediaController;
		private readonly TickerController _tickerController;

		public event PropertyChangedEventHandler PropertyChanged;
		public void SendPropertyChanged(string propertyName)
			=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		public IMediaObject CurrentMedia => _mediaController.CurrentMedia;

		public IEnumerable<IMediaObject> MediaCollection => _mediaController.MediaCollection;

		public IEnumerable<string> TextCollection => _tickerController.TextCollection;

		public bool Run
		{
			get { return _tickerController.Run; }
			set
			{
				_tickerController.Run = value;
				SendPropertyChanged(nameof(Run));
			}
		}

		public bool Play
		{
			get { return _mediaController.Play; }
			set
			{
				_mediaController.Play = value;
				Run = value;
				SendPropertyChanged(nameof(Play));
			}
		}

		private bool _mute = true;
		public bool Mute
		{
			get { return _mute; }
			set
			{
				if (_mute == value)
					return;
				_mute = value;
				SendPropertyChanged(nameof(Mute));
			}
		}

		private bool _blank = false;
		public bool Blank
		{
			get { return _blank; }
			set
			{
				if (_blank == value)
					return;
				_blank = value;
				SendPropertyChanged(nameof(Blank));
			}
		}

		public bool UseFade => _settingsProvider.UseFade;

		public bool ShowPreview => _settingsProvider.ShowPreview;

		private PresentationView _presentationView;

		private RenderTargetBitmap _previewImage;
		public RenderTargetBitmap PreviewImage
		{
			get { return _previewImage; }
			set
			{
				if (_previewImage == value)
					return;
				_previewImage = value;
				SendPropertyChanged(nameof(PreviewImage));
			}
		}

        public bool ShowTicker
        {
            get { return MediaPlayerRowSpan == 1; }
            set
            {
                MediaPlayerRowSpan = value ? 1 : 2;
                SendPropertyChanged(nameof(MediaPlayerRowSpan));
            }
        }

        public bool ShowSidebar
        {
            get { return MediaPlayerColumnSpan == 1; }
            set
            {
                MediaPlayerColumnSpan = value ? 1 : 2;
                SendPropertyChanged(nameof(MediaPlayerColumnSpan));
            }
        }

        public int MediaPlayerRowSpan { get; private set; }
        public int MediaPlayerColumnSpan { get; private set; }
        
        public SettingsWindowView SettingsView { get; }

		public MainWindowViewModel(
									ISettingsProvider settingsProvider, 
									ISaveSettings settingsSaver, 
									IIntervalTimer previewTimer, 
									IGetViewPreview viewPreviewProvider,
									IPresentOnSecondScreenHandler presentViewHandler,
									MediaController mediaController,
									TickerController tickerController)
		{
            _settingsProvider = settingsProvider ?? throw new ArgumentNullException(nameof(settingsProvider));
			_settingsSaver = settingsSaver ?? throw new ArgumentNullException(nameof(settingsSaver));
			_previewTimer = previewTimer ?? throw new ArgumentNullException(nameof(previewTimer));
			_viewPreviewProvider = viewPreviewProvider ?? throw new ArgumentNullException(nameof(viewPreviewProvider));
			_presentViewHandler = presentViewHandler ?? throw new ArgumentNullException(nameof(presentViewHandler));
			_mediaController = mediaController ?? throw new ArgumentNullException(nameof(mediaController));
			_tickerController = tickerController ?? throw new ArgumentNullException(nameof(tickerController));
			
			_settingsProvider.SettingsChanged += (s, o) =>
			{
				var settingName = (o as System.Configuration.SettingChangingEventArgs).SettingName;
				switch (settingName)
				{
					case nameof(_settingsProvider.UseFade):
						App.Current.Dispatcher.BeginInvoke(new Action(() => SendPropertyChanged(nameof(UseFade))));
						break;
				}
			};

			_mediaController.PropertyChanged += (s, o) =>
				App.Current.Dispatcher.BeginInvoke(new Action(() => SendPropertyChanged((o as PropertyChangedEventArgs).PropertyName)));

			_tickerController.PropertyChanged += (s, o) =>
				App.Current.Dispatcher.BeginInvoke(new Action(() => SendPropertyChanged((o as PropertyChangedEventArgs).PropertyName)));
		
			SendPropertyChanged(nameof(_settingsProvider.ShowPreview));

			_previewTimer.Initialize(TimeSpan.FromSeconds(1), () => UpdatePreview());
			_previewTimer.Start();

            ShowTicker = true;
            ShowSidebar = true; 
		}

		private void UpdatePreview()
		{
			if (_presentationView != null)
			{
				App.Current.Dispatcher.Invoke(new Action(() =>
				{
					PreviewImage = _viewPreviewProvider.RenderPreviewBitmap(_presentationView);
				}));
			}
		}

		public void MediaHasEnded() => _mediaController.NextHandler();
		public void ResetHandler() => _mediaController.ResetHandler();
		public void NextHandler() => _mediaController.NextHandler();
		public void PreviousHandler() => _mediaController.PreviousHandler();
		public void SelectMedia(IMediaObject media) => _mediaController.SelectMedia(media);

		public void ItemSelected(System.Windows.Controls.ListBox sender) => sender.ScrollIntoView(sender.SelectedItem);

		public void PresentHandler() => _presentViewHandler.PresentationView(_presentationView);

		internal void AddPresentationView(PresentationView presentationView) => _presentationView = presentationView;
		
		internal void ClosePresentationView()
		{
			if (_presentationView != null)
				_presentationView.Close();
		}

		public void SettingsHandler()
		{
			var settingsWindow = new SettingsWindowView(new SettingsWindowViewModel(_settingsProvider, _settingsSaver));
			settingsWindow.ShowDialog();
		}
	}
}
