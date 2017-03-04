using LoopingMediaGallery.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace LoopingMediaGallery.Controllers
{
	public class TickerController
	{
		private readonly ITickerTextProvider _tickerTextProvider;

		public IEnumerable<string> TextCollection => _tickerTextProvider.TextCollection;

		public event EventHandler PropertyChanged;
		void SendPropertyChange(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		private bool _run;
		public bool Run
		{
			get { return _run; }
			set
			{
				_run = value;
				SendPropertyChange(nameof(Run));
			}
		}

		public TickerController(ITickerTextProvider tickerTextProvider)
		{
			if (tickerTextProvider == null) throw new ArgumentNullException(nameof(tickerTextProvider));

			_tickerTextProvider = tickerTextProvider;

			_tickerTextProvider.TextCollectionChanged 
				+= (s, o) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TextCollection)));
		}
	}
}
