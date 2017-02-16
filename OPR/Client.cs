
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace OPR
{
    public partial class ClientForm : Form
    {
        public ClientForm()
        {
            InitializeComponent();
        }


        void SendMessageFromSocket(String data)
        {
            try
            {
                // Буфер для входящих данных
                byte[] bytes = new byte[1024];

                // Соединяемся с удаленным устройством

                // Устанавливаем удаленную точку для сокета
                IPHostEntry ipHost = Dns.GetHostEntry("localhost");
                IPAddress ipAddr = ipHost.AddressList[0];
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 11000);

                Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                // Соединяем сокет с удаленной точкой
                sender.Connect(ipEndPoint);

                Console.Write("Введите сообщение: ");
                string message = data;

                Console.WriteLine("Сокет соединяется с {0} ", sender.RemoteEndPoint.ToString());
                byte[] msg = Encoding.UTF8.GetBytes(message);

                // Отправляем данные через сокет
                int bytesSent = sender.Send(msg);

                // Получаем ответ от сервера
                int bytesRec = sender.Receive(bytes);

                Console.WriteLine("\nОтвет от сервера: {0}\n\n", Encoding.UTF8.GetString(bytes, 0, bytesRec));
                outTextBox.Text += Encoding.UTF8.GetString(bytes, 0, bytesRec);

                // Освобождаем сокет
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
            }
            catch (System.Net.Sockets.SocketException e) { MessageBox.Show("Сервер не включен!"); }
        }


        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (simplex.Checked) { bion.Checked = false; this.Height = 335; }
        }

        private void bion_CheckedChanged(object sender, EventArgs e)
        {
            if (bion.Checked) { simplex.Checked = false; this.Height = 370; }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            outTextBox.Text = "";
            double[,] simplex_table = {
                                 {Convert.ToDouble(sum_1.Text), Convert.ToDouble(x1_1.Text),  Convert.ToDouble(x2_1.Text)},
                                {Convert.ToDouble(sum_2.Text), Convert.ToDouble(x1_2.Text),  Convert.ToDouble(x2_2.Text)},
                                {Convert.ToDouble(sum_3.Text), Convert.ToDouble(x1_3.Text),  Convert.ToDouble(x2_3.Text)},
                                 {Convert.ToDouble(sum_4.Text), Convert.ToDouble(x1_4.Text),  Convert.ToDouble(x2_4.Text)},
                                 {0, -Convert.ToDouble(mX1.Text),  -Convert.ToDouble(mX2.Text)}
            };
            double[,] bion_table = {
                                 { Convert.ToDouble(x1_1.Text),  Convert.ToDouble(x2_1.Text),Convert.ToDouble(sum_1.Text)},
                                { Convert.ToDouble(x1_2.Text),  Convert.ToDouble(x2_2.Text), Convert.ToDouble(sum_2.Text)},
                                { Convert.ToDouble(x1_3.Text),  Convert.ToDouble(x2_3.Text),Convert.ToDouble(sum_3.Text)},
                                 { Convert.ToDouble(x1_4.Text),  Convert.ToDouble(x2_4.Text),Convert.ToDouble(sum_4.Text)},
                                 {Convert.ToDouble(mX1.Text), Convert.ToDouble(mX2.Text),0}
            };
            String data = "";
            if (simplex.Checked)
            {

                for (int i = 0; i < simplex_table.GetLength(0); i++)
                {
                    for (int j = 0; j < simplex_table.GetLength(1); j++)
                    {
                        data += (simplex_table[i, j] + "|");
                    }
                }
                data += "СИМПЛЕКС|";
            }
            else if (bion.Checked)
            {
                for (int i = 0; i < bion_table.GetLength(0); i++)
                {
                    for (int j = 0; j < bion_table.GetLength(1); j++)
                    {
                        data += (bion_table[i, j] + "|");
                    }
                }
                data += "БИОНИЧЕСКИЙ|";
                data += Convert.ToDouble(genCount.Text) + "|";
                data += Convert.ToDouble(childCount.Text) + "|";
            }

            String mes = Cryptography.HachCipler.EncryptText(data);
            SendMessageFromSocket(mes);
        }

        private void About_Click(object sender, EventArgs e)
        {
            About about = new OPR.About();
            about.Show();
        }

        private void Lesson_Click(object sender, EventArgs e)
        {
            Lesson lesson = new OPR.Lesson();
            lesson.Show();
        }

    }
}
