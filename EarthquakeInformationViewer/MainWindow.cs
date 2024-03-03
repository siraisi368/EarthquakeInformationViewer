using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EarthquakeInformationViewer
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private readonly Font StatusFont = new Font("Koruri Light", 20);    // 状態表示用フォント
        private readonly Font EnglishStatusFont = new Font("Koruri Light", 15);   // 英文状態表示用フォント
        private readonly Font AlertTypeFont = new Font("Koruri Regular", 15); // 速報情報 報版表示用フォント
        private readonly Font RegionFont = new Font("Koruri Regular", 20); // 地域表示用フォント
        private readonly Font DetailLabelFont = new Font("Koruri Regular", 11);  // 震度、マグニチュード、深さ情報 接頭語、単位表示用フォント
        private readonly Font DetailFont = new Font("Koruri Light", 22);   // マグニチュード、深さ情報 表示用フォント
        private readonly Font IntensityFont = new Font("Koruri Light", 30);   // 震度表示用フォント
        private readonly Font ShindoInfoFont = new Font("Koruri Regular", 10);    //振動レベル文字
        private readonly Font SLvINFOFont = new Font("Koruri Regular", 11);    //警告文字

        //緊急地震速報用(背景, 枠, 上部帯)
        private readonly (Color?, Color?, Color?, Color?) StartUpGeneralInfoColor = (Color.FromArgb(40, 60, 60), Color.FromArgb(47, 79, 79), null, Color.White);

        private readonly (Color?, Color?, Color?, Color?) GeneralInfoColor = (Color.FromArgb(40, 60, 60), Color.FromArgb(47, 79, 79), Color.FromArgb(47, 79, 79), Color.FromArgb(240, 240, 240));

        private readonly (Color?, Color?, Color?, Color?) PLUMForecastColor = (Color.FromArgb(0, 50, 76), Color.FromArgb(0, 80, 164), Color.FromArgb(0, 80, 164), Color.White);

        private readonly (Color?, Color?, Color?, Color?) ForecastColor = (Color.FromArgb(238, 195, 2), Color.FromArgb(255, 219, 0), Color.FromArgb(255, 219, 0), Color.Black);

        private readonly (Color?, Color?, Color?, Color?) WarningColor = (Color.FromArgb(142, 0, 0), Color.FromArgb(212, 0, 0), Color.FromArgb(212, 0, 0), Color.White);

        private readonly (Color?, Color?, Color?, Color?) SWarningColor = (Color.FromArgb(142, 0, 130), Color.FromArgb(192, 0, 185), Color.FromArgb(192, 0, 185), Color.White);

        private Dictionary<int, Color> ColorScheme = new Dictionary<int, Color>() {
            {-1,Color.FromArgb(152,152,152)},
            {10,Color.FromArgb(1,173,197)},
            {20,Color.FromArgb(0,197,102)},
            {30,Color.FromArgb(1,96,188)},
            {40,Color.FromArgb(215,175,0)},
            {45,Color.FromArgb(214,117,0)},
            {46,Color.FromArgb(214,117,0)},
            {50,Color.FromArgb(214,78,0)},
            {55,Color.FromArgb(200, 60, 130)},
            {60,Color.FromArgb(170, 60, 90)},
            {70,Color.FromArgb(121,40,150)},
        };

        int Ycenter = -2600;
        int Xcenter = -2400;
        double CenterLon = 30*0.8f;
        double CenterLat = 130;
        double Zoom = 31;

        public List<P2PQuake.DetailPrompt> detailPrompts = new List<P2PQuake.DetailPrompt>();

        JObject geojson_dataEq;

        private void WriteMapToDisplay(List<P2PQuake.DetailPrompt> prompts)
        {
            label1.Text = Xcenter.ToString();
            label3.Text = Ycenter.ToString();
            label4.Text = Zoom.ToString();
            label5.Text = CenterLon.ToString();
            label6.Text = CenterLat.ToString();

            Bitmap canvas = new Bitmap(MapBox.Width, MapBox.Height);
            using (Graphics g = Graphics.FromImage(canvas))
            {
                g.Clear(Color.FromArgb(0, 27, 59));
                g.SmoothingMode = SmoothingMode.HighQuality;

                foreach (JToken json_1 in geojson_dataEq.SelectToken("features"))
                {
                    GraphicsPath Maps = new GraphicsPath();
                    Maps.StartFigure();
                    if ((string)json_1.SelectToken("geometry.type") == "Polygon")
                    {
                        List<Point> points = new List<Point>();
                        foreach (JToken json_2 in json_1.SelectToken($"geometry.coordinates[0]"))
                        {
                            double x = (double)json_2.SelectToken("[0]");
                            double y = (double)json_2.SelectToken("[1]");
                            int px = (int)((((x*0.8f) - CenterLon) * Zoom) + Xcenter);
                            int py = (int)(((CenterLat - y) * Zoom) + Ycenter);

                            Point point = new Point(px, py);
                            points.Add(point);
                        }
                        if (points.Count > 2)
                        {
                            Maps.AddPolygon(points.ToArray());
                        }
                    }
                    else
                    {
                        JToken coordinatesToken = json_1.SelectToken("$.geometry.coordinates");
                        if (coordinatesToken != null)
                        {
                            foreach (JToken json_2 in coordinatesToken)
                            {
                                List<Point> points = new List<Point>();
                                foreach (JToken json_3 in json_2.SelectToken($"[0]"))
                                {
                                    double x = (double)json_3.SelectToken("[0]");
                                    double y = (double)json_3.SelectToken("[1]");
                                    int px = (int)((((x * 0.8f) - CenterLon) * Zoom) + Xcenter);
                                    int py = (int)(((CenterLat - y) * Zoom) + Ycenter);
                                    Point point = new Point(px, py);
                                    points.Add(point);
                                }
                                if (points.Count > 2)
                                {
                                    Maps.AddPolygon(points.ToArray());
                                }
                            }
                        }
                    }
                    bool flg = false;
                    foreach(P2PQuake.DetailPrompt dp in detailPrompts)
                    {
                        if (((string)json_1.SelectToken("properties.name")) == dp.Area)
                        {
                            g.FillPath(new SolidBrush(ColorScheme[dp.AreaMaxIntn]),Maps);
                            flg = true;
                            break;
                        }
                    }
                    if (!flg)
                    {
                        g.FillPath(new SolidBrush(Color.FromArgb(38,38,38)), Maps);
                    }
                    

                    using (Pen pen = new Pen(Color.White, 1))
                    {
                        pen.LineJoin = LineJoin.Round;
                        pen.StartCap = LineCap.Round;
                        pen.EndCap = LineCap.Round;

                        Point startPoint = new Point(10, 10);
                        Point endPoint = new Point(100, 100);

                        g.DrawPath(pen, Maps);
                    }
                    MapBox.Image = canvas;
                }
            }
        }
        private void WriteInformationToDisplay((Color?, Color?, Color?, Color?) InfoColorSchemes, (string, string)? status = null, string primarydata = null, string region = null, string intensity = null, float? magnitude = null, int? depthKm = null, string al_flg = null, int? rpt_num = 0, (string, string)? otherInfo = null)
        {
            Brush foreColor = new SolidBrush(InfoColorSchemes.Item4.Value);
            Bitmap canvas = new Bitmap(InformationDialog.Width, InformationDialog.Height);
            using (Graphics g = Graphics.FromImage(canvas))
            {
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                if (InfoColorSchemes.Item1.HasValue)
                    using (SolidBrush b = new SolidBrush(InfoColorSchemes.Item1.Value))
                        g.FillRectangle(b, 0, 0, 300, 120); //文字部分

                if (InfoColorSchemes.Item2.HasValue)
                    using (Pen p = new Pen(InfoColorSchemes.Item2.Value, 3))
                        g.DrawRectangle(p, 1, 1, 297, 117); //枠1

                if (InfoColorSchemes.Item3.HasValue)
                    using (SolidBrush b2 = new SolidBrush(InfoColorSchemes.Item3.Value))
                        g.FillRectangle(b2, 0, 0, 300, 28); //枠2

                if (status != null)
                {
                    g.DrawString(status.Value.Item1 ?? string.Empty, StatusFont, Brushes.White, 3, 2);
                    g.DrawString(status.Value.Item2 ?? string.Empty, EnglishStatusFont, Brushes.White, 4, 30);
                }

                {
                    if (primarydata != null)
                        g.DrawString(primarydata, AlertTypeFont, foreColor, 0, 0);
                    if (region != null)
                        g.DrawString(region, RegionFont, foreColor, 0, 30);
                    if (intensity != null)
                    {
                        g.DrawString("震度", DetailLabelFont, foreColor, 3, 97);
                        g.DrawString(intensity, IntensityFont, foreColor, 30, 67);
                    }
                    if (otherInfo != null)
                    {
                        g.DrawString(otherInfo.Value.Item1, AlertTypeFont, foreColor, 150, 70);
                        g.DrawString(otherInfo.Value.Item2, AlertTypeFont, foreColor, 150, 90);
                    }
                    if (magnitude != null && magnitude != -1)
                    {
                        g.DrawString("M", DetailLabelFont, foreColor, 120, 97);
                        g.DrawString(string.Format("{0:F1}", magnitude), DetailFont, foreColor, 133, 81);
                    }
                    if (depthKm != null && depthKm != -1)
                    {
                        g.DrawString("深さ", DetailLabelFont, foreColor, 180, 97);
                        g.DrawString(depthKm.ToString(), DetailFont, foreColor, 210, 81);
                        g.DrawString("km", DetailLabelFont, foreColor, 265, 97);
                    }
                }
            }

            InformationDialog.Image = canvas;
        }

        /// <summary>
        /// 返り値: true -> EEW発表中  false -> EEW非発表中
        /// </summary>
        /// <param name="issueTime">緊急地震速報の発表時刻(AnnouncedTime)</param>
        /// <param name="final_flg">最終報か否か(isFinal)</param>
        /// <returns></returns>
        private bool is_eewflg(string issueTime, bool final_flg)
        {
            bool is_eew;
            DateTime EewIssueTime = DateTime.Parse(issueTime);
            DateTime NowTime = DateTime.Now;
            TimeSpan ts = NowTime - EewIssueTime;
            if (ts.TotalMinutes > 3 && final_flg)
            {
                is_eew = false;
            }
            else
            {
                is_eew = true;
            }
            return is_eew;
        }
        private string al_flgParser(string Title)
        {
            string resData = null;
            if (Title.Contains("（予報）")) resData = "予報";
            else if (Title.Contains("（警報）")) resData = "警報";
            return resData;
        }
        private string al_flgChecker(string Title, bool eew_flg, (string, string, string) Data,float mag,int deph ,string intn)
        {
            string resData = null;
            if (Title.Contains("（予報）")) 
                resData = "予報";
            else if (Title.Contains("（警報）")) 
                resData = "警報"; 
            if (Data.Item1.Contains("または仮定震源要素") && Data.Item2.Contains("または仮定震源要素") && Data.Item3.Contains("または仮定震源要素") && Title.Contains("予報") && mag == 1.0f && deph == 10) 
                resData = "PLUMF";
            if (Data.Item1.Contains("または仮定震源要素") && Data.Item2.Contains("または仮定震源要素") && Data.Item3.Contains("または仮定震源要素") && Title.Contains("警報") && mag == 1.0f && deph == 10)
                resData = "PLUMW";
            if (!eew_flg) 
                resData = null;
            return resData;
        }

        private readonly HttpClient client = new HttpClient();

        private async void EEW_Timer_Tick(object sender, EventArgs e)
        {
            this.label2.Text = DateTime.Now.ToString("yyyy/dd/MM HH:mm:ss");

            try
            {
                var url = "https://api.wolfx.jp/jma_eew.json";
                var json = await client.GetStringAsync(url); //awaitを用いた非同期JSON取得
                textBox1.Text = json;
                var eew = JsonConvert.DeserializeObject<WolfxEEWAPI>(json);//EEWクラスを用いてJSONを解析(デシリアライズ)

                string reg = eew.Hypocenter;
                string intn = eew.MaxIntensity;
                bool end_flg = eew.isFinal;
                bool eew_flg = is_eewflg(eew.AnnouncedTime, true);
                string eew_id = eew.EventID;
                float mag = (float)eew.Magunitude;
                int depth = eew.Depth;
                int rpt_no = eew.Serial;
                string al_flg = al_flgChecker(eew.Title, eew_flg, (eew.Accuracy.Epicenter, eew.Accuracy.Depth, eew.Accuracy.Magnitude),mag,depth,intn);

                switch (al_flg)
                {
                    case "警報":
                        //Program.LastEewResult = EewResult.Warning;
                        if (intn == "6強" || intn == "7")
                            WriteInformationToDisplay(SWarningColor, null, $"緊急地震速報(警報) #{rpt_no}{(end_flg ? " 最終" : "")}", reg, intn, mag, depth, al_flg, rpt_no);
                            //label4.Text = "";
                            //label5.Text = "警報";
                            //label6.Text = rpt_no.ToString();
                        else
                            WriteInformationToDisplay(WarningColor, null, $"緊急地震速報(警報) #{rpt_no}{(end_flg ? " 最終" : "")}", reg, intn, mag, depth, al_flg, rpt_no);
                            //label4.Text = "";
                            //label5.Text = "警報";
                            //label6.Text = rpt_no.ToString();
                        break;

                    case "予報":
                        //Program.LastEewResult = EewResult.Forecast;
                        WriteInformationToDisplay(ForecastColor, null, $"緊急地震速報(予報) #{rpt_no}{(end_flg ? " 最終" : "")}", reg, intn, mag, depth, al_flg, rpt_no);
                        //label4.Text = "予報";
                        //label5.Text = "";
                        //label6.Text = rpt_no.ToString();
                        break;

                    case "PLUMF":
                        WriteInformationToDisplay(PLUMForecastColor, null, $"緊急地震速報(予報) #{rpt_no}{(end_flg ? " 最終" : "")}", reg, intn, -1, -1, al_flg, rpt_no, ("PLUM法による", "仮定震源要素"));
                        break;

                    case "PLUMW":
                        if (intn == "6強" || intn == "7")
                            WriteInformationToDisplay(SWarningColor, null, $"緊急地震速報(警報) #{rpt_no}{(end_flg ? " 最終" : "")}", reg, intn, -1, -1, al_flg, rpt_no, ("PLUM法による", "仮定震源要素"));
                        else
                            WriteInformationToDisplay(WarningColor, null, $"緊急地震速報(警報) #{rpt_no}{(end_flg ? " 最終" : "")}", reg, intn, -1, -1, al_flg, rpt_no, ("PLUM法による", "仮定震源要素"));
                        break;

                    case null:
                        //Program.LastEewResult = EewResult.None;
                        if (Properties.Settings.Default.eew_lastada == false)
                            WriteInformationToDisplay(StartUpGeneralInfoColor, ("受信待機中", "No Data..."));
                        else
                            WriteInformationToDisplay(GeneralInfoColor, null, $"EEWを受信していません。", reg, intn, mag, depth);
                        break;
                }
            }
            catch
            {
                goto OnError;
            }
            return;
        OnError:
            EEW_Timer.Enabled = false;
            WriteInformationToDisplay(StartUpGeneralInfoColor, ("再接続中", "Reconnecting..."));
            await Task.Delay(10);
            EEW_Timer.Enabled = true;
        }
        private readonly HttpClient EqClient = new HttpClient();
        public P2PQuake.P2PEqAPI lastdata = new P2PQuake.P2PEqAPI();
        private async void P2PQTimer_Tick(object sender, EventArgs e)
        {
            P2PQTimer.Interval = 10000;
            P2PQuake p2p = new P2PQuake();
            var url = "https://api.p2pquake.net/v2/history?codes=551&limit=1";
            //url = "https://api.p2pquake.net/v2/jma/quake?limit=1&min_scale=60&quake_type=DetailScale";
            var json = await client.GetStringAsync(url); //awaitを用いた非同期JSON取得
            var eqAPI = JsonConvert.DeserializeObject<List<P2PQuake.P2PEqAPI>>(json);//EEWクラスを用いてJSONを解析(デシリアライズ)
            if (eqAPI[0].id == lastdata.id) return;
            else lastdata = eqAPI[0];
            if (eqAPI[0].issue.type != "DetailScale") return;

            detailPrompts = p2p.ConvertDetailToPrompt(eqAPI[0]);
            textBox2.Text = "";
            foreach (P2PQuake.DetailPrompt detail in p2p.ConvertDetailToPrompt(eqAPI[0]))
            {
                textBox2.AppendText($"{detail.Area} 震度{p2p.IntenToShindo(detail.AreaMaxIntn)}\r\n");
            }
            WriteMapToDisplay(detailPrompts);
        }

        private void MapBox_MouseWheel(object sender, MouseEventArgs e)
        {
            {
                double currentZoom = Zoom;

                int mouseX = e.X;
                int mouseY = e.Y;

                double zoomX = (mouseX - Xcenter) / currentZoom;
                double zoomY = (mouseY - Ycenter) / currentZoom;

                double zoomFactor = e.Delta > 0 ? 1.2 : 0.8;

                double newZoom = Zoom * zoomFactor;

                if (newZoom < 5.0)
                {
                    newZoom = 5.0;
                }
                else if (newZoom > 400.0)
                {
                    newZoom = 400.0;
                }
                int newMouseX = (int)(zoomX * newZoom + Xcenter);
                int newMouseY = (int)(zoomY * newZoom + Ycenter);

                Xcenter += mouseX - newMouseX;
                Ycenter += mouseY - newMouseY;

                Zoom = newZoom;
            }
            WriteMapToDisplay(detailPrompts);
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            MapBox.MouseWheel += new MouseEventHandler(MapBox_MouseWheel);
            this.Location = Properties.Settings.Default.main;
            //this.MaximizeBox = false;
            //this.MaximumSize = this.Size;
            //this.MinimumSize = this.Size;

            using (StreamReader sread = new StreamReader("lib/geojson/EqForeAreaData_400.json", Encoding.UTF8))
            {
                geojson_dataEq = JObject.Parse(sread.ReadToEnd()); // GeoJsonの文字列を引数に入れる。
            }

            WriteInformationToDisplay(StartUpGeneralInfoColor, ("接続中", "Now Loading..."));
            WriteMapToDisplay(detailPrompts);
            EEW_Timer.Enabled = true;
        }

        private void 設定ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings f = new Settings();
            f.Show();
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.main = this.Location;
            Properties.Settings.Default.Save();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private bool isDragging;
        private Point lastMousePosition;
        private void MapBox_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                isDragging = true;
                lastMousePosition = e.Location;
                timer1.Enabled = true;
            }
        }

        private void MapBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                int deltaX = e.X - lastMousePosition.X;
                int deltaY = e.Y - lastMousePosition.Y;

                Xcenter += deltaX;
                Ycenter += deltaY;

                lastMousePosition = e.Location;
            }
        }

        private void MapBox_MouseUp(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                isDragging = false;
                timer1.Enabled = false;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            WriteMapToDisplay(detailPrompts);
        }

        private void MapBox_Paint(object sender, PaintEventArgs e)
        {
            WriteMapToDisplay(detailPrompts);
        }

    }
}
