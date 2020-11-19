// ReSharper disable InconsistentNaming

using System.Net;
using System.Threading;

namespace Memegenbot.WebApi.Integration
{
    /// <summary>
    /// Telegram models record sets.
    /// </summary>
    public static class Telegram
    {
        public sealed record Update(int update_id, Message message);

        public sealed record Message(Chat chat, string text);

        public sealed record Chat(int id);

        /// <summary>
        /// Send message as bot to telegram room.
        /// </summary>
        /// <param name="botToken">b</param>
        /// <param name="chatId"></param>
        /// <param name="text"></param>
        public static void SendMessage(string botToken, int chatId, string text)
        {
            var message = (HttpWebRequest) WebRequest.Create($"https://api.telegram.org/bot{botToken}/sendMessage?chat_id={chatId}&text={text}");
            ThreadPool.QueueUserWorkItem(_ => { message.GetResponse(); });
        }
    }
}