namespace Skcrwblr
{
    partial class SkcrwblrForm
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
                scrobbler.Dispose();
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
            this.timerGetTrack = new System.Windows.Forms.Timer(this.components);
            this.textBoxArtist = new System.Windows.Forms.TextBox();
            this.textBoxTitle = new System.Windows.Forms.TextBox();
            this.textBoxAlbum = new System.Windows.Forms.TextBox();
            this.buttonCorrectSpelling = new System.Windows.Forms.Button();
            this.buttonScrobble = new System.Windows.Forms.Button();
            this.checkBoxAutoScrobble = new System.Windows.Forms.CheckBox();
            this.checkBoxAutoCorrect = new System.Windows.Forms.CheckBox();
            this.buttonOriginalSpelling = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.buttonPause = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.buttonPlay = new System.Windows.Forms.Button();
            this.textBoxLog = new System.Windows.Forms.TextBox();
            this.contextMenuLog = new System.Windows.Forms.ContextMenu();
            this.menuItemClear = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.menuItemCopy = new System.Windows.Forms.MenuItem();
            this.menuItemSelectAll = new System.Windows.Forms.MenuItem();
            this.linkLabelArtist = new System.Windows.Forms.LinkLabel();
            this.linkLabelTitle = new System.Windows.Forms.LinkLabel();
            this.linkLabelAlbum = new System.Windows.Forms.LinkLabel();
            this.albumArt = new System.Windows.Forms.PictureBox();
            this.buttonLove = new System.Windows.Forms.Button();
            this.contextMenuAbout = new System.Windows.Forms.ContextMenu();
            this.menuItemAbout = new System.Windows.Forms.MenuItem();
            this.linkLabelLogin = new System.Windows.Forms.LinkLabel();
            this.contextMenuLastFm = new System.Windows.Forms.ContextMenu();
            this.menuItemLogin = new System.Windows.Forms.MenuItem();
            this.menuItemProfile = new System.Windows.Forms.MenuItem();
            this.menuItemLogout = new System.Windows.Forms.MenuItem();
            this.labelProgram = new System.Windows.Forms.Label();
            this.comboBoxStream = new System.Windows.Forms.ComboBox();
            this.labelAlbumArt = new System.Windows.Forms.Label();
            this.volumeControl = new Skcrwblr.VolumeControl();
            ((System.ComponentModel.ISupportInitialize)(this.albumArt)).BeginInit();
            this.SuspendLayout();
            // 
            // timerGetTrack
            // 
            this.timerGetTrack.Enabled = true;
            this.timerGetTrack.Interval = 15000;
            this.timerGetTrack.Tick += new System.EventHandler(this.timerGetTrack_Tick);
            // 
            // textBoxArtist
            // 
            this.textBoxArtist.Location = new System.Drawing.Point(57, 32);
            this.textBoxArtist.Name = "textBoxArtist";
            this.textBoxArtist.Size = new System.Drawing.Size(180, 20);
            this.textBoxArtist.TabIndex = 5;
            this.textBoxArtist.TextChanged += new System.EventHandler(this.textBoxArtist_TextChanged);
            this.textBoxArtist.Enter += new System.EventHandler(this.textBoxArtist_Enter);
            this.textBoxArtist.Leave += new System.EventHandler(this.textBoxArtist_Leave);
            // 
            // textBoxTitle
            // 
            this.textBoxTitle.Location = new System.Drawing.Point(57, 58);
            this.textBoxTitle.Name = "textBoxTitle";
            this.textBoxTitle.Size = new System.Drawing.Size(180, 20);
            this.textBoxTitle.TabIndex = 6;
            this.textBoxTitle.TextChanged += new System.EventHandler(this.textBoxTitle_TextChanged);
            this.textBoxTitle.Enter += new System.EventHandler(this.textBoxTitle_Enter);
            this.textBoxTitle.Leave += new System.EventHandler(this.textBoxTitle_Leave);
            // 
            // textBoxAlbum
            // 
            this.textBoxAlbum.Location = new System.Drawing.Point(57, 84);
            this.textBoxAlbum.Name = "textBoxAlbum";
            this.textBoxAlbum.Size = new System.Drawing.Size(180, 20);
            this.textBoxAlbum.TabIndex = 7;
            this.textBoxAlbum.TextChanged += new System.EventHandler(this.textBoxAlbum_TextChanged);
            this.textBoxAlbum.Enter += new System.EventHandler(this.textBoxAlbum_Enter);
            this.textBoxAlbum.Leave += new System.EventHandler(this.textBoxAlbum_Leave);
            // 
            // buttonCorrectSpelling
            // 
            this.buttonCorrectSpelling.Location = new System.Drawing.Point(12, 110);
            this.buttonCorrectSpelling.Name = "buttonCorrectSpelling";
            this.buttonCorrectSpelling.Size = new System.Drawing.Size(100, 23);
            this.buttonCorrectSpelling.TabIndex = 10;
            this.buttonCorrectSpelling.Text = "Correct spelling";
            this.buttonCorrectSpelling.UseVisualStyleBackColor = true;
            this.buttonCorrectSpelling.Click += new System.EventHandler(this.buttonCorrectSpelling_Click);
            // 
            // buttonScrobble
            // 
            this.buttonScrobble.Enabled = false;
            this.buttonScrobble.Location = new System.Drawing.Point(12, 139);
            this.buttonScrobble.Name = "buttonScrobble";
            this.buttonScrobble.Size = new System.Drawing.Size(100, 23);
            this.buttonScrobble.TabIndex = 13;
            this.buttonScrobble.Text = "Scrobble now";
            this.toolTip.SetToolTip(this.buttonScrobble, "Immediately scrobble the currently playing song.");
            this.buttonScrobble.UseVisualStyleBackColor = true;
            this.buttonScrobble.Click += new System.EventHandler(this.buttonScrobble_Click);
            // 
            // checkBoxAutoScrobble
            // 
            this.checkBoxAutoScrobble.AutoSize = true;
            this.checkBoxAutoScrobble.Location = new System.Drawing.Point(224, 143);
            this.checkBoxAutoScrobble.Name = "checkBoxAutoScrobble";
            this.checkBoxAutoScrobble.Size = new System.Drawing.Size(88, 17);
            this.checkBoxAutoScrobble.TabIndex = 15;
            this.checkBoxAutoScrobble.Text = "Autoscrobble";
            this.toolTip.SetToolTip(this.checkBoxAutoScrobble, "Automatically scrobbles the previously played song when a new song begins.");
            this.checkBoxAutoScrobble.UseVisualStyleBackColor = true;
            // 
            // checkBoxAutoCorrect
            // 
            this.checkBoxAutoCorrect.AutoSize = true;
            this.checkBoxAutoCorrect.Location = new System.Drawing.Point(224, 114);
            this.checkBoxAutoCorrect.Name = "checkBoxAutoCorrect";
            this.checkBoxAutoCorrect.Size = new System.Drawing.Size(81, 17);
            this.checkBoxAutoCorrect.TabIndex = 12;
            this.checkBoxAutoCorrect.Text = "Autocorrect";
            this.toolTip.SetToolTip(this.checkBoxAutoCorrect, "Automatically corrects the supplied metadata to the canonical data on Last.fm. No" +
        "te that this is not necessary if spelling correction is enabled in your Last.fm " +
        "settings.");
            this.checkBoxAutoCorrect.UseVisualStyleBackColor = true;
            this.checkBoxAutoCorrect.CheckedChanged += new System.EventHandler(this.checkBoxAutoCorrect_CheckedChanged);
            // 
            // buttonOriginalSpelling
            // 
            this.buttonOriginalSpelling.Location = new System.Drawing.Point(118, 110);
            this.buttonOriginalSpelling.Name = "buttonOriginalSpelling";
            this.buttonOriginalSpelling.Size = new System.Drawing.Size(100, 23);
            this.buttonOriginalSpelling.TabIndex = 11;
            this.buttonOriginalSpelling.Text = "Original spelling";
            this.buttonOriginalSpelling.UseVisualStyleBackColor = true;
            this.buttonOriginalSpelling.Click += new System.EventHandler(this.buttonOriginalSpelling_Click);
            // 
            // buttonPause
            // 
            this.buttonPause.Enabled = false;
            this.buttonPause.Image = global::Skcrwblr.Properties.Resources.Pause;
            this.buttonPause.Location = new System.Drawing.Point(166, 224);
            this.buttonPause.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.buttonPause.Name = "buttonPause";
            this.buttonPause.Size = new System.Drawing.Size(24, 24);
            this.buttonPause.TabIndex = 19;
            this.toolTip.SetToolTip(this.buttonPause, "Pause");
            this.buttonPause.UseVisualStyleBackColor = true;
            this.buttonPause.Click += new System.EventHandler(this.buttonPause_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Enabled = false;
            this.buttonStop.Image = global::Skcrwblr.Properties.Resources.Stop;
            this.buttonStop.Location = new System.Drawing.Point(193, 224);
            this.buttonStop.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(24, 24);
            this.buttonStop.TabIndex = 20;
            this.toolTip.SetToolTip(this.buttonStop, "Stop");
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // buttonPlay
            // 
            this.buttonPlay.Image = global::Skcrwblr.Properties.Resources.Play;
            this.buttonPlay.Location = new System.Drawing.Point(139, 224);
            this.buttonPlay.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.buttonPlay.Name = "buttonPlay";
            this.buttonPlay.Size = new System.Drawing.Size(24, 24);
            this.buttonPlay.TabIndex = 18;
            this.toolTip.SetToolTip(this.buttonPlay, "Play");
            this.buttonPlay.UseVisualStyleBackColor = true;
            this.buttonPlay.Click += new System.EventHandler(this.buttonPlay_Click);
            // 
            // textBoxLog
            // 
            this.textBoxLog.ContextMenu = this.contextMenuLog;
            this.textBoxLog.Location = new System.Drawing.Point(12, 168);
            this.textBoxLog.Multiline = true;
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.ReadOnly = true;
            this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxLog.Size = new System.Drawing.Size(303, 50);
            this.textBoxLog.TabIndex = 16;
            // 
            // contextMenuLog
            // 
            this.contextMenuLog.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemClear,
            this.menuItem4,
            this.menuItemCopy,
            this.menuItemSelectAll});
            this.contextMenuLog.Popup += new System.EventHandler(this.contextMenuLog_Popup);
            // 
            // menuItemClear
            // 
            this.menuItemClear.Index = 0;
            this.menuItemClear.Text = "Clear";
            this.menuItemClear.Click += new System.EventHandler(this.menuItemClear_Click);
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 1;
            this.menuItem4.Text = "-";
            // 
            // menuItemCopy
            // 
            this.menuItemCopy.Index = 2;
            this.menuItemCopy.Text = "Copy";
            this.menuItemCopy.Click += new System.EventHandler(this.menuItemCopy_Click);
            // 
            // menuItemSelectAll
            // 
            this.menuItemSelectAll.Index = 3;
            this.menuItemSelectAll.Text = "Select All";
            this.menuItemSelectAll.Click += new System.EventHandler(this.menuItemSelectAll_Click);
            // 
            // linkLabelArtist
            // 
            this.linkLabelArtist.AutoSize = true;
            this.linkLabelArtist.Location = new System.Drawing.Point(12, 35);
            this.linkLabelArtist.Name = "linkLabelArtist";
            this.linkLabelArtist.Size = new System.Drawing.Size(33, 13);
            this.linkLabelArtist.TabIndex = 2;
            this.linkLabelArtist.TabStop = true;
            this.linkLabelArtist.Text = "Artist:";
            this.linkLabelArtist.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelArtist_LinkClicked);
            // 
            // linkLabelTitle
            // 
            this.linkLabelTitle.AutoSize = true;
            this.linkLabelTitle.Location = new System.Drawing.Point(12, 61);
            this.linkLabelTitle.Name = "linkLabelTitle";
            this.linkLabelTitle.Size = new System.Drawing.Size(30, 13);
            this.linkLabelTitle.TabIndex = 3;
            this.linkLabelTitle.TabStop = true;
            this.linkLabelTitle.Text = "Title:";
            this.linkLabelTitle.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelTitle_LinkClicked);
            // 
            // linkLabelAlbum
            // 
            this.linkLabelAlbum.AutoSize = true;
            this.linkLabelAlbum.Location = new System.Drawing.Point(12, 87);
            this.linkLabelAlbum.Name = "linkLabelAlbum";
            this.linkLabelAlbum.Size = new System.Drawing.Size(39, 13);
            this.linkLabelAlbum.TabIndex = 4;
            this.linkLabelAlbum.TabStop = true;
            this.linkLabelAlbum.Text = "Album:";
            this.linkLabelAlbum.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelAlbum_LinkClicked);
            // 
            // albumArt
            // 
            this.albumArt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.albumArt.Location = new System.Drawing.Point(243, 32);
            this.albumArt.Name = "albumArt";
            this.albumArt.Size = new System.Drawing.Size(72, 72);
            this.albumArt.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.albumArt.TabIndex = 18;
            this.albumArt.TabStop = false;
            // 
            // buttonLove
            // 
            this.buttonLove.Location = new System.Drawing.Point(118, 139);
            this.buttonLove.Name = "buttonLove";
            this.buttonLove.Size = new System.Drawing.Size(100, 23);
            this.buttonLove.TabIndex = 14;
            this.buttonLove.Text = "Love track";
            this.buttonLove.UseVisualStyleBackColor = true;
            this.buttonLove.Click += new System.EventHandler(this.buttonLove_Click);
            // 
            // contextMenuAbout
            // 
            this.contextMenuAbout.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemAbout});
            // 
            // menuItemAbout
            // 
            this.menuItemAbout.Index = 0;
            this.menuItemAbout.Text = "About";
            this.menuItemAbout.Click += new System.EventHandler(this.menuItemAbout_Click);
            // 
            // linkLabelLogin
            // 
            this.linkLabelLogin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabelLogin.ContextMenu = this.contextMenuLastFm;
            this.linkLabelLogin.Location = new System.Drawing.Point(194, 9);
            this.linkLabelLogin.Name = "linkLabelLogin";
            this.linkLabelLogin.Size = new System.Drawing.Size(121, 13);
            this.linkLabelLogin.TabIndex = 1;
            this.linkLabelLogin.TabStop = true;
            this.linkLabelLogin.Text = "Login to Last.fm";
            this.linkLabelLogin.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.linkLabelLogin.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelLogin_LinkClicked);
            // 
            // contextMenuLastFm
            // 
            this.contextMenuLastFm.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemLogin,
            this.menuItemProfile,
            this.menuItemLogout});
            // 
            // menuItemLogin
            // 
            this.menuItemLogin.Index = 0;
            this.menuItemLogin.Text = "Login";
            this.menuItemLogin.Click += new System.EventHandler(this.menuItemLogin_Click);
            // 
            // menuItemProfile
            // 
            this.menuItemProfile.Index = 1;
            this.menuItemProfile.Text = "Profile";
            this.menuItemProfile.Visible = false;
            this.menuItemProfile.Click += new System.EventHandler(this.menuItemProfile_Click);
            // 
            // menuItemLogout
            // 
            this.menuItemLogout.Index = 2;
            this.menuItemLogout.Text = "Logout";
            this.menuItemLogout.Visible = false;
            this.menuItemLogout.Click += new System.EventHandler(this.menuItemLogout_Click);
            // 
            // labelProgram
            // 
            this.labelProgram.AutoSize = true;
            this.labelProgram.Location = new System.Drawing.Point(12, 9);
            this.labelProgram.Name = "labelProgram";
            this.labelProgram.Size = new System.Drawing.Size(52, 13);
            this.labelProgram.TabIndex = 0;
            this.labelProgram.Text = "Program: ";
            // 
            // comboBoxStream
            // 
            this.comboBoxStream.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxStream.FormattingEnabled = true;
            this.comboBoxStream.Location = new System.Drawing.Point(12, 225);
            this.comboBoxStream.Name = "comboBoxStream";
            this.comboBoxStream.Size = new System.Drawing.Size(121, 21);
            this.comboBoxStream.TabIndex = 17;
            this.comboBoxStream.SelectedIndexChanged += new System.EventHandler(this.comboBoxStream_SelectedIndexChanged);
            // 
            // labelAlbumArt
            // 
            this.labelAlbumArt.BackColor = System.Drawing.Color.Transparent;
            this.labelAlbumArt.Location = new System.Drawing.Point(244, 33);
            this.labelAlbumArt.Name = "labelAlbumArt";
            this.labelAlbumArt.Size = new System.Drawing.Size(70, 70);
            this.labelAlbumArt.TabIndex = 9;
            this.labelAlbumArt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // volumeControl
            // 
            this.volumeControl.Location = new System.Drawing.Point(220, 225);
            this.volumeControl.Name = "volumeControl";
            this.volumeControl.Size = new System.Drawing.Size(95, 22);
            this.volumeControl.TabIndex = 21;
            this.volumeControl.VolumeAsPercent = 100;
            this.volumeControl.VolumeChanged += new System.EventHandler(this.volumeControl_VolumeChanged);
            // 
            // SkcrwblrForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(327, 260);
            this.ContextMenu = this.contextMenuAbout;
            this.Controls.Add(this.labelProgram);
            this.Controls.Add(this.linkLabelLogin);
            this.Controls.Add(this.linkLabelArtist);
            this.Controls.Add(this.textBoxArtist);
            this.Controls.Add(this.linkLabelTitle);
            this.Controls.Add(this.textBoxTitle);
            this.Controls.Add(this.linkLabelAlbum);
            this.Controls.Add(this.textBoxAlbum);
            this.Controls.Add(this.albumArt);
            this.Controls.Add(this.labelAlbumArt);
            this.Controls.Add(this.buttonCorrectSpelling);
            this.Controls.Add(this.buttonOriginalSpelling);
            this.Controls.Add(this.checkBoxAutoCorrect);
            this.Controls.Add(this.buttonScrobble);
            this.Controls.Add(this.buttonLove);
            this.Controls.Add(this.checkBoxAutoScrobble);
            this.Controls.Add(this.textBoxLog);
            this.Controls.Add(this.comboBoxStream);
            this.Controls.Add(this.buttonPlay);
            this.Controls.Add(this.buttonPause);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.volumeControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "SkcrwblrForm";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "skcrwblr";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Skcrwblr_FormClosing);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SkcrwblrForm_MouseDown);
            ((System.ComponentModel.ISupportInitialize)(this.albumArt)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Timer timerGetTrack;
        private System.Windows.Forms.TextBox textBoxArtist;
        private System.Windows.Forms.TextBox textBoxTitle;
        private System.Windows.Forms.TextBox textBoxAlbum;
        private System.Windows.Forms.Button buttonCorrectSpelling;
        private System.Windows.Forms.Button buttonScrobble;
        private System.Windows.Forms.CheckBox checkBoxAutoScrobble;
        private System.Windows.Forms.CheckBox checkBoxAutoCorrect;
        private System.Windows.Forms.Button buttonOriginalSpelling;
        private System.Windows.Forms.PictureBox albumArt;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.TextBox textBoxLog;
        private System.Windows.Forms.LinkLabel linkLabelArtist;
        private System.Windows.Forms.LinkLabel linkLabelTitle;
        private System.Windows.Forms.LinkLabel linkLabelAlbum;
        private System.Windows.Forms.Button buttonPlay;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Button buttonPause;
        private VolumeControl volumeControl;
        private System.Windows.Forms.Button buttonLove;
        private System.Windows.Forms.ContextMenu contextMenuAbout;
        private System.Windows.Forms.MenuItem menuItemAbout;
        private System.Windows.Forms.LinkLabel linkLabelLogin;
        private System.Windows.Forms.ContextMenu contextMenuLastFm;
        private System.Windows.Forms.MenuItem menuItemProfile;
        private System.Windows.Forms.MenuItem menuItemLogout;
        private System.Windows.Forms.MenuItem menuItemLogin;
        private System.Windows.Forms.Label labelProgram;
        private System.Windows.Forms.ComboBox comboBoxStream;
        private System.Windows.Forms.Label labelAlbumArt;
        private System.Windows.Forms.ContextMenu contextMenuLog;
        private System.Windows.Forms.MenuItem menuItemClear;
        private System.Windows.Forms.MenuItem menuItem4;
        private System.Windows.Forms.MenuItem menuItemCopy;
        private System.Windows.Forms.MenuItem menuItemSelectAll;
    }
}

