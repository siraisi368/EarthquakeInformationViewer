using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace EarthquakeInformationViewer
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }

        private int fromGPBox_Selectedradio(string gpBoxName)
        {
            int respData = 0;

            GroupBox gpBox = this.Controls.Find(gpBoxName, true).FirstOrDefault() as GroupBox;

            if (gpBox != null)
            {
                RadioButton checkedRadio = gpBox.Controls
                    .OfType<RadioButton>()
                    .FirstOrDefault(rb => rb.Checked);

                if (checkedRadio != null)
                {
                    Match match = Regex.Match(checkedRadio.Name, @"\d+$");
                    if (match.Success)
                    {
                        respData = int.Parse(match.Value);
                    }
                }
            }
            return respData;
        }

        private Control fromSavedSelRadio_To_Control(int selradio, string fromGPBox, string prefix)
        {
            Control control = null;

            GroupBox gpBox = this.Controls.Find(fromGPBox, true).FirstOrDefault() as GroupBox;

            if (gpBox != null)
            {
                string targetName = prefix + selradio.ToString();

                control = gpBox.Controls.Find(targetName, false).FirstOrDefault();
            }
            return control;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (EEW_radioButton4.Checked)
            {
                MessageBox.Show("AXIS APIを情報元として使用する場合\n\rYouTubeやその他SNSへのスクリーンショットの投稿や配信を行うことはできません。\n\r\n\r違反した場合、アカウントの停止やAXISのEEW配信自体が終了される可能性があります。", "注意", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            Properties.Settings.Default.eew_lastada_taiki = radioButton8.Checked;
            Properties.Settings.Default.jushin_taiki = radioButton9.Checked;
            Properties.Settings.Default.eqinfor_taiki = radioButton12.Checked;
            Properties.Settings.Default.is_eqcolor = checkBox1.Checked;

            SettingsLosa setLS = new SettingsLosa();
            setLS.is_WebSocket = is_WebSc.Checked;
            setLS.EEWAPI.selectedAPI = fromGPBox_Selectedradio("EEW_API_SelectBox");
            setLS.EarthquakeAPI.selectedAPI = fromGPBox_Selectedradio("Eq_API_SelectBox");
            setLS.TunamiAPI.selectedAPI = fromGPBox_Selectedradio("Tsunami_API_SelectBox");

            Properties.Settings.Default.InfoGetModes = JsonConvert.SerializeObject(setLS);
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            radioButton8.Checked = Properties.Settings.Default.eew_lastada_taiki;
            radioButton9.Checked = Properties.Settings.Default.jushin_taiki;
            radioButton12.Checked = Properties.Settings.Default.eqinfor_taiki;
            checkBox1.Checked = Properties.Settings.Default.is_eqcolor;

            SettingsLosa setLS = new SettingsLosa();
            setLS = JsonConvert.DeserializeObject<SettingsLosa>(Properties.Settings.Default.InfoGetModes);

            is_JsonAPI.Checked = !setLS.is_WebSocket;
            is_WebSc.Checked = setLS.is_WebSocket;

            Control EEWTarget = fromSavedSelRadio_To_Control(setLS.EEWAPI.selectedAPI, "EEW_API_SelectBox", "EEW_radioButton");
            Control EqTarget = fromSavedSelRadio_To_Control(setLS.EarthquakeAPI.selectedAPI, "Eq_API_SelectBox", "Eq_radioButton");
            Control TsuTarget = fromSavedSelRadio_To_Control(setLS.EarthquakeAPI.selectedAPI, "Tsunami_API_SelectBox", "Tsu_radioButton");
            if (EEWTarget != null && EEWTarget is RadioButton)
            {
                ((RadioButton)EEWTarget).Checked = true;
            }
            if (EqTarget != null && EqTarget is RadioButton)
            {
                ((RadioButton)EqTarget).Checked = true;
            }
            if (TsuTarget != null && TsuTarget is RadioButton)
            {
                ((RadioButton)TsuTarget).Checked = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            KeySettingWindow f = new KeySettingWindow();
            f.Show();
        }
    }
}
