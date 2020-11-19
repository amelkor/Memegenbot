using System;
using System.Collections.Generic;
using System.Linq;

namespace Memegenbot.Tests
{
    public static class TestHelper
    {
        public static class Url
        {
            public static bool IsValid(string uri)
            {
                return Uri.TryCreate(uri, UriKind.Absolute, out var uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            }
        }

        public static IEnumerable<string> CreateRandomCaptions(ImgflipMemeGenerator.Meme meme)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            const int length = 20;
            var random = new Random();

            for (var i = 0; i < meme.box_count; i++)
            {
                yield return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
            }
        }
    }
}