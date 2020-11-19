using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using RestSharp;

// ReSharper disable InconsistentNaming

[assembly: InternalsVisibleTo("Memegenbot.Tests")]

namespace Memegenbot
{
    /// <summary>
    /// imgflip memes generator. See the documentation on <see href="https://imgflip.com/api"/>.
    /// </summary>
    public partial class ImgflipMemeGenerator
    {
        public const string Imgflip = "Imgflip";
        public const string URL = "https://api.imgflip.com";

        #region imgflip API models

        private const string COLOR_BLACK = "#ffffff";
        private const string COLOR_WHITE = "#000000";
        private const int DEFAULT_FONT_SIZE = 14;

        // @formatter:off
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedMember.Global
        public enum EFonts { arial, impact }
        public sealed record GetMemesResponse(bool success, MemesData data);
        // ReSharper disable once ParameterTypeCanBeEnumerable.Local
        public sealed record MemesData(Meme[] memes);
        public sealed record Meme(int id, string name, string url, int width, int height, int box_count);
        public sealed record CaptionImageRequest(int template_id, string text0 = null, string text1 = null, EFonts? font = null, int? max_font_size = null, params Box[] boxes);
        public sealed partial record Box(string text, int? x = null, int? y = null, int? width = null, int? height = null, string color = null, string outline_color = null);
        public sealed partial record CaptionImageResponse(bool success, CaptionImageData data, string error_message);
        public sealed record CaptionImageData(string url, string page_url);
        // @formatter:on

        #endregion

        private readonly RestClient _client;
        private readonly NetworkCredential _credentials;

        public ImgflipMemeGenerator(NetworkCredential credentials)
        {
            _credentials = credentials;
            _client = new RestClient(URL);
        }

        public CaptionImageResponse CreateRandomMeme(string message)
        {
            if (message is {Length: 0}) throw new ArgumentNullException(nameof(message), "Message can not be empty");
            
            const int minRequiredWords = 4;
            var split = message.Split(' ');
            if (split.Length < minRequiredWords) throw new ArgumentException("Message contains too few words", nameof(message));

            var allMemes = GetMemes();
            var meme = allMemes[new Random().Next(0, allMemes.Length)];
            
            var boxes = new Box[meme.box_count];
            var step = split.Length / meme.box_count;

            var queue = new Queue<string>(split);
            for (var i = 0; i < meme.box_count; i++)
            {
                if(i == meme.box_count - 1)
                    boxes[i] = new Box(string.Join(' ', queue.Dequeue(split.Length - step * i)));
                else
                    boxes[i] = new Box(string.Join(' ', queue.Dequeue(step)));
            }

            var request = new CaptionImageRequest(meme.id, boxes: boxes);
            return CaptionImage(request);
        }

        #region internal: API methods

        internal Meme[] GetMemes()
        {
            var request = new RestRequest("get_memes", DataFormat.Json);
            var response = _client.Get(request);

            var memesResponse = JsonConvert.DeserializeObject<GetMemesResponse>(response.Content);
            return memesResponse.data.memes;
        }

        /// <summary>
        /// Create a meme.
        /// </summary>
        /// <returns></returns>
        // ReSharper disable once UseDeconstructionOnParameter
        internal CaptionImageResponse CaptionImage(CaptionImageRequest captionImageRequest)
        {
            var request = new RestRequest("caption_image", Method.POST, DataFormat.Json);
            request.AddParameter("username", _credentials.UserName);
            request.AddParameter("password", _credentials.Password);
            request.AddParameter(nameof(CaptionImageRequest.template_id), captionImageRequest.template_id);

            if (captionImageRequest.max_font_size != null) request.AddParameter(nameof(CaptionImageRequest.max_font_size), captionImageRequest.max_font_size);
            if (captionImageRequest.font != null) request.AddParameter(nameof(CaptionImageRequest.font), captionImageRequest.font);

            if (captionImageRequest.text0 is {Length: > 0} ||
                captionImageRequest.text1 is {Length: > 0})
            {
                request.AddParameter(nameof(CaptionImageRequest.text0), captionImageRequest.text0);
                request.AddParameter(nameof(CaptionImageRequest.text1), captionImageRequest.text1);
            }
            else
            {
                for (var i = 0; i < captionImageRequest.boxes.Length; i++)
                {
                    var box = captionImageRequest.boxes[i];
                    if (box.text != null) request.AddParameter($"{nameof(CaptionImageRequest.boxes)}[{i}][{nameof(Box.text)}]", box.text);
                    if (box.color != null) request.AddParameter($"{nameof(CaptionImageRequest.boxes)}[{i}][{nameof(Box.color)}]", box.color);
                    if (box.outline_color != null) request.AddParameter($"{nameof(CaptionImageRequest.boxes)}[{i}][{nameof(Box.color)}]", box.outline_color);
                    if (box.width != null) request.AddParameter($"{nameof(CaptionImageRequest.boxes)}[{i}][{nameof(Box.width)}]", box.width);
                    if (box.height != null) request.AddParameter($"{nameof(CaptionImageRequest.boxes)}[{i}][{nameof(Box.height)}]", box.height);
                    if (box.x != null) request.AddParameter($"{nameof(CaptionImageRequest.boxes)}[{i}][{nameof(Box.x)}]", box.x);
                    if (box.y != null) request.AddParameter($"{nameof(CaptionImageRequest.boxes)}[{i}][{nameof(Box.y)}]", box.y);
                }
            }

            var response = _client.Post(request);

            return JsonConvert.DeserializeObject<CaptionImageResponse>(response.Content);

            #endregion
        }
    }
}