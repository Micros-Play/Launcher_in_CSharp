using System;
using System.IO;
using System.Windows.Forms;
using System.Net;
using System.Threading.Tasks;
using System.Drawing;
using System.IO.Compression;
using System.Diagnostics;
using System.Linq;

namespace Launcher
{
    public partial class main : Form
    {
        public main()
        {
            InitializeComponent();
            Play.Enabled = false; //По дефолту
            Loader.Enabled = false; //Эти кнопки заблокированы
        }
        public void Form1_Load(object sender, EventArgs e)
        {
            //Модуль новости
            try //Если всё успешно
            {
                string urlimage = "https://raw.githubusercontent.com/Micros-Play/Launcher/main/imagenew.png"; //Ресурс на изображение
                string urldescription = "https://raw.githubusercontent.com/Micros-Play/Launcher/main/Description.cfg"; //Ресурс на описание
                WebClient news = new WebClient();
                pictureBox1.ImageLocation = urlimage; //Отображать в pictureBox1
                textBox1.Text = news.DownloadString(urldescription); //Отображать в textBox1

            }
            catch (Exception) { goto next; }
        next:;
            //Модуль проверки директории
            if (!Directory.Exists($@"{ Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\Launcher")) //Если не существует директории
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Launcher"); //Создаётся директория
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Launcher\setting"); //Создаётся директория параметров
                File.Create(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Launcher\setting\settings.cfg"); //Создаётся файл настроек
                File.Create(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Launcher\setting\ResourcesCheck.cfg"); //Создаётся файл проверки ресурсов
                string DownloadPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\Launcher"; //Переменная для скачивания в указанную директорию
                Size = new Size(607, 439);//Увеличивается окошко
                string url = "https://github.com/Micros-Play/Launcher/releases/download/download/client.zip"; //Переменная на репозиторий
                using (WebClient web = new WebClient()) //Модуль скачивания
                {
                    web.OpenRead(url);//Открывается из репозитория архив
                    string size = (Convert.ToDouble(web.ResponseHeaders["Content-Length"]) / 1048576).ToString("#.# MB"); //Запрос на отображение размера в МБ

                    web.DownloadProgressChanged += (s, r) => //Вывод в label и progressBar значений
                    {
                        label1.Text = $"{rus.sizefile} {size}\n{rus.loaded} {r.ProgressPercentage}% ({((double)r.BytesReceived / 1048576).ToString("#.# MB")})"; //Размер файла 100 МБ Загрузка 1% (1 МБ)
                        progressBar1.Value = r.ProgressPercentage; //1%
                    };
                    web.DownloadFileAsync(new Uri(url), DownloadPath + @"\client.zip"); //Асинхронное скачивание
                    CheckDownloaded(); //Запускаем модуль проверки загрузки
                }
            }
            else //Если существует
            {
                Size = new Size(607, 345); //Уменьшается окошко
                Play.Enabled = true; //Кнопки становятся
                Loader.Enabled = true; //Доступными
                progressBar1.Visible = false;
                label1.Visible = false;

                //Модуль загрузки настроек
                string checkSettings = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Launcher\setting\settings.cfg"); //Чтение из файла параметров
                if (checkSettings.Contains(setting.local_ru)) //Если русская
                {
                    Play.Text = rus.play;
                    Loader.Text = rus.loader;
                    label2.Text = rus.textres;
                    label5.Text = rus.newres;
                }
                if (checkSettings.Contains(setting.local_en)) //Если английский
                {
                    Play.Text = eng.play;
                    Loader.Text = eng.loader;
                    label2.Text = eng.textres;
                    label5.Text = eng.newres;
                }

                //Модуль обновления ресурсов
                try
                {
                    Play.Enabled = false; //Блокируем кнопки
                    Loader.Enabled = false; //Обе
                    string Resources = "https://raw.githubusercontent.com/Micros-Play/Launcher/main/resources.cfg"; //Файл на сервере актуальной версии
                    string PathDownFileRes = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\Launcher\setting\resources.cfg"; //Куда скачивать
                    string ResourcesCheck = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Launcher\setting\ResourcesCheck.cfg"; //Файл на компьютере с установленной на данный момент версией
                    string curver = File.ReadLines(ResourcesCheck).Skip(0).FirstOrDefault(); //Переменная для считывания текущей версии ресурсов
                    currentverres.Text = curver; //Отображение текущей версии
                    using (WebClient web = new WebClient())
                    {
                        web.DownloadFile(Resources, PathDownFileRes); //Скачиваем
                    }
                    string PathNewResFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Launcher\setting\resources.cfg";
                    string СheckResources = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Launcher\setting\resources.cfg"); //Открываем файл для чтения resources.cfg                    
                    string CheckResourcesCheck = File.ReadAllText(ResourcesCheck); //Открываем файл для чтения ResourcesCheck.cfg
                    string newver = File.ReadLines(PathNewResFile).Skip(0).FirstOrDefault(); //Переменная для считывания новой версии ресурсов
                    newverres.Text = newver;
                    if (CheckResourcesCheck != СheckResources) //Если версия, которая на сервере не соответствует
                    {
                        string UpdatePath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\Launcher"; //Переменная для скачивания в указанную директорию
                        Size = new Size(607, 439);//Увеличивается окошко
                        progressBar1.Visible = true; //Отображаем прогресс бар
                        progressBar1.Value = progressBar1.Minimum; //Ставим значение 0%
                        label1.Visible = true; //Отображаем Информацию о состоянии
                        string url = "https://github.com/Micros-Play/Launcher/releases/download/download/update.zip"; //Переменная на репозиторий
                        using (WebClient web = new WebClient()) //Модуль обновления
                        {
                            web.OpenRead(url);//Открывается из репозитория архив
                            string size = (Convert.ToDouble(web.ResponseHeaders["Content-Length"]) / 1048576).ToString("#.# MB"); //Запрос на отображение размера в МБ

                            web.DownloadProgressChanged += (s, r) => //Вывод в label и progressBar значений
                            {
                                if (checkSettings.Contains(setting.local_ru))
                                {
                                    label1.Text = $"{rus.sizefile} {size}\n{rus.loaded} {r.ProgressPercentage}% ({((double)r.BytesReceived / 1048576).ToString("#.# MB")})"; //Размер файла 100 МБ Загрузка 1% (1 МБ)
                                }

                                if (checkSettings.Contains(setting.local_en))
                                {
                                    label1.Text = $"{eng.sizefile} {size}\n{eng.loaded} {r.ProgressPercentage}% ({((double)r.BytesReceived / 1048576).ToString("#.# MB")})"; //Размер файла 100 МБ Загрузка 1% (1 МБ)
                                }
                                progressBar1.Value = r.ProgressPercentage; //1%
                            };
                            web.DownloadFileAsync(new Uri(url), UpdatePath + @"\client.zip"); //Асинхронное скачивание
                            File.WriteAllText(ResourcesCheck, СheckResources); //Вносит изменения в файл ResourcesCheck.cfg
                            CheckUpdates(); //Запускаем модуль проверки загрузки
                        }
                    }
                    else
                    {
                        Play.Enabled = true;
                        Loader.Enabled = true;
                    }
                }
                catch (Exception) {}

                //Модуль проверки целостности каталога с ресурсами
                if (!Directory.Exists($@"{ Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\Launcher\client")) //Если отсутствует главная директория ресурсов
                {
                    string url = "https://github.com/Micros-Play/Launcher/releases/download/download/update.zip"; //Переменная на репозиторий
                    string DownloadPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\Launcher"; //Переменная для скачивания в указанную директорию
                    Size = new Size(607, 439);//Увеличивается окошко
                    progressBar1.Visible = true; //Отображаем прогресс бар
                    progressBar1.Value = progressBar1.Minimum; //Ставим значение 0%
                    label1.Visible = true; //Отображаем Информацию о состоянии
                    Play.Enabled = false;
                    Loader.Enabled = false;
                    using (WebClient web = new WebClient()) //Модуль скачивания
                    {
                        web.OpenRead(url);//Открывается из репозитория архив
                        string size = (Convert.ToDouble(web.ResponseHeaders["Content-Length"]) / 1048576).ToString("#.# MB"); //Запрос на отображение размера в МБ

                        web.DownloadProgressChanged += (s, r) => //Вывод в label и progressBar значений
                        {
                            if (checkSettings.Contains(setting.local_ru))
                            {
                                label1.Text = $"{rus.sizefile} {size}\n{rus.loaded} {r.ProgressPercentage}% ({((double)r.BytesReceived / 1048576).ToString("#.# MB")})"; //Размер файла 100 МБ Загрузка 1% (1 МБ)
                            }
                            if (checkSettings.Contains(setting.local_en))
                            {
                                label1.Text = $"{eng.sizefile} {size}\n{eng.loaded} {r.ProgressPercentage}% ({((double)r.BytesReceived / 1048576).ToString("#.# MB")})"; //Размер файла 100 МБ Загрузка 1% (1 МБ)
                            }
                            progressBar1.Value = r.ProgressPercentage; //1%
                        };
                        web.DownloadFileAsync(new Uri(url), DownloadPath + @"\client.zip"); //Асинхронное скачивание
                        CheckRepair(); //Запускаем модуль проверки загрузки
                    }

                }
            }
        }
        async void CheckDownloaded() //Модуль проверки загрузки и распаковки
        {
            
            await Task.Run(() =>
                {
                    while (progressBar1.Value != progressBar1.Maximum) { } //Цикл для проверки загрузки
                });
            while (progressBar1.Value == progressBar1.Maximum) //Если загрузился архив с репозитория
            {
                string zipPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Launcher\client.zip"; //Путь, где находится архив
                string extractPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Launcher\client"; //Путь, где будет распакован архив
                ZipFile.ExtractToDirectory(zipPath, extractPath); //Распаковываем
                File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Launcher\client.zip"); //Остатки архива, удаляем
                Size = new Size(607, 345); //Уменьшается окошко
                Play.Enabled = true;
                Loader.Enabled = true;
                progressBar1.Visible = false;
                label1.Visible = false;
                goto exit; //Выход из цикла
            }
        exit:; //Точка выхода из цикла, после завершения
            //Запись дефолтного значения для ResourcesCheck.cfg
            string writePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Launcher\setting\ResourcesCheck.cfg"; //Переменная пути для StreamWriter
            using (StreamWriter version = new StreamWriter(writePath, false, System.Text.Encoding.UTF8)) //Записываем первое значение при первой загрузки
            {
                version.WriteLine("1.0"); //Версия 1.0
            }
            //Запись дефолтного значения для settings.cfg
            string SetSettingsDefault = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Launcher\setting\settings.cfg"; //Переменная пути для StreamWriter
            using (StreamWriter version = new StreamWriter(SetSettingsDefault, false, System.Text.Encoding.UTF8)) //Записываем первое значение при первой загрузки
            {
                version.WriteLine(setting.local_ru); //Версия 1.0
            }
            using (WebClient web = new WebClient()) //Модуль вывода версий
            {
                string PathDownFileRes = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\Launcher\setting\resources.cfg"; //Куда скачивать файл с сервера
                string Resources = "https://raw.githubusercontent.com/Micros-Play/Launcher/main/resources.cfg"; //Файл на сервере актуальной версии                
                web.DownloadFile(new Uri (Resources), PathDownFileRes);
                string ResourcesCheck = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Launcher\setting\ResourcesCheck.cfg";
                string СheckResources = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Launcher\setting\resources.cfg"; ;
                string curver = File.ReadLines(ResourcesCheck).Skip(0).FirstOrDefault(); //Переменная для считывания текущей версии ресурсов
                string newver = File.ReadLines(СheckResources).Skip(0).FirstOrDefault(); //Переменная для считывания новой версии ресурсов
                currentverres.Text = curver;
                newverres.Text = newver;
            }

        }
        async void CheckUpdates() //Модуль проверки загрузки и распаковки
        {
            await Task.Run(() =>
            {
                while (progressBar1.Value != progressBar1.Maximum) { } //Цикл для проверки загрузки
            });
            while (progressBar1.Value == progressBar1.Maximum) //Если загрузился архив с репозитория
            {
                if (Directory.Exists($@"{ Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\Launcher\client")) //Проверка наличия директории client
                {
                    Directory.Delete(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Launcher\client", true); //Удаляем директорию
                    string zipPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Launcher\client.zip"; //Путь, где находится архив
                    string extractPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Launcher\client"; //Путь, где будет распакован архив
                    ZipFile.ExtractToDirectory(zipPath, extractPath); //Распаковываем
                    File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Launcher\client.zip"); //Остатки архива, удаляем
                    Size = new Size(607, 345);
                    Play.Enabled = true;
                    Loader.Enabled = true;
                    progressBar1.Visible = false;
                    label1.Visible = false;
                    string ResourcesCheck = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Launcher\setting\ResourcesCheck.cfg";
                    string curver = File.ReadLines(ResourcesCheck).Skip(0).FirstOrDefault(); //Переменная для считывания текущей версии ресурсов
                    currentverres.Text = curver; //Отображение текущей версии
                    string PathNewResFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Launcher\setting\resources.cfg";
                    string newver = File.ReadLines(PathNewResFile).Skip(0).FirstOrDefault(); //Переменная для считывания новой версии ресурсов
                    newverres.Text = newver;
                    goto exit; //Выход из цикла
                }
            }
        exit:;  //Точка выхода из цикла, после завершения
        }

        async void CheckRepair() //Модуль проверки загрузки и распаковки
        {

            await Task.Run(() =>
            {
                while (progressBar1.Value != progressBar1.Maximum) { } //Цикл для проверки загрузки
            });
            while (progressBar1.Value == progressBar1.Maximum) //Если загрузился архив с репозитория
            {
                string zipPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Launcher\client.zip"; //Путь, где находится архив
                string extractPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Launcher\client"; //Путь, где будет распакован архив
                ZipFile.ExtractToDirectory(zipPath, extractPath); //Распаковываем
                File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Launcher\client.zip"); //Остатки архива, удаляем
                Size = new Size(607, 345); //Уменьшается окошко
                Play.Enabled = true;
                Loader.Enabled = true;
                progressBar1.Visible = false;
                label1.Visible = false;
                goto exit; //Выход из цикла
            }
        exit:; //Точка выхода из цикла, после завершения
        }

        private void Play_Click(object sender, EventArgs e)
        { //Загрузка локализации игры
            if (Play.Text == rus.play) //Если русская
            {
                Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Launcher\client\ETLoader-ru.exe");
            }
            if (Play.Text == eng.play) //Если английская
            {
                Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Launcher\client\ETLoader-en.exe");
            }
            Application.Exit(); //Закрывает лаунчер
        }

        private void Loader_Click(object sender, EventArgs e)
        {
            settings load = new settings();
            string checkSettings = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Launcher\setting\settings.cfg"); //Чтение из файла параметров
            if (checkSettings.Contains(setting.local_ru)) //Если русская
            {
                load.Text = rus.loader;
                load.radioButton1.Checked = true;
                load.radioButton2.Checked = false;
                load.groupBox1.Text = rus.gb1;
                load.groupBox2.Text = rus.gb2;
                load.radioButton1.Text = rus.rd1;
                load.radioButton2.Text = rus.rd2;
                load.label2.Text = rus.port;
                load.Apply.Text = rus.but1;
            }
            if (checkSettings.Contains(setting.local_en)) //Если английская
            {
                load.Text = eng.loader;
                load.radioButton1.Checked = false;
                load.radioButton2.Checked = true;
                load.groupBox1.Text = eng.gb1;
                load.groupBox2.Text = eng.gb2;
                load.radioButton1.Text = eng.rd1;
                load.radioButton2.Text = eng.rd2;
                load.label2.Text = eng.port;
                load.Apply.Text = eng.but1;
            }
            load.ShowDialog();
        }
    }
}
