// Code adapted from NAudio source code. Used under the Microsoft Public License.
// https://naudio.codeplex.com/SourceControl/latest#NAudio/Gui/VolumeSlider.cs

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Skcrwblr
{
    public partial class VolumeControl : UserControl
    {
        private float volume = 1.0f;
        private float MinDb = -48;
        /// <summary>
        /// Volume changed event
        /// </summary>
        public event EventHandler VolumeChanged;

        public VolumeControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            float db = 20 * (float)Math.Log10(Volume);
            float percent = 1 - (db / MinDb);
            if (percent < 0) percent = 0;

            Point bottomLeft = new Point(5, Height - 3);
            Point bottomRight = new Point(Width - 6, Height - 3);
            Point topRight = new Point(Width - 6, 4);

            // triangle
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            e.Graphics.DrawLine(SystemPens.ControlLightLight, bottomLeft, bottomRight);
            e.Graphics.DrawLine(SystemPens.ControlLightLight, bottomRight, topRight);
            e.Graphics.DrawLine(SystemPens.ControlDark, bottomLeft, topRight);

            // slider
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
            e.Graphics.DrawRectangle(SystemPens.ButtonShadow, (int)((this.Width - 11) * percent), 0, 10, Height - 1);
            e.Graphics.FillRectangle(SystemBrushes.ButtonFace, (int)((this.Width - 11) * percent) + 1, 1, 9, Height - 2);

            base.OnPaint(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                SetVolumeFromMouse(e.X);
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            SetVolumeFromMouse(e.X);
            base.OnMouseDown(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            float db = 20 * (float)Math.Log10(Volume);

            if (e.Delta < 0)
            {
                db -= 1f;
            }
            else
            {
                db += 1f;
            }

            Volume = (float)Math.Pow(10, db / 20);
        }

        private void SetVolumeFromMouse(int x)
        {
            // linear Volume = (float) x / this.Width;
            float dbVolume = (1 - (float)(x - 5) / (Width - 11)) * MinDb;
            if (dbVolume < MinDb)
                dbVolume = MinDb;
            if (x <= 4)
                Volume = 0;
            else
                Volume = (float)Math.Pow(10, dbVolume / 20);
        }

        /// <summary>
        /// The volume for this control
        /// </summary>
        [DefaultValue(1.0f)]
        public float Volume
        {
            get
            {
                return volume;
            }
            set
            {
                if (value < 0.0f)
                    value = 0.0f;
                if (value > 1.0f)
                    value = 1.0f;
                if (volume != value)
                {
                    volume = value;
                    if (VolumeChanged != null)
                        VolumeChanged(this, EventArgs.Empty);
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// The volume for this control, expressed as an integer percentage.
        /// </summary>
        public int VolumeAsPercent
        {
            get
            {
                return (int)(100 * (1 - (20 * (float)Math.Log10(Volume) / MinDb)));
            }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 100)
                    value = 100;
                Volume = (float)Math.Pow(10, MinDb * (1 - ((float)value / 100)) / 20);
            }
        }
    }
}
