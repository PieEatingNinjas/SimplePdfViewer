using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Data.Pdf;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace SimplePdfViewer
{
    public sealed partial class SimplePdfViewerControl : UserControl
    {
        public SimplePdfViewerControl()
        {
            this.InitializeComponent();
        }

        public Uri Source
        {
            get { return (Uri)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(Uri), typeof(SimplePdfViewerControl), new PropertyMetadata(null, OnSourceChanged));

        public bool AutoLoad { get; set; }

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            ((SimplePdfViewerControl)d).OnSourceChanged();
        }

        private void OnSourceChanged()
        {
            if(AutoLoad)
            {
                LoadAsync();
            }
        }

        public async Task LoadAsync()
        {
            if(Source == null)
            {
                PdfPages.Clear();
            }
            else
            {
                if(Source.IsFile || !CheckIsHttp(Source))
                {
                    await LoadFromLocalAsync();
                }
                else if(CheckIsHttp(Source))
                {
                    await LoadFromRemoteAsync();
                }
                else
                {
                    //problem
                }
            }
        }

        private bool CheckIsHttp(Uri uri)
        {
            if (uri != null)
            {
                var str = uri.ToString();
                return str.StartsWith("http://") || str.StartsWith("https://");
            }
            return false;
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

        async void Load(PdfDocument pdfDoc)
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

        internal ObservableCollection<BitmapImage> PdfPages
        {
            get;
            set;
        } = new ObservableCollection<BitmapImage>();
    }
}
