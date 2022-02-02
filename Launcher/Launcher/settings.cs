using System;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace Launcher
{
    public partial class settings : Form
    {
        public settings()
        {
            InitializeComponent();
        }

        private void settings_Load(object sender, EventArgs e)
        {
            string PathSetting = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Launcher\setting\settings.cfg";
            string ip = File.ReadLines(PathSetting).Skip(2).FirstOrDefault();
            string port = File.ReadLines(PathSetting).Skip(4).FirstOrDefault();
            textBox1.Text = ip;
            textBox2.Text = port;
        }

        private void Apply_Click(object sender, EventArgs e)
        {
            string PathSetting = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Launcher\setting\settings.cfg";
            if (radioButton1.Checked)
            {
                using (StreamWriter settings = new StreamWriter(PathSetting, false, System.Text.Encoding.UTF8))
                {
                    settings.WriteLine("[SetLocale = RU]");
                    settings.WriteLine("[IP Address]");
                    settings.WriteLine(textBox1.Text);
                    settings.WriteLine("[Port Connect]");
                    settings.WriteLine(textBox2.Text);
                }
                
                
            }
            if (radioButton2.Checked)
            {
                using (StreamWriter settings = new StreamWriter(PathSetting, false, System.Text.Encoding.UTF8))
                {
                    settings.WriteLine("[SetLocale = EN]");
                    settings.WriteLine("[IP Address]");
                    settings.WriteLine(textBox1.Text);
                    settings.WriteLine("[Port Connect]");
                    settings.WriteLine(textBox2.Text);
                }
            }
            if (textBox1.Text == "") { }
            else
            {
                string ChangeSocket = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Launcher\client\socket.cfg";
                using (StreamWriter change = new StreamWriter(ChangeSocket, false, System.Text.Encoding.UTF8))
                {
                    const string quote = "\"";
                    const string thudot = ":";
                    const string zap = ",";
                    change.WriteLine("{");
                    change.WriteLine("	" + quote + "ip" + quote + thudot + " " + quote + textBox1.Text + quote + zap);
                    change.WriteLine("	" + quote + "port" + quote + thudot + " " + textBox2.Text);
                    change.WriteLine("}");
                }
            }
            this.Close();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar; //Переменная ввода
            //Только цифры, только точка ".", только клавиша Backspace
            if (!Char.IsDigit(number) && number != 8 && number != 46) //Если несоответствует
            {
                e.Handled = true; //Отклоняем
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar; //Переменная ввода
            //Только цифры, только клавиша Backspace
            if (!Char.IsDigit(number) && number != 8) //Если несоответствует
            {
                e.Handled = true; //Отклоняем
            }
        }
    }
}
