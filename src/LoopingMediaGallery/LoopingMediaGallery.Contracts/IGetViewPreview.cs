using System.Windows;
using System.Windows.Media.Imaging;

namespace LoopingMediaGallery.Interfaces
{
	public interface IGetViewPreview
	{
		RenderTargetBitmap RenderPreviewBitmap(FrameworkElement view);
	}
}
