using LoopingMediaGallery.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopingMediaGallery.Objects
{
	public class MediaServer : IServeMedia
	{

		public IMediaObject CurrentMedia
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public int MaxIndex
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public void ClearCurrent()
		{
			throw new NotImplementedException();
		}

		public void NextMedia()
		{
			throw new NotImplementedException();
		}

		public void PreviousMedia()
		{
			throw new NotImplementedException();
		}

		public void ServeSpecific(int index)
		{
			throw new NotImplementedException();
		}
	}
}
