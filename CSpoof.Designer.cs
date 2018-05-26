namespace ChromecastSpoof
{
    partial class frmCSpoof
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.sendOnce = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtIP = new System.Windows.Forms.TextBox();
            this.sendContinuous = new System.Windows.Forms.Button();
            this.tmrSend = new System.Windows.Forms.Timer(this.components);
            this.dgvChromecasts = new System.Windows.Forms.DataGridView();
            this.chkUltra = new System.Windows.Forms.CheckBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvChromecasts)).BeginInit();
            this.SuspendLayout();
            // 
            // sendOnce
            // 
            this.sendOnce.Location = new System.Drawing.Point(29, 94);
            this.sendOnce.Name = "sendOnce";
            this.sendOnce.Size = new System.Drawing.Size(75, 23);
            this.sendOnce.TabIndex = 2;
            this.sendOnce.Text = "Send Once";
            this.sendOnce.UseVisualStyleBackColor = true;
            this.sendOnce.Click += new System.EventHandler(this.sendOnce_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Name:";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(56, 6);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(129, 20);
            this.txtName.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(30, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(20, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "IP:";
            // 
            // txtIP
            // 
            this.txtIP.Location = new System.Drawing.Point(56, 34);
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new System.Drawing.Size(129, 20);
            this.txtIP.TabIndex = 1;
            this.txtIP.Text = "192.168.2.110";
            // 
            // sendContinuous
            // 
            this.sendContinuous.Location = new System.Drawing.Point(124, 94);
            this.sendContinuous.Name = "sendContinuous";
            this.sendContinuous.Size = new System.Drawing.Size(75, 23);
            this.sendContinuous.TabIndex = 3;
            this.sendContinuous.Text = "Continuous";
            this.sendContinuous.UseVisualStyleBackColor = true;
            this.sendContinuous.Click += new System.EventHandler(this.sendContinuous_Click);
            // 
            // tmrSend
            // 
            this.tmrSend.Interval = 2500;
            this.tmrSend.Tick += new System.EventHandler(this.tmrSend_Tick);
            // 
            // dgvChromecasts
            // 
            this.dgvChromecasts.AllowDrop = true;
            this.dgvChromecasts.AllowUserToOrderColumns = true;
            this.dgvChromecasts.AllowUserToResizeColumns = false;
            this.dgvChromecasts.AllowUserToResizeRows = false;
            this.dgvChromecasts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvChromecasts.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnF2;
            this.dgvChromecasts.Location = new System.Drawing.Point(217, 6);
            this.dgvChromecasts.Name = "dgvChromecasts";
            this.dgvChromecasts.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvChromecasts.Size = new System.Drawing.Size(346, 256);
            this.dgvChromecasts.TabIndex = 5;
            this.dgvChromecasts.DragDrop += new System.Windows.Forms.DragEventHandler(this.dgvChromecasts_DragDrop);
            this.dgvChromecasts.DragOver += new System.Windows.Forms.DragEventHandler(this.dgvChromecasts_DragOver);
            this.dgvChromecasts.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dgvChromecasts_MouseDown);
            this.dgvChromecasts.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dgvChromecasts_MouseMove);
            // 
            // chkUltra
            // 
            this.chkUltra.AutoSize = true;
            this.chkUltra.Location = new System.Drawing.Point(56, 60);
            this.chkUltra.Name = "chkUltra";
            this.chkUltra.Size = new System.Drawing.Size(48, 17);
            this.chkUltra.TabIndex = 6;
            this.chkUltra.Text = "Ultra";
            this.chkUltra.UseVisualStyleBackColor = true;
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(189, 6);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(22, 22);
            this.btnAdd.TabIndex = 7;
            this.btnAdd.Text = "+";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(189, 34);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(22, 22);
            this.btnRemove.TabIndex = 8;
            this.btnRemove.Text = "-";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(189, 62);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(22, 22);
            this.btnClear.TabIndex = 9;
            this.btnClear.Text = "C";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // frmCSpoof
            // 
            this.AcceptButton = this.sendOnce;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(575, 270);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.chkUltra);
            this.Controls.Add(this.dgvChromecasts);
            this.Controls.Add(this.sendContinuous);
            this.Controls.Add(this.txtIP);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.sendOnce);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmCSpoof";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "CSpoof";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmCSpoof_FormClosing);
            this.Load += new System.EventHandler(this.frmCSpoof_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvChromecasts)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button sendOnce;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.Button sendContinuous;
        private System.Windows.Forms.Timer tmrSend;
        private System.Windows.Forms.DataGridView dgvChromecasts;
        private System.Windows.Forms.CheckBox chkUltra;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnClear;
    }
}

