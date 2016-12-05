using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;
using System.Collections;

namespace MathNetODE
{
    [DataContract]
    class SerializeInfo
    {
        [DataMember]
        public string text1;
        [DataMember]
        public string text2;
        [DataMember]
        public string text3;
        [DataMember]
        public string text4;
        [DataMember]
        public string text5;
        [DataMember]
        public string text6;
        [DataMember]
        public string text7;
        [DataMember]
        public string text8;
        [DataMember]
        public string text9;
        [DataMember]
        public bool @checked;
        [DataMember]
        public List<SeriesInfo> seriesInfoList;

        public SerializeInfo()
        {
            this.seriesInfoList = new List<SeriesInfo>(); 
        }

        public SerializeInfo(string text1, string text2, string text3, string text4, string text5, string text6, string text7, string text8, 
            string text9, bool @checked, List<Series> seriesList)
        {
            this.text1 = text1;
            this.text2 = text2;
            this.text3 = text3;
            this.text4 = text4;
            this.text5 = text5;
            this.text6 = text6;
            this.text7 = text7;
            this.text8 = text8;
            this.text9 = text9;
            this.@checked = @checked;
            this.seriesInfoList = new List<SeriesInfo>();
            foreach(Series series in seriesList)
            {
                seriesInfoList.Add(new SeriesInfo(series));
            }
        }

        public void Save(string nameOfFile)
        {
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(SerializeInfo));

            using (FileStream fs = new FileStream(nameOfFile, FileMode.OpenOrCreate))
            {
                jsonFormatter.WriteObject(fs, this);
            }
        }

        public SerializeInfo(string loadFileName)
        {
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(SerializeInfo));

            using (FileStream fs = new FileStream(loadFileName, FileMode.OpenOrCreate))
            {
                SerializeInfo a = (SerializeInfo)jsonFormatter.ReadObject(fs);
                this.text1 = a.text1;
                this.text2 = a.text2;
                this.text3 = a.text3;
                this.text4 = a.text4;
                this.text5 = a.text5;
                this.text6 = a.text6;
                this.text7 = a.text7;
                this.text8 = a.text8;
                this.text9 = a.text9;
                this.@checked = a.@checked;
                this.seriesInfoList = a.seriesInfoList;
            }            
        }

        internal void BuildSeriesCollection(SeriesCollection series)
        {
            series.Clear();
            for(int i = 0; i<seriesInfoList.Count(); i++)
            {
                Series tempSeries = new Series();
                foreach(MyPoint myPoint in seriesInfoList[i].points)
                {
                    tempSeries.Points.AddXY(myPoint.X, myPoint.Y);
                }
                tempSeries.Color = seriesInfoList[i].color;
                tempSeries.ChartType = seriesInfoList[i].chartType;
                tempSeries.BorderWidth = seriesInfoList[i].borderWidth;
                series.Add(tempSeries);
            }
        }
    }
}
