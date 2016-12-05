using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MathNet.Numerics.LinearAlgebra;
using System.Windows.Forms.DataVisualization.Charting;

namespace MathNetODE
{
    public partial class Form1 : Form
    {
        int t = -1;
        public PointF[,] points;
        double MaxX;
        double MaxY;

        public Form1()
        {
            InitializeComponent();
            MainChart.Series.Clear();
            
            //for(int i = 0; i<)
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            double x0 = Convert.ToDouble(textBox1.Text.Replace(".", ","));
            double y0 = Convert.ToDouble(textBox2.Text.Replace(".", ","));
            double r = Convert.ToDouble(textBox3.Text.Replace(".", ","));
            double K = Convert.ToDouble(textBox4.Text.Replace(".", ","));
            double omega = Convert.ToDouble(textBox5.Text.Replace(".", ","));
            double D = Convert.ToDouble(textBox6.Text.Replace(".", ","));
            double S = Convert.ToDouble(textBox7.Text.Replace(".", ","));
            double J = Convert.ToDouble(textBox8.Text.Replace(".", ","));
            double tEnd = Convert.ToDouble(textBox9.Text.Replace(".", ","));
            if (checkBox1.Checked)
            {                
                System.Windows.Forms.DataVisualization.Charting.Series newSeries1 = new System.Windows.Forms.DataVisualization.Charting.Series();
                newSeries1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                System.Windows.Forms.DataVisualization.Charting.Series newSeries2 = new System.Windows.Forms.DataVisualization.Charting.Series();
                newSeries2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                Result currentGraph = new Result(MathNet.Numerics.OdeSolvers.RungeKutta.FourthOrder(
                    Vector<double>.Build.Dense(new double[] { x0, y0 }),
                    0, tEnd, 3000,
                    (t, x) => Vector<double>.Build.Dense(new double[]
                    {
                    r * (1 - x[0]/K)*x[0] - ( omega * x[0] * x[1] )/( D + x[0] ),
                    S * (1 - ( J * x[1] )/x[0]) * x[1]
                    })));
                for (int i = 0; i < currentGraph.T.Length; i++)
                {
                    newSeries1.Points.AddXY((double)i/(currentGraph.T.Length)*tEnd, currentGraph.X[i]);
                    newSeries2.Points.AddXY((double)i/(currentGraph.T.Length)*tEnd, currentGraph.Y[i]);
                }
                MainChart.Series.Add(newSeries1);
                MainChart.Series.Add(newSeries2);
            }
            else
            {
                if (checkBox2.Checked)
                {
                    Series series2;
                    Series series3;
                    try
                    {
                        series2 = MainChart.Series["SeriesLine1"];
                        series3 = MainChart.Series["SeriesLine2"];
                    }
                    catch
                    {
                        series2 = new Series();
                        series3 = new Series();
                        series2.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.DashDotDot;
                        series2.BorderWidth = 2;
                        series2.ChartArea = "Default";
                        series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                        series2.Color = System.Drawing.Color.Gray;
                        series2.Legend = "Default";
                        series2.Name = "SeriesLine1";
                        series3.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.DashDotDot;
                        series3.BorderWidth = 2;
                        series3.ChartArea = "Default";
                        series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                        series3.Color = System.Drawing.Color.Gray;
                        series3.Legend = "Default";
                        series3.Name = "SeriesLine2";
                        this.MainChart.Series.Add(series2);
                        this.MainChart.Series.Add(series3);
                    }
                    series2.Points.Clear();
                    series3.Points.Clear();
                    series2.Points.AddXY(0, 0);
                    
                    //series3.Points.AddXY(0, a2 / b2);
                    //series3.Points.AddXY(a2 / c1, 0);
                    
                    for(double i = 0; i<=K; i+=0.1)
                    {
                        series3.Points.AddXY(i, Parabola(i, r, omega, K, D));
                    }
                    double rootX = MathNet.Numerics.RootFinding.Bisection.FindRoot((x)=> { return r / omega * (1 - x / K) * (D + x) - x / J; }, 0, K);
                    double rootY = rootX / J;
                    double k = K / rootX;
                    double d = D / rootX;
                    series2.Points.AddXY(rootX+0.3, (rootX + 0.3) / J);
                    if ( r * (k-d-2)/( k*(1+d) ) - S > 0 )
                    {
                        MessageBox.Show(String.Format( "Особая точка: \nX={0}\nY={1}\nНеустойчивый фокус, имеется предельный цикл", rootX, rootY ));
                    }
                    else
                    {
                        MessageBox.Show(String.Format("Особая точка: \nX={0}\nY={1}\nУстойчивый фокус, предельного цикла нет", rootX, rootY));
                    }
                }
                System.Windows.Forms.DataVisualization.Charting.Series newSeries = new System.Windows.Forms.DataVisualization.Charting.Series();
                newSeries.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                Result currentGraph = new Result(MathNet.Numerics.OdeSolvers.RungeKutta.FourthOrder(
                    Vector<double>.Build.Dense(new double[] { x0, y0 }),
                    0, tEnd, 3000,
                    (t, x) => Vector<double>.Build.Dense(new double[]
                    {
                    r * (1 - x[0]/K)*x[0] - ( omega * x[0] * x[1] )/( D + x[0] ),
                    S * (1 - ( J * x[1] )/x[0]) * x[1]
                    })));
                for (int i = 0; i < currentGraph.T.Length; i++)
                {
                    newSeries.Points.AddXY(currentGraph.X[i], currentGraph.Y[i]);
                }
                MainChart.Series.Add(newSeries);
            }
        }

        public double Parabola(double x1, double r, double omega, double K, double D)
        {
            return r / omega * (1 - x1 / K) * (D + x1);
        }

        public double Solve(double x1, double r, double omega, double K, double D, double J)
        {
            return r / omega * (1 - x1 / K) * (D + x1) - x1 / J;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            MainChart.Series.Clear();
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            MainChart.Size = new Size(this.Size.Width - 130, this.Size.Height - 50);// 1078; 662
            panel1.Location = new Point(MainChart.Width + 10, 40);
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            SerializeInfo info = new SerializeInfo(textBox1.Text, textBox2.Text, textBox3.Text, textBox4.Text, textBox5.Text, textBox6.Text,
                textBox7.Text, textBox8.Text, textBox9.Text, checkBox1.Checked, MainChart.Series.ToList<System.Windows.Forms.DataVisualization.Charting.Series>());
            var dialogResult = saveFileDialog.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            { 
                info.Save(saveFileDialog.FileName);
            }
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            var dialogResult = openFileDialog.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                SerializeInfo info1 = new SerializeInfo(openFileDialog.FileName);
                textBox1.Text = info1.text1;
                textBox2.Text = info1.text2;
                textBox3.Text = info1.text3;
                textBox4.Text = info1.text4;
                textBox5.Text = info1.text5;
                textBox6.Text = info1.text6;
                textBox7.Text = info1.text7;
                textBox8.Text = info1.text8;
                textBox9.Text = info1.text9;
                checkBox1.Checked = info1.@checked;
                for (int i = 0; i < info1.seriesInfoList.Count(); i++)
                {
                    info1.BuildSeriesCollection(MainChart.Series);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MainChart.Series.Remove(MainChart.Series.Last());
        }

        private void MainChart_MouseClick(object sender, MouseEventArgs e)
        {
            //MainChart.ChartAreas[0].AxisX.Maximum = (double)AutoSizeMode;
            //MainChart.ChartAreas[0].AxisY.Maximum = (double)AutoSizeMode;
            if (MainChart.Series.Count() != 0)
            {
                double r = Convert.ToDouble(textBox3.Text.Replace(".", ","));
                double K = Convert.ToDouble(textBox4.Text.Replace(".", ","));
                double omega = Convert.ToDouble(textBox5.Text.Replace(".", ","));
                double D = Convert.ToDouble(textBox6.Text.Replace(".", ","));
                double S = Convert.ToDouble(textBox7.Text.Replace(".", ","));
                double J = Convert.ToDouble(textBox8.Text.Replace(".", ","));
                double tEnd = Convert.ToDouble(textBox9.Text.Replace(".", ","));
                MainChart.ChartAreas[0].AxisX.Minimum = 0;
            MainChart.ChartAreas[0].AxisY.Minimum = 0;
            double x0 = ((double)e.X - 170)/(1739-170)*MainChart.ChartAreas[0].AxisX.Maximum;
            double y0 = (896 - (double)e.Y)/(896-49)*MainChart.ChartAreas[0].AxisY.Maximum;

                if (checkBox1.Checked)
                {
                    Series newSeries1 = new Series();
                    newSeries1.ChartType = SeriesChartType.Line;
                    Series newSeries2 = new Series();
                    newSeries2.ChartType = SeriesChartType.Line;
                    Result currentGraph = new Result(MathNet.Numerics.OdeSolvers.RungeKutta.FourthOrder(
                        Vector<double>.Build.Dense(new double[] { x0, y0 }),
                        0, tEnd, 3000,
                        (t, x) => Vector<double>.Build.Dense(new double[]
                        {
                    r * (1 - x[0]/K)*x[0] - ( omega * x[0] * x[1] )/( D + x[0] ),
                    S * (1 - ( J * x[1] )/x[0]) * x[1]
                        })));
                    for (int i = 0; i < currentGraph.T.Length; i++)
                    {
                        newSeries1.Points.AddXY((double)i / (currentGraph.T.Length) * tEnd, currentGraph.X[i]);
                        newSeries2.Points.AddXY((double)i / (currentGraph.T.Length) * tEnd, currentGraph.Y[i]);
                    }
                    MainChart.Series.Add(newSeries1);
                    MainChart.Series.Add(newSeries2);
                }
                else
                {
                    if (checkBox2.Checked)
                    {
                        ;
                    }
                    System.Windows.Forms.DataVisualization.Charting.Series newSeries = new System.Windows.Forms.DataVisualization.Charting.Series();
                    newSeries.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                    Result currentGraph = new Result(MathNet.Numerics.OdeSolvers.RungeKutta.FourthOrder(
                        Vector<double>.Build.Dense(new double[] { x0, y0 }),
                        0, tEnd, 3000,
                        (t, x) => Vector<double>.Build.Dense(new double[]
                        {
                    r * (1 - x[0]/K)*x[0] - ( omega * x[0] * x[1] )/( D + x[0] ),
                    S * (1 - ( J * x[1] )/x[0]) * x[1]
                        })));
                    for (int i = 0; i < currentGraph.T.Length; i++)
                    {
                        newSeries.Points.AddXY(currentGraph.X[i], currentGraph.Y[i]);
                    }
                    MainChart.Series.Add(newSeries);
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            t++;
            points = new PointF[MainChart.Series.Count, 3000];
            MaxX = double.MinValue;
            MaxY = double.MinValue;
            for(int i = 0; i<MainChart.Series.Count; i++)
            { 
                if ((MainChart.Series[i].Name == "SeriesLine1")||(MainChart.Series[i].Name == "SeriesLine2"))
                {
                    continue;
                }
                for(int j = 0; j<3000; j++)
                {
                    if (MainChart.Series[i].Points[j].XValue > MaxX)
                    {
                        MaxX = MainChart.Series[i].Points[j].XValue;
                    }
                    if (MainChart.Series[i].Points[j].YValues[0] > MaxY)
                    {
                        MaxY = MainChart.Series[i].Points[j].YValues[0];
                    }
                    points[i, j] = new PointF((float)MainChart.Series[i].Points[j].XValue, (float)MainChart.Series[i].Points[j].YValues[0]);
                }
                MainChart.Series[i].Points.Clear();
            }
            //MainChart.ChartAreas[0].AxisX.Maximum = MaxX;
            //MainChart.ChartAreas[0].AxisY.Maximum = MaxY;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            if (t >= 3000)
            {
                t = -1;
                timer1.Enabled = false;
            }
            for(int i = 0; i<points.GetLength(0); i++)
            {
                if ((MainChart.Series[i].Name == "SeriesLine1") || (MainChart.Series[i].Name == "SeriesLine2"))
                {
                    continue;
                }
                MainChart.Series[i].Points.AddXY(points[i, t].X, points[i, t].Y);
            }
            t+=10;
        }

        private void chart1_PostPaint(object sender, ChartPaintEventArgs e)
        {
            chart1.ChartAreas[0].AxisX.Minimum = -10;
            chart1.ChartAreas[0].AxisX.Maximum = 10;
            chart1.ChartAreas[0].AxisY.Minimum = -10;
            chart1.ChartAreas[0].AxisY.Maximum = 10;
            int sizeX = (int)((10 + 10) / 0.5);
            int sizeY = (int)((10 + 10) / 0.5);
            int sizeZ = sizeX * sizeY;
            double tempX, tempY;
            List<Point3D> list = new List<Point3D>();
            Point3D point3d;
            for (int i = 0; i < sizeX; i++)
            {
                tempX = chart1.ChartAreas[0].AxisX.ValueToPosition(-10 + i * 0.5);
                for (int j = 0; j < sizeY; j++)
                {
                    tempY = chart1.ChartAreas[0].AxisX.ValueToPosition(-10 + j * 0.5);
                    point3d = new Point3D((float)tempX, (float)tempY, (float)(tempX * tempX / 4 - tempY * tempY / 9));
                    list.Add(point3d);
                }
            }
            //double x = chart1.ChartAreas[0].AxisX.ValueToPosition(10);
            //double y = chart1.ChartAreas[0].AxisY.ValueToPosition(10);
            //double z = chart1.ChartAreas[0].GetSeriesZPosition(chart1.Series[0]);
            //point3d = new Point3D((float)x, (float)y, (float)z);
            chart1.ChartAreas[0].TransformPoints(list.ToArray());
            for (int i = 0; i < list.Count; i++)
            {
                var temp = e.ChartGraphics.GetAbsolutePoint(new PointF(list[i].X, list[i].Y));
                e.ChartGraphics.Graphics.FillEllipse(Brushes.Black, temp.X, temp.Y, 1, 1);
            }// e.ChartGraphics.GetAbsolutePoint();
        }
    }
}
