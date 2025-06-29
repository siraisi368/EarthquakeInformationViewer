using EarthquakeInformationViewer.Tsunami;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

namespace EarthquakeInformationViewer
{ 
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public class MainStatus
        {
            public string eewState { get; set; }
            public string eqState { get; set; }
            public string TsuState { get; set; }
        }

        public MainStatus Status = new MainStatus()
        {
            eewState = "None",
            eqState = "None",
            TsuState = "None",
        };

        public List<WolfxEEWAPI> wolfxEEWAPIs = new List<WolfxEEWAPI>();

        public readonly Font StatusFont = new Font("Koruri Light", 20);    // 状態表示用フォント
        public readonly Font EnglishStatusFont = new Font("Koruri Light", 15);   // 英文状態表示用フォント
        public readonly Font AlertTypeFont = new Font("Koruri Regular", 15); // 速報情報 報版表示用フォント
        public readonly Font RegionFont = new Font("Koruri Regular", 20); // 地域表示用フォント
        public readonly Font DetailLabelFont = new Font("Koruri Regular", 11);  // 震度、マグニチュード、深さ情報 接頭語、単位表示用フォント
        public readonly Font DetailFont = new Font("Koruri Light", 22);   // マグニチュード、深さ情報 表示用フォント
        public readonly Font IntensityFont = new Font("Koruri Light", 30);   // 震度表示用フォント

        //緊急地震速報用(背景, 枠, 上部帯)
        public readonly (Color?, Color?, Color?, Color?) StartUpGeneralInfoColor = (Color.FromArgb(40, 60, 60), Color.FromArgb(47, 79, 79), null, Color.White);

        public readonly (Color?, Color?, Color?, Color?) GeneralInfoColor = (Color.FromArgb(40, 60, 60), Color.FromArgb(47, 79, 79), Color.FromArgb(47, 79, 79), Color.FromArgb(240, 240, 240));

        public readonly (Color?, Color?, Color?, Color?) PLUMForecastColor = (Color.FromArgb(0, 50, 76), Color.FromArgb(0, 80, 164), Color.FromArgb(0, 80, 164), Color.White);

        public readonly (Color?, Color?, Color?, Color?) ForecastColor = (Color.FromArgb(238, 195, 2), Color.FromArgb(255, 219, 0), Color.FromArgb(255, 219, 0), Color.Black);

        public readonly (Color?, Color?, Color?, Color?) WarningColor = (Color.FromArgb(142, 0, 0), Color.FromArgb(212, 0, 0), Color.FromArgb(212, 0, 0), Color.White);

        public readonly (Color?, Color?, Color?, Color?) SWarningColor = (Color.FromArgb(142, 0, 130), Color.FromArgb(192, 0, 185), Color.FromArgb(192, 0, 185), Color.White);

        public readonly (Color?, Color?, Color?, Color?) CancelInfoColor = (Color.FromArgb(40, 60, 60), Color.FromArgb(47, 79, 79), Color.FromArgb(47, 79, 79), Color.FromArgb(180, 180, 180));

        //地震情報別カラー(背景, 枠, 上部枠, 文字色)
        public Dictionary<int, (Color, Color, Color, Color)> ColorScheme = new Dictionary<int, (Color, Color, Color, Color)>() {
            {-1,(Color.FromArgb(132,132,132),Color.FromArgb(152,152,152),Color.FromArgb(152,152,152),Color.White)},
            {10,(Color.FromArgb(0, 133, 157),Color.FromArgb(1,173,197),Color.FromArgb(1,173,197),Color.White)},
            {20,(Color.FromArgb(0, 168, 87), Color.FromArgb(0,197,102),Color.FromArgb(0,197,102),Color.White)},
            {30,(Color.FromArgb(0, 36, 108), Color.FromArgb(1, 96, 188),Color.FromArgb(1, 96, 188),Color.White)},
            {40,(Color.FromArgb(175, 135, 0), Color.FromArgb(215, 175, 0),Color.FromArgb(215, 175, 0),Color.White)},
            {45,(Color.FromArgb(184, 87, 0), Color.FromArgb(214, 117, 0),Color.FromArgb(214, 117, 0),Color.White)},
            {46,(Color.FromArgb(184, 87, 0), Color.FromArgb(214, 117, 0),Color.FromArgb(214, 117, 0),Color.White)},
            {50,(Color.FromArgb(184, 68, 0), Color.FromArgb(214, 78, 0),Color.FromArgb(214, 78, 0),Color.White)},
            {55,(Color.FromArgb(170, 30, 100), Color.FromArgb(200, 60, 130),Color.FromArgb(200, 60, 130),Color.White)},
            {60,(Color.FromArgb(140, 30, 60), Color.FromArgb(170, 60, 90),Color.FromArgb(170, 60, 90),Color.White)},
            {70,(Color.FromArgb(101, 20, 130), Color.FromArgb(121, 40, 150),Color.FromArgb(121, 40, 150),Color.White)},
            //{140, },
            //{160, }
        };

        private static Dictionary<int, int[]> maxIntToAreaRange = new Dictionary<int, int[]>()
        {
            {-1,new int[]{ } },
            {10,new int[]{10 }},
            {20,new int[]{10,20 }},
            {30,new int[]{10,20,30 }},
            {40,new int[]{ 10,20,30,40}},
            {45,new int[]{ 30,40,46,45}},
            {50,new int[]{ 30,40,46,45,50}},
            {55,new int[]{ 30,40,46,45,50,55}},
            {60,new int[]{ 40,45,46,50,55,60}},
            {70,new int[]{ 40,45,46,50,55,60,70}},
        };

        public class AreaInfo
        {
            public string Name { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }

        private static HashSet<string> areaPointNames = new HashSet<string>();

        Image EpicenterImg;
        public P2PQuake.P2PEqAPI lastdata = new P2PQuake.P2PEqAPI();

        public static List<AreaInfo> QuakeAreaData;

        int Ycenter = -2600;
        int Xcenter = -2400;
        double CenterLon = 30 * 0.8f;
        double CenterLat = 130;
        double Zoom = 31;

        public List<P2PQuake.DetailPrompt> detailPrompts = new List<P2PQuake.DetailPrompt>();

        JObject geojson_dataEq;
        JObject geojson_dataTsu;

        float minX;
        float minY;
        float maxX;
        float maxY;
        float hypoLat;
        float hypoLon;

        private async Task WriteMapToDisplay(dynamic prompts, bool is_eqinfo = true, bool is_eew = false, bool isWheel = false)
        {
            if (!(isDragging ^ isWheel))
            {
                Xcenter = MapBox.Width / 2;
                Ycenter = MapBox.Height / 2;
            }

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
                    if (is_eqinfo)
                    {
                        foreach (P2PQuake.DetailPrompt dp in prompts)
                        {
                            if (((string)json_1.SelectToken("properties.name")) == dp.Area)
                            {
                                g.FillPath(new SolidBrush(ColorScheme[dp.AreaMaxIntn].Item2), Maps);
                                flg = true;
                                break;
                            }
                        }
                        if (!flg)
                        {
                            g.FillPath(new SolidBrush(Color.FromArgb(38, 38, 38)), Maps);
                        }
                    }
                    else if (is_eew)
                    {
                        foreach (string values in prompts)
                        {
                            if (((string)json_1.SelectToken("properties.name")) == values)
                            {
                                g.FillPath(new SolidBrush(ColorScheme[40].Item2), Maps);
                                flg = true;
                                break;
                            }
                        }
                        if (!flg)
                        {
                            g.FillPath(new SolidBrush(Color.FromArgb(38, 38, 38)), Maps);
                        }
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

                    if (lastdata.id != null)
                    {
                        int EpiX = (int)((lastdata.earthquake.hypocenter.longitude * 0.8 - CenterLon) * Zoom) + Xcenter;
                        int EpiY = (int)((CenterLat - lastdata.earthquake.hypocenter.latitude) * Zoom) + Ycenter;
                        int rectWidth = EpicenterImg.Width;
                        int rectHeight = EpicenterImg.Height;
                        int rectX = EpiX - rectWidth / 2;
                        int rectY = EpiY - rectHeight / 2;

                        g.DrawImage(EpicenterImg, rectX, rectY);
                    }
                    MapBox.Image = canvas;
                }
            }
        }
        /// <summary>
        /// 画面に情報を描画する関数
        /// </summary>
        /// <param name="InfoColorSchemes">描画カラー</param>
        /// <param name="status">備考</param>
        /// <param name="primarydata"></param>
        /// <param name="region"></param>
        /// <param name="intensity"></param>
        /// <param name="magnitude"></param>
        /// <param name="depthKm"></param>
        /// <param name="al_flg"></param>
        /// <param name="rpt_num"></param>
        /// <param name="otherInfo"></param>
        public void WriteInformationToDisplay((Color?, Color?, Color?, Color?) InfoColorSchemes, (string, string)? status = null, string primarydata = null, string region = null, string intensity = null, float? magnitude = null, int? depthKm = null, string al_flg = null, int? rpt_num = 0, (string, string)? otherInfo = null)
        {
            StringFormat CenterRight = new StringFormat()
            {
                Alignment = StringAlignment.Far,
                LineAlignment = StringAlignment.Center
            };
            StringFormat CenterCenter = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

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
                        g.DrawString(intensity, IntensityFont, foreColor, 79, 100, CenterCenter);
                    }
                    if (otherInfo != null)
                    {
                        g.DrawString(otherInfo.Value.Item1, AlertTypeFont, foreColor, 150, 70);
                        g.DrawString(otherInfo.Value.Item2, AlertTypeFont, foreColor, 150, 90);
                    }
                    if (magnitude != null && magnitude != -1)
                    {
                        g.DrawString("M", DetailLabelFont, foreColor, 120, 97);
                        g.DrawString(string.Format("{0:F1}", magnitude), DetailFont, foreColor, 185, 103, CenterRight);
                    }
                    if (depthKm != null && depthKm != -1 && depthKm != 0)
                    {
                        g.DrawString("深さ", DetailLabelFont, foreColor, 180, 97);
                        g.DrawString(depthKm.ToString(), DetailFont, foreColor, 270, 103, CenterRight);
                        g.DrawString("km", DetailLabelFont, foreColor, 265, 97);
                    }
                    else if (depthKm == 0)
                    {
                        g.DrawString("深さ", DetailLabelFont, foreColor, 180, 97);
                        g.DrawString("ごく浅い", AlertTypeFont, foreColor, 300, 107, CenterRight);
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
        private string al_flgChecker(WolfxEEWAPI eew)
        {
            string resData = null;
            if (eew != null)
            {
                if (eew.isWarn)
                    resData = "警報";
                else
                    resData = "予報";

                if (eew.isAssumption)
                    if (eew.isWarn)
                        resData = "PLUMW";
                    else
                        resData = "PLUMF";

                if (eew.isCancel)
                    resData = "キャンセル";

                if (eew.isTraining)
                    resData = null;

                if (!eew_flg)
                    resData = null;
            }
            return resData;
        }

        private readonly HttpClient client = new HttpClient();
        private bool eew_flg = false;
        private async void EEW_Timer_Tick(object sender, EventArgs e)
        {
            this.label2.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

            try
            {
                var url = "https://api.wolfx.jp/jma_eew.json";
                var json = await client.GetStringAsync(url); //awaitを用いた非同期JSON取得
                textBox1.Text = json;
                var eew = JsonConvert.DeserializeObject<WolfxEEWAPI>(json);//EEWクラスを用いてJSONを解析(デシリアライズ)

                string reg = eew.Hypocenter;
                string intn = eew.MaxIntensity;
                bool end_flg = eew.isFinal;
                eew_flg = is_eewflg(eew.AnnouncedTime, true);
                string eew_id = eew.EventID;
                float mag = (float)eew.Magunitude;
                int depth = eew.Depth;
                int rpt_no = eew.Serial;
                string al_flg = al_flgChecker(eew);

                if (Properties.Settings.Default.eqinfor_taiki && eew_flg == false && Status.eewState == "End")
                    return;

                switch (al_flg)
                {
                    case "警報":
                        Status.eewState = "Warning";
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
                        Status.eewState = "Forecast";
                        //Program.LastEewResult = EewResult.Forecast;
                        WriteInformationToDisplay(ForecastColor, null, $"緊急地震速報(予報) #{rpt_no}{(end_flg ? " 最終" : "")}", reg, intn, mag, depth, al_flg, rpt_no);
                        //label4.Text = "予報";
                        //label5.Text = "";
                        //label6.Text = rpt_no.ToString();
                        break;

                    case "PLUMF":
                        Status.eewState = "PLUMForecast";
                        WriteInformationToDisplay(PLUMForecastColor, null, $"緊急地震速報(予報) #{rpt_no}{(end_flg ? " 最終" : "")}", reg, intn, -1, -1, al_flg, rpt_no, ("PLUM法による", "仮定震源要素"));
                        break;

                    case "PLUMW":
                        Status.eewState = "PLUMWarning";
                        if (intn == "6強" || intn == "7")
                            WriteInformationToDisplay(SWarningColor, null, $"緊急地震速報(警報) #{rpt_no}{(end_flg ? " 最終" : "")}", reg, intn, -1, -1, al_flg, rpt_no, ("PLUM法による", "仮定震源要素"));
                        else
                            WriteInformationToDisplay(WarningColor, null, $"緊急地震速報(警報) #{rpt_no}{(end_flg ? " 最終" : "")}", reg, intn, -1, -1, al_flg, rpt_no, ("PLUM法による", "仮定震源要素"));
                        break;

                    case "キャンセル":
                        Status.eewState = "Cancel";
                        WriteInformationToDisplay(CancelInfoColor, null, $"緊急地震速報(取り消し) #{rpt_no}", reg, intn, mag, depth);
                        break;

                    case null:
                        Status.eewState = "None";
                        //Program.LastEewResult = EewResult.None;
                        if (Properties.Settings.Default.jushin_taiki)
                            WriteInformationToDisplay(StartUpGeneralInfoColor, ("受信待機中", "No Data..."));
                        if (Properties.Settings.Default.eew_lastada_taiki)
                            WriteInformationToDisplay(GeneralInfoColor, null, $"EEWを受信していません。", reg, intn, mag, depth);
                        if (Properties.Settings.Default.eqinfor_taiki)
                        {
                            return;
                        }
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

        string eqJText;
        private async void P2PQTimer_Tick(object sender, EventArgs e)
        {
            P2PQTimer.Interval = 10000;
            P2PQuake p2p = new P2PQuake();
            var url = "https://api.p2pquake.net/v2/history?codes=551&limit=1";
            //url = "https://api.p2pquake.net/v2/jma/quake?limit=1&min_scale=60&quake_type=DetailScale";
            var json = await EqClient.GetStringAsync(url);

            var eqAPI = JsonConvert.DeserializeObject<List<P2PQuake.P2PEqAPI>>(json);

            if (Properties.Settings.Default.eqinfor_taiki && eew_flg == false && Status.eewState == "End" || Status.eewState == "None")
            {
                if (Properties.Settings.Default.is_eqcolor)
                {
                    WriteInformationToDisplay(ColorScheme[eqAPI[0].earthquake.maxScale], null, $"{eqAPI[0].earthquake.time}発生", eqAPI[0].earthquake.hypocenter.name, p2p.IntenToShindo(eqAPI[0].earthquake.maxScale), (float)eqAPI[0].earthquake.hypocenter.magnitude, eqAPI[0].earthquake.hypocenter.depth);
                    Status.eqState = "View";
                }
                else
                {
                    WriteInformationToDisplay(GeneralInfoColor, null, $"{eqAPI[0].earthquake.time}発生", eqAPI[0].earthquake.hypocenter.name, p2p.IntenToShindo(eqAPI[0].earthquake.maxScale), (float)eqAPI[0].earthquake.hypocenter.magnitude, eqAPI[0].earthquake.hypocenter.depth);
                    Status.eqState = "View";
                }
            }


            if (eqAPI[0].id == lastdata.id) return;
            else lastdata = eqAPI[0];
            if (eqAPI[0].issue.type == "DetailScale")
                detailPrompts = p2p.ConvertDetailToPrompt(eqAPI[0]);
            if (eqAPI[0].issue.type == "ScalePrompt")
                detailPrompts = p2p.ConvertDetailToPrompt(eqAPI[0], true);

            if (json == eqJText)
                return;
            else
                eqJText = json;

            textBox3.Text = json;
            textBox2.Text = "";
            areaPointNames.Clear();
            foreach (P2PQuake.DetailPrompt detail in detailPrompts)
            {
                textBox2.AppendText($"{detail.Area} 震度{p2p.IntenToShindo(detail.AreaMaxIntn)}\r\n"); //Debug
                if (eqAPI[0].earthquake.maxScale == -1) { continue; }
                var minInts = maxIntToAreaRange[eqAPI[0].earthquake.maxScale];
                if (Array.Exists(minInts, element => element == detail.AreaMaxIntn))
                {
                    areaPointNames.Add(detail.Area);
                }
            }
            if (eqAPI[0].earthquake.maxScale != -1)
            {
                minX = float.MaxValue;
                minY = float.MaxValue;
                maxX = float.MinValue;
                maxY = float.MinValue;

                foreach (AreaInfo value in QuakeAreaData)
                {
                    if (areaPointNames.Contains(value.Name))
                    {
                        float lat = (float)value.Latitude;
                        float lon = (float)value.Longitude;

                        if (maxY == null || lat > maxY)
                            maxY = lat;
                        if (minY == null || lat < minY)
                            minY = lat;
                        if (maxX == null || lon > maxX)
                            maxX = lon;
                        if (minX == null || lon < minX)
                            minX = lon;
                    }
                }
                if (eqAPI[0].issue.type != "ScalePrompt")
                {
                    hypoLat = (float)eqAPI[0].earthquake.hypocenter.latitude;
                    hypoLon = (float)eqAPI[0].earthquake.hypocenter.longitude;
                    minX = Math.Min(minX, hypoLon);
                    maxX = Math.Max(maxX, hypoLon);
                    minY = Math.Min(minY, hypoLat);
                    maxY = Math.Max(maxY, hypoLat);
                }
                CenterLon = (minX + maxX) / 2 * 0.8f;
                CenterLat = (minY + maxY) / 2;
                int pxMinX = (int)(((minX - CenterLon) * Zoom) + Xcenter);
                int pxMinY = (int)(((minY - CenterLat) * Zoom) + Ycenter);
                int pxMaxX = (int)(((maxX - CenterLon) * Zoom) + Xcenter);
                int pxMaxY = (int)(((maxY - CenterLat) * Zoom) + Ycenter);
                RectangleF combinedRect = new RectangleF(pxMinX, pxMinY, pxMaxX - pxMinX, pxMaxY - pxMinY);

                int rectWidth = (int)(combinedRect.Width / Zoom);
                int rectHeight = (int)(combinedRect.Height / Zoom);

                double newZoom = Math.Min((double)(MapBox.Width) / rectWidth - 180, (double)(MapBox.Height) / rectHeight - 300);
                if (newZoom < 10.0)
                {
                    newZoom = 10.0;
                }
                else if (newZoom > 400.0)
                {
                    newZoom = 400.0;
                }
                Zoom = newZoom;
            }
            await WriteMapToDisplay(detailPrompts);
        }

        private async void P2PTsuTimer_Tick(object sender, EventArgs e)
        {
            P2PTsuTimer.Interval = 30000;
            var url = "https://api.p2pquake.net/v2/history?codes=551&limit=1";
            var json = await EqClient.GetStringAsync(url); //awaitを用いた非同期JSON取得
            var tsuAPI = JsonConvert.DeserializeObject<List<P2PTsunami>>(json);

        }

        private async void MapBox_MouseWheel(object sender, MouseEventArgs e)
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
            await WriteMapToDisplay(detailPrompts, isWheel: true);
        }

        private async void MainWindow_Load(object sender, EventArgs e)
        {
            MapBox.MouseWheel += new MouseEventHandler(MapBox_MouseWheel);
            this.Location = Properties.Settings.Default.main;

            byte[] jsonDataBytes = Properties.Resources.KubunMarkLatLon;
            string json = Encoding.UTF8.GetString(jsonDataBytes);
            QuakeAreaData = JsonConvert.DeserializeObject<List<AreaInfo>>(json);

            using (StreamReader sread = new StreamReader("lib/geojson/EqForeAreaData_400.json", Encoding.UTF8))
            {
                geojson_dataEq = JObject.Parse(sread.ReadToEnd()); // GeoJsonの文字列を引数に入れる。
            }

            using (StreamReader sread = new StreamReader("lib/geojson/TsuForeAreaData.json", Encoding.UTF8))
            {
                geojson_dataTsu = JObject.Parse(sread.ReadToEnd()); // GeoJsonの文字列を引数に入れる。
            }

            using (FileStream sread = new FileStream("lib/img/regmark.png", FileMode.Open))
            {
                EpicenterImg = Image.FromStream(sread);
            }

            WriteInformationToDisplay(StartUpGeneralInfoColor, ("接続中", "Now Loading..."));
            await WriteMapToDisplay(detailPrompts);
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
            if (e.Button == MouseButtons.Left)
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
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
                timer1.Enabled = false;
            }
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            await WriteMapToDisplay(detailPrompts);
        }

        private async void MapBox_Paint(object sender, PaintEventArgs e)
        {
            await WriteMapToDisplay(detailPrompts);
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            double.TryParse(textBox4.Text, out double cLon);
            double.TryParse(textBox5.Text, out double cLat);
            CenterLon = cLon;
            CenterLat = cLat;
            await WriteMapToDisplay(detailPrompts);
        }

        private async void MainWindow_SizeChanged(object sender, EventArgs e)
        {
            int pxMinX = (int)(((minX - CenterLon) * Zoom) + Xcenter);
            int pxMinY = (int)(((minY - CenterLat) * Zoom) + Ycenter);
            int pxMaxX = (int)(((maxX - CenterLon) * Zoom) + Xcenter);
            int pxMaxY = (int)(((maxY - CenterLat) * Zoom) + Ycenter);
            RectangleF combinedRect = new RectangleF(pxMinX, pxMinY, pxMaxX - pxMinX, pxMaxY - pxMinY);

            int rectWidth = (int)(combinedRect.Width / Zoom);
            int rectHeight = (int)(combinedRect.Height / Zoom);

            double newZoom = Math.Min((double)(MapBox.Width) / rectWidth - 180, (double)(MapBox.Height) / rectHeight - 300);
            if (newZoom < 10.0)
            {
                newZoom = 10.0;
            }
            else if (newZoom > 400.0)
            {
                newZoom = 400.0;
            }
            Zoom = newZoom;
            await WriteMapToDisplay(detailPrompts);
        }
    }
}