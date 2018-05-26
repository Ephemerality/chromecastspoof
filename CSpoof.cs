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
using System.Drawing;
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
        BindingList<Chromecast> Chromecasts = new BindingList<Chromecast>();

        public frmCSpoof()
        {
            InitializeComponent();
        }

        private void frmCSpoof_Load(object sender, EventArgs e)
        {
            txtName.Text = settings.name;
            txtIP.Text = settings.ip;
            var source = new BindingSource();
            source.DataSource = Chromecasts;
            dgvChromecasts.DataSource = source;
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
            {
                foreach (Chromecast c in Chromecasts)
                    spoofAllPrefixes(c);
            }
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
            {
                foreach (Chromecast c in Chromecasts)
                    spoofAllPrefixes(c);
            }
        }

        private bool checkIP(string ip)
        {
            return Regex.IsMatch(ip, ipPattern);
        }

        private void spoofAllPrefixes(Chromecast c)
        {
            for (int i = 0; i < prefixes.Length; i++)
            {
                spoof(c, prefixes[i]);
            }
        }

        private void spoof(Chromecast c, string prefix = "")
        {
            UdpClient client = new UdpClient();
            IPAddress mcaddr = IPAddress.Parse("224.0.0.251");
            client.JoinMulticastGroup(mcaddr);
            IPEndPoint remoteep = new IPEndPoint(mcaddr, 5353);
            byte[] buffer = c.GetBroadcastPacket(prefix);
            client.Send(buffer, buffer.Length, remoteep);
            client.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtName.Text == "")
                MessageBox.Show("Name cannot be blank.");
            else if (!checkIP(txtIP.Text))
                MessageBox.Show("Invalid IP.");
            else
                Chromecasts.Add(new Chromecast(txtName.Text, txtIP.Text, chkUltra.Checked));
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (dgvChromecasts.SelectedRows.Count > 0)
            {
                int i = dgvChromecasts.SelectedRows[0].Index;
                Chromecasts.RemoveAt(i);
                if (i == dgvChromecasts.Rows.Count) i--;
                if (i >= 0)
                    dgvChromecasts.Rows[i].Selected = true;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Chromecasts.Clear();
        }

        // http://www.inforbiro.com/blog-eng/c-sharp-datagridview-drag-and-drop-rows-reorder/
        #region Reorder Datagrid
        private Rectangle dragBoxFromMouseDown;
        private int rowIndexFromMouseDown;
        private int rowIndexOfItemUnderMouseToDrop;

        private void dgvChromecasts_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                // If the mouse moves outside the rectangle, start the drag.
                if (dragBoxFromMouseDown != Rectangle.Empty &&
                !dragBoxFromMouseDown.Contains(e.X, e.Y))
                {
                    // Proceed with the drag and drop, passing in the list item.                    
                    DragDropEffects dropEffect = dgvChromecasts.DoDragDrop(
                          dgvChromecasts.Rows[rowIndexFromMouseDown],
                          DragDropEffects.Move);
                }
            }
        }

        private void dgvChromecasts_MouseDown(object sender, MouseEventArgs e)
        {
            // Get the index of the item the mouse is below.
            rowIndexFromMouseDown = dgvChromecasts.HitTest(e.X, e.Y).RowIndex;

            if (rowIndexFromMouseDown != -1)
            {
                // Remember the point where the mouse down occurred. 
                // The DragSize indicates the size that the mouse can move 
                // before a drag event should be started.                
                Size dragSize = SystemInformation.DragSize;

                // Create a rectangle using the DragSize, with the mouse position being
                // at the center of the rectangle.
                dragBoxFromMouseDown = new Rectangle(
                          new Point(
                            e.X - (dragSize.Width / 2),
                            e.Y - (dragSize.Height / 2)),
                      dragSize);
            }
            else
                // Reset the rectangle if the mouse is not over an item in the ListBox.
                dragBoxFromMouseDown = Rectangle.Empty;
        }

        private void dgvChromecasts_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void dgvChromecasts_DragDrop(object sender, DragEventArgs e)
        {
            // The mouse locations are relative to the screen, so they must be 
            // converted to client coordinates.
            Point clientPoint = dgvChromecasts.PointToClient(new Point(e.X, e.Y));

            // Get the row index of the item the mouse is below. 
            rowIndexOfItemUnderMouseToDrop = dgvChromecasts.HitTest(clientPoint.X, clientPoint.Y).RowIndex;

            // If the drag operation was a move then remove and insert the row.
            if (e.Effect == DragDropEffects.Move)
            {
                Chromecast c = Chromecasts[rowIndexFromMouseDown];
                Chromecasts.Remove(c);
                if (rowIndexOfItemUnderMouseToDrop >= Chromecasts.Count)
                    Chromecasts.Add(c);
                else
                    Chromecasts.Insert(rowIndexOfItemUnderMouseToDrop, c);

            }
        }
        #endregion
    }
}
