using WkHtmlToPdfDotNet;
using WkHtmlToPdfDotNet.Contracts;

namespace Domain.Helpers;

public sealed class PdfConverter
{
    private PdfConverter() { }

    private static readonly Lazy<SynchronizedConverter> _lazyConverter = new(() =>
    {
        return new SynchronizedConverter(new PdfTools());
    });

    public static SynchronizedConverter Instance => _lazyConverter.Value;

    public SynchronizedConverter GetConverter() => Instance;
}
