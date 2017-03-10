using LoopingMediaGallery.Interfaces;
using System;
using System.Linq;

namespace LoopingMediaGallery.Objects
{
	public class MediaServer : IServeMedia
	{
		private IMediaProvider _mediaProvider;

		public event EventHandler CurrentMediaUpdated;

		public MediaServer(IMediaProvider mediaProvider)
		{
            _mediaProvider = mediaProvider ?? throw new ArgumentNullException(nameof(mediaProvider));

			if (_mediaProvider.MediaObjectCollection.Any())
			{
				CurrentMedia = _mediaProvider.MediaObjectCollection.FirstOrDefault();
				CurrentMediaUpdated?.Invoke(this, new EventArgs());
			}

			_mediaProvider.MediaCollectionPopulated += (s, o) =>
			{
				CurrentMedia = _mediaProvider.MediaObjectCollection.FirstOrDefault();
				CurrentMediaUpdated?.Invoke(this, new EventArgs());
			};

			_mediaProvider.MediaCollectionChanged += (s, o) =>
			{
				if (!_mediaProvider.MediaObjectCollection.Contains(CurrentMedia))
				{
					CurrentMedia = _mediaProvider.MediaObjectCollection.FirstOrDefault();
					CurrentMediaUpdated?.Invoke(this, new EventArgs());
				}
			};
		}

		private IMediaObject _currentMedia;
		public IMediaObject CurrentMedia
		{
			get
			{
				return _currentMedia;
			}
			private set
			{
				_currentMedia = value;
				CurrentMediaUpdated?.Invoke(this, new EventArgs());
			}
		}

		public void NextMedia()
		{
			if (CurrentMedia != null)
			{
				var mediaCollection = _mediaProvider.MediaObjectCollection.ToList();
				var currentIndex = mediaCollection.ToList().IndexOf(CurrentMedia);
				if (currentIndex + 1 < mediaCollection.Count)
					CurrentMedia = mediaCollection[currentIndex + 1];
				else
					CurrentMedia = mediaCollection.FirstOrDefault();
			}
			else
			{
				if (_mediaProvider.MediaObjectCollection.Any())
					CurrentMedia = _mediaProvider.MediaObjectCollection.FirstOrDefault();
			}
		}

		public void PreviousMedia()
		{
			if (CurrentMedia != null)
			{
				var mediaCollection = _mediaProvider.MediaObjectCollection.ToList();
				var currentIndex = mediaCollection.ToList().IndexOf(CurrentMedia);
				if (currentIndex - 1 >= 0)
					CurrentMedia = mediaCollection[currentIndex - 1];
				else
					CurrentMedia = mediaCollection.Last();
			}
			else
			{
				if (_mediaProvider.MediaObjectCollection.Any())
					CurrentMedia = _mediaProvider.MediaObjectCollection.FirstOrDefault();
			}
		}

		public void ServeSpecific(int index)
		{
			if (_mediaProvider.MediaObjectCollection.Count > index && index > -1)
				CurrentMedia = _mediaProvider.MediaObjectCollection.ToList()[index];
		}

		public void Reset()
		{
			if (!_mediaProvider.MediaObjectCollection.Any())	return;
			CurrentMedia = _mediaProvider.MediaObjectCollection.First();
		}
	}
}
