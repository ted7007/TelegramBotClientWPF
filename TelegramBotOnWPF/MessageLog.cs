using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotOnWPF
{
    /// <summary>
    /// Сообщение
    /// </summary>
    class MessageLog
    {
        public string Time { get; set; }

        //public string Id { get; set; }

        //public string FirstName { get; set; }

        public string Text { get; set; }

        public bool IsUser { get; set; }
        public MessageLog(string time, string text, bool isUser)
        {
            Time = time;
            //Id = id;
            //FirstName = firstName;
            Text = text;
            IsUser = isUser;
        }
    }
}
