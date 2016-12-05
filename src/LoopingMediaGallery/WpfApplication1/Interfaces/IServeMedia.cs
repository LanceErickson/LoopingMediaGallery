namespace LoopingMediaGallery.Interfaces
{
	public interface IServeMedia
	{
		IMediaObject CurrentMedia { get; }

		int MaxIndex { get; }

		void NextMedia();

		void PreviousMedia();

		void ServeSpecific(int index);

		void ClearCurrent();
	}
}
