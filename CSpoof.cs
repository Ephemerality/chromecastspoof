/*
 * Copyright 2015 Nick Niemi
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ChromecastSpoof
{
    public partial class frmCSpoof : Form
    {
        // List of service prefixes
        private string[] prefixes = { "_233637DE", "_096084372", "_5FD0CDC9", "_70CF0F1E", "_CC1AD845", "_9AC194DC", "" };
        // From https://www.safaribooksonline.com/library/view/regular-expressions-cookbook/9780596802837/ch07s16.html
        private string ipPattern = @"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
        private Properties.Settings settings = ChromecastSpoof.Properties.Settings.Default;
        static Random random = new Random();

        public frmCSpoof()
        {
            InitializeComponent();
        }

        private void frmCSpoof_Load(object sender, EventArgs e)
        {
            txtName.Text = settings.name;
            txtIP.Text = settings.ip;
        }

        private void frmCSpoof_FormClosing(object sender, FormClosingEventArgs e)
        {
            settings.name = txtName.Text;
            settings.ip = txtIP.Text;
            settings.Save();
        }

        private void sendContinuous_Click(object sender, EventArgs e)
        {
            toggleContinuous();
        }

        private void toggleContinuous()
        {
            if (tmrSend.Enabled)
            {
                tmrSend.Enabled = false;
                sendOnce.Enabled = true;
                sendContinuous.Text = "Continuous";
            }
            else
            {
                tmrSend.Enabled = true;
                sendOnce.Enabled = false;
                sendContinuous.Text = "Stop";
            }
        }

        private void sendOnce_Click(object sender, EventArgs e)
        {
            if (txtName.Text == "")
                MessageBox.Show("Name cannot be blank.", "CSpoof");
            else if (!checkIP(txtIP.Text))
                MessageBox.Show("Invalid IP.", "CSpoof");
            else
                spoofAllPrefixes(txtName.Text, txtIP.Text);
        }

        private void tmrSend_Tick(object sender, EventArgs e)
        {
            if (txtName.Text == "")
            {
                toggleContinuous();
                MessageBox.Show("Name cannot be blank.", "CSpoof");
            }
            else if (!checkIP(txtIP.Text))
            {
                toggleContinuous();
                MessageBox.Show("Invalid IP.", "CSpoof");
            }
            else
                spoofAllPrefixes(txtName.Text, txtIP.Text);
                //spoof(txtName.Text, txtIP.Text);
        }

        private bool checkIP(string ip)
        {
            return Regex.IsMatch(ip, ipPattern);
        }

        private void spoofAllPrefixes(string ccName, string ip)
        {
            for (int i = 0; i < prefixes.Length; i++)
            {
                spoof(ccName, ip, prefixes[i]);
            }
        }

        private void spoof(string ccName, string ip, string prefix = "")
        {
            UdpClient client = new UdpClient();
            IPAddress mcaddr = IPAddress.Parse("224.0.0.251");
            client.JoinMulticastGroup(mcaddr);
            IPEndPoint remoteep = new IPEndPoint(mcaddr, 5353);
            
            string test = BitConverter.ToString(buffer);
            test = test.Replace("-", "");
            client.Send(buffer, buffer.Length, remoteep);
            client.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            spoof(txtName.Text, txtIP.Text);
        }
    }
}
