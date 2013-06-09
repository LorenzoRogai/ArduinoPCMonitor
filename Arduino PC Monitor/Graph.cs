using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Arduino_PC_Monitor
{
    class Graph
    {
        Panel p;
        public int lowest;
        public int mid;
        public int highest;
        Point startPoint;
        List<int> Values = new List<int>();
      
        public Graph(Panel p, int lowest, int mid, int highest)
        {
            p.Paint += new System.Windows.Forms.PaintEventHandler(Paint);
            this.p = p;
            this.lowest = lowest;
            this.mid = mid;
            this.highest = highest;
            this.startPoint = GetPixelByPercentage(p, 5, 75);           
            DrawGrid(p.CreateGraphics());
        }

        private void Paint(object sender, PaintEventArgs e)
        {
            DrawGrid(e.Graphics);

            Point lastpoint = startPoint;
            int X = startPoint.X;

            for (int i = 0; i < Values.Count; i++)
            {
                int Value = Values[i];
                int percentage = (int)Math.Round((double)100 * (highest - Value) / highest, 0);
                Point point = new Point(X, GetPixelByPercentage(p, 0, percentage, p.Width - startPoint.X, p.Height - (p.Height - startPoint.Y)).Y);
                if (i != 0)
                    e.Graphics.DrawLine(new Pen(Color.Gray, (float)1.5), lastpoint, point);
                lastpoint = point;
                X += 3;
            }

            X = startPoint.X;
            lastpoint = startPoint;
        }

        public void UpdateValues(int lowest, int mid, int highest)
        {
            this.lowest = lowest;
            this.mid = mid;
            this.highest = highest;
        }

        void DrawGrid(Graphics g)
        {
            SolidBrush br = new SolidBrush(Color.Black);
            Font f = new Font("Tahoma", 5);
            g.DrawString("10", f, br, GetPixelByPercentage(p, 10, 80));
            g.DrawString("20", f, br, GetPixelByPercentage(p, 20, 80));
            g.DrawString("30", f, br, GetPixelByPercentage(p, 30, 80));
            g.DrawString("40", f, br, GetPixelByPercentage(p, 40, 80));
            g.DrawString("50", f, br, GetPixelByPercentage(p, 50, 80));
            g.DrawString("60", f, br, GetPixelByPercentage(p, 60, 80));
            g.DrawString("70", f, br, GetPixelByPercentage(p, 70, 80));
            g.DrawString("80", f, br, GetPixelByPercentage(p, 80, 80));
            g.DrawString("90", f, br, GetPixelByPercentage(p, 90, 80));

            g.DrawLine(new Pen(Color.Black, 1), GetPixelByPercentage(p, 5, 0), GetPixelByPercentage(p, 5, 100));

            if (highest == -1)
                g.DrawString("N/D", f, br, GetPixelByPercentage(p, 0, 0));
            else
                g.DrawString(highest.ToString(), f, br, GetPixelByPercentage(p, 0, 0));
            if (mid == -1)
                g.DrawString("N/D", f, br, GetPixelByPercentage(p, 0, 31));
            else
                g.DrawString(mid.ToString(), f, br, GetPixelByPercentage(p, 0, 31));
            g.DrawString(lowest.ToString(), f, br, GetPixelByPercentage(p, 0, 62));

            g.DrawLine(new Pen(Color.Black, 1), GetPixelByPercentage(p, 0, 78), GetPixelByPercentage(p, 100, 78));
        }

        private Point GetPixelByPercentage(Panel p, int widthPercentage, int heightPercentage, int width = -1, int height = -1)
        {
            if (width == -1 && height == -1)
                return new Point((int)Math.Round((double)p.Width * widthPercentage / 100, 0), (int)Math.Round((double)p.Height * heightPercentage / 100, 0));
            else
                return new Point((int)Math.Round((double)width * widthPercentage / 100, 0), (int)Math.Round((double)height * heightPercentage / 100, 0));
        }
    
        public void AddValue(int value)
        {
            Values.Add(value);

            if (Values.Count == 100)
            {
                Values.RemoveAt(0);
                for (int i = 1; i < Values.Count; i++)
                {
                    Values[i - 1] = Values[i];
                }
            }

            p.Refresh();
        }
    }
}
