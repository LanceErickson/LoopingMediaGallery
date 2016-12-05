using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopingMediaGallery.Interfaces
{
	public interface ISettingsProvider
	{
		ISet<string> ImageFormats { get; }
		ISet<string> VideoFormats {get; }
		int Duration { get; set; }
		int FileRefreshRate { get; set; }
		string FileFolderPath { get; set; }
	}
}
