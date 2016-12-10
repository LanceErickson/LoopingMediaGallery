using System;
using LoopingMediaGallery.Interfaces;
using System.Timers;

namespace LoopingMediaGallery.Objects
{
	public class IntervalTimer : IIntervalTimer, IDisposable
	{
		private Timer _timer;

		public void Initialize(TimeSpan interval, Action callBack)
		{
			if (_timer != null)
				_timer.Dispose();

			_timer = new Timer(interval.TotalMilliseconds);
			_timer.Elapsed += (s, o) => callBack();
		}

		public void Start()
			=> _timer.Enabled = true;

		public void Stop()
			=> _timer.Enabled = false;

		public void Dispose()
			=> _timer?.Dispose();
	}
}
