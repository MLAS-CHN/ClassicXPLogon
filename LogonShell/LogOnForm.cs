using FlaUI.UIA3;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace LogonShell
{
    public partial class LogOnForm : Form
    {
        [DllImport("advapi32.dll", SetLastError = true, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool LogonUser([MarshalAs(UnmanagedType.LPStr)] string pszUserName, [MarshalAs(UnmanagedType.LPStr)] string pszDomain, [MarshalAs(UnmanagedType.LPStr)] string pszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);
        [DllImport("User32.dll")]
        static extern int SetForegroundWindow(IntPtr point);

        class UserInfo
        {
            public string Name, SID;
            public Image Avatar;
            public float Opacity = 1f, Target = 1f;
            public long AnimStart;
        }

        List<UserInfo> _users = new List<UserInfo>();
        int _selectedIndex = -1, _hoverIndex = -1;
        bool _isLoggingIn;
        string _lastUser;
        Bitmap _bgCache;

        Image _imgUp, _imgDown, _imgBg, _imgLine, _imgLogo, _imgSelected;
        Image _imgCh, _imgGo, _imgIns, _imgDefaultUser;
        Font _fontName = new Font("\u5B8B\u4F53", 14f);
        Font _fontSmall = new Font("\u5B8B\u4F53", 9f);

        TextBox _passwordBox;
        Button _goBtn, _closeButton;
        PictureBox _chPic, _insPic;
        Timer _animTimer;
        BackgroundWorker _closeWorker;

        int CenterX => ClientSize.Width / 2;
        int BgH => ClientSize.Height - 80 - 96;
        int UserTop => 80 + (BgH - _users.Count * 80) / 2;

        public LogOnForm()
        {
            InitializeComponent();
            BackColor = Color.FromArgb(58, 110, 165);
            LoadImages();
            EnumerateUsers();
            _lastUser = GetLastLoggedOnUser();
            CreateControls();
            RenderBgCache();

            if (Config.Args.Length > 0 && Config.Args[0] == "-startup" && File.Exists(Environment.CurrentDirectory + "\\updates"))
            {
                File.Delete(Environment.CurrentDirectory + "\\updates");
                System.Threading.Thread.Sleep(250);
                Environment.Exit(-1);
            }
            if (Config.Args.Length > 0 && Config.Args[0] == "-shutdown")
                _isLoggingIn = true;

            Program.sw.Stop();
        }

        void LoadImages()
        {
            string dir = Environment.CurrentDirectory + "\\resources\\";
            _imgUp = LoadImg(dir + "up.png");
            _imgDown = LoadImg(dir + "down.png");
            _imgBg = LoadImg(dir + "bg.png");
            _imgLine = LoadImg(dir + "line.png");
            _imgLogo = LoadImg(dir + "windows_logo.png");
            _imgSelected = LoadImg(dir + "selected.png");
            _imgCh = LoadImg(dir + "ch.png");
            _imgGo = LoadImg(dir + "go.png");
            _imgIns = LoadImg(dir + "ins.png");
            _imgDefaultUser = LoadImg(dir + "default_user.png");
        }

        Image LoadImg(string path)
        {
            if (File.Exists(path))
                return Image.FromStream(new MemoryStream(File.ReadAllBytes(path)));
            return new Bitmap(1, 1);
        }

        void EnumerateUsers()
        {
            var exclude = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                { "Administrator", "Guest", "DefaultAccount", "WDAGUtilityAccount" };
            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_UserAccount WHERE LocalAccount=True AND Disabled=False");
            foreach (ManagementObject o in searcher.Get())
            {
                string name = (string)o["Name"];
                string sid = (string)o["SID"];
                if (exclude.Contains(name) || name.EndsWith("$")) continue;
                _users.Add(new UserInfo { Name = name, SID = sid, Avatar = LoadAvatar(sid) });
            }
            _users.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));
        }

        Image LoadAvatar(string sid)
        {
            try
            {
                string regPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\AccountPicture\Users\" + sid;
                using (var key = Registry.LocalMachine.OpenSubKey(regPath))
                {
                    if (key != null)
                    {
                        foreach (string val in new[] { "Image128", "Image96", "Image48" })
                        {
                            string path = key.GetValue(val) as string;
                            if (path != null && File.Exists(path))
                                return Image.FromStream(new MemoryStream(File.ReadAllBytes(path)));
                        }
                    }
                }
            }
            catch { }
            return _imgDefaultUser;
        }

        void CreateControls()
        {
            _passwordBox = new TextBox
            {
                Font = _fontName, Size = new Size(161, 27),
                UseSystemPasswordChar = true, Visible = false
            };
            _passwordBox.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; DoLogin(); } };
            Controls.Add(_passwordBox);

            _goBtn = new Button
            {
                Size = new Size(24, 24), FlatStyle = FlatStyle.Flat,
                BackgroundImage = _imgGo, BackgroundImageLayout = ImageLayout.None,
                Visible = false, Cursor = Cursors.Hand
            };
            _goBtn.FlatAppearance.BorderSize = 0;
            _goBtn.Click += (s, e) => DoLogin();
            Controls.Add(_goBtn);

            _chPic = new PictureBox { Size = new Size(16, 16), Image = _imgCh, SizeMode = PictureBoxSizeMode.Normal, Visible = false };
            _insPic = new PictureBox { Size = new Size(24, 24), Image = _imgIns, SizeMode = PictureBoxSizeMode.Normal, Visible = false };
            Controls.Add(_chPic);
            Controls.Add(_insPic);

            _closeButton = new Button
            {
                Size = new Size(32, 32), FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent, Cursor = Cursors.Hand, Text = "X",
                ForeColor = Color.White, Font = new Font("Arial", 10f, FontStyle.Bold)
            };
            _closeButton.FlatAppearance.BorderSize = 0;
            _closeButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(80, 255, 255, 255);
            _closeButton.Click += (s, e) => Application.Exit();
            Controls.Add(_closeButton);

            _animTimer = new Timer { Interval = 16 };
            _animTimer.Tick += AnimTick;
            _animTimer.Start();

            _closeWorker = new BackgroundWorker();
            _closeWorker.DoWork += (s, e) =>
            {
                Process p;
                do { p = Process.GetProcessesByName("LogonUI").FirstOrDefault(); System.Threading.Thread.Sleep(100); }
                while (p != null);
                Application.Exit();
            };
        }

        void RenderBgCache()
        {
            int w = ClientSize.Width, h = ClientSize.Height;
            if (w < 1 || h < 1) return;
            _bgCache?.Dispose();
            _bgCache = new Bitmap(w, h);
            using (var g = Graphics.FromImage(_bgCache))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBilinear;
                g.DrawImage(_imgUp, 0, 0, w, 80);
                g.DrawImage(_imgBg, 0, 80, w, BgH);
                g.DrawImage(_imgDown, 0, h - 96, w, 96);
                g.DrawImage(_imgLine, CenterX, 80, 1, BgH);
                int lw = _imgLogo.Width, lh = _imgLogo.Height;
                g.DrawImage(_imgLogo, CenterX - lw, 80 + (BgH - lh) / 2, lw, lh);
                string hint = "\u8981\u5F00\u59CB\uFF0C\u8BF7\u5355\u51FB\u60A8\u7684\u7528\u6237\u540D    ";
                var sz = g.MeasureString(hint, _fontName);
                g.DrawString(hint, _fontName, Brushes.White, CenterX - sz.Width, 80 + (BgH - lh) / 2 + lh - 18);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            if (_bgCache != null) g.DrawImage(_bgCache, 0, 0);

            if (_users.Count == 0) return;
            int top = UserTop;

            if (_selectedIndex >= 0 && _selectedIndex < _users.Count)
            {
                int sy = top + _selectedIndex * 80 + (80 - _imgSelected.Height) / 2;
                g.DrawImage(_imgSelected, CenterX + 1, sy);
            }

            for (int i = 0; i < _users.Count; i++)
            {
                var u = _users[i];
                float op = u.Opacity;
                int rowY = top + i * 80;
                int sqX = CenterX + 36, sqY = rowY + 14;

                DrawShadow(g, sqX, sqY, 52, 52, 3, op);
                using (var br = new SolidBrush(Color.FromArgb((int)(255 * op), 204, 214, 235)))
                using (var path = RoundedRect(new Rectangle(sqX, sqY, 52, 52), 3))
                    g.FillPath(br, path);

                DrawAvatar(g, u.Avatar, sqX + 2, sqY + 2, 48, 48, op);
                using (var br = new SolidBrush(Color.FromArgb((int)(255 * op), 255, 255, 255)))
                    g.DrawString(u.Name, _fontName, br, sqX + 52 + 9, sqY);

                if (i == _selectedIndex)
                {
                    using (var br = new SolidBrush(Color.FromArgb((int)(255 * op), 255, 255, 255)))
                        g.DrawString("\u8F93\u5165\u5BC6\u7801", _fontSmall, br, sqX + 52 + 13, sqY + 29);
                }
            }
        }

        void DrawShadow(Graphics g, int x, int y, int w, int h, int r, float op)
        {
            int[] offsets = { 1, 2, 3, 4 };
            int[] alphas = { 50, 32, 16, 6 };
            for (int i = 3; i >= 0; i--)
            {
                int a = (int)(alphas[i] * op);
                using (var br = new SolidBrush(Color.FromArgb(a, 0, 0, 0)))
                using (var path = RoundedRect(new Rectangle(x + offsets[i], y + offsets[i], w, h), r))
                    g.FillPath(br, path);
            }
        }

        void DrawAvatar(Graphics g, Image img, int x, int y, int w, int h, float op)
        {
            var cm = new ColorMatrix();
            cm.Matrix33 = op;
            var attr = new ImageAttributes();
            attr.SetColorMatrix(cm);
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.DrawImage(img, new Rectangle(x, y, w, h), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, attr);
        }

        GraphicsPath RoundedRect(Rectangle r, int rad)
        {
            var p = new GraphicsPath();
            int d = rad * 2;
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }

        int HitTestUser(int mx, int my)
        {
            if (mx <= CenterX) return -1;
            int top = UserTop;
            for (int i = 0; i < _users.Count; i++)
            {
                int ry = top + i * 80;
                if (my >= ry && my < ry + 80) return i;
            }
            return -1;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            int idx = HitTestUser(e.X, e.Y);
            if (idx == _hoverIndex) return;
            _hoverIndex = idx;
            long now = DateTime.UtcNow.Ticks;
            for (int i = 0; i < _users.Count; i++)
            {
                float t = (i == _hoverIndex || i == _selectedIndex || _hoverIndex == -1) ? 1f : 0.6f;
                if (i == _selectedIndex) t = 1f;
                if (Math.Abs(_users[i].Target - t) > 0.01f)
                {
                    _users[i].Target = t;
                    _users[i].AnimStart = now;
                }
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            int idx = HitTestUser(e.X, e.Y);
            if (idx >= 0) SelectUser(idx);
            else DeselectUser();
        }

        void AnimTick(object sender, EventArgs e)
        {
            bool dirty = false;
            long now = DateTime.UtcNow.Ticks;
            for (int i = 0; i < _users.Count; i++)
            {
                var u = _users[i];
                if (Math.Abs(u.Opacity - u.Target) < 0.01f) { u.Opacity = u.Target; continue; }
                float elapsed = (float)(now - u.AnimStart) / TimeSpan.TicksPerMillisecond;
                float progress = Math.Min(elapsed / 300f, 1f);
                float prev = u.Opacity;
                u.Opacity = prev + (u.Target - prev) * progress;
                if (progress >= 1f) u.Opacity = u.Target;
                u.AnimStart = now;
                dirty = true;
            }
            if (dirty) Invalidate();
        }

        void SelectUser(int idx)
        {
            _selectedIndex = idx;
            var u = _users[idx];
            int sqX = CenterX + 36, sqY = UserTop + idx * 80 + 14;
            int boxX = sqX + 52 + 10, boxY = sqY + 43;

            _passwordBox.Location = new Point(boxX, boxY);
            _passwordBox.Text = "";
            _passwordBox.Visible = true;
            _passwordBox.Focus();

            _chPic.Location = new Point(boxX + 161 + 5, boxY + (27 - 16) / 2);
            _chPic.Visible = true;
            _goBtn.Location = new Point(boxX + 161 + 5 + 16 + 5, boxY + (27 - 24) / 2);
            _goBtn.Visible = true;
            _insPic.Location = new Point(boxX + 161 + 5 + 16 + 5 + 24 + 8, boxY + (27 - 24) / 2);
            _insPic.Visible = true;

            for (int i = 0; i < _users.Count; i++)
                _users[i].Target = (i == idx) ? 1f : 0.6f;
            Invalidate();
        }

        void DeselectUser()
        {
            _selectedIndex = -1;
            _passwordBox.Visible = false;
            _goBtn.Visible = false;
            _chPic.Visible = false;
            _insPic.Visible = false;
            for (int i = 0; i < _users.Count; i++) _users[i].Target = 1f;
            Invalidate();
        }

        void DoLogin()
        {
            if (_selectedIndex < 0) return;
            var u = _users[_selectedIndex];
            string pwd = _passwordBox.Text;

            IntPtr token = IntPtr.Zero;
            bool ok = LogonUser(u.Name, Environment.MachineName, pwd, 3, 0, ref token);
            if (!ok)
            {
                _passwordBox.Text = "";
                _passwordBox.Focus();
                return;
            }

            _isLoggingIn = true;
            string cleanPwd = new string(pwd.Where(c => !char.IsControl(c)).ToArray());
            bool isLast = string.Compare(_lastUser, u.Name, CultureInfo.CurrentCulture,
                CompareOptions.IgnoreCase | CompareOptions.IgnoreSymbols) == 0;

            try
            {
                Logon.Login(u.Name, cleanPwd, isLast);
            }
            catch { Application.Exit(); return; }

            _passwordBox.Visible = false;
            _goBtn.Visible = false;
            _chPic.Visible = false;
            _insPic.Visible = false;
            _closeWorker.RunWorkerAsync();
        }

        string GetLastLoggedOnUser()
        {
            try
            {
                string loc = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Authentication\LogonUI";
                var view = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
                using (var hive = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view))
                using (var key = hive.OpenSubKey(loc))
                {
                    var val = key?.GetValue("LastLoggedOnUser")?.ToString();
                    if (val != null && val.Contains("\\"))
                        return val.Substring(val.LastIndexOf('\\') + 1);
                    return val ?? "";
                }
            }
            catch { return ""; }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            RenderBgCache();
            if (_closeButton != null)
                _closeButton.Location = new Point(ClientSize.Width - 32, 0);
            if (_selectedIndex >= 0) SelectUser(_selectedIndex);
            Invalidate();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _animTimer?.Stop();
            Application.Exit();
        }
    }
}
