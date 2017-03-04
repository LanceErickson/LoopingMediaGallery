using LoopingMediaGallery.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopingMediaGallery.Objects
{
	public class TestTickerTextProvider : ITickerTextProvider
	{
		public IReadOnlyList<string> _TextCollection = new List<string>()
		{
			"TestString1",
			"TestStrinng2",
			"ReallyLongTestString3",
			"EvenLongerReallyLongTestString4"
		};

		IReadOnlyList<string> ITickerTextProvider.TextCollection => _TextCollection;

		public event EventHandler TextCollectionChanged;

		public void AddText(string value)
		{
			throw new NotImplementedException();
		}

		public void Remove(int index)
		{
			throw new NotImplementedException();
		}
	}
}
