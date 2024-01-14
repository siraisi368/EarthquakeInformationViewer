using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Net.Http;
using System.Reflection.Emit;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

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
        private readonly Font AlertTypeFont = new Font("Koruri Regular", 12); // 速報情報 報版表示用フォント
        private readonly Font RegionFont = new Font("Koruri Regular", 15); // 地域表示用フォント
        private readonly Font DetailLabelFont = new Font("Koruri Regular", 8);  // 震度、マグニチュード、深さ情報 接頭語、単位表示用フォント
        private readonly Font DetailFont = new Font("Koruri Light", 20);   // マグニチュード、深さ情報 表示用フォント
        private readonly Font IntensityFont = new Font("Koruri Light", 22);   // 震度表示用フォント
        private readonly Font ShindoInfoFont = new Font("Koruri Regular", 10);    //振動レベル文字
        private readonly Font SLvINFOFont = new Font("Koruri Regular", 11);    //警告文字

        //緊急地震速報用(背景, 枠, 上部帯)
        private readonly (Color?, Color?, Color?, Color?) StartUpGeneralInfoColor = (Color.FromArgb(40, 60, 60), Color.FromArgb(47, 79, 79), null, Color.White);

        private readonly (Color?, Color?, Color?, Color?) GeneralInfoColor = (Color.FromArgb(40, 60, 60), Color.FromArgb(47, 79, 79), Color.FromArgb(47, 79, 79), Color.FromArgb(240, 240, 240));

        private readonly (Color?, Color?, Color?, Color?) PLUMForecastColor = (Color.FromArgb(0, 50, 76), Color.FromArgb(0, 80, 164), Color.FromArgb(0, 80, 164), Color.White);

        private readonly (Color?, Color?, Color?, Color?) ForecastColor = (Color.FromArgb(238, 195, 2), Color.FromArgb(255, 219, 0), Color.FromArgb(255, 219, 0), Color.Black);

        private readonly (Color?, Color?, Color?, Color?) WarningColor = (Color.FromArgb(142, 0, 0), Color.FromArgb(212, 0, 0), Color.FromArgb(212, 0, 0), Color.White);

        private readonly (Color?, Color?, Color?, Color?) SWarningColor = (Color.FromArgb(142, 0, 130), Color.FromArgb(192, 0, 185), Color.FromArgb(192, 0, 185), Color.White);

        private void WriteInformationToDisplay((Color?, Color?, Color?, Color?) InfoColorSchemes, (string, string)? status = null, string primarydata = null, string region = null, string intensity = null, float? magnitude = null, int? depthKm = null, string al_flg = null, int? rpt_num = 0, (string, string)? otherInfo = null)
        {
            Brush foreColor = new SolidBrush(InfoColorSchemes.Item4.Value);
            Bitmap canvas = new Bitmap(InformationDialog.Width, InformationDialog.Height);
            using (Graphics g = Graphics.FromImage(canvas))
            {
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                if (InfoColorSchemes.Item1.HasValue)
                    using (SolidBrush b = new SolidBrush(InfoColorSchemes.Item1.Value))
                        g.FillRectangle(b, 0, 0, 230, 85); //文字部分

                if (InfoColorSchemes.Item2.HasValue)
                    using (Pen p = new Pen(InfoColorSchemes.Item2.Value, 3))
                        g.DrawRectangle(p, 1, 1, 227, 82); //枠1

                if (InfoColorSchemes.Item3.HasValue)
                    using (SolidBrush b2 = new SolidBrush(InfoColorSchemes.Item3.Value))
                        g.FillRectangle(b2, 0, 0, 230, 20); //枠2

                if (status != null)
                {
                    g.DrawString(status.Value.Item1 ?? string.Empty, StatusFont, Brushes.White, 3, 2);
                    g.DrawString(status.Value.Item2 ?? string.Empty, EnglishStatusFont, Brushes.White, 4, 30);
                }

                {
                    if (primarydata != null)
                        g.DrawString(primarydata, AlertTypeFont, foreColor, 0, 0);
                    if (region != null)
                        g.DrawString(region, RegionFont, foreColor, 0, 20);
                    if (intensity != null)
                    {
                        g.DrawString("震度", DetailLabelFont, foreColor, 3, 67);
                        g.DrawString(intensity, IntensityFont, foreColor, 23, 45);
                    }
                    if (otherInfo != null)
                    {
                        g.DrawString(otherInfo.Value.Item1, AlertTypeFont, foreColor, 110, 45);
                        g.DrawString(otherInfo.Value.Item2, AlertTypeFont, foreColor, 110, 60);
                    }
                    if (magnitude != null && magnitude != -1)
                    {
                        g.DrawString("M", DetailLabelFont, foreColor, 85, 67);
                        g.DrawString(string.Format("{0:F1}", magnitude), DetailFont, foreColor, 95, 50);
                    }
                    if (depthKm != null && depthKm != -1)
                    {
                        g.DrawString("深さ", DetailLabelFont, foreColor, 140, 67);
                        g.DrawString(depthKm.ToString(), DetailFont, foreColor, 160, 50);
                        g.DrawString("km", DetailLabelFont, foreColor, 205, 67);
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
        private string al_flgChecker(string Title, bool eew_flg, string epicenter_Title,float mag,int deph ,string intn)
        {
            string resData = null;
            if (Title.Contains("（予報）")) 
                resData = "予報";
            else if (Title.Contains("（警報）")) 
                resData = "警報"; 
            if (epicenter_Title.Contains("または仮定震源要素") && Title.Contains("予報") && mag == 1.0f && deph == 10) 
                resData = "PLUMF";
            if (epicenter_Title.Contains("または仮定震源要素") && Title.Contains("警報") && mag == 1.0f && deph == 10)
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
                string al_flg = al_flgChecker(eew.Title, eew_flg, eew.Accuracy.Epicenter,mag,depth,intn);
                al_flg = "PLUMW";
                intn = "6強";
                reg = "ほげほげ";

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

        private void MainWindow_Load(object sender, EventArgs e)
        {
            this.Location = Properties.Settings.Default.main;
            this.MaximizeBox = false;
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;

            WriteInformationToDisplay(StartUpGeneralInfoColor, ("接続中", "Now Loading..."));
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
    }
}
