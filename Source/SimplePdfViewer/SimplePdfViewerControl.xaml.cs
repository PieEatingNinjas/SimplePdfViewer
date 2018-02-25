using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Data.Pdf;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace SimplePdfViewer
{
    public sealed partial class SimplePdfViewerControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Uri Source
        {
            get { return (Uri)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(Uri), typeof(SimplePdfViewerControl),
                new PropertyMetadata(null, OnSourceChanged));

        public StorageFile File
        {
            get { return (StorageFile)GetValue(FileProperty); }
            set { SetValue(FileProperty, value); }
        }

        public static readonly DependencyProperty FileProperty =
            DependencyProperty.Register("File", typeof(StorageFile), typeof(SimplePdfViewerControl),
                new PropertyMetadata(null, OnFileChanged));

        public bool IsZoomEnabled
        {
            get { return (bool)GetValue(IsZoomEnabledProperty); }
            set { SetValue(IsZoomEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsZoomEnabledProperty =
            DependencyProperty.Register("IsZoomEnabled", typeof(bool), typeof(SimplePdfViewerControl),
                new PropertyMetadata(true, OnIsZoomEnabledChanged));
   
        internal ZoomMode ZoomMode
        {
            get { return IsZoomEnabled ? ZoomMode.Enabled : ZoomMode.Disabled; }
        }

        public bool AutoLoad { get; set; }

        internal ObservableCollection<BitmapImage> PdfPages
        {
            get;
            set;
        } = new ObservableCollection<BitmapImage>();

        public SimplePdfViewerControl()
        {
            this.Background = new SolidColorBrush(Colors.DarkGray);
            this.InitializeComponent();
        }

        private static void OnIsZoomEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SimplePdfViewerControl)d).OnIsZoomEnabledChanged();
        }

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SimplePdfViewerControl)d).OnSourceChanged();
        }

        private static void OnFileChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SimplePdfViewerControl)d).OnFileChanged();
        }

        private void OnIsZoomEnabledChanged()
        {
            OnPropertyChanged(nameof(ZoomMode));
        }

        private void OnSourceChanged()
        {
            if(AutoLoad)
            {
                LoadAsync();
            }
        }

        private void OnFileChanged()
        {
            if (AutoLoad)
            {
                LoadAsync();
            }
        }

        public async Task LoadAsync()
        {
            if(Source == null)
            {
                PdfPages.Clear();

                if(File != null)
                {
                    await LoadFromFileAsync();
                }
            }
            else
            {
                if(Source.IsFile || !Source.IsWebUri())
                {
                    await LoadFromLocalAsync();
                }
                else if(Source.IsWebUri())
                {
                    await LoadFromRemoteAsync();
                }
                else
                {
                    throw new ArgumentException($"Source '{Source.ToString()}' could not be recognized!");
                }
            }
        }

        private async Task LoadFromFileAsync()
        {
            if (File != null)
            {
                PdfDocument doc = await PdfDocument.LoadFromFileAsync(File);
                Load(doc);
            }
        }

        private async Task LoadFromRemoteAsync()
        {
            HttpClient client = new HttpClient();
            var stream = await
                client.GetStreamAsync(Source);
            var memStream = new MemoryStream();
            await stream.CopyToAsync(memStream);
            memStream.Position = 0;
            PdfDocument doc = await PdfDocument.LoadFromStreamAsync(memStream.AsRandomAccessStream());

            Load(doc);
        }

        private async Task LoadFromLocalAsync()
        {
            StorageFile f = await
                StorageFile.GetFileFromApplicationUriAsync(Source);
            PdfDocument doc = await PdfDocument.LoadFromFileAsync(f);

            Load(doc);
        }

        private async void Load(PdfDocument pdfDoc)
        {
            PdfPages.Clear();

            for (uint i = 0; i < pdfDoc.PageCount; i++)
            {
                BitmapImage image = new BitmapImage();

                var page = pdfDoc.GetPage(i);

                using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
                {
                    await page.RenderToStreamAsync(stream);
                    await image.SetSourceAsync(stream);
                }

                PdfPages.Add(image);
            }
        }

        public void OnPropertyChanged([CallerMemberName]string property = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
