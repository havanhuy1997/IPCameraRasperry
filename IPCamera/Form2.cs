using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using Onvif.IP.Camera.Viewer;
using IPCamera.Properties;
using System.IO;
namespace IPCamera
{
    public partial class Form2 : Form
    {
        // for ip adrress
        static string IpCameraA;
        static string IpCameraB;
        static string IpRaspberry;
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            listHotKey.Add(Settings.Default.keyLeft1);
            listHotKey.Add(Settings.Default.keyRight1);
            listHotKey.Add(Settings.Default.keyCenter1);
            listHotKey.Add(Settings.Default.keySceenShot);
            listHotKey.Add(Settings.Default.keyRecoder);
            listHotKey.Add(Settings.Default.keyCam1);
            listHotKey.Add(Settings.Default.keyCam2);
            listHotKey.Add(Settings.Default.keyLeft2);
            listHotKey.Add(Settings.Default.keyRight2);
            listHotKey.Add(Settings.Default.keyUp);
            listHotKey.Add(Settings.Default.keyDown);
            listHotKey.Add(Settings.Default.keyMin);
            listHotKey.Add(Settings.Default.keyMax);

            keyLeft1.Text = (Settings.Default.keyLeft1);
            keyRight1.Text = (Settings.Default.keyRight1);
            keyCenter1.Text = (Settings.Default.keyCenter1);
            keySceenShot.Text = (Settings.Default.keySceenShot);
            keyRecoder.Text = (Settings.Default.keyRecoder);
            keyCam1.Text = (Settings.Default.keyCam1);
            keyCam2.Text = (Settings.Default.keyCam2);
            keyLeft2.Text = (Settings.Default.keyLeft2);
            keyRight2.Text = (Settings.Default.keyRight2);
            keyUp.Text = (Settings.Default.keyUp);
            keyDown.Text = (Settings.Default.keyDown);
            keyMin.Text = (Settings.Default.keyMin);
            keyMax.Text = (Settings.Default.keyMax);

            txtLoginCa1.Text = Settings.Default.loginCa1;
            txtLoginCa2.Text = Settings.Default.loginCa2;
            txtPassCa1.Text = Settings.Default.passCa1;
            txtPassCa2.Text = Settings.Default.passCa2;
            txtStreamURLCa1.Text = Settings.Default.streamURLCa1;
            txtStreamURLCa2.Text = Settings.Default.streamURLCa2;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            IpCameraA = txtIPCaA.Text;
            IpCameraB = txtIPCaB.Text;
            IpRaspberry = txtIPRas.Text;
            Form1.IpCameraA = IpCameraA;
            Form1.IpCameraB = IpCameraB;
            Form1.IpRaspberry = IpRaspberry;
            Settings.Default.IPRaspberry = IpRaspberry;
            Settings.Default.IPCamera1 = IpCameraA;
            Settings.Default.IPCamera2 = IpCameraB;
            Settings.Default.loginCa1 = txtLoginCa1.Text;
            Settings.Default.loginCa2 = txtLoginCa2.Text;
            Settings.Default.passCa1 = txtPassCa1.Text;
            Settings.Default.passCa2 = txtPassCa2.Text;
            Settings.Default.streamURLCa1 = txtStreamURLCa1.Text;
            Settings.Default.streamURLCa2 = txtStreamURLCa2.Text;
            if (checkAutoStart.Checked)
            {
                Settings.Default.autoStart = true;
            }
            else
            {
                Settings.Default.autoStart = false;
                MessageBox.Show("Press 'Enter' to Start");
            }
            Settings.Default.Save();    
            this.Close();
        }
        Boolean checkHostKey(string hotKey)
        {
            foreach (string item in listHotKey)
            {
                if (item == hotKey)
                {
                    return false;                   
                }
            }
            return true;
        }
        private void label11_Click(object sender, EventArgs e)
        {

        }
        List<string> listHotKey = new List<string>();        
        private void keyLeft1_Click(object sender, EventArgs e)
        {
            keyLeft1.EnabledChanged += keyLeft1_Click;
            keyLeft1.Enabled = true;
            keyLeft1.Text = "";            
        }

        private void keyLeft1_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void keyLeft1_KeyDown(object sender, KeyEventArgs e)
        {
            string key = e.KeyCode.ToString();
            if (!checkHostKey(key))
            {
                MessageBox.Show("Used key!!");
            }
            else
            {
                Settings.Default.keyLeft1 = key;
                keyLeft1.Enabled = false;
            }
        }

        private void keyRight1_Click(object sender, EventArgs e)
        {
            keyRight1.EnabledChanged += keyRight1_Click;
            keyRight1.Enabled = true;
            keyRight1.Text = "";
        }

        private void keyRight1_KeyDown(object sender, KeyEventArgs e)
        {
            string key = e.KeyCode.ToString();
            if (!checkHostKey(key))
            {
                MessageBox.Show("Used key!!");
            }
            else
            {
                Settings.Default.keyRight1 = key;
                keyRight1.Enabled = false;
            }
        }

        private void keyCenter1_Click(object sender, EventArgs e)
        {
            keyCenter1.EnabledChanged += keyCenter1_Click;
            keyCenter1.Enabled = true;
            keyCenter1.Text = "";
        }

        private void keyCenter1_KeyDown(object sender, KeyEventArgs e)
        {
            string key = e.KeyCode.ToString();
            if (!checkHostKey(key))
            {
                MessageBox.Show("Used key!!");
            }
            else
            {
                Settings.Default.keyCenter1 = key;
                keyCenter1.Enabled = false;
            }
        }

        private void keySceenShot_Click(object sender, EventArgs e)
        {
            keySceenShot.EnabledChanged += keySceenShot_Click;
            keySceenShot.Enabled = true;
            keySceenShot.Text = "";
        }

        private void keySceenShot_KeyDown(object sender, KeyEventArgs e)
        {
            string key = e.KeyCode.ToString();
            if (!checkHostKey(key))
            {
                MessageBox.Show("Used key!!");
            }
            else
            {
                Settings.Default.keySceenShot = key;
                keySceenShot.Enabled = false;
            }
        }

        private void keyRecoder_Click(object sender, EventArgs e)
        {
            keyRecoder.EnabledChanged += keyRecoder_Click;
            keyRecoder.Enabled = true;
            keyRecoder.Text = "";
        }

        private void keyRecoder_KeyDown(object sender, KeyEventArgs e)
        {
            string key = e.KeyCode.ToString();
            if (!checkHostKey(key))
            {
                MessageBox.Show("Used key!!");
            }
            else
            {
                Settings.Default.keyRecoder = key;
                keyRecoder.Enabled = false;
            }
        }

        private void keyCam1_Click(object sender, EventArgs e)
        {
            keyCam1.EnabledChanged += keyCam1_Click;
            keyCam1.Enabled = true;
            keyCam1.Text = "";
        }

        private void keyCam1_KeyDown(object sender, KeyEventArgs e)
        {
            string key = e.KeyCode.ToString();
            if (!checkHostKey(key))
            {
                MessageBox.Show("Used key!!");
            }
            else
            {
                Settings.Default.keyCam1 = key;
                keyCam1.Enabled = false;
            }
        }

        private void keyCam2_Click(object sender, EventArgs e)
        {
            keyCam2.EnabledChanged += keyCam2_Click;
            keyCam2.Enabled = true;
            keyCam2.Text = "";
        }

        private void keyCam2_KeyDown(object sender, KeyEventArgs e)
        {
            string key = e.KeyCode.ToString();
            if (!checkHostKey(key))
            {
                MessageBox.Show("Used key!!");
            }
            else
            {
                Settings.Default.keyCam2 = key;
                keyCam2.Enabled = false;
            }
        }

        private void keyLeft2_Click(object sender, EventArgs e)
        {
            keyLeft2.EnabledChanged += keyLeft2_Click;
            keyLeft2.Enabled = true;
            keyLeft2.Text = "";
        }

        private void keyLeft2_KeyDown(object sender, KeyEventArgs e)
        {
            string key = e.KeyCode.ToString();
            if (!checkHostKey(key))
            {
                MessageBox.Show("Used key!!");
            }
            else
            {
                Settings.Default.keyLeft2 = key;
                keyLeft2.Enabled = false;
            }
        }

        private void keyRight2_Click(object sender, EventArgs e)
        {
            keyRight2.EnabledChanged += keyRight2_Click;
            keyRight2.Enabled = true;
            keyRight2.Text = "";
        }

        private void keyRight2_KeyDown(object sender, KeyEventArgs e)
        {
            string key = e.KeyCode.ToString();
            if (!checkHostKey(key))
            {
                MessageBox.Show("Used key!!");
            }
            else
            {
                Settings.Default.keyRight2 = key;
                keyRight2.Enabled = false;
            }
        }

        private void keyUp_Click(object sender, EventArgs e)
        {
            keyUp.EnabledChanged += keyUp_Click;
            keyUp.Enabled = true;
            keyUp.Text = "";
        }

        private void keyUp_KeyDown(object sender, KeyEventArgs e)
        {
            string key = e.KeyCode.ToString();
            if (!checkHostKey(key))
            {
                MessageBox.Show("Used key!!");
            }
            else
            {
                Settings.Default.keyUp = key;
                keyUp.Enabled = false;
            }
        }

        private void keyDown_Click(object sender, EventArgs e)
        {
            keyDown.EnabledChanged += keyDown_Click;
            keyDown.Enabled = true;
            keyDown.Text = "";
        }

        private void keyDown_KeyDown(object sender, KeyEventArgs e)
        {
            string key = e.KeyCode.ToString();
            if (!checkHostKey(key))
            {
                MessageBox.Show("Used key!!");
            }
            else
            {
                Settings.Default.keyDown = key;
                keyDown.Enabled = false;
            }
        }

        private void keyMin_Click(object sender, EventArgs e)
        {
            keyMin.EnabledChanged += keyMin_Click;
            keyMin.Enabled = true;
            keyMin.Text = "";
        }

        private void keyMin_KeyDown(object sender, KeyEventArgs e)
        {
            string key = e.KeyCode.ToString();
            if (!checkHostKey(key))
            {
                MessageBox.Show("Used key!!");
            }
            else
            {
                Settings.Default.keyMin = key;
                keyMin.Enabled = false;
            }
        }

        private void keyMax_Click(object sender, EventArgs e)
        {
            keyMax.EnabledChanged += keyMax_Click;
            keyMax.Enabled = true;
            keyMax.Text = "";
        }

        private void keyMax_KeyDown(object sender, KeyEventArgs e)
        {
            string key = e.KeyCode.ToString();
            if (!checkHostKey(key))
            {
                MessageBox.Show("Used key!!");
            }
            else
            {
                Settings.Default.keyMax= key;
                keyMax.Enabled = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    Settings.Default.outPutSceenShot = fbd.SelectedPath;
                    Settings.Default.Save();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using(var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    Settings.Default.outPutAudio = fbd.SelectedPath;
                    Settings.Default.Save();
                }
            }
        }

        private void keyRight1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
