using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Newtonsoft.Json;

namespace TelegramBotOnWPF
{
    class TGBotMessageClient
    {
        #region Поля

        MainWindow w;

        TelegramBotClient bot;

        ObservableCollection<User> users;

        #endregion

        #region Свойства 

        public ObservableCollection<User> Users 
        {
            get
            {
                return users;
            }
            private set
            {
                users = value;
            }
        }


        #endregion

        #region Конструкторы
        public TGBotMessageClient(MainWindow w, string token )
        {
            this.w = w;
            bot = new TelegramBotClient(token);
            users = new ObservableCollection<User>();
            DownLoadUsers();
            bot.OnMessage += MessageListener;

            bot.StartReceiving();
        }
        #endregion

        #region Методы
        /// <summary>
        /// Метод, обрабатывающий сообщение
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="e"></param>
        public void MessageListener(object Sender, Telegram.Bot.Args.MessageEventArgs e)
        {

            string text = $"{DateTime.Now.ToLongTimeString()}: {e.Message.Chat.FirstName} {e.Message.Chat.Id} {e.Message.Text}";

            Console.WriteLine(text + " - " + e.Message.Type.ToString());

            User currentUser = new User(e.Message.Chat.Id, e.Message.Chat.Username, e.Message.Chat.FirstName);

            if (e.Message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return;

            w.Dispatcher.Invoke(() =>
            {
                AddUser(ref currentUser);
                currentUser.CreateDirectory();
                if (UseFile(e, currentUser))
                {
                    bot.SendTextMessageAsync(currentUser.ChatId, $"{currentUser.Firstname}, ваш файл был успешно загружен!");
                }

                UseText(e, currentUser);

            });
        }

        /// <summary>
        /// Метод посылающий сообщение от имени бота
        /// </summary>
        /// <param name="text">Текст сообщения</param>
        /// <param name="id">Индетефикатор чата</param>
        public void SendMessage(string text, long id)
        {
            bot.SendTextMessageAsync(id, text);
            MessageLog message = new MessageLog(DateTime.Now.ToLongTimeString(), text, false);
            foreach(var i in Users)
            {
                if (i.ChatId == id)
                    i.AddMessage(message);
            }    
        }

        #region Методы для работы с ботом из прошлого проекта

        /// <summary>
        /// Метод обработки файла в сообщении
        /// </summary>
        /// <param name="e">Сообщение</param>
        /// <param name="user">Ползователь, отправивший сообщение</param>
        /// <returns></returns>
        private bool UseFile(Telegram.Bot.Args.MessageEventArgs e, User user)
        {
            int count;
            switch (e.Message.Type)
            {
                case Telegram.Bot.Types.Enums.MessageType.Photo:
                    count = user.GetCountExtension(".png");
                    Download(e.Message.Photo[e.Message.Photo.Length - 1].FileId, $@"{user.DirectoryInfo}\Photo{count}.png");
                    break;
                case Telegram.Bot.Types.Enums.MessageType.Audio:
                    count = user.GetCountExtension(".mp3");
                    Download(e.Message.Audio.FileId, $@"{user.DirectoryInfo}\Audio{count}.mp3");
                    break;
                case Telegram.Bot.Types.Enums.MessageType.Document:
                    Download(e.Message.Document.FileId, $@"{user.DirectoryInfo}\{e.Message.Document.FileName}");
                    break;
                case Telegram.Bot.Types.Enums.MessageType.Video:
                    count = user.GetCountExtension(".mp4");
                    Download(e.Message.Video.FileId, $@"{user.DirectoryInfo}\Video{count}.mp4");
                    break;
                default:
                    return false;
                    break;
            }
            return true;
        }

        /// <summary>
        /// Метод обработки текста
        /// </summary>
        /// <param name="e">сообщение</param>
        /// <param name="user">польхователь, передавший сообщение</param>
        private async void UseText(Telegram.Bot.Args.MessageEventArgs e, User user)
        {
            string[] split = e.Message.Text.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            if (!(split.Length > 0))
                return;
            string SendMessage = "";
            switch (split[0])
            {
                case "/start":
                    SendMessage = $"Приветствую тебя, {user.Firstname}! Это моя новая разработка, карманное облако!\n" +
                                  $"Здесь ты можешь хранить свои файлы. Для этого просто отправь сюда любой файл, а я сохраню его!\n" +
                                  $"Для просмотри своих файлов используй команду /list.\n" +
                                  $"Для загрузки файлов используй команду /upload (index), где index это номер файла из списка.";
                    await bot.SendTextMessageAsync(user.ChatId, SendMessage);
                    break;
                case "/list":
                    SendMessage = $"{user.Firstname}, вывожу список твоих файлов! Не забудь скачать их с помощью команды /upload (index)\n";
                    SendMessage += user.GetNamesOfFiles();
                    await bot.SendTextMessageAsync(user.ChatId, SendMessage);
                    break;
                case "/upload":
                    if (!(split.Length > 1))
                        break;
                    int res;
                    if (int.TryParse(split[1], out res))
                    {
                        string extention = Path.GetExtension(user.GetNameOfFile(res));
                        if (user.GetNameOfFile(res) == "")
                        {
                            SendMessage = $"{user.Firstname}, файл не был найден.\n" +
                                          $"Убедитесь в том, что вы указали правильный индекс.";
                            await bot.SendTextMessageAsync(user.ChatId, SendMessage);
                            return;
                        }
                        using (FileStream fs = new FileStream($@"Users\{user.Username}\{user.GetNameOfFile(res)}", FileMode.Open))
                        {
                            Telegram.Bot.Types.InputFiles.InputOnlineFile file = new Telegram.Bot.Types.InputFiles.InputOnlineFile(fs);
                            Upload(Convert.ToString(user.ChatId), extention, file);
                        }
                    }
                    break;
                default:
                    break;



            }

            MessageLog message = new MessageLog(DateTime.Now.ToLongTimeString(), e.Message.Text, true);
            user.AddMessage(message);
            if (SendMessage != "")
            {
                MessageLog botMessage = new MessageLog(DateTime.Now.ToLongTimeString(), SendMessage, false);
                user.AddMessage(botMessage);
            }
            SaveUsers();
        }

        /// <summary>
        /// Метод загрузки файлов
        /// </summary>
        /// <param name="fileId">индетефикатор файла</param>
        /// <param name="path">имя файла</param>
        private async void Download(string fileId, string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                var file = await bot.GetFileAsync(fileId);
                await bot.DownloadFileAsync(file.FilePath, fs);
            }
        }

        /// <summary>
        /// Метод загрузки файла
        /// </summary>
        /// <param name="chatId">id чата, куда следует отправить файл</param>
        /// <param name="extention">расширение файла</param>
        /// <param name="file">сам файл</param>
        private async void Upload(string chatId, string extention, Telegram.Bot.Types.InputFiles.InputOnlineFile file)
        {
            switch (extention)
            {
                case ".png":
                    await bot.SendPhotoAsync(chatId, file);
                    break;
                case ".mp3":
                    await bot.SendAudioAsync(chatId, file);
                    break;
                case ".mp4":
                    await bot.SendVideoAsync(chatId, file);
                    break;
                default:
                    await bot.SendDocumentAsync(chatId, file);
                    break;
            }
        }


        /// <summary>
        /// Загрузка списка пользователей
        /// </summary>
        /// <returns></returns>
        private bool DownLoadUsers()
        {
            if (!File.Exists("Users.json"))
                return false;

            string json = File.ReadAllText("Users.json");
            users = JsonConvert.DeserializeObject<ObservableCollection<User>>(json);
            return true;
        }


        /// <summary>
        /// Сохранение списка пользователей
        /// </summary>
        private void SaveUsers()
        {
            string json = JsonConvert.SerializeObject(users);
            File.WriteAllText("Users.json", json);
        }


        /// <summary>
        /// Проверка на наличие пользователя в списке
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private bool isUserKnown(ref User user)
        {
            foreach (var i in users)
            {
                if (user.ChatId == i.ChatId)
                {
                    user = i;
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Добавление пользователя
        /// </summary>
        /// <param name="user"></param>
        private void AddUser(ref User user)
        {
            if (isUserKnown(ref user))
            {
                return;
            }
            users.Add(user);

        }

        #endregion

        #endregion


    }
}
