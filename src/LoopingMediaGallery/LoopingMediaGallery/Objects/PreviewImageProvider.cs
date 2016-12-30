using System.Windows.Media.Imaging;
using LoopingMediaGallery.Interfaces;
using System.Windows;
using System.Windows.Media;

namespace LoopingMediaGallery.Objects
{
	public class PreviewImageProvider : IGetViewPreview
	{
		public RenderTargetBitmap RenderPreviewBitmap(FrameworkElement view)
		{
			// https://blogs.msdn.microsoft.com/jaimer/2009/07/03/rendertargetbitmap-tips/
	
			if (!(view != null && view.ActualHeight > 0 && view.ActualWidth > 0))
				return null;

			Rect bounds = VisualTreeHelper.GetDescendantBounds(view);
			RenderTargetBitmap rtb = new RenderTargetBitmap((int)bounds.Width,
															(int)bounds.Height,
															96,
															96,
															PixelFormats.Pbgra32);
			DrawingVisual dv = new DrawingVisual();
			using (DrawingContext ctx = dv.RenderOpen())
			{
				VisualBrush vb = new VisualBrush(view);
				ctx.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
			}
			rtb.Render(dv);
			return rtb;
		}
	}
}
