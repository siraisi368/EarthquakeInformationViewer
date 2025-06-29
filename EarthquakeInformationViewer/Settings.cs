using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EarthquakeInformationViewer
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(radioButton18.Checked)
            {
                MessageBox.Show("AXIS APIを情報元として使用する場合\n\rYouTubeやその他SNSへのスクリーンショットの投稿や配信を行うことはできません。\n\r\n\r違反した場合、アカウントの停止やAXISのEEW配信自体が終了される可能性があります。", "注意",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
            }

            Properties.Settings.Default.eew_lastada_taiki = radioButton8.Checked;
            Properties.Settings.Default.jushin_taiki = radioButton9.Checked;
            Properties.Settings.Default.eqinfor_taiki = radioButton12.Checked;
            Properties.Settings.Default.is_eqcolor = checkBox1.Checked;

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
