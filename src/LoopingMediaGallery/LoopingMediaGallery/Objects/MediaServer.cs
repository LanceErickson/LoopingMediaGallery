using LoopingMediaGallery.Interfaces;
using System;
using System.Linq;

namespace LoopingMediaGallery.Objects
{
	public class MediaServer : IServeMedia
	{
		private IMediaProvider _mediaProvider;
		private uint _currentIndex = 0;

		public MediaServer(IMediaProvider mediaProvider)
		{
			if (mediaProvider == null) throw new ArgumentNullException(nameof(mediaProvider));

			_mediaProvider = mediaProvider;
		}

		public IMediaObject CurrentMedia
			=> _mediaProvider.MediaObjectCollection.Count() >= _currentIndex + 1
						? _mediaProvider.MediaObjectCollection[(int)_currentIndex]
						: null;

		public int MaxIndex => _mediaProvider.MediaObjectCollection?.Count() - 1 ?? 0;

		public void NextMedia()
		{
			if (_currentIndex + 1 > MaxIndex)
				_currentIndex = 0;
			else
				_currentIndex++;
		}

		public void PreviousMedia()
		{
			if (_currentIndex - 1 > MaxIndex || _currentIndex - 1 < 0)
				_currentIndex = 0;
			else
				_currentIndex--;
		}

		public void ServeSpecific(int index)
		{
			if (index > 0 && index <= MaxIndex)
				_currentIndex = (uint)index;
		}

		public void Reset()
		{
			_currentIndex = 0;
		}
	}
}
