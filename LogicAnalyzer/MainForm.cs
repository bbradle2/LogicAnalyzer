using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using CyUSB;
using ByteExtensionMethods;
using SharpGL;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace LogicAnalyzer
{

    public partial class MainForm : Form
    {


        public class Signal
        {
            public String name;
            public bool active;
            public Color color;
            public UInt32 N;
            public byte[] data;
        }

        
        //[DllImport("SpeedCapture.dll", SetLastError = true)]
        //unsafe static extern void CaptureBuffer(byte[] captureBufOut, byte[] inData, int xFerSize, int maxBufferLength, int start);

        const int nSignals = 8;
        CyUSBDevice logicDevice = null;
        USBDeviceList usbDevices = null;
        CyBulkEndPoint inEndpoint = null;
        bool bRunning = false;
        const int XFERSIZE = 16384 * 4;
        //const int XFERSIZE = 16384 * 4; // l sec sample
        const int MaxCapture = XFERSIZE * 1463; // 1 sec sample

        int zoomXMin = 0;
        int zoomXMax = (XFERSIZE) - 1;
        int zoomCenter = XFERSIZE;

        Signal[] signals;

        Thread tXfers;

        delegate void UpdateUICallback(int inCount);
        UpdateUICallback updateUI;

        Color[] someColors = new Color[nSignals] {
        Color.Red, Color.Violet, Color.Green, Color.Yellow, Color.Tomato, Color.Teal, Color.Salmon, Color.RoyalBlue};

        Stopwatch timer;

        //byte[] captureBuffer = new byte[MaxCapture];

        public MainForm()
        {
            InitializeComponent();

            updateUI = new UpdateUICallback(StatusUpdate);
            signals = new Signal[nSignals];

            usbDevices = new USBDeviceList(CyConst.DEVICES_CYUSB);
            usbDevices.DeviceAttached += UsbDevices_DeviceAttached;
            usbDevices.DeviceRemoved += UsbDevices_DeviceRemoved;

            InitializeSignals();
            wavePanel.FrameRate = 120;
            StartBtn.BackColor = Color.Green;

            setDevice();
        }

        private void InitializeSignals()
        {

            for (int i = 0; i < nSignals; i++)
            {
                signals[i] = new Signal();
                signals[i].data = new byte[MaxCapture];
                signals[i].active = true;
                signals[i].color = someColors[i];
                //mySignals[i].name = sigLabels[i].Text;
            }
         }

        private void UsbDevices_DeviceRemoved(object sender, EventArgs e)
        {
            setDevice();
        }

        private void UsbDevices_DeviceAttached(object sender, EventArgs e)
        {
            setDevice();
        }

        public void setDevice()
        {
            int nCurSelection = 0;
            if (cboDeviceConnected.Items.Count > 0)
            {
                nCurSelection = cboDeviceConnected.SelectedIndex;
                cboDeviceConnected.Items.Clear();
                cboINEndpoint.Items.Clear();
            }
            int nDeviceList = usbDevices.Count;
            for (int nCount = 0; nCount < nDeviceList; nCount++)
            {
                USBDevice fxDevice = usbDevices[nCount];
                string strmsg;
                strmsg = "(0x" + fxDevice.VendorID.ToString("X4") + " - 0x" + fxDevice.ProductID.ToString("X4") + ") " + fxDevice.FriendlyName;
                cboDeviceConnected.Items.Add(strmsg);
            }

            if (cboDeviceConnected.Items.Count > 0)
                cboDeviceConnected.SelectedIndex = nCurSelection;

            logicDevice = usbDevices[cboDeviceConnected.SelectedIndex] as CyUSBDevice;

            //StartBtn.Enabled = (loopDevice != null);

            if (logicDevice != null)
                Text = logicDevice.FriendlyName;
            else
                Text = "C# Bulkloop - no device";

            if (logicDevice != null) GetEndpointsOfNode(logicDevice.Tree);
            if (cboINEndpoint.Items.Count > 0) cboINEndpoint.SelectedIndex = 0;
            
            ConstructEndpoints();
        }


        private void GetEndpointsOfNode(TreeNode devTree)
        {
            cboINEndpoint.Items.Clear();

            foreach (TreeNode node in devTree.Nodes)
            {
                if (node.Nodes.Count > 0)
                    GetEndpointsOfNode(node);
                else
                {
                    CyUSBEndPoint ept = node.Tag as CyUSBEndPoint;
                    if (ept == null)
                    {
                        //return;
                    }
                    else if (node.Text.Contains("0x81"))
                    {
                        
                        CyUSBInterface ifc = node.Parent.Tag as CyUSBInterface;
                        string s = string.Format("ALT-{0}, {1} Byte {2}", ifc.bAlternateSetting, ept.MaxPktSize, node.Text);
                        cboINEndpoint.Items.Add(s);
                    }

                }
            }

        }

        private void ConstructEndpoints()
        {
            if (logicDevice != null && cboINEndpoint.Items.Count > 0)
            {

                string sAltIn = cboINEndpoint.Text.Substring(4, 1);
                byte inAltInferface = Convert.ToByte(sAltIn);

                // Get the endpoint
                int aX = cboINEndpoint.Text.LastIndexOf("0x");
                string sAddr = cboINEndpoint.Text.Substring(aX, 4);
                byte addrIn = (byte)Util.HexToInt(sAddr);

                inEndpoint = logicDevice.EndPointOf(addrIn) as CyBulkEndPoint;

                if ((inEndpoint != null))
                {
                    //make sure that the device configuration doesn't contain anything other than bulk endpoint
                    if ((inEndpoint.Attributes & 0x03) != 0x02)
                    {
                        Text = "Device Configuration mismatch";
                        StartBtn.Enabled = false;
                        return;
                    }
                    inEndpoint.TimeOut = 1000;
                }
                else
                {

                    Text = "Device Configuration mismatch";
                    StartBtn.Enabled = false;
                    return;
                }

            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            // If close was selected while running the loopback, shut it down.
            if (bRunning)
                StartBtn_Click(this, null);

            if (usbDevices != null) usbDevices.Dispose();
        }

        private void StartBtn_Click(object sender, EventArgs e)
        {
            if(cboDeviceConnected.SelectedItem == null)
            {
                MessageBox.Show(this, "Please Select a Device", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if(cboINEndpoint.SelectedItem == null)
            {
                MessageBox.Show(this, "Please Select EndPoint", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!bRunning)
            {
                bRunning = true;
                StartBtn.Text = "Stop";
                StartBtn.BackColor = Color.Red;

                //creates new thread
                tXfers = new Thread(new ThreadStart(TransferThread))
                {
                    IsBackground = false,
                    Priority = ThreadPriority.Highest
                };

                ////Starts the new thread
                tXfers.Start();
            }
            else
            {
                //Makes the thread stop and aborts the thread
                bRunning = false;
                StartBtn.Text = "Start";
                StartBtn.BackColor = Color.Green;

                if (tXfers == null) return;

                if (tXfers.IsAlive)
                {
                    tXfers.Abort();
                    tXfers.Join();
                    tXfers = null;
                }
            }
        }

        public void StatusUpdate(int inCount)
        {
           
            lblSuccess.Text = string.Format("{0:n0} Bytes", inCount);
            timer.Stop();
            Text = timer.ElapsedMilliseconds.ToString();
            wavePanel.DoRender();
           
            StartBtn.Text = bRunning ? "Stop" : "Start";
            StartBtn.BackColor = bRunning ? Color.Red : Color.Green;
        }

        public void TransferThread()
        {
            int xferLen = XFERSIZE;
            
            bool bResult = true;
            bRunning = true;
            int inCount = 0;
            int start = 0;
            GC.Collect();
            timer = Stopwatch.StartNew();

            while (bResult)
            {
                
                inEndpoint.TimeOut = 1000;
                byte[] outData = null;
                outData = new byte[XFERSIZE];
                
                bResult = inEndpoint.XferData(ref outData, ref xferLen, false);
                
                if (inCount < MaxCapture)
                {
          
                    Parallel.For(start, outData.Length, addr => 
                    {
                        object obj = addr;
                        lock (obj)
                        {
                            signals[0].data[start + addr] = (byte)((outData[addr] & (1 << 0)) >> 0);
                            signals[1].data[start + addr] = (byte)((outData[addr] & (1 << 1)) >> 1);
                            signals[2].data[start + addr] = (byte)((outData[addr] & (1 << 2)) >> 2);
                            signals[3].data[start + addr] = (byte)((outData[addr] & (1 << 3)) >> 3);
                            signals[4].data[start + addr] = (byte)((outData[addr] & (1 << 4)) >> 4);
                            signals[5].data[start + addr] = (byte)((outData[addr] & (1 << 5)) >> 5);
                            signals[6].data[start + addr] = (byte)((outData[addr] & (1 << 6)) >> 6);
                            signals[7].data[start + addr] = (byte)((outData[addr] & (1 << 7)) >> 7);
                        }
                    });

                    start += outData.Length;
                  
                }
                else
                {
                    bRunning = false;
                }

                inCount += xferLen;
               
                if (bRunning == false)
                {
                    Invoke(updateUI, inCount);
                    break;
                }
            }
        }
       
        private void wavePanel_DoubleClick(object sender, EventArgs e)
        {
            MouseEventArgs me = e as MouseEventArgs;
            int zoomwidth = zoomXMax - zoomXMin + 1;
            int xmin, xmax;

            zoomCenter = zoomXMin + (int)Math.Round(me.X * zoomwidth / (double)wavePanel.Width);
            xmin = zoomCenter - zoomwidth / 2;
            xmax = zoomCenter + zoomwidth / 2;
            if (xmin < 0)
            {
                xmin = 0;
            }
            if (xmax >= XFERSIZE)
            {
                xmax = XFERSIZE - 1;
            }
            // If we click too close to the edge at the current zoom setting
            // we can't do it without rescaling also.
            if ((xmax - xmin + 1) == zoomwidth)
            {
                zoomXMin = xmin;
                zoomXMax = xmax;
                wavePanel.DoRender();
            }
        }

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            if ((zoomXMax - zoomXMin) <= 16)
                return;

            // Scale the range by a constant factor
            zoomCenter = (zoomXMin + zoomXMax) / 2;
            int newHalfRange = (int)Math.Round(0.5 * 0.5 * (zoomXMax - zoomXMin + 1));
            zoomXMin = zoomCenter - newHalfRange;
            zoomXMax = zoomCenter + newHalfRange;
            if (zoomXMin < 0)
            {
                zoomXMin = 0;
            }
            if (zoomXMax >= XFERSIZE)
            {
                zoomXMax = XFERSIZE - 1;
            }
            wavePanel.DoRender();
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            zoomCenter = (zoomXMin + zoomXMax) / 2;
            int newHalfRange = (int)Math.Round(0.5 * 2.0 * (zoomXMax - zoomXMin + 1));
            zoomXMin = zoomCenter - newHalfRange;
            zoomXMax = zoomCenter + newHalfRange;
            if (zoomXMin < 0)
            {
                zoomXMin = 0;
            }
            if (zoomXMax >= XFERSIZE)
            {
                zoomXMax = XFERSIZE - 1;
            }
            wavePanel.DoRender();
        }


        private void wavePanel_OpenGLDraw(object sender, SharpGL.RenderEventArgs args)
        {
            
            OpenGL GL = wavePanel.OpenGL;
            int dy_sig = wavePanel.Height / (nSignals + 1);

            GL.Viewport(0, 0, wavePanel.Width, wavePanel.Height);

            GL.MatrixMode(OpenGL.GL_PROJECTION);
            GL.LoadIdentity();

            GL.Ortho(0, wavePanel.Width, wavePanel.Height, 0, -1, 1);
            GL.MatrixMode(OpenGL.GL_MODELVIEW);
            GL.LoadIdentity();

            GL.Clear(OpenGL.GL_COLOR_BUFFER_BIT);
            
            GL.Begin(OpenGL.GL_LINES);
            
            for (int i = 1; i < nSignals + 1; i++)
            {
                GL.Color(Color.White.R, Color.White.G, Color.White.B);
                GL.Vertex(0.0f, i * dy_sig - 1, 0.0f); // origin of the line
                GL.Color(Color.White.R, Color.White.G, Color.White.B);
                GL.Vertex(wavePanel.Width, i * dy_sig - 1, 0.0f); // ending point of the line
            }

            double pdx = wavePanel.Width / (double)(zoomXMax - zoomXMin);
           
            int x1, y1, x2, y2;
            for (int i = 0; i < nSignals; i++)
            {
                if (signals[i].data == null) break;
                for (int p = zoomXMin; p < zoomXMax; p++)
                {
                    x1 = (int)Math.Round(pdx * (double)(p - zoomXMin));
                    x2 = (int)Math.Round(pdx * (double)(p + 1 - zoomXMin));
                    y1 = (int)Math.Round((i + 2) * dy_sig - 0.25 * dy_sig - signals[i].data[p] * 0.5 * dy_sig);
                    y2 = (int)Math.Round((i + 2) * dy_sig - 0.25 * dy_sig - signals[i].data[p] * 0.5 * dy_sig);
                    GL.Color(signals[i].color.R, signals[i].color.G, signals[i].color.B);
                    GL.Vertex(x1, y1, 0.0f); // origin of the line
                    GL.Color(signals[i].color.R, signals[i].color.G, signals[i].color.B);
                    GL.Vertex(x2, y2, 0.0f); // ending point of the line
                    
                    if (signals[i].data[p + 1] != signals[i].data[p])
                    {
                        x1 = x2 = (int)Math.Round(pdx * (p + 1 - zoomXMin));
                        y1 = (int)Math.Round((i + 2) * dy_sig - 0.25 * dy_sig - signals[i].data[p] * 0.5 * dy_sig);
                        y2 = (int)Math.Round((i + 2) * dy_sig - 0.25 * dy_sig - signals[i].data[p + 1] * 0.5 * dy_sig);
                        GL.Color(signals[i].color.R, signals[i].color.G, signals[i].color.B);
                        GL.Vertex(x1, y1, 0.0f); // origin of the line
                        GL.Color(signals[i].color.R, signals[i].color.G, signals[i].color.B);
                        GL.Vertex(x2, y2, 0.0f); // ending point of the line

                    }
                }
            }
            
            GL.End();
            GL.Flush();

        }

        private void Form1_FormClosing_1(object sender, FormClosingEventArgs e)
        {
            if (StartBtn.Text == "Stop")
            {
                StartBtn.PerformClick();
            }
        }

        private void btnPanLeft_Click(object sender, EventArgs e)
        {
            int zoomwidth = zoomXMax - zoomXMin;
            int xmin = zoomXMin, xmax = zoomXMax;
            int dx = (int)Math.Round(0.2 * zoomwidth);
            xmin -= dx;
            xmax -= dx;
            if (xmin < 0)
            {
                xmin = 0;
            }
            if (xmax >= XFERSIZE)
            {
                xmax = XFERSIZE - 1;
            }
            if ((xmax - xmin) == zoomwidth)
            {
                zoomXMin = xmin;
                zoomXMax = xmax;
                zoomCenter = (xmax + xmin) / 2;
                wavePanel.DoRender();
            }
        }

        private void btnPanRight_Click(object sender, EventArgs e)
        {
            int zoomwidth = zoomXMax - zoomXMin;
            int xmin = zoomXMin, xmax = zoomXMax;
            int dx = (int)Math.Round(0.2 * zoomwidth);
            xmin += dx;
            xmax += dx;
            if (xmin < 0)
            {
                xmin = 0;
            }
            if (xmax >= XFERSIZE)
            {
                xmax = XFERSIZE - 1;
            }
            if ((xmax - xmin) == zoomwidth)
            {
                zoomXMin = xmin;
                zoomXMax = xmax;
                zoomCenter = (xmax + xmin) / 2;
                wavePanel.DoRender();
            }
        }

        private void btnZoomFull_Click(object sender, EventArgs e)
        {
            zoomXMin = 0;
            zoomXMax = XFERSIZE - 1;
            wavePanel.DoRender();
        }
    }
}
