using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Skia;
using SkiaSharp;
using Splat;

namespace Nein.Controls;

public class SkiaImage : Control
{
    public static readonly StyledProperty<SKBitmap> SourceProperty =
        AvaloniaProperty.Register<Image, SKBitmap>(nameof(Source));

    public static readonly StyledProperty<string> UriSourceProperty =
        AvaloniaProperty.Register<Image, string>(nameof(UriSource));

    public static readonly StyledProperty<Stretch> StretchProperty =
        AvaloniaProperty.Register<Image, Stretch>(nameof(Stretch), Stretch.UniformToFill);

    public static readonly StyledProperty<float> BlurStrengthProperty =
        AvaloniaProperty.Register<Image, float>(nameof(BlurStrength));

    public static readonly StyledProperty<BitmapInterpolationMode> BitMapInterpolationModeProperty =
        AvaloniaProperty.Register<Image, BitmapInterpolationMode>(nameof(BitmapInterpolationMode));

    private RenderTargetBitmap _renderTarget;
    private SKBitmap _effectBitmap;
    private SKCanvas _skiaCanvas;
    private DrawingContext _skiaContext;
    private SKPaint _skPaint;

    public SKBitmap? Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public string UriSource
    {
        get => GetValue(UriSourceProperty);
        set
        {
            SetValue(UriSourceProperty, value);
            var assets = Locator.Current.GetService<IAssetLoader>();
            var uri = new Uri(value, UriKind.Absolute);
            if (assets!.Exists(uri))
            {
                var stream = assets.Open(uri);
                SetValue(SourceProperty, SKBitmap.Decode(stream));
            }
        }
    }

    public Stretch Stretch
    {
        get => GetValue(StretchProperty);
        set => SetValue(StretchProperty, value);
    }

    public float BlurStrength
    {
        get => GetValue(BlurStrengthProperty);
        set => SetValue(BlurStrengthProperty, value);
    }

    public BitmapInterpolationMode BitmapInterpolationMode
    {
        get => GetValue(BitMapInterpolationModeProperty);
        set => SetValue(BitMapInterpolationModeProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (Source != null)
        {
            if (change.Property == SourceProperty)
            {
                _skPaint = new SKPaint();
                _skPaint.FilterQuality = SKFilterQuality.High;
                _skPaint.IsAntialias = true;
                _skPaint.ImageFilter = SKImageFilter.CreateBlur(BlurStrength, BlurStrength);

                _renderTarget = new RenderTargetBitmap(new PixelSize(Source.Width, Source.Height));

                _effectBitmap = Source.Copy();

                _skiaCanvas = new SKCanvas(_effectBitmap);

                _skiaCanvas.Clear(new SKColor(255, 255, 255, 0));
                _skiaCanvas.DrawBitmap(Source, 0, 0, _skPaint);

                _renderTarget.CopyPixels(new PixelRect(0, 0, Source.Width, Source.Height), _effectBitmap.GetPixels(), _effectBitmap.Pixels.Length, 0);

                InvalidateVisual();
            }
            else if (change.Property == BlurStrengthProperty)
            {
                _skPaint.ImageFilter = SKImageFilter.CreateBlur(BlurStrength, BlurStrength);

                _skiaCanvas.Clear(new SKColor(255, 255, 255));
                _skiaCanvas.DrawBitmap(Source, 0, 0, _skPaint);

                _renderTarget.CopyPixels(new PixelRect(0, 0, Source.Width, Source.Height), _effectBitmap.GetPixels(), _effectBitmap.Pixels.Length, 0);

                InvalidateVisual();
            }
        }

        base.OnPropertyChanged(change);
    }

    public override void Render(DrawingContext context)
    {
        if (Source != null)
        {
            var srcSize = new Size(Source.Width, Source.Height);
            var scale = Stretch.CalculateScaling(Bounds.Size, srcSize);
            var scaledSize = srcSize * scale;
            var destRect = new Rect(Bounds.Size).CenterRect(new Rect(scaledSize)).Intersect(new Rect(Bounds.Size));
            var sourceRect = new Rect(srcSize).CenterRect(new Rect(destRect.Size / scale));
            context.DrawImage(_renderTarget, sourceRect, destRect);
        }
    }
}