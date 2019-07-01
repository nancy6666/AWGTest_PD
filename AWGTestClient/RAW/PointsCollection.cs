using System;
using System.Collections.Generic;
using MathNet.Numerics;

namespace SG_Filter_Demo.RAW
{
    public class PointsCollection : List<Point>
    {
        public PointsCollection() : base() { }

        public PointsCollection(int Capacity): base(Capacity) {  }

        public PointsCollection(IEnumerable<Point> Points) : base(Points) {  }

        public List<Point> DoSGFilter(int Window, int Order)
        {
            if ((Window < 1) || (Window % 2 != 1))
                throw new ArgumentException("the argument Window must be odd integer.");

            if(Order > Window || Order < 1)
                throw new ArgumentException("the order is error.");

            List<Point> processed = new List<Point>();

            // smooth the curve with S-G Filter
            for(int n = 0; n < this.Count; n++)
            {
                var subset = GetSubsetInWindow(this, n, Window);
                var coefficient = PolyFit(subset, Order);
                var Y = PolyVal(coefficient, this[n].Wavelength);

                processed.Add(new Point(this[n].Wavelength, Y));
            }

            return processed;
        }

        private List<Point> GetSubsetInWindow(List<Point> DataSet, int StartIndex, int Window)
        {
            if ((Window < 1) || (Window % 2 != 1))
                throw new ArgumentException("the argument Window must be odd integer.");

            List<Point> subset = new List<Point>();

            int half = (Window - 1) / 2;
            if(StartIndex < half) // there are no enough points at the begining
            {
                // fill the sub dataset with the raw data reversed
                for (int i = (half - StartIndex); i > 0; i--)
                {
                    subset.Add(DataSet[i]);
                }

                // fill the sub dataset with raw data
                for (int i = 0; i < (Window - (half - StartIndex)); i++)
                {
                    subset.Add(DataSet[i]);
                }
            }
            else if((DataSet.Count - StartIndex - 1) < half) // if the count of the remained data points are less than the window
            {
                for (int i = StartIndex - half; i < DataSet.Count; i++)
                {
                    subset.Add(DataSet[i]);
                }

                for (int i = DataSet.Count - 2; i > (2 * DataSet.Count - half - StartIndex - 3); i--)
                {
                    subset.Add(DataSet[i]);
                }
            }
            else
            {
                for (int i = (StartIndex - half); i < (StartIndex + half + 1); i++)
                {
                    subset.Add(DataSet[i]);
                }
            }

            return subset;
        }
        
        public double[] PolyFit(List<Point> DataSet, int Order)
        {
            List<double> xList = new List<double>();
            List<double> yList = new List<double>();

            foreach(var point in DataSet)
            {
                xList.Add(point.Wavelength);
                yList.Add(point.Intensity);
            }

            return Fit.Polynomial(xList.ToArray(), yList.ToArray(), Order);
        }

        public double PolyVal(double[] Coefficient, double XValue)
        {
            double y = 0;
            for (int p = 0; p < Coefficient.Length; p++)
                y += Coefficient[p] * Math.Pow(XValue, p);
            return y;
        }
    }
}
