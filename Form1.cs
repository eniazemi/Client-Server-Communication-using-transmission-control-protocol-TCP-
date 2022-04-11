using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using SuperSimpleTcp;



namespace TCPClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        SimpleTcpClient client;

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                client.Connect();
                btnSend.Enabled = true;
                btnConnect.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"Message",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (client.IsConnected)
            {
                if (!string.IsNullOrEmpty(txtMessage.Text))
                {
                    client.Send(txtMessage.Text);
                    txtInfo.Text += $"Me:{txtMessage.Text}{Environment.NewLine}";
                    txtMessage.Text = string.Empty;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
            client= new(txtIP.Text);
            client.Events.Connected += Events_Connected;
            client.Events.Disconnected += Events_Disconnected;
            client.Events.DataReceived += Events_DataReceived;
            btnSend.Enabled = false;
        }

        private void Events_Connected(object sender, ConnectionEventArgs e)
        {
            txtInfo.Text += $"Server  connected.{Environment.NewLine}";
        }

        private void Events_DataReceived(object  sender, DataReceivedEventArgs e)
        {
            txtInfo.Text += $"Server:{Encoding.UTF8.GetString(e.Data)}{Environment.NewLine}";
        }

        private void Events_Disconnected(object sender, ConnectionEventArgs e)
        {
            txtInfo.Text += $"Server disconnected.{Environment.NewLine}";
        }
    }
}