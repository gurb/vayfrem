using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using draftio.models.dtos;
using draftio.viewmodels;
using SkiaSharp;
using System;
using System.IO;

namespace draftio
{
    public partial class ShortsView : UserControl
    {
        ShortsViewModel ViewModel { get; set; }



        private Image _saveImageFile_original;
        private Image _grayScaleImageFile;

        public ShortsView()
        {
            ViewModel = App.GetService<ShortsViewModel>();
            ViewModel.changeDelegate += changeViews;
            DataContext = ViewModel;
            InitializeComponent();


            // this operation valid for one project
            OpenFile.Click += OpenFile_Click;
            // this operation just valid for single page
            SaveFile.Click += SaveFile_Click;

            GenerateImages();
            changeViews();
        }

        private async void OpenFile_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var topLevel = TopLevel.GetTopLevel(this);

            FilePickerFileType fileType = new FilePickerFileType("Draft File")
            {
                Patterns = new[] { "*.gdraft" },
            };

            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new Avalonia.Platform.Storage.FilePickerOpenOptions
            {
                Title = "Open Project",
                AllowMultiple = false,
                FileTypeFilter = new[] { fileType }
            });

            if(files.Count >= 1)
            {
                await using var stream = await files[0].OpenReadAsync();
                using var streamReader = new StreamReader(stream);
                
                var fileContent = await streamReader.ReadToEndAsync();

                VMResponse response = ViewModel.LoadProject(fileContent);
                if (response.Success)
                {
                    
                }
                else
                {
                    await MessageBox.Show(this, "Error", response.Message!, MessageBox.MessageBoxButtons.Ok);
                }
            }
        }

        private async void SaveFile_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var topLevel = TopLevel.GetTopLevel(this);

            FilePickerFileType fileType = new FilePickerFileType("Draft File")
            {
                Patterns = new[] { "*.gdraft" },
            };

            var file = await topLevel!.StorageProvider.SaveFilePickerAsync(new Avalonia.Platform.Storage.FilePickerSaveOptions
            {
                Title = "Save File",
                FileTypeChoices = new[] { fileType }, 
            });

            if(file is not null)
            {
                VMResponse response = ViewModel.Save();

                if (response.Success)
                {
                    await using var stream = await file.OpenWriteAsync();
                    using var streamWriter = new StreamWriter(stream);
                    await streamWriter.WriteAsync(response.Result!.ToString());
                } else
                {
                    await MessageBox.Show(this, "Error", response.Message!, MessageBox.MessageBoxButtons.Ok);
                }
            }
        }

        public void changeViews()
        {
            if(ViewModel.IsSaved)
            {
                SaveImageFile.Source = _grayScaleImageFile.Source;
            } 
            else
            {
                SaveImageFile.Source = _saveImageFile_original.Source;
            }
        }

        private void GenerateImages ()
        {
            _saveImageFile_original = new Image();
            _saveImageFile_original.Source = new Avalonia.Media.Imaging.Bitmap(AssetLoader.Open(new Uri("avares://draftio/assets/save.png")));

            _grayScaleImageFile = new Image();
            _grayScaleImageFile.Source = GetGrayScaleImage("avares://draftio/assets/save.png");
        }

        public Avalonia.Media.Imaging.Bitmap GetGrayScaleImage(string path)
        {
            Stream stream = AssetLoader.Open(new Uri(path));

            SKBitmap originalBitmap;
            using (SKManagedStream skStream = new SKManagedStream(stream))
            {
                originalBitmap = SKBitmap.Decode(skStream);
            }

            SKBitmap grayScaleBitmap = ConvertToGrayScale(originalBitmap);

            return SKBitmapToAvaloniaBitmap(grayScaleBitmap);
        }

        private SKBitmap ConvertToGrayScale(SKBitmap originalBitmap)
        {
            SKBitmap grayScaleBitmap = new SKBitmap(originalBitmap.Width, originalBitmap.Height);

            using (SKCanvas canvas = new SKCanvas(grayScaleBitmap))
            {
                SKPaint paint = new SKPaint
                {
                    ColorFilter = SKColorFilter.CreateColorMatrix(new float[]
                    {
                        0.33f, 0.33f, 0.33f, 0, 0,
                        0.33f, 0.33f, 0.33f, 0, 0,
                        0.33f, 0.33f, 0.33f, 0, 0,
                        0, 0, 0, 1, 0
                    })
                };

                canvas.DrawBitmap(originalBitmap, 0,0, paint);
            }

            return grayScaleBitmap;
        }

        public Avalonia.Media.Imaging.Bitmap SKBitmapToAvaloniaBitmap(SKBitmap skBitmap)
        {
            SKData data = skBitmap.Encode(SKEncodedImageFormat.Png, 100);
            using (Stream stream = data.AsStream())
            {
                return new Avalonia.Media.Imaging.Bitmap(stream);
            }
        }
       
    }
}
