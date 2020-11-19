using System;
using System.Collections.Generic;

namespace Memegenbot
{
    public partial class ImgflipMemeGenerator
    {
        public sealed partial record Box
        {
            /// <summary>
            /// Simply creates new meme boxes from provided meme template.
            /// </summary>
            /// <param name="meme">Meme template</param>
            /// <param name="captions">Captions for the meme's image</param>
            /// <returns>Boxes for meme creation</returns>
            /// <exception cref="ArgumentNullException">captions are empty</exception>
            /// <exception cref="ArgumentException">Captions length is 0 or exceeds max meme captions count</exception>
            public static IEnumerable<Box> FromMeme(Meme meme, params string[] captions)
            {
                if (captions == null) throw new ArgumentNullException(nameof(captions));
                if (captions.Length == 0) throw new ArgumentException("Captions must not be empty", nameof(captions));
                if (captions.Length > meme.box_count) throw new ArgumentException("Captions count exceeds max meme captions count", nameof(captions));

                const int xOffset = 10;
                const int yOffset = 10;
                var yStep = meme.height / meme.box_count;

                for (int i = 0, y = yOffset; i < meme.box_count; i++, y += yStep)
                {
                    yield return new Box(captions[i], xOffset, y, meme.width / 2, meme.height / 2);
                }
            }
        }
    }
}