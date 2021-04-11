using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotOnWPF
{
    class User
    {
        public long ChatId { get; private set; }

        public string Username { get; private set; }

        public string Firstname { get; private set; }

        public string DirectoryInfo { get; private set; }

        public ObservableCollection<MessageLog> MessageLogs { get; private set; }



        public User(long chatId, string username, string firstname)
        {
            ChatId = chatId;
            Username = username;
            Firstname = firstname;
            DirectoryInfo = $@"Users\{username}";
            Directory.CreateDirectory(DirectoryInfo);
            MessageLogs = new ObservableCollection<MessageLog>();
        }

        /// <summary>
        /// Метод получает количество файлов с определённым расширением
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public int GetCountExtension(string extention)
        {
            DirectoryInfo dir = new DirectoryInfo(DirectoryInfo);
            int count = 0;
            foreach (var i in dir.GetFiles())
            {
                if (i.Extension == extention)
                    count++;
            }
            return count;
        }

        /// <summary>
        /// Метод получеения списка файлов(string) в дирректории юзера
        /// /// </summary>
        /// <returns></returns>
        public string GetNamesOfFiles()
        {
            DirectoryInfo dir = new DirectoryInfo(DirectoryInfo);
            if (!dir.Exists || dir.GetFiles().Length == 0)
                return "";
            string text = "";
            for (var i = 0; i < dir.GetFiles().Length; i++)
            {
                text += $"{i,3} - {dir.GetFiles()[i].Name}\n";
            }
            return text;
        }


        /// <summary>
        /// Метод получения имени конкретного файла по номеру в списке
        /// </summary>
        /// <param name="index">номер в списке файлов</param>
        /// <returns></returns>
        public string GetNameOfFile(int index)
        {

            DirectoryInfo dir = new DirectoryInfo(DirectoryInfo);
            if (!dir.Exists || dir.GetFiles().Length - 1 < index || index < 0)
                return "";
            return dir.GetFiles()[index].Name;
        }

        /// <summary>
        /// Метод создания директории для хранения фалов
        /// </summary>
        public void CreateDirectory()
        {
            DirectoryInfo dir = new DirectoryInfo(DirectoryInfo);
            if (dir.Exists)
                return;
            dir.Create();
        }

        /// <summary>
        /// Метод добавления сообщения в логи
        /// </summary>
        /// <param name="message">сообщение, которое требуется передать</param>
        public void AddMessage(MessageLog message)
        {
            MessageLogs.Add(message);
        }
    }
}
