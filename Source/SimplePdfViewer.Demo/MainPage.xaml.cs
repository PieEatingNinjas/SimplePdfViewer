using System;
using System.ComponentModel;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SimplePdfViewer.Demo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        public Uri Source { get; set; }

        public StorageFile File { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            File = e.Parameter as StorageFile;
            if (File != null)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(File)));
        }


        public async void OpenLocal()
        {
            var uri = new Uri("ms-appx:///Assets/pdffile.pdf");

            Source = uri;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Source)));
        }

        public async void OpenRemote()
        {
            var uri = new Uri("http://www.adobe.com/content/dam/Adobe/en/accessibility/products/acrobat/pdfs/acrobat-x-accessible-pdf-from-word.pdf");

            Source = uri;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Source)));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
