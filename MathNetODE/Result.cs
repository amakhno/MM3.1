using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace MathNetODE
{
    class Result
    {
        public double[] X { get; private set; }
        public double[] Y { get; private set; }
        public double[] T { get; private set; }
        public double MaxX { get; private set; }
        public double MinX { get; private set; }
        public double MaxY { get; private set; }
        public double MinY { get; private set; }

        public Result(Vector<double>[] input)
        {
            X = new double[input.Length];
            Y = new double[input.Length];
            T = new double[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                double[] a = input[i].ToArray();
                X[i] = a[0];
                Y[i] = a[1];
                T[i] = i;
            }
            MaxX = X.Max();
            MaxY = Y.Max();
            MinY = Y.Min();
            MinX = X.Min();
        }
    }
}



