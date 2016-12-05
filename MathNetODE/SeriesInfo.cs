using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms.DataVisualization.Charting;

namespace MathNetODE
{
    [DataContract]
    public class SeriesInfo
    {
        [DataMember]
        public List<MyPoint> points;
        [DataMember]
        public Color color;
        [DataMember]
        public SeriesChartType chartType;
        [DataMember]
        public int borderWidth;

        public SeriesInfo()
        {
            points = new List<MyPoint>();
        }

        public SeriesInfo(Series series)
        {
            points = new List<MyPoint>();
            for (int i = 0; i< series.Points.Count; i++)
            {
                points.Add(new MyPoint(series.Points[i].XValue, series.Points[i].YValues[0]));
            }
            this.color = series.Color;
            this.chartType = series.ChartType;
            this.borderWidth = series.BorderWidth;
        }
    }
}