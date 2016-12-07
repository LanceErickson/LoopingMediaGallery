namespace LoopingMediaGallery.Interfaces
{
	public interface ISaveSettings
	{
		int Duration { set; }
		int RefreshRate { set; }
		string FolderPath { set; }
		bool UseFade { set; }
		bool ShowPreview { set; }
		void Save();
	}
}
