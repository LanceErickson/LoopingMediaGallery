using LoopingMediaGallery.Interfaces;
using System.Collections.Generic;

namespace LoopingMediaGallery.Objects
{
	public class SettingsProvider : ISettingsProvider
	{
		public ISet<string> ImageFormats => new HashSet<string> { ".jpg", ".png", ".bmp", ".jpeg", ".tiff" };

		public ISet<string> VideoFormats => new HashSet<string> { ".mp4", ".wmv" };

		public int Duration
		{
			get
			{
				return (int?)Properties.Settings.Default["Duration"] ?? 10;
			}

			set
			{
				Properties.Settings.Default["Duration"] = value;
				Properties.Settings.Default.Save();
			}
		}

		public string FileFolderPath
		{
			get
			{
				return (string)Properties.Settings.Default["FolderPath"] ?? string.Empty;
			}

			set
			{
				Properties.Settings.Default["FolderPath"] = value;
				Properties.Settings.Default.Save();
			}
		}

		public int FileRefreshRate
		{
			get
			{
				return (int?)Properties.Settings.Default["RefreshRate"] ?? 1;
			}

			set
			{
				Properties.Settings.Default["RefreshRate"] = value;
				Properties.Settings.Default.Save();
			}
		}
	}
}
