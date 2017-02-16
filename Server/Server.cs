using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GenAlg;


namespace Server
{
    public partial class ServerForm : Form
    {
        public ServerForm()
        {
            InitializeComponent();
        }
        double[,] table = new double[5, 3];
        Socket sListener;


        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            // Устанавливаем для сокета локальную конечную точку
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 11000);

            // Создаем сокет Tcp/Ip
            sListener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Назначаем сокет локальной конечной точке и слушаем входящие сокеты
            try
            {
                sListener.Bind(ipEndPoint);
                sListener.Listen(10);

                // Начинаем слушать соединения
                while (true)
                {


                    // Программа приостанавливается, ожидая входящее соединение
                    Socket handler = sListener.Accept();
                    string data = null;

                    // Мы дождались клиента, пытающегося с нами соединиться
                    byte[] bytes = new byte[1024];
                    int bytesRec = handler.Receive(bytes);

                    data += Encoding.UTF8.GetString(bytes, 0, bytesRec);
                    server_logs.Invoke(new Action(() => server_logs.Text += ("Данные успешно получены!\n")));
                    data = Cryptography.HachCipler.DecryptText(data);
                    int lvl = 0;
                    String[] values = new String[40];
                    String str = "";
                    for (int i = 0; i < data.Length; i++)
                    {
                        if (!data[i].Equals('|')) { str += data[i]; }
                        if (data[i].Equals('|')) { values[lvl] = str; lvl++; str = ""; };
                    }
                    lvl = 0;
                    for (int i = 0; i < 5; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {

                            table[i, j] = Convert.ToDouble(values[lvl]); lvl++;

                            server_logs.Invoke(new Action(() => server_logs.Text += table[i, j] + " "));
                        }
                        server_logs.Invoke(new Action(() => server_logs.Text += "\n"));
                    }
                    server_logs.Invoke(new Action(() => server_logs.Text += "\n"));
                    String reply = "";
                    double[] result = new double[2];
                    double[,] table_result;
                    int genCount = Convert.ToInt32(values[16]);
                    int childCount = Convert.ToInt32(values[17]);
                    switch (values[15])
                    {
                        case "СИМПЛЕКС":
                            Simplex.Simplex S = new Simplex.Simplex(table);
                            table_result = S.Calculate(result);
                            server_logs.Invoke(new Action(() => server_logs.Text += "Решение \n" + S.str +
                             ("X[1] = " + result[0] + "\n") +
                            ("X[2] = " + result[1] + "\n") +
                            ("Сумма = " + table_result[4, 0] + "\n")));

                            reply += ("Задача была решена Симплекс-Методом") + "\n";
                            reply += ("Для получения максимальной прибыли в размере ") + table_result[4, 0] + " долларов\n";
                            reply += ("Необходимо ежемесячно производить полок:\n");
                            reply += "Типа А в количестве " + result[0] + " штук\n";
                            reply += "Типа B в количестве " + result[1] + " штук\n";

                            break;
                        case "БИОНИЧЕСКИЙ":
                            Bionical temp = new Bionical(table, childCount, genCount);

                            List<GenPoint> answer = new List<GenPoint>();
                            answer = temp.getX();
                            double[] ans = new double[3] { (double)answer.First().x1, (double)answer.First().x2, (double)answer.First().y };
                            server_logs.Invoke(new Action(() => server_logs.Text += "Решение \n" +
                            ("X[1] = " + ans[0] + "\n") +
                            ("X[2] = " + ans[1] + "\n") +
                            ("Сумма = " + ans[2] + "\n")));
                            reply += ("Задача была решена Бионическим Алгоритмом") + "\n";
                            reply += ("Для получения максимальной прибыли в размере ") + ans[2] + " долларов\n";
                            reply += ("Необходимо ежемесячно производить полок:\n");
                            reply += "Типа А в количестве " + ans[0] + " штук\n";
                            reply += "Типа B в количестве " + ans[1] + " штук\n";
                            break;
                    }

                    // Отправляем ответ клиенту
                    byte[] msg = Encoding.UTF8.GetBytes(reply);
                    handler.Send(msg);

                    if (data.IndexOf("<TheEnd>") > -1)
                    {
                        server_logs.Invoke(new Action(() => server_logs.Text += "Сервер завершил соединение с клиентом."));
                        break;
                    }

                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {

            server_logs.Invoke(new Action(() => server_logs.Text += ("Сервер выключен!\n")));
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 11000);
            server_logs.Text += ("Сервер включен!" + "\n");
        }

        private void button2_Click_1(object sender, EventArgs e)
        {

            //backgroundWorker1.Dispose(); backgroundWorker1.CancelAsync();
            sListener.Close();
            server_logs.Text += ("Сервер выключен!" + "\n");
        }
    }
}
