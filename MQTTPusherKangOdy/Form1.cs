using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace MQTTPusherKangOdy
{
    public partial class Form1 : Form
    {
        private MqttClient client;

        public Form1()
        {
            InitializeComponent();
            button3.Enabled = false; // Menonaktifkan tombol Disconnect Diawal Aplikasi Run

        }


        private void button1_Click(object sender, EventArgs e)
        {
            string brokerAddress = textBox1.Text;
            int port = int.Parse(textBox3.Text);
            string username = textBox4.Text;
            string password = textBox5.Text;


            //Inisialisasi MQTT Client
            client = new MqttClient(brokerAddress, port, false, null, null, MqttSslProtocols.None);


            // Event handler untuk menerima pesan
            client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;


            //Session Connection
            client.Connect(Guid.NewGuid().ToString(), username, password);

            if (client.IsConnected)
            {
                MessageBox.Show("Berhasil Terhubung!!");
                button1.Enabled = false;
                button3.Enabled = true;

                // Subscribe ke topik
                string topic = textBox2.Text;
                client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
            }
            else
            {
                MessageBox.Show("Koneksi Gagal ke Broker Boy!");
            }

        }

        private void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            // Mendapatkan pesan dan menambahkannya ke ListBox
            string message = Encoding.UTF8.GetString(e.Message);
            this.Invoke((MethodInvoker)delegate
            {
                listBox1.Items.Add($"Received: {message}");
            });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (client != null && client.IsConnected)
            {
                string topic = textBox2.Text;
                string payload = textBox6.Text;
                client.Publish(topic, Encoding.UTF8.GetBytes(payload), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
                //MessageBox.Show("Pesan dikirim Coy!!");
                // Kosongkan TextBox setelah pesan dikirim
                textBox6.Text = String.Empty;
            }
            else
            {
                MessageBox.Show("Tidak terhubung ke broker. Silakan hubungkan terlebih dahulu.");
            }

        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (client != null && client.IsConnected)
            {
                client.Disconnect();
                MessageBox.Show("Koneksi terputus dari broker MQTT.");
                button1.Enabled = true; // Mengaktifkan kembali tombol Connect setelah putus
                button3.Enabled = false; // Menonaktifkan tombol Disconnect setelah putus
            }
            else
            {
                MessageBox.Show("Tidak ada koneksi yang perlu diputus.");
            }
        }
       
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (client != null && client.IsConnected)
            { 
                client.Disconnect(); 
            }
            Application.Exit();
        }

    }
}
