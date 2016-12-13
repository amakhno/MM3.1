using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MathNet.Numerics;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;
using MathNet.Numerics.LinearAlgebra;
using System.Diagnostics;

namespace MathNetODE
{
    public partial class Form1 : Form
    {
        int t = -1;
        public PointF[,] points;
        double MaxX;
        double MaxY;
        bool isWrite = false;
        int countOfPoint = 1;
        double rPrev = -1;
        double rNext = -1;
        double rCurrent = -1;
        List<double> deltas = new List<double>();


        public Form1()
        {
            InitializeComponent();
            radioButton1.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            double x0 = Convert.ToDouble(textBox1.Text.Replace(".", ","));
            double y0 = Convert.ToDouble(textBox3.Text.Replace(".", ","));
            double z0 = Convert.ToDouble(textBox4.Text.Replace(".", ","));
            double step = Convert.ToDouble(textBox2.Text.Replace(".", ","));
            isWrite = checkBoxWrite.Checked;
            int N = Convert.ToInt32(textBoxT.Text.Replace(".", ","));
            MainChart.Series[0].Points.Clear();
            if (radioButton1.Checked)
            {
                MainChart.Series[0].ChartType = SeriesChartType.FastPoint;
                if (!isWrite)
                {
                    rPrev = -1;
                    rNext = -1;
                    rCurrent = -1;
                    countOfPoint = 1;
                    for (double r = 0.5; r < 1; r += step)
                    {                        
                        DrawSolutionForFeghen(x0, N, r, step);
                    }
                    MessageBox.Show("delta = " + ((rCurrent - rPrev) / (rNext - rCurrent)).ToString());
                }
                else
                {
                    StreamWriter writer = new StreamWriter("out1.txt");
                    for (double r = 0.1; r < 1; r += step)
                    {
                        DrawSolutionForFeghen(x0, N, r, writer);
                    }
                    writer.Close();
                }
            }
            if (radioButton3.Checked)
            {
                MainChart.Series[0].ChartType = SeriesChartType.FastLine;
                if (!isWrite)
                {
                    DrawSolutionForLorenz(x0, y0, z0, N, 28, 10, 8.0 / 3.0);
                }
            }
        }



        public void DrawSolutionForFeghen(double x0, double N, double r, double step)
        {
            double xPrevious;
            double xCurrent = x0;
            double[] Points = new double[100];
            for (int i = 0; i < N; i++)
            {
                xPrevious = xCurrent;
                xCurrent = 4 * r * xPrevious * (1 - xPrevious);
            }
            for (int i = 0; i < 100; i++)
            {
                xPrevious = xCurrent;
                xCurrent = 4 * r * xPrevious * (1 - xPrevious);
                Points[i] = xCurrent;
            }
            var EPoints = Points.Distinct().ToArray();
            for(int i = 0; i< EPoints.Length; i++)
            {
                EPoints[i] = Convert.ToDouble(EPoints[i].ToString(textBox5.Text)); //G2 - 0.001 //G6 - 0.0001 //
            }
            EPoints = EPoints.Distinct().ToArray();
            if (EPoints.Count() > countOfPoint)
            {
                countOfPoint *= 2;
                bool isAlreadyWrite = false;
                if(rPrev == -1)
                {
                    rPrev = r;
                    isAlreadyWrite = true;
                }
                if ((rCurrent == -1) && (rPrev != -1) && !isAlreadyWrite)
                {
                    rCurrent = r;
                    isAlreadyWrite = true;
                }
                if ((rNext == -1) && (rCurrent != -1) && (rPrev != -1) && !isAlreadyWrite)
                {
                    rNext = r;
                }
            }            
                foreach (double point in EPoints)
            {
                MainChart.Series[0].Points.AddXY(r, point);
            }
        }

        public void DrawSolutionForFeghen(double x0, double N, double r, StreamWriter writer)
        {
            double xPrevious;
            double xCurrent = x0;
            double[] Points = new double[100];
            for (int i = 0; i < N; i++)
            {
                xPrevious = xCurrent;
                xCurrent = 4 * r * xPrevious * (1 - xPrevious);
            }
            for (int i = 0; i < 100; i++)
            {
                xPrevious = xCurrent;
                xCurrent = 4 * r * xPrevious * (1 - xPrevious);
                Points[i] = xCurrent;
            }
            var EPoints = Points.Distinct();
            foreach (double point in EPoints)
            {
                MainChart.Series[0].Points.AddXY(r, point);
                writer.WriteLine(r.ToString().Replace(",", ".") + " " + point.ToString().Replace(",", "."));
            }
        }

        public void DrawSolutionForHenon(double x0, double y0, double N, double a)
        {
            double xPrevious;
            double xCurrent = x0;
            double yPrevious;
            double yCurrent = y0;
            double[] xPoints = new double[100];
            double[] yPoints = new double[100];
            for (int i = 0; i < N; i++)
            {
                xPrevious = xCurrent;
                yPrevious = yCurrent;
                xCurrent = 1 - a * xPrevious * xPrevious + yPrevious;
                yCurrent = xPrevious;
            }
            for (int i = 0; i < 100; i++)
            {
                xPrevious = xCurrent;
                yPrevious = yCurrent;
                xCurrent = 1 - a * xPrevious * xPrevious + yPrevious;
                yCurrent = xPrevious;
                xPoints[i] = xCurrent;
                yPoints[i] = yCurrent;
            }
            var E1Points = xPoints.Distinct().ToArray();
            var E2Points = yPoints.Distinct().ToArray();
            for (int i = 0; i < xPoints.Length; i++)
            {
                for (int j = 0; j < yPoints.Length; j++)
                {
                    MainChart.Series[0].Points.AddXY(xPoints[i], yPoints[j]);
                }
            }
        }

        public void DrawSolutionForLorenz(double x0, double y0, double z0, int N, double r, double sigma, double b)
        {
            var result = MathNet.Numerics.OdeSolvers.RungeKutta.FourthOrder(
                    Vector<double>.Build.Dense(new double[] { x0, y0, z0 }),
                    0, N, N * 100,
                    (t, x) => Vector<double>.Build.Dense(new double[]
                    {
                    -sigma*x[0] + sigma * x[1],
                    -x[0]*x[2]+r*x[0]-x[1],
                    x[0]*x[1] - b*x[2]
                    }));
            double[] X = new double[result.Length];
            double[] Y = new double[result.Length];
            double[] Z = new double[result.Length];
            double[] T = new double[result.Length];
            StreamWriter writer = new StreamWriter("out3.txt");
            for (int i = 0; i < result.Length; i++)
            {
                double[] a = result[i].ToArray();
                X[i] = a[0];
                Y[i] = a[1];
                Z[i] = a[2];
                MainChart.Series[0].Points.AddXY(i/100, X[i]);
                writer.WriteLine(X[i].ToString().Replace(",",".") + " " + Y[i].ToString().Replace(",", ".") + " " + Z[i].ToString().Replace(",", "."));
            }
            writer.Close();
        }
    }
}
