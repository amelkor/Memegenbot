using System.Linq;
using System.Net;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Memegenbot.Tests
{
    public class GeneratorApiTests
    {
        private IConfiguration _configuration;

        private ImgflipMemeGenerator _api;
        private readonly ImgflipMemeGenerator.Meme[] _memes =
        {
            new(
                131087935,
                "Running Away Balloon",
                @"https://i.imgflip.com/261o3j.jpg",
                761,
                1024,
                5),
            new(
                181913649,
                "Drake Hotline Bling",
                @"https://i.imgflip.com/30b1gx.jpg",
                1200,
                1200,
                2)
        };

        [SetUp]
        public void Setup()
        {
            _configuration = new ConfigurationBuilder().AddUserSecrets<GeneratorApiTests>().Build();
            _api = new ImgflipMemeGenerator(new NetworkCredential(_configuration["Imgflip:login"], _configuration["Imgflip:password"]));
        }

        [Test]
        public void GetAllMemes_AssertNotNullAny()
        {
            var memes = _api.GetMemes();

            Assert.NotNull(memes);
            Assert.IsTrue(memes.Any());
        }

        [TestCase(0)]
        [TestCase(1)]
        public void CreateMeme_AssertResultUrlNotEmpty(int memeId)
        {
            var boxes = ImgflipMemeGenerator.Box.FromMeme(_memes[memeId], TestHelper.CreateRandomCaptions(_memes[memeId]).ToArray());
            var meme = _memes[memeId];
            var request = new ImgflipMemeGenerator.CaptionImageRequest(meme.id, boxes: boxes.ToArray());
            
            var url = _api.CaptionImage(request).data.url;

            Assert.NotNull(url);
            Assert.IsTrue(TestHelper.Url.IsValid(url));
        }
        
        [TestCase("А мои маленькие сосочки полетели на юг")]
        [TestCase("Сколько здесь всего я дремал шатал. Давайте уже поедем")]
        public void CreateMeme_AssertResultUrlNotEmpty(string message)
        {
            var url = _api.CreateRandomMeme(message).data.url;

            Assert.NotNull(url);
            Assert.IsTrue(TestHelper.Url.IsValid(url));
        }
    }
}