using System;
using System.Windows.Forms;
using AForge.Video;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.IO;
using System.Net.Sockets;
using IPCamera;
using NAudio.Wave;
using System.Threading;
using IPCamera.Properties;
using System.Diagnostics;
namespace Onvif.IP.Camera.Viewer
{
    public partial class Form1 : Form
    {
        // General
        private bool sutostartCompleted = false;
        //FOR VIDEO
        static public string IpCameraA = Settings.Default.IPCamera1;
        static public string IpCameraB = Settings.Default.IPCamera2;
        static public string IpRaspberry = Settings.Default.IPRaspberry;
        string loginCa1 = Settings.Default.loginCa1;
        string loginCa2 = Settings.Default.loginCa2;
        string passCa1 = Settings.Default.passCa1;
        string passCa2 = Settings.Default.passCa2;
        string streamURLCa1 = Settings.Default.streamURLCa1;
        string streamURLCa2 = Settings.Default.streamURLCa2;
        MJPEGStream stream;
        Bitmap currentBmp;
        Bitmap currentBmpSceenShot;
        //Insert image to current image
        void insertImage(string fileName, string positionInsert)
        {
            Bitmap originalOverImage = (Bitmap)Image.FromFile(fileName);
            Bitmap overImage = new Bitmap(originalOverImage, new Size(originalOverImage.Width, originalOverImage.Height));
            Point point = new Point();
            if (positionInsert == "left")
            {
                point = new Point(0, currentBmp.Height - overImage.Height);
            }
            else if (positionInsert == "center")
            {
                overImage = new Bitmap(originalOverImage, new Size(originalOverImage.Width * currentBmp.Height / originalOverImage.Height, currentBmp.Height));
                point = new Point(currentBmp.Width / 2 - overImage.Width / 2, 0);
            }
            else if (positionInsert == "right")
            {
                point = new Point(currentBmp.Width - overImage.Width, currentBmp.Height - overImage.Height);
            }
            using (Graphics g = Graphics.FromImage(currentBmp))
            {
                g.DrawImage(overImage, point);
                g.Save();
            }

        }
        Boolean workCamera = false;
        //for connecting camera
        void connectCamera(string ipAddress, string login, string pass, string streamURL)
        {
            // TODO: Debug code remove
            /*
            currentBmp = new Bitmap(@"C:\Users\PS\Desktop\SceenShot1.png");
            workCamera = true;
            pictureBox1.Image = currentBmp;
            enableButtonCamera();
            return;
            */
            //workCamera = true;
            stream = new MJPEGStream();
            stream.Source = "http://" + ipAddress + "/" + streamURL;
            stream.Login = login;
            stream.Password = pass;
            stream.NewFrame += stream_NewFrame;
            stream.VideoSourceError += stream_VideoError;
            stream.Start();
        }
        void disconnectCamera()
        {
            workCamera = false;
            try
            {
                if (stream != null)
                    stream.Stop();
                pictureBox1.Image = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        void stream_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                if (!workCamera)
                {
                    BeginInvoke((Action)(() =>
                    {
                        enableButtonCamera();
                    }));
                }
                workCamera = true;
                currentBmp = (Bitmap)eventArgs.Frame.Clone();
                
                if (capture)
                {
                    /*
                    capture = false;
                    currentBmpSceenShot = currentBmp;
                    string fileName = Settings.Default.outPutSceenShot + "\\SceenShot" + (Settings.Default.fileNameSceenShot + 1).ToString() + ".png";
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.FileName = fileName;
                    ImageFormat format = ImageFormat.Png;
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        currentBmpSceenShot.Save(sfd.FileName, format);
                    }
                    MessageBox.Show(fileName, "Saved SceenShot");
                    Settings.Default.fileNameSceenShot = Settings.Default.fileNameSceenShot + 1;
                    Settings.Default.Save();
                    */
                }
                currentBmp = new Bitmap(currentBmp, new Size(pictureBox1.Width, pictureBox1.Height));
                pictureBox1.Image = currentBmp;

            }
            catch (Exception e)
            {
                workCamera = false;
            }

        }

        void stream_VideoError(object sender, VideoSourceErrorEventArgs eventArgs)
        {
            stream.SignalToStop();
            workCamera = false;
            MessageBox.Show("Connect camera fail!! " + eventArgs.Description);
        }

        Boolean capture = false;
        // thanks to https://stackoverflow.com/questions/14522540/close-a-messagebox-after-several-seconds
        public class AutoClosingMessageBox
        {
            System.Threading.Timer _timeoutTimer;
            string _caption;
            AutoClosingMessageBox(string text, string caption, int timeout)
            {
                _caption = caption;
                _timeoutTimer = new System.Threading.Timer(OnTimerElapsed,
                    null, timeout, System.Threading.Timeout.Infinite);
                MessageBox.Show(text, caption);
            }
            public static void Show(string text, string caption, int timeout)
            {
                new AutoClosingMessageBox(text, caption, timeout);
            }
            void OnTimerElapsed(object state)
            {
                IntPtr mbWnd = FindWindow(null, _caption);
                if (mbWnd != IntPtr.Zero)
                    SendMessage(mbWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                _timeoutTimer.Dispose();
            }
            const int WM_CLOSE = 0x0010;
            [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
            static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
            [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
            static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
        }
        void sceenShot()
        {
            capture = true;
            currentBmpSceenShot = currentBmp;            
            string fileName = "SceenShot"+ DateTime.Now.ToString("yyyy-MM-dd h.mm.ss") +".png";
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = fileName;
            sfd.InitialDirectory = Settings.Default.outPutSceenShot;
            sfd.Filter = "Images|*.png;*.bmp;*.jpg";
            ImageFormat format = ImageFormat.Png;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string ext = Path.GetExtension(sfd.FileName);
                switch (ext)
                {
                    case ".jpg":
                        format = ImageFormat.Jpeg;
                        break;
                    case ".bmp":
                        format = ImageFormat.Bmp;
                        break;
                }
                fileName = sfd.FileName;
                currentBmpSceenShot.Save(fileName, format);
                AutoClosingMessageBox.Show("Saved "+fileName, "Notification", 1500);
            }            
            pressUp(ref picSceenShot);
        }
        void enableButtonCamera()
        {
            picCenter1.Enabled = true;
            picSceenShot.Enabled = true;
            picCa1.Enabled = true;
            picCa2.Enabled = true;

        }
        void disableButtonCamera()
        {
            picSceenShot.Enabled = false;
            picCa1.Enabled = false;
            picCa2.Enabled = false;
            picCenter1.Enabled = false;
        }


        //FOR AUDIO
        static MemoryStream mp3Buffered = new MemoryStream();
        bool audioRecording = false;
        int BUFFER_SIZE = 8192 * 2;
        int PORT_AUDIO = 50004;
        TcpClient clientSocket;
        Socket socketAudio;
        byte[] data;
        Boolean connectedAudio = false;
        WaveFormat audioFormat;
        //Receive audio
        void connectAudio()
        {
            try
            {
                connectedAudio = true;
                enableButtonAudio();
                clientSocket = new TcpClient();
                clientSocket.Connect(IpRaspberry, PORT_AUDIO);
                socketAudio = clientSocket.Client;
            }
            catch (Exception ex)
            {
                connectedAudio = false;
                MessageBox.Show("Fail to connect to Audio !! Try again Ip of Raspberry !!");
            }
        }
        void receiveAudio()
        {
            audioRecording = false;
            try
            {
                data = new byte[BUFFER_SIZE];
                BufferedWaveProvider provider;
                WaveOut _waveOut = new WaveOut();
                WaveFormat format;
                format = new WaveFormat(44100, 1);
                provider = new BufferedWaveProvider(format);
                audioFormat = provider.WaveFormat;
                _waveOut.Init(provider);
                _waveOut.Play();
                while (true)
                {
                    int count = socketAudio.Receive(data, BUFFER_SIZE, SocketFlags.None);
                    if (count == 0)
                    {
                        connectedAudio = false;
                        break;
                    }
                    if (audioRecording)
                    {
                        mp3Buffered.Write(data, 0, data.Length);
                    }
                    provider.AddSamples(data, 0, count);
                    Console.WriteLine(count.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Buffer full!");
                connectedAudio = false;
                disconnectAudio();
            }
        }
        void saveFileAudio(string fileNme)
        {
            MemoryStream mp3BufferedSave = mp3Buffered;
            var reader = new RawSourceWaveStream(mp3BufferedSave, audioFormat);// new WaveFormat(44100, 1));
            //byte[] array = frames.SelectMany(a => a).ToArray();
            mp3BufferedSave.Position = 0;
            using (var convertedStream = WaveFormatConversionStream.CreatePcmStream(reader))
            {
                WaveFileWriter.CreateWaveFile(fileNme, convertedStream);
            }
            mp3Buffered.SetLength(0);
            mp3Buffered.Position = 0;
        }
        void disconnectAudio()
        {

            connectedAudio = false;
            if (clientSocket != null)
            {
                clientSocket.Close();
                clientSocket.Dispose();
            }

        }
        void disableButtonAudio()
        {
            picAudio.Enabled = false;
        }
        void enableButtonAudio()
        {
            picAudio.Enabled = true;
        }



        //FOR CONTROL ENGINE
        TcpClient socket;
        NetworkStream serverStream;
        //connect to raspberry
        Boolean connectedEngine = false;
        void connectEngine()
        {
            try
            {
                connectedEngine = true;
                enbleButtonEngine();
                socket = new TcpClient();
                socket.Connect(IpRaspberry, 5007);
                serverStream = socket.GetStream();
                //
                if (Settings.Default.minmax == 0)
                    picMin_MouseDown(this, e1);
                else
                    picMax_MouseDown(this, e1);
            }
            catch (Exception e)
            {
                disableButtonEngine();
                connectedEngine = false;
                MessageBox.Show("Fail to connect to engine !! Try again IpAddress of Raspberry !!");
            }
        }
        void disConnectEngine()
        {
            if (serverStream != null)
            {
                serverStream.Dispose();
            }
            if (socket != null)
            {
                socket.Close();
                socket.Dispose();
            }
            connectedEngine = false;
        }
        //for control engine
        void controlEngine(string command)
        {
            try
            {

                byte[] outStream = Encoding.ASCII.GetBytes(command);
                serverStream.Write(outStream, 0, outStream.Length);
                //serverStream.Flush();
            }
            catch (Exception ex)
            {
                disConnectEngine();
            }

        }
        void disableButtonEngine()
        {
            picLeft1.Enabled = false;
            picRight1.Enabled = false;
            picRight2.Enabled = false;
            picLeft2.Enabled = false;
            picUp2.Enabled = false;
            picDown2.Enabled = false;
            picMin.Enabled = false;
            picMax.Enabled = false;
        }
        void enbleButtonEngine()
        {
            picLeft1.Enabled = true;
            picRight1.Enabled = true;
            picRight2.Enabled = true;
            picLeft2.Enabled = true;
            picUp2.Enabled = true;
            picDown2.Enabled = true;
            picMin.Enabled = true;
            picMax.Enabled = true;
        }

        // PS:
        void openSettingsDialog()
        {
            Form2 connectForm = new Form2();
            connectForm.Show();
        }

        // Thanks to: https://stackoverflow.com/questions/505167/how-do-i-make-a-winforms-app-go-full-screen
        void EnterFullScreenMode()
        {
            WindowState = FormWindowState.Normal;
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            isFullScreen = true;
        }

        void LeaveFullScreenMode()
        {
            FormBorderStyle = FormBorderStyle.Sizable;
            WindowState = FormWindowState.Normal;
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            RestoreOriginalSize();
            isFullScreen = false;
        }

        void RestoreOriginalSize()
        {
            // Correct size on demand
            //if (pictureBox1.Image != null)
            //    Settings.Default.FrameSize = pictureBox1.Image.Size;
            // Resize this frame window
            Size = Settings.Default.FrameSize;
        }

        void RepositionControls(int dw, int dh)
        {
            dh += controlPanelDistance;
            pictureBox2.Location = new Point(pictureBox2.Location.X, pictureBox2.Location.Y + dh);
            picControl2.Location = new Point(picControl2.Location.X + dw, picControl2.Location.Y + dh);
            picCa1.Location = new Point(picCa1.Location.X, picCa1.Location.Y + dh);
            picCa2.Location = new Point(picCa2.Location.X, picCa2.Location.Y + dh);
            picSceenShot.Location = new Point(picSceenShot.Location.X, picSceenShot.Location.Y + dh);
            picAudio.Location = new Point(picAudio.Location.X, picAudio.Location.Y + dh);
            picMin.Location = new Point(picMin.Location.X + dw, picMin.Location.Y + dh);
            picMax.Location = new Point(picMax.Location.X + dw, picMax.Location.Y + dh);
        }

        //For hotkeys
        string keyLeft1 = Settings.Default.keyLeft1;
        string keyRight1 = Settings.Default.keyRight1;
        string keyCenter1 = Settings.Default.keyCenter1;
        string keySceenShot = Settings.Default.keySceenShot;
        string keyRecoder = Settings.Default.keyRecoder;
        string keyCam1 = Settings.Default.keyCam1;
        string keyCam2 = Settings.Default.keyCam2;
        string keyLeft2 = Settings.Default.keyLeft2;
        string keyRight2 = Settings.Default.keyRight2;
        string keyUp = Settings.Default.keyUp;
        string keyDown = Settings.Default.keyDown;
        string keyMin = Settings.Default.keyMin;
        string keyMax = Settings.Default.keyMax;
        // Locker
        private SecretCombination keysReader;
        private int controlPanelDistance;

        // EVENT FORM 1
        public Form1()
        {
            keysReader = new SecretCombination("E.N.I.G.M.A");
            InitializeComponent();
            this.Focus();
            picSceenShot.Parent = pictureBox1;
            picAudio.Parent = pictureBox1;
            picCa1.Parent = pictureBox1;
            picCa2.Parent = pictureBox1;
            pictureBox2.Parent = pictureBox1;
            picControl2.Parent = pictureBox1;
            picMin.Parent = pictureBox1;
            picMax.Parent = pictureBox1;

            picLeft1.Parent = pictureBox2;
            picLeft1.Location = new Point(30, 57);
            picCenter1.Parent = pictureBox2;
            picCenter1.Location = new Point(60, 57);
            picRight1.Parent = pictureBox2;
            picRight1.Location = new Point(90, 57);

            picLeft2.Parent = picControl2;
            picLeft2.Location = new Point(30, 57);
            picRight2.Parent = picControl2;
            picRight2.Location = new Point(90, 57);
            picUp2.Parent = picControl2;
            picUp2.Location = new Point(60, 30);
            picDown2.Parent = picControl2;
            picDown2.Location = new Point(60, 90);

            picOver.Parent = pictureBox1;
            picOver.Image = Image.FromFile("..\\..\\Resources\\Images\\overImage.png");
            //
            controlPanelDistance = 0;
            disableButtonCamera();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            int w = (int) (2.0/3 * SystemInformation.VirtualScreen.Width);
            int h = (int) (2.0/3 * SystemInformation.VirtualScreen.Height);
            Size = new Size(w, h);
            Settings.Default.FrameSize = Size;
            //
            if (Settings.Default.autoStart)
            {
                if (!sutostartCompleted)
                {
                    connectAll();
                    sutostartCompleted = true;
                }
            }
        }
        Boolean key1 = false;
        Boolean key2 = false;
        object sender1;
        MouseEventArgs e1;
        int i = 0;
        int countPressSceenShot = 0;
        int countPressAudio = 0;
        int countPressCa1 = 0;
        int countPressCa2 = 0;
        int countPressLeft1 = 0;
        int countPressRight1 = 0;
        int countPressCenter1 = 0;
        int countPressLeft2 = 0;
        int countPressRight2 = 0;
        int countPressUp2 = 0;
        int countPressDown2 = 0;
        int countPressMin = 0;
        int countPressMax = 0;
        //
        bool keyCtrlPress = false;
        bool isFullScreen = false;
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // Call dialog if secret key combination is applied
            if (keysReader.feedKey(e.KeyCode))
            {
                openSettingsDialog();
                return;
            }

            if (e.KeyCode == Keys.ControlKey)
                keyCtrlPress = true;

            if (e.KeyCode == Keys.Oemcomma)
            {
                key1 = true;
            }
            if (e.KeyCode == Keys.OemPeriod)
            {
                key2 = true;
            }
            if (key1 && key2)
            {
                //openSettingsDialog();
                key1 = false;
                key2 = false;
            }
            Console.WriteLine(e.KeyCode.ToString());
            Console.WriteLine(i.ToString());

            if (workCamera)
            {
                if (e.KeyCode.ToString() == keySceenShot)
                {
                    countPressSceenShot++;
                    if (countPressSceenShot == 1)
                    {
                        // SceenShot
                        picSceenShot_MouseDown(sender1, e1);
                    }

                }

                if (e.KeyCode.ToString() == keyCenter1)
                {
                    countPressCenter1++;
                    if (countPressCenter1 == 1)
                    {
                        picCenter1_MouseDown(sender1, e1);
                        // center control 1
                    }
                }
            }

            if (e.KeyCode.ToString() == keyCam1)
            {
                countPressCa1++;
                if (countPressCa1 == 1)
                {
                    picCa1_MouseDown(sender1, e1);
                    // camera 1
                }
            }
            if (e.KeyCode.ToString() == keyCam2)
            {
                countPressCa2++;
                if (countPressCa2 == 1)
                {
                    picCa2_MouseDown(sender1, e1);
                    // camera 2
                }
            }

            if (connectedAudio)
            {
                if (e.KeyCode.ToString() == keyRecoder)
                {

                    if (++countPressAudio == 1)
                    {
                        picAudio_MouseDown(sender1, e1);
                        // audio
                    }
                    countPressAudio = 0;
                }
            }

            if (connectedEngine)
            {
                if (e.KeyCode.ToString() == keyRight1)
                {
                    if (++countPressRight1 == 1)
                    {
                        picRight1_MouseDown(sender1, e1);
                        //Right control 1
                    }
                }
                if (e.KeyCode.ToString() == keyLeft1)
                {
                    if (++countPressLeft1 == 1)
                    {
                        picLeft1_MouseDown(sender1, e1);
                        // left control 1
                    }
                }
                if (e.KeyCode.ToString() == keyLeft2)
                {
                    if (++countPressLeft2 == 1)
                    {
                        picLeft2_MouseDown(sender1, e1);
                        // left control 2
                    }
                }
                if (e.KeyCode.ToString() == keyRight2)
                {
                    if (++countPressRight2 == 1)
                    {
                        picRight2_MouseDown(sender1, e1);
                        // right control 2
                    }
                }
                if (e.KeyCode.ToString() == keyUp)
                {
                    if (++countPressUp2 == 1)
                    {
                        picUp2_MouseDown(sender1, e1);
                        // up control 2
                    }
                }
                if (e.KeyCode.ToString() == keyDown)
                {
                    if (++countPressDown2 == 1)
                    {
                        picDown2_MouseDown(sender1, e1);
                        // down control 2
                    }
                }
                if (e.KeyCode.ToString() == keyMin)
                {
                    if (++countPressMin == 1)
                    {
                        //picMax.ImageLocation = "..\\..\\Resources\\Images\\maxButton.png";
                        //picMin.ImageLocation = "..\\..\\Resources\\Images\\minPressButton.png";
                        picMin_MouseDown(sender1, e1);
                        // min control 2
                    }
                }
                if (e.KeyCode.ToString() == keyMax)
                {
                    picMin.ImageLocation = "..\\..\\Resources\\Images\\minButton.png";
                    picMax.ImageLocation = "..\\..\\Resources\\Images\\maxPressButton.png";
                    if (++countPressMax == 1)
                    {
                        picMax_MouseDown(sender1, e1);
                        // max control 2               
                    }
                }
            }
        }
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Oemcomma)
            {
                key1 = false;
            }
            if (e.KeyCode == Keys.OemPeriod)
            {
                key2 = false;
            }
            if (workCamera)
            {
                if (e.KeyCode.ToString() == keySceenShot)
                {
                    countPressSceenShot = 0;
                    picSceenShot_MouseUp(sender1, e1);
                    // SceenShot
                }
                if (e.KeyCode.ToString() == keyCenter1)
                {
                    countPressCenter1 = 0;
                    picCenter1_MouseUp(sender1, e1);
                }

            }
            if (e.KeyCode.ToString() == keyCam1)
            {
                countPressCa1 = 0;
                picCa1_MouseUp(sender1, e1);
                // camera 1
            }
            if (e.KeyCode.ToString() == keyCam2)
            {
                countPressCa2 = 0;
                picCa2_MouseUp(sender1, e1);
                // camera 2
            }

            if (connectedAudio)
            {
                if (e.KeyCode.ToString() == keyRecoder)
                {
                    countPressAudio = 0;
                    picAudio_MouseUp(sender1, e1);
                    // audio
                    pressAudio = false;
                }
            }
            if (connectedEngine)
            {
                if (e.KeyCode.ToString() == keyRight1)
                {
                    countPressRight1 = 0;
                    picRight1_MouseUp(sender1, e1);
                    // Right control 1
                }
                if (e.KeyCode.ToString() == keyLeft1)
                {
                    countPressLeft1 = 0;
                    picLeft1_MouseUp(sender1, e1);
                    // left control 1
                }
                if (e.KeyCode.ToString() == keyLeft2)
                {
                    countPressLeft2 = 0;
                    picLeft2_MouseUp(sender1, e1);
                    // left control 2
                }
                if (e.KeyCode.ToString() == keyRight2)
                {
                    countPressRight2 = 0;
                    picRight2_MouseUp(sender1, e1);
                    // right control 2
                }
                if (e.KeyCode.ToString() == keyUp)
                {
                    countPressUp2 = 0;
                    picUp2_MouseUp(sender1, e1);
                    // up control 2
                }
                if (e.KeyCode.ToString() == keyDown)
                {
                    countPressDown2 = 0;
                    picDown2_MouseUp(sender1, e1);
                    // down control 2
                }
                if (e.KeyCode.ToString() == keyMin)
                {
                    countPressMin = 0;
                    picMin_MouseUp(sender1, e1);
                    // min control 2
                }
                if (e.KeyCode.ToString() == keyMax)
                {
                    countPressMax = 0;
                    picMax_MouseUp(sender1, e1);
                    // max control 2
                }
            }
        }
        void connectAll()
        {

            IpCameraA = Settings.Default.IPCamera1;
            IpCameraB = Settings.Default.IPCamera2;
            IpRaspberry = Settings.Default.IPRaspberry;
            loginCa1 = Settings.Default.loginCa1;
            loginCa2 = Settings.Default.loginCa2;
            passCa1 = Settings.Default.passCa1;
            passCa2 = Settings.Default.passCa2;
            streamURLCa1 = Settings.Default.streamURLCa1;
            streamURLCa2 = Settings.Default.streamURLCa2;
            if (!workCamera)
            {
                picCa2.ImageLocation = "..\\..\\Resources\\Images\\ca2Button.png";
                picCa1.ImageLocation = "..\\..\\Resources\\Images\\ca1PressButton.png";
                connectCamera(IpCameraA, loginCa1, passCa1, streamURLCa1);
            }
            else
            {
                disableButtonCamera();
            }



            if (!connectedAudio)
            {
                connectAudio();
            }

            if (thAudio != null)
            {
                if (thAudio.ThreadState == System.Threading.ThreadState.Stopped)
                {
                    thAudio = new Thread(receiveAudio);
                    thAudio.Name = "audio";
                    thAudio.Start();
                }
            }
            else
            {
                thAudio = new Thread(receiveAudio);
                thAudio.Name = "audio";
                thAudio.Start();
            }

            if (!connectedEngine)
            {
                connectEngine();
            }
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Settings.Default.Save();
            //
            Application.Exit();
            if (thAudio != null)
            {
                thAudio.Abort();
            }
            Process[] MainProcess = Process.GetProcessesByName("IPCamera");
            foreach (Process ps in MainProcess)
            {
                ps.Kill();
            }
        }
        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                if (keyCtrlPress)
                {
                    if (!isFullScreen)
                        EnterFullScreenMode();
                }
                else
                {
                    connectAll();
                }
            }
            if (e.KeyChar == Convert.ToChar(Keys.Escape))
            {
                if (isFullScreen)
                    LeaveFullScreenMode();
            }
            if (e.KeyChar == Convert.ToChar(Keys.Space))
            {
                if (controlPanelDistance == 0)
                {
                    controlPanelDistance = 300;
                    RepositionControls(0, 0);
                }
                else
                {
                    controlPanelDistance = -300;
                    RepositionControls(0, 0);
                    controlPanelDistance = 0;
                }
            }
            keyCtrlPress = false;
        }


        // EVENT PICTURE BOX
        Thread thAudio;
        Boolean pressSceenShot = false;
        Boolean pressAudio = false;
        Boolean pressCamera1 = false;
        Boolean pressCamera2 = false;
        Boolean pressCentralControl1 = false;
        Boolean pressLeftControl1 = false;
        Boolean pressRightControl1 = false;
        Boolean pressLeftControl2 = false;
        Boolean pressRightControl2 = false;
        Boolean pressUpControl2 = false;
        Boolean pressDownControl2 = false;
        Boolean pressMinControl2 = false;
        Boolean pressMaxControl2 = false;
        Boolean recoding = false;

        void changeToCamera1()
        {
            picCa2.ImageLocation = "..\\..\\Resources\\Images\\ca2Button.png";
            picCa1.ImageLocation = "..\\..\\Resources\\Images\\ca1PressButton.png";
            disconnectCamera();
            connectCamera(IpCameraA, loginCa1, passCa1, streamURLCa1);
        }
        void changeToCamera2()
        {
            picCa2.ImageLocation = "..\\..\\Resources\\Images\\ca2PressButton.png";
            picCa1.ImageLocation = "..\\..\\Resources\\Images\\ca1Button.png";
            disconnectCamera();
            connectCamera(IpCameraB, loginCa2, passCa2, streamURLCa2);
        }



        // FOR BUTTON

        void pressDown(ref PictureBox pic)
        {
            pic.Location = new Point(pic.Location.X + 5, pic.Location.Y + 5);
        }
        void pressUp(ref PictureBox pic)
        {
            pic.Location = new Point(pic.Location.X - 5, pic.Location.Y - 5);
            if (connectedEngine == false)
            {
                disableButtonEngine();
            }
            if (connectedAudio == false)
            {
                disableButtonAudio();
            }
            if (workCamera == false)
            {
                disableButtonCamera();
            }
        }
        private void picSceenShot_MouseDown(object sender, MouseEventArgs e)
        {
            Console.WriteLine("SceenShot press");
            pressDown(ref picSceenShot);
            pressSceenShot = true;
            sceenShot();
        }

        private void picSceenShot_MouseUp(object sender, MouseEventArgs e)
        {
            pressUp(ref picSceenShot);
            pressSceenShot = false;
        }

        private void picAudio_MouseDown(object sender, MouseEventArgs e)
        {
            Console.WriteLine("Audio press");
            pressDown(ref picAudio);
            // audio
            if (connectedAudio)
            {
                pressAudio = !pressAudio;
                if (!recoding)
                {
                    audioRecording = true;
                    picAudio.ImageLocation = "..\\..\\Resources\\Images\\audioPressButton.png";
                    mp3Buffered = new MemoryStream();
                    recoding = !recoding;
                    MessageBox.Show("Recoding....");
                }
                else
                {
                    picAudio.ImageLocation = "..\\..\\Resources\\Images\\audioButton.png";
                    recoding = !recoding;
                    MessageBox.Show("End recoding");
                    string fileName = "audio" + (Settings.Default.fileNameAudio + 1).ToString() + ".wav";
                    audioRecording = false;
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter = "Audio wav file|*.wav";
                    sfd.FileName = fileName;
                    sfd.InitialDirectory = Settings.Default.outPutAudio;
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        fileName = sfd.FileName;
                        saveFileAudio(fileName);
                        Settings.Default.outPutAudio = Path.GetDirectoryName(fileName);
                    }
                    Settings.Default.fileNameAudio = Settings.Default.fileNameAudio + 1;
                    Settings.Default.Save();
                    
                }
            }
            pressUp(ref picAudio);
        }

        private void picAudio_MouseUp(object sender, MouseEventArgs e)
        {
           // pressUp(ref picAudio);
        }

        private void picCa1_MouseDown(object sender, MouseEventArgs e)
        {
            Console.WriteLine("picCa1 press");
            pressDown(ref picCa1);
            // camera 1
            pressCamera1 = true;
            changeToCamera1();
        }

        private void picCa1_MouseUp(object sender, MouseEventArgs e)
        {
            pressUp(ref picCa1);
            // camera 1
            pressCamera1 = false;
        }

        private void picCa2_MouseDown(object sender, MouseEventArgs e)
        {
            Console.WriteLine("picCa2 press");
            pressDown(ref picCa2);
            // camera 2
            pressCamera2 = true;
            changeToCamera2();
        }

        private void picCa2_MouseUp(object sender, MouseEventArgs e)
        {
            pressUp(ref picCa2);
            // camera 2
            pressCamera2 = false;
        }

        private void picLeft1_MouseDown(object sender, MouseEventArgs e)
        {
            Console.WriteLine("picLef1 press");
            pressDown(ref picLeft1);
            // left control 1
            pressLeftControl1 = true;
            controlEngine("left1");
        }

        private void picLeft1_MouseUp(object sender, MouseEventArgs e)
        {
            pressUp(ref picLeft1);
            // left control 1
            pressLeftControl1 = false;
            controlEngine("stop1");
        }

        private void picCenter1_MouseDown(object sender, MouseEventArgs e)
        {
            Console.WriteLine("picCenter1 press");
            pressDown(ref picCenter1);
            // center control 1
            pressCentralControl1 = !pressCentralControl1;
            if (pressCentralControl1)
                picOver.Visible = true;
            else
                picOver.Visible = false;
        }

        private void picCenter1_MouseUp(object sender, MouseEventArgs e)
        {
            pressUp(ref picCenter1);
        }

        private void picRight1_MouseDown(object sender, MouseEventArgs e)
        {
            Console.WriteLine("picRight1 press");
            pressDown(ref picRight1);
            // Right control 1
            pressRightControl1 = true;
            controlEngine("right1");
        }

        private void picRight1_MouseUp(object sender, MouseEventArgs e)
        {
            pressUp(ref picRight1);
            // Right control 1
            pressRightControl1 = false;
            controlEngine("stop1");
        }

        private void picMin_MouseDown(object sender, MouseEventArgs e)
        {
            Console.WriteLine("picMin press");
            picMax.ImageLocation = "..\\..\\Resources\\Images\\maxButton.png";
            picMin.ImageLocation = "..\\..\\Resources\\Images\\minPressButton.png";
            pressDown(ref picMin);
            // min control 2
            pressMinControl2 = true;
            Settings.Default.minmax = 0;
            controlEngine("min");
        }

        private void picMin_MouseUp(object sender, MouseEventArgs e)
        {
            pressUp(ref picMin);
            // min control 2
            pressMinControl2 = false;
        }

        private void picMax_MouseDown(object sender, MouseEventArgs e)
        {
            Console.WriteLine("picMax press");
            picMax.ImageLocation = "..\\..\\Resources\\Images\\maxPressButton.png";
            picMin.ImageLocation = "..\\..\\Resources\\Images\\minButton.png";
            pressDown(ref picMax);
            // max control 2
            pressMaxControl2 = true;
            Settings.Default.minmax = 1;
            controlEngine("max");
        }

        private void picMax_MouseUp(object sender, MouseEventArgs e)
        {
            pressUp(ref picMax);
            // max control 2
            pressMaxControl2 = false;
        }

        private void picLeft2_MouseDown(object sender, MouseEventArgs e)
        {
            Console.WriteLine("picLeft2 press");
            pressDown(ref picLeft2);
            // left control 2
            pressLeftControl2 = true;
            controlEngine("left2");
        }

        private void picLeft2_MouseUp(object sender, MouseEventArgs e)
        {
            pressUp(ref picLeft2);
            // left control 2
            pressLeftControl2 = false;
            controlEngine("stop2");
        }

        private void picUp2_MouseDown(object sender, MouseEventArgs e)
        {
            Console.WriteLine("picUp press");
            pressDown(ref picUp2);
            // up control 2
            pressUpControl2 = true;
            controlEngine("up2");
        }

        private void picUp2_MouseUp(object sender, MouseEventArgs e)
        {
            pressUp(ref picUp2);
            // up control 2
            pressUpControl2 = false;
            controlEngine("stop2");
        }

        private void picDown2_MouseDown(object sender, MouseEventArgs e)
        {
            Console.WriteLine("picDown press");
            pressDown(ref picDown2);
            // down control 2
            pressDownControl2 = true;
            controlEngine("down2");
        }

        private void picDown2_MouseUp(object sender, MouseEventArgs e)
        {
            pressUp(ref picDown2);
            // down control 2
            pressDownControl2 = false;
            controlEngine("stop2");
        }

        private void picRight2_MouseDown(object sender, MouseEventArgs e)
        {
            Console.WriteLine("picRight2 press");
            pressDown(ref picRight2);
            // right control 2
            pressRightControl2 = true;
            controlEngine("right2");
        }

        private void picRight2_MouseUp(object sender, MouseEventArgs e)
        {
            pressUp(ref picRight2);
            // right control 2
            pressRightControl2 = false;
            controlEngine("stop2");
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            Size oldSize = pictureBox1.Size;
            Size newSize = Size;
            pictureBox1.Size = Size;
            pictureBox1.Location = new Point(0, 0);
            int dw = newSize.Width - oldSize.Width;
            int dh = newSize.Height - oldSize.Height;
            RepositionControls(dw, dh);
            int sq = newSize.Height - 2 * 20;
            picOver.Size = new Size(sq, sq);
            //
            Point p = new Point();
            p.X = newSize.Width / 2 - picOver.Size.Width / 2;
            p.Y = newSize.Height / 2 - picOver.Size.Height / 2;
            if (!isFullScreen)
            {
                p.Y -= 18;
                //RepositionControls(0, -15);
            }
            else
            {
                //RepositionControls(0, +15);
            }
            picOver.Location = p;
        }
    }

    class SecretCombination
    {
        int keysCursor;
        int keysCount;
        string[] keys;
        DateTime prev;
        TimeSpan threshold;

        public SecretCombination(string pass)
        {
            keys = pass.Split('.');
            keysCursor = 0;
            keysCount = keys.Length;
            //
            setThreshold(800);
        }

        public void setThreshold(int milliseconds)
        {
            threshold = TimeSpan.FromMilliseconds(milliseconds);
        }

        public void addKey(int index, string k)
        {
            keys[index] = k;
            keysCursor = 0;
        }

        public bool feedKey(Keys e)
        {
            string hc = e.ToString();
            if (keys[keysCursor] == hc)
            {
                keysCursor += 1;
                if (keysCursor == 1)
                {
                    // Start chain
                    prev = DateTime.Now;
                }
                else
                if (keysCursor == keysCount)
                {
                    keysCursor = 0;
                    return true;
                }
                //
                if (keysCursor > 1)
                {
                    // Check time delta between this and prev keys
                    var next = DateTime.Now;
                    var delta = next - prev;
                    if (delta.CompareTo(threshold) > 0)
                    {
                        // too much time delta, reset the chain
                        keysCursor = 0;
                    }
                    prev = next;
                }
            }
            return false;
        }
    }
}