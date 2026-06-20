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
        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        const int GWL_EXSTYLE = -20;
        const int WS_EX_LAYERED = 0x80000;
        const int WS_EX_TRANSPARENT = 0x20;

        class UserInfo
        {
            public string Name, SID;
            public Image Avatar;
            public float Opacity = 1f, Target = 1f;
            public long AnimStart;
        }

        List<UserInfo> _users = new List<UserInfo>();
        int _selectedIndex = -1, _hoverIndex = -1;
        bool _isLoggingIn, _showError, _loggedIn;
        string _lastUser, _hintText;
        Bitmap _bgCache;

        Image _imgUp, _imgDown, _imgBg, _imgLine, _imgLogo, _imgSelected;
        Image _imgCh, _imgGo, _imgIns, _imgDefaultUser, _imgPower;
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
            _imgPower = LoadImg(dir + "power.png");
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
            var log = new System.Text.StringBuilder();
            log.AppendLine("LoadAvatar: " + sid);
            try
            {
                string regPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\AccountPicture\Users\" + sid;
                var view = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
                using (var hive = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view))
                using (var key = hive.OpenSubKey(regPath))
                {
                    log.AppendLine("  key: " + (key != null ? "found" : "null"));
                    if (key != null)
                    {
                        foreach (string val in new[] { "Image240", "Image192", "Image96", "Image64", "Image48" })
                        {
                            string path = key.GetValue(val) as string;
                            log.AppendLine("  " + val + " = " + (path ?? "null"));
                            if (path != null)
                            {
                                log.AppendLine("  exists: " + File.Exists(path));
                                if (File.Exists(path))
                                {
                                    try
                                    {
                                        var img = Image.FromStream(new MemoryStream(File.ReadAllBytes(path)));
                                        log.AppendLine("  OK: " + img.Width + "x" + img.Height);
                                        File.AppendAllText(Environment.CurrentDirectory + "\\avatar.log", log.ToString());
                                        return img;
                                    }
                                    catch (Exception ex) { log.AppendLine("  load error: " + ex.Message); }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { log.AppendLine("  outer error: " + ex.Message); }
            File.AppendAllText(Environment.CurrentDirectory + "\\avatar.log", log.ToString());
            return _imgDefaultUser;
        }

        void CreateControls()
        {
            _passwordBox = new TextBox
            {
                Font = new Font("\u5B8B\u4F53", 12f, FontStyle.Regular, GraphicsUnit.Pixel),
                Size = new Size(161, 27),
                PasswordChar = '\u25CF', Visible = false
            };
            _passwordBox.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; DoLogin(); } };
            _passwordBox.TextChanged += (s, e) => { if (_passwordBox.Text.Length > 0 && _showError) { _showError = false; Invalidate(); } };
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
            _insPic = new PictureBox { Size = new Size(24, 24), Image = _imgIns, SizeMode = PictureBoxSizeMode.Normal, Visible = false, Cursor = Cursors.Hand };
            _insPic.Click += (s, e) => ShowPasswordHint();
            Controls.Add(_chPic);
            Controls.Add(_insPic);

            _closeButton = new Button
            {
                Size = new Size(32, 32), FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent, Cursor = Cursors.Default, Text = "",
                TabStop = false
            };
            _closeButton.FlatAppearance.BorderSize = 0;
            _closeButton.FlatAppearance.MouseOverBackColor = Color.Transparent;
            _closeButton.FlatAppearance.MouseDownBackColor = Color.Transparent;
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
            if (w < 1 || h < 1 || _imgUp == null) return;
            _bgCache?.Dispose();
            _bgCache = new Bitmap(w, h);
            using (var g = Graphics.FromImage(_bgCache))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBilinear;
                g.DrawImage(_imgUp, 0, 0, w, 80);
                g.DrawImage(_imgBg, 0, 80, w, BgH);
                g.DrawImage(_imgDown, 0, h - 96, w, 96);
                g.DrawImage(_imgLine, CenterX, 80, 1, BgH);
                if (!_loggedIn)
                {
                    int lw = _imgLogo.Width, lh = _imgLogo.Height;
                    g.DrawImage(_imgLogo, CenterX - lw, 80 + (BgH - lh) / 2, lw, lh);
                    string hint = "\u8981\u5F00\u59CB\uFF0C\u8BF7\u5355\u51FB\u60A8\u7684\u7528\u6237\u540D    ";
                    var sf = new StringFormat(StringFormat.GenericTypographic);
                    sf.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
                    var sz = g.MeasureString(hint, _fontName, 0, sf);
                    g.DrawString(hint, _fontName, Brushes.White, CenterX - sz.Width, 80 + (BgH - lh) / 2 + lh - 18, sf);
                    int py = h - 96 + 22;
                    g.DrawImage(_imgPower, 25, py, 24, 24);
                    string powerText = "\u5173\u95ED\u8BA1\u7B97\u673A";
                    var pSz = g.MeasureString(powerText, _fontName);
                    g.DrawString(powerText, _fontName, Brushes.White, 25 + 24 + 3, py + (24 - pSz.Height) / 2);
                    var tipFont = new Font("\u5B8B\u4F53", 12f, FontStyle.Regular, GraphicsUnit.Pixel);
                    string tipText = "\u767B\u5F55\u540E\uFF0C\u60A8\u53EF\u4EE5\u6DFB\u52A0\u6216\u66F4\u6539\u8D26\u6237\uFF0C\u8BF7\u8F6C\u5230\u201C\u63A7\u5236\u9762\u677F\u201D\uFF0C\n\u5E76\u5355\u51FB\u201C\u7528\u6237\u8D26\u6237\u201D\u3002";
                    var tipSz = g.MeasureString(tipText, tipFont);
                    g.DrawString(tipText, tipFont, Brushes.White, w - 30 - tipSz.Width, py);
                    tipFont.Dispose();
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            if (_bgCache != null) g.DrawImage(_bgCache, 0, 0);

            if (_users.Count == 0) return;

            if (_loggedIn && _selectedIndex >= 0)
            {
                var welcomeFont = new Font("\u5B8B\u4F53", 36f, FontStyle.Bold | FontStyle.Italic);
                string welcomeText = "\u6B22\u8FCE\u4F7F\u7528";
                var wsz = g.MeasureString(welcomeText, welcomeFont);
                float wx = CenterX - 20 - wsz.Width;
                float wy = 80 + (BgH - wsz.Height) / 2f;
                g.DrawString(welcomeText, welcomeFont, new SolidBrush(Color.FromArgb(100, 0, 0, 0)), wx + 3, wy + 3);
                g.DrawString(welcomeText, welcomeFont, Brushes.White, wx, wy);
                welcomeFont.Dispose();

                var u = _users[_selectedIndex];
                int rowY = 80 + (BgH - 80) / 2;
                int sqX = CenterX + 36, sqY = rowY + 14;
                DrawShadow(g, sqX, sqY, 52, 52, 3, 1f);
                using (var path = RoundedRect(new Rectangle(sqX, sqY, 52, 52), 3))
                    g.FillPath(Brushes.White, path);
                DrawAvatar(g, u.Avatar, sqX + 2, sqY + 2, 48, 48, 1f);
                g.DrawString(u.Name, _fontName, Brushes.White, sqX + 52 + 9, sqY);
                return;
            }

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
                Color sqColor = (i == _selectedIndex) ? Color.FromArgb((int)(255 * op), 255, 200, 0) : Color.FromArgb((int)(255 * op), 204, 214, 235);
                using (var br = new SolidBrush(sqColor))
                using (var path = RoundedRect(new Rectangle(sqX, sqY, 52, 52), 3))
                    g.FillPath(br, path);

                DrawAvatar(g, u.Avatar, sqX + 2, sqY + 2, 48, 48, op);
                using (var br = new SolidBrush(Color.FromArgb((int)(255 * op), 255, 255, 255)))
                    g.DrawString(u.Name, _fontName, br, sqX + 52 + 9, sqY);

                if (i == _selectedIndex)
                {
                    using (var br = new SolidBrush(Color.FromArgb((int)(255 * op), 255, 255, 255)))
                        g.DrawString("\u8F93\u5165\u5BC6\u7801", _fontSmall, br, sqX + 52 + 13, sqY + 29);
                    int boxX = sqX + 52 + 10, boxY = sqY + 43;
                    float nameW = g.MeasureString(u.Name, _fontName).Width;
                    int bubbleX = sqX + 52 + 9 + (int)nameW + 8;
                    var bubbleFont = new Font("\u5B8B\u4F53", 12f, FontStyle.Regular, GraphicsUnit.Pixel);
                    var bubbleFontBold = new Font("\u5B8B\u4F53", 12f, FontStyle.Bold, GraphicsUnit.Pixel);
                    if (_hintText != null)
                    {
                        string dispHint = "\u63D0\u793A: " + _hintText;
                        var tsz = g.MeasureString(dispHint, bubbleFont);
                        int bw = (int)tsz.Width + 12, bh = (int)tsz.Height + 8;
                        int by = sqY + ((int)g.MeasureString(u.Name, _fontName).Height - bh) / 2;
                        var br2 = new Rectangle(bubbleX, by, bw, bh);
                        g.FillRectangle(Brushes.White, br2);
                        g.DrawRectangle(Pens.Black, br2);
                        g.DrawString(dispHint, bubbleFont, Brushes.Black, bubbleX + 6, by + 4);
                    }
                    if (_showError)
                    {
                        string l1 = "\u6CA1\u6709\u8BB0\u4F4F\u5BC6\u7801?";
                        string l2 = "\u60A8\u53EF\u4EE5\u5355\u51FB\u201C?\u201D\u6309\u94AE\uFF0C\u67E5\u770B\u5BC6\u7801\u6697\u793A\u3002";
                        string l3 = "\u8BF7\u91CD\u65B0\u952E\u5165\u5BC6\u7801\u3002";
                        string l4 = "\u5927\u5C0F\u5199\u5FC5\u987B\u6B63\u786E\u3002";
                        float lh = g.MeasureString(l1, bubbleFontBold).Height;
                        float maxW = 0;
                        foreach (string s in new[] { l1, l2, l3, l4 })
                        {
                            float sw = g.MeasureString(s, bubbleFont).Width;
                            if (sw > maxW) maxW = sw;
                        }
                        int ew = (int)maxW + 16, eh = (int)(lh * 4) + 16;
                        int ey = boxY - eh - 4;
                        var er = new Rectangle(bubbleX, ey, ew, eh);
                        g.FillRectangle(Brushes.White, er);
                        g.DrawRectangle(Pens.Black, er);
                        float dy = ey + 8;
                        g.DrawString(l1, bubbleFontBold, Brushes.Black, bubbleX + 8, dy); dy += lh;
                        g.DrawString(l2, bubbleFont, Brushes.Black, bubbleX + 8, dy); dy += lh;
                        g.DrawString(l3, bubbleFont, Brushes.Black, bubbleX + 8, dy); dy += lh;
                        g.DrawString(l4, bubbleFont, Brushes.Black, bubbleX + 8, dy);
                    }
                    bubbleFont.Dispose();
                    bubbleFontBold.Dispose();
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

        Rectangle PowerRect => new Rectangle(25, ClientSize.Height - 96 + 22, 130, 24);

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            int idx = HitTestUser(e.X, e.Y);
            Cursor = PowerRect.Contains(e.Location) ? Cursors.Hand : Cursors.Default;
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
            if (_loggedIn) return;
            if (PowerRect.Contains(e.Location))
            {
                OpenPowerShortcut();
                return;
            }
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
            _hintText = null;
            _showError = false;
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
            _hintText = null;
            _showError = false;
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
                _showError = true;
                _hintText = null;
                _passwordBox.Text = "";
                _passwordBox.Focus();
                Invalidate();
                return;
            }

            _isLoggingIn = true;
            string cleanPwd = new string(pwd.Where(c => !char.IsControl(c)).ToArray());
            bool isLast = string.Compare(_lastUser, u.Name, CultureInfo.CurrentCulture,
                CompareOptions.IgnoreCase | CompareOptions.IgnoreSymbols) == 0;

            _loggedIn = true;
            _passwordBox.Visible = false;
            _goBtn.Visible = false;
            _chPic.Visible = false;
            _insPic.Visible = false;
            RenderBgCache();
            Refresh();
            SetWindowLong(this.Handle, GWL_EXSTYLE, GetWindowLong(this.Handle, GWL_EXSTYLE) | WS_EX_LAYERED | WS_EX_TRANSPARENT);
            BackgroundForm.form.MakeTransparent();

            var loginWorker = new BackgroundWorker();
            var capturedHandle = this.Handle;
            loginWorker.DoWork += (s2, e2) =>
            {
                try { Logon.Login(u.Name, cleanPwd, isLast); }
                catch (Exception ex)
                {
                    File.AppendAllText(Environment.CurrentDirectory + "\\login.log",
                        DateTime.Now + " Login failed: " + ex.Message + "\n");
                    this.Invoke((MethodInvoker)delegate
                    {
                        SetWindowLong(capturedHandle, GWL_EXSTYLE,
                            GetWindowLong(capturedHandle, GWL_EXSTYLE) & ~(WS_EX_LAYERED | WS_EX_TRANSPARENT));
                        BackgroundForm.form.RemoveTransparent();
                        _loggedIn = false;
                        _selectedIndex = -1;
                        _showError = false;
                        _hintText = null;
                        _passwordBox.Visible = false;
                        _goBtn.Visible = false;
                        _chPic.Visible = false;
                        _insPic.Visible = false;
                        RenderBgCache();
                        Refresh();
                    });
                    return;
                }
                Process lp;
                do { lp = Process.GetProcessesByName("LogonUI").FirstOrDefault(); System.Threading.Thread.Sleep(100); }
                while (lp != null);
                Application.Exit();
            };
            loginWorker.RunWorkerAsync();
        }

        void ShowPasswordHint()
        {
            if (_selectedIndex < 0) return;
            _showError = false;
            var u = _users[_selectedIndex];
            try
            {
                var rid = u.SID.Split('-').Last();
                var hex = int.Parse(rid).ToString("X").PadLeft(8, '0');
                var view = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
                using (var hive = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view))
                using (var key = hive.OpenSubKey(@"SAM\SAM\Domains\Account\Users\" + hex))
                {
                    if (key != null)
                    {
                        byte[] data = (byte[])key.GetValue("UserPasswordHint");
                        if (data != null)
                        {
                            string hint = System.Text.Encoding.Unicode.GetString(data).TrimEnd('\0');
                            if (!string.IsNullOrEmpty(hint))
                                _hintText = hint;
                        }
                    }
                }
            }
            catch { }
            Invalidate();
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

        void OpenPowerShortcut()
        {
            try
            {
                string conf = Environment.CurrentDirectory + "\\power.conf";
                string target = null;
                if (File.Exists(conf))
                    target = File.ReadAllLines(conf).FirstOrDefault(l => !string.IsNullOrWhiteSpace(l));
                if (string.IsNullOrEmpty(target))
                    target = Environment.CurrentDirectory + "\\shutdown.lnk";
                if (File.Exists(target))
                    Process.Start(target);
            }
            catch { }
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
