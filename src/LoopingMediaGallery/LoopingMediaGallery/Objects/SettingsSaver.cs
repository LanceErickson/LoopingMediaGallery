using LoopingMediaGallery.Interfaces;

namespace LoopingMediaGallery.Objects
{
	public class SettingsSaver : ISaveSettings
	{
		public int Duration { set {	Properties.Settings.Default["Duration"] = value; } }

		public string FolderPath { set { Properties.Settings.Default["FolderPath"] = value; } }

		public int RefreshRate { set { Properties.Settings.Default["RefreshRate"] = value; } }

		public bool UseFade { set { Properties.Settings.Default["UseFade"] = value; } }

		public bool ShowPreview { set { Properties.Settings.Default["ShowPreview"] = value; } }

		public void Save() => Properties.Settings.Default.Save();
	}
}
