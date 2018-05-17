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
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MiscUtil.IO;

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
            // Generate a random id based on the name
            MD5 md5 = MD5.Create();
            byte[] hashBytes = md5.ComputeHash(Encoding.ASCII.GetBytes(ccName));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
                sb.Append(hashBytes[i].ToString("x2"));
            string hash1 = sb.ToString();
            string hostname = "Chromecast-Ultra-" + hash1;
            string hostnamesplit = Regex.Replace(hash1, "^(.{8})(.{4})(.{4})(.{4})(.{12})$", "$1-$2-$3-$4-$5");

            UdpClient client = new UdpClient();
            IPAddress mcaddr = IPAddress.Parse("224.0.0.251");
            client.JoinMulticastGroup(mcaddr);
            IPEndPoint remoteep = new IPEndPoint(mcaddr, 5353);
            MemoryStream stream = new MemoryStream();
            EndianBinaryWriter write = new EndianBinaryWriter(MiscUtil.Conversion.EndianBitConverter.Big, stream);
            write.Write((ushort)0);
            write.Write((ushort)0x8400); //flags
            write.Write((ushort)0); //questions
            write.Write((ushort)1); //answer RRs
            write.Write((ushort)0); //authority RRs
            write.Write((ushort)3); //additional answer RRs
            // Write answer record
            if (prefix != "")
                writeOctets(new string[] { prefix, "_sub", "_googlecast", "_tcp", "local" }, write);
            else
                writeOctets(new string[] { "_googlecast", "_tcp", "local" }, write);
            write.Write((ushort)12); //type (PTR, domain name PoinTeR)
            write.Write((ushort)1); //class
            write.Write((uint)120); //ttl
            writeOctets(new string[] { hostname, "_googlecast", "_tcp", "local" }, write, true);

            // Additional records
            // Write TXT record
            writeOctets(new string[] { hostname, "_googlecast", "_tcp", "local" }, write);
            write.Write((ushort)16); //type (TXT)
            write.Write((ushort)0x8001); //class
            write.Write((uint)4500); //ttl
            string[] txtRecs = { "id=" + hash1,
                "cd=",
                "rm=",
                "ve=05",
                "md=Chromecast Ultra",
                "ic=/setup/icon.png",
                "fn=" + ccName,
                "ca=4101",
                "st=0",
                "bs=",
                "nf=1",
                "rs="};
            writeTXT(txtRecs, write);

            // Write SRV record
            writeOctets(new string[] { hostname, "_googlecast", "_tcp", "local" }, write);
            write.Write((ushort)33); //type (SRV)
            write.Write((ushort)0x8001); //class
            write.Write((uint)120); //ttl
            MemoryStream tempstr = new MemoryStream();
            EndianBinaryWriter tempwrite = new EndianBinaryWriter(MiscUtil.Conversion.EndianBitConverter.Big, tempstr);
            tempwrite.Write((ushort)0); //priority
            tempwrite.Write((ushort)0); //weight
            tempwrite.Write((ushort)8009); //port
            writeOctets(new string[] { hostnamesplit, "local" }, tempwrite);
            byte[] buffer = tempstr.ToArray();
            write.Write((ushort)buffer.Length);
            write.Write(buffer);

            // Write A record
            writeOctets(new string[] { hostnamesplit, "local" }, write);
            write.Write((ushort)1); //type (A)
            write.Write((ushort)0x8001); //class
            write.Write((uint)120); //ttl
            write.Write((ushort)4); //length
            write.Write(ip.Split('.').Select<string, byte>(i => byte.Parse(i)).ToArray());
            buffer = stream.ToArray();
            string test = BitConverter.ToString(buffer);
            test = test.Replace("-", "");
            client.Send(buffer, buffer.Length, remoteep);
            client.Close();
        }

        private void writeOctets(string[] octets, EndianBinaryWriter write, bool writeLen = false)
        {
            if (writeLen)
            {
                int count = octets.Select(i => i.Length).Sum() + octets.Length + 1;
                write.Write((ushort)count);
            }
            for (int i = 0; i < octets.Length; i++)
            {
                write.Write((byte)octets[i].Length);
                write.Write(Encoding.ASCII.GetBytes(octets[i]));
            }
            write.Write((byte)0);
        }

        private void writeTXT(string[] text, EndianBinaryWriter write)
        {
            MemoryStream tempstr = new MemoryStream();
            EndianBinaryWriter tempwrite = new EndianBinaryWriter(MiscUtil.Conversion.EndianBitConverter.Big, tempstr);
            for (int i = 0; i < text.Length; i++)
            {
                tempwrite.Write(text[i]);
            }
            byte[] buffer = tempstr.ToArray();
            write.Write((ushort)buffer.Length);
            write.Write(buffer);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            spoof(txtName.Text, txtIP.Text);
        }
    }
}
