using System;

namespace LoopingMediaGallery.Interfaces
{
	public interface IIntervalTimer
	{
		void Initialize(TimeSpan interval, Action callBack);
		void Start();
		void Stop();
	}
}
