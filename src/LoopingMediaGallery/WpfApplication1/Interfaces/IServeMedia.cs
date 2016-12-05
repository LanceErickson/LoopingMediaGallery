﻿namespace LoopingMediaGallery.Interfaces
{
	public interface IServeMedia
	{
		IMediaObject CurrentMedia { get; }

		int MaxIndex { get; }

		void NextMedia();

		void PreviousMedia();

		void Reset();

		void ServeSpecific(int index);
	}
}
