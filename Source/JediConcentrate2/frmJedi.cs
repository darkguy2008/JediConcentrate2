using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace JediConcentrate2
{
    public partial class frmJedi : Form
    {
        public enum GWL
        {
            ExStyle = -20
        }

        public enum WS_EX
        {
            Transparent = 0x20,
            Layered = 0x80000
        }

        public enum LWA
        {
            ColorKey = 0x1,
            Alpha = 0x2
        }

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        public static extern int GetWindowLong(IntPtr hWnd, GWL nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        public static extern int SetWindowLong(IntPtr hWnd, GWL nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetLayeredWindowAttributes")]
        public static extern bool SetLayeredWindowAttributes(IntPtr hWnd, int crKey, byte alpha, LWA dwFlags);

        public frmJedi()
        {
            InitializeComponent();
            this.Shown += FrmJedi_Shown;
        }

        private void FrmJedi_Shown(object sender, EventArgs e)
        {
            Rectangle rect = new Rectangle(int.MaxValue, int.MaxValue, int.MinValue, int.MinValue);
            foreach (Screen screen in Screen.AllScreens)
                rect = Rectangle.Union(rect, screen.Bounds);
            Location = new Point(rect.Left, rect.Top);
            Size = new Size(rect.Width, rect.Height);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            int initialStyle = GetWindowLong(this.Handle, GWL.ExStyle);
            SetWindowLong(this.Handle, GWL.ExStyle, initialStyle | 0x80000 | 0x20);
        }
    }
}
