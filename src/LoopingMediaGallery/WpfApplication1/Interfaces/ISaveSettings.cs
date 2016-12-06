using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopingMediaGallery.Interfaces
{
	public interface ISaveSettings
	{
		int Duration { set; }
		int RefreshRate { set; }
		string FolderPath { set; }
		void Save();
	}
}
