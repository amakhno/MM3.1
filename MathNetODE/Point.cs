using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace MathNetODE
{
    [DataContract]
    public class MyPoint
    {
        [DataMember]
        public double X { get; private set; }
        [DataMember]
        public double Y { get; private set; }

        public MyPoint(double X, double Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }
}
