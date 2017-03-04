using LoopingMediaGallery.Interfaces;
using System;
using System.Collections.Generic;

namespace LoopingMediaGallery.Objects
{
	public class TickerTextProvider : ITickerTextProvider
	{
		private IList<string> _textCollection = new List<string>();
		public IReadOnlyList<string> TextCollection => (IReadOnlyList<string>)_textCollection;

		public event EventHandler TextCollectionChanged;
		private void SendTextCollectionChanged()
			=>	TextCollectionChanged?.Invoke(this, new EventArgs());

		public void AddText(string value)
		{
			_textCollection.Add(value);
			SendTextCollectionChanged();
		}
		
		public void Remove(int index)
		{
			if (_textCollection.Count -1 >= index)
			{
				_textCollection.RemoveAt(index);
				SendTextCollectionChanged();
			}
			throw new ArgumentOutOfRangeException();
		}
	}
}
