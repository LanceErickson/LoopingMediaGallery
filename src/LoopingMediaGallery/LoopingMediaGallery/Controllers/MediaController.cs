using LoopingMediaGallery.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace LoopingMediaGallery.Controllers
{
	public class MediaController
	{
		private readonly IServeMedia _mediaServer;
		private readonly IMediaProvider _mediaProvider;
		private readonly IIntervalTimer _mediaTimer;

		public IMediaObject CurrentMedia => _mediaServer?.CurrentMedia;

		public IList<IMediaObject> MediaCollection => new List<IMediaObject>(_mediaProvider.MediaObjectCollection);

		private bool _play = false;
		public bool Play
		{
			get { return _play; }
			set
			{
				if (CurrentMedia == null)
					return;

				if (_play == value)
					return;

				if (value)
				{
					_mediaTimer.Initialize(CurrentMedia.Duration, () => MediaHasEnded());
					_mediaTimer.Start();
				}
				else
				{
					_mediaTimer.Stop();
				}

				_play = value;
			}
		}

		public event EventHandler PropertyChanged;

		public MediaController(
									IServeMedia mediaServer,
									IMediaProvider mediaProvider,
									IIntervalTimer mediaTimer)
		{
			if (mediaServer == null) throw new ArgumentNullException(nameof(mediaServer));
			if (mediaProvider == null) throw new ArgumentNullException(nameof(mediaProvider));
			if (mediaTimer == null) throw new ArgumentNullException(nameof(mediaTimer));
			
			_mediaServer = mediaServer;
			_mediaProvider = mediaProvider;
			_mediaTimer = mediaTimer;

			_mediaProvider.MediaCollectionPopulated += (s, o) =>
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MediaCollection)));

			_mediaProvider.MediaCollectionChanged += (s, o) =>
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MediaCollection)));

			_mediaServer.CurrentMediaUpdated += (s, o) =>
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentMedia)));

			_mediaProvider.ForceUpdate();
		}

		
		public void MediaHasEnded()
		{
			NextHandler();
		}

		public void ResetHandler()
		{
			_mediaServer.Reset();
			_mediaTimer.Initialize(CurrentMedia.Duration, () => MediaHasEnded());
			if (Play)
				_mediaTimer.Start();
		}

		public void NextHandler()
		{
			_mediaServer.NextMedia();
			_mediaTimer.Initialize(CurrentMedia.Duration, () => MediaHasEnded());
			if (Play)
				_mediaTimer.Start();
		}

		public void PreviousHandler()
		{
			_mediaServer.PreviousMedia();
			_mediaTimer.Initialize(CurrentMedia.Duration, () => MediaHasEnded());
			if (Play)
				_mediaTimer.Start();
		}

		public void SelectMedia(IMediaObject media)
		{
			int index = MediaCollection.IndexOf(media);
			_mediaServer.ServeSpecific(index);
			_mediaTimer.Initialize(CurrentMedia.Duration, () => MediaHasEnded());
			if (Play)
				_mediaTimer.Start();
		}

		public void ItemSelected(System.Windows.Controls.ListBox sender)
			=> sender.ScrollIntoView(sender.SelectedItem);
	}
}

