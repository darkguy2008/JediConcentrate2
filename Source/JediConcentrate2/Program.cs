using System;
using System.Windows.Forms;
using MovablePython;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;

namespace JediConcentrate2
{
    static class Program
    {
        static Form frm;
        static NotifyIcon icon;
        static Hotkey hk;

        [DllImport("user32.dll")]
        static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        static IntPtr _curWin = IntPtr.Zero;
        static IntPtr _intWin = IntPtr.Zero;
        static IntPtr _lastWin = IntPtr.Zero;

        static bool isRunning = true;
        static Thread thPoll;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            frm = new frmJedi();
            frm.Show();
            frm.Hide();

            ContextMenu menu = new ContextMenu();
            menu.MenuItems.Add(new MenuItem("About"));
            menu.MenuItems.Add(new MenuItem("Exit"));
            menu.MenuItems[0].Click += About_Click;
            menu.MenuItems[1].Click += Exit_Click;

            icon = new NotifyIcon() { Icon = Properties.Resources.yoda, Visible = true, ContextMenu = menu };

            hk = new Hotkey();
            hk.KeyCode = Keys.J;
            hk.Windows = true;
            hk.Pressed += delegate { ToggleConcentrate(); };

            if (hk.GetCanRegister(frm)) { hk.Register(frm); }

            thPoll = new Thread(new ThreadStart(Poll));
            thPoll.Start();

            Application.Run();
        }

        private static void Exit_Click(object sender, EventArgs e)
        {
            hk.Unregister();
            isRunning = false;
            Application.Exit();
        }

        private static void About_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/darkguy2008/JediConcentrate2");
        }

        private static void Poll()
        {
            _intWin = GetForegroundWindow();
            while(isRunning)
            {
                if(frm.InvokeRequired)
                {
                    frm.Invoke((MethodInvoker) delegate {
                        _curWin = GetForegroundWindow();
                        if (frm != null && frm.Visible)
                        {
                            if (_curWin == frm.Handle)
                                _curWin = _intWin;
                            _intWin = _curWin;
                            if (_lastWin != _curWin)
                            {
                                frm.Opacity = .70;
                                SetForegroundWindow(frm.Handle);
                                SetForegroundWindow(_curWin);
                            }
                        }

                        _lastWin = _curWin;
                    });
                }
                Thread.Sleep(10);
            }
        }

        private static void ToggleConcentrate()
        {
            if (frm != null && frm.Visible)
                UnConcentrate();
            else
                Concentrate();
        }

        private static void Concentrate()
        {
            _curWin = GetForegroundWindow();
            _lastWin = _curWin;
            if (frm.InvokeRequired)
            {
                frm.Invoke(new MethodInvoker(ConcentrateThread));
            }
            else
            {
                ConcentrateThread();
            }
        }

        public static void UnConcentrate()
        {
            _curWin = _lastWin;
            if (frm.InvokeRequired)
            {
                frm.Invoke(new MethodInvoker(UnConcentrateThread));
            }
            else
            {
                UnConcentrateThread();
            }
        }

        private static void ConcentrateThread()
        {
            frm.Opacity = 0;
            frm.Show();
            SetForegroundWindow(_curWin);
            while (frm.Opacity < .70)
            {
                Application.DoEvents();
                Thread.Sleep(5);
                frm.Opacity += .04;
            }
            _curWin = IntPtr.Zero;
        }

        private static void UnConcentrateThread()
        {
            frm.Opacity = .70;
            frm.Show();
            SetForegroundWindow(_curWin);
            while (frm.Opacity > 0)
            {
                Application.DoEvents();
                Thread.Sleep(5);
                frm.Opacity -= .04;
            }
            frm.Hide();
            _curWin = IntPtr.Zero;
        }

    }
}
