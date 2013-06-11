using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Runtime.InteropServices;
using GetCoreTempInfoNET;
using System.Diagnostics;
using System.Threading;

namespace Arduino_PC_Monitor
{
    public partial class Form1 : Form
    {      
        SerialPort port;
        static CoreTempInfo CTInfo;
        GpuzWrapper gpuz;
        Graph CPUusage_Graph;
        Graph GPUusage_Graph;
        Graph CPUtemp_Graph;
        Graph GPUtemp_Graph;
        Graph Downloadspeed_Graph;
        Graph RamUsed_Graph;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.SetStyle(
                System.Windows.Forms.ControlStyles.UserPaint |
                System.Windows.Forms.ControlStyles.AllPaintingInWmPaint |
                System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer,
                true);
            if (!IsProcessOpen("Core Temp"))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo("Core Temp.exe");
                startInfo.WindowStyle = ProcessWindowStyle.Minimized;

                Process.Start(startInfo);
            }

            if (!IsProcessOpen("GPU-Z"))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo("GPU-Z.0.7.1.exe");
                startInfo.Arguments = "-minimized";

                Process.Start(startInfo); 
            }

            Thread.Sleep(5000);

            CTInfo = new CoreTempInfo();

            gpuz = new GpuzWrapper();
            gpuz.Open();

            port = new SerialPort("COM3", 9600, Parity.None, 8, StopBits.One);
            button1.PerformClick();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!port.IsOpen)
            {
                button1.Text = "Stop";
                timer1.Enabled = true;
                label1.Text = "Started";
                label1.ForeColor = Color.Green;

                port.Open();
            }
            else
            {
                button1.Text = "Start";
                timer1.Enabled = false;
                label3.Text = "0";
                label1.Text = "Not started";
                label1.ForeColor = Color.Black;

                port.Write("r");
                port.Close();
            }
        }

        float totalbytes = 0;

        private void timer1_Tick(object sender, EventArgs e)
        {
            CTInfo.GetData();
            if (port.IsOpen)
            {
                label3.Text = (int.Parse(label3.Text) + 1).ToString();
                port.Write("r");
                int cpuusage = (int)Math.Round(performanceCounter1.NextValue(), 0);
                int gpuusage = (int)Math.Round(gpuz.GetUsage(), 0);

                string usage = cpuusage + "," + gpuusage + "%";

                port.Write(usage);

                int UsedRam = GetUsedRam();

                string ram = UsedRam + "Mb";
                for (int i = 1; i <= 16 - usage.Length - ram.Length; i++)
                    port.Write(" ");
                port.Write(ram);
                port.Write("n");
                int kbs = (int)Math.Round(performanceCounter3.NextValue() / 1024f, 0);

                string kbsec = kbs + "Kb";
                totalbytes += performanceCounter3.NextValue();
                port.Write(kbsec);
                int cputemp = (int)Math.Round(CTInfo.GetTemp[0], 0);
                int gputemp = (int)Math.Round(gpuz.GetTemp(), 0);
                string temp = " " + cputemp + "," + gputemp + "C";

                port.Write(temp);
                string totalmega = Math.Round((totalbytes / 1024f) / 1024f, 0).ToString() + "Mb";
                for (int i = 1; i <= 16 - temp.Length - kbsec.Length - totalmega.Length; i++)
                    port.Write(" ");
                port.Write(totalmega);

                CPUusage_Graph.AddValue(cpuusage);
                GPUusage_Graph.AddValue(gpuusage);

                if (UsedRam > RamUsed_Graph.highest)
                {
                    RamUsed_Graph.highest = UsedRam;
                    RamUsed_Graph.mid = (int)Math.Round((double)UsedRam / 2, 0);
                }

                RamUsed_Graph.AddValue(UsedRam);

                if (kbs > Downloadspeed_Graph.highest)
                {
                    Downloadspeed_Graph.highest = kbs;
                    Downloadspeed_Graph.mid = (int)Math.Round((double)kbs / 2, 0);
                }

                Downloadspeed_Graph.AddValue(kbs);

                if (cputemp > CPUtemp_Graph.highest)
                {
                    CPUtemp_Graph.highest = cputemp;
                    CPUtemp_Graph.mid = (int)Math.Round((double)cputemp / 2, 0);
                }

                CPUtemp_Graph.AddValue(cputemp);

                if (gputemp > GPUtemp_Graph.highest)
                {
                    GPUtemp_Graph.highest = gputemp;
                    GPUtemp_Graph.mid = (int)Math.Round((double)gputemp / 2, 0);
                }

                GPUtemp_Graph.AddValue(gputemp);
            }
        }

        int GetUsedRam()
        {
            return (int)Math.Round((new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory / 1024f) / 1024f, 0) - (int)Math.Round(performanceCounter2.NextValue(), 0);
        }

        public bool IsProcessOpen(string name)
        {
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName.Contains(name))
                {
                    return true;
                }
            }
            return false;
        }
        
        private void Form1_Shown(object sender, EventArgs e)
        {
            CPUusage_Graph = new Graph(panel1, 0, 50, 100);
            GPUusage_Graph = new Graph(panel2, 0, 50, 100);
            CPUtemp_Graph = new Graph(panel3, 0, -1, -1);
            GPUtemp_Graph = new Graph(panel4, 0, -1, -1);
            Downloadspeed_Graph = new Graph(panel5, 0, -1, -1);
            RamUsed_Graph = new Graph(panel6, 0, -1, -1);
        }
    }
}
