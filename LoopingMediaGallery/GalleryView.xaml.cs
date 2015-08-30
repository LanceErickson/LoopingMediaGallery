using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LoopingMediaGallery
{
    /// <summary>
    /// Interaction logic for GalleryView.xaml
    /// </summary>
    public partial class GalleryView : Window
    {
        MainWindowViewModel viewModel;

        public GalleryView()
        {
            InitializeComponent();
        }

        public GalleryView(MainWindowViewModel viewModel) : this()
        {
            this.viewModel = viewModel;
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Maximized;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            loopingMediaControl.Dispose();
        }
    }
}
