namespace Memegenbot
{
    public partial class ImgflipMemeGenerator
    {
        public sealed partial record CaptionImageResponse
        {
            public static CaptionImageResponse Error(string message) => new(false, new CaptionImageData(string.Empty, string.Empty), message);
        }
    }
}