using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using TestSystem.TestLibrary.Algorithms;

// Disable missing XML comment warnings for this
#pragma warning disable 1591

namespace TestSystem.TestLibrary.Algorithms
{
    [TestFixture]
    public class Alg_DevFromPolyFit_TEST
    {
        [Test]
        public void TestDevFromPolyFit1()
        { 
        
            double[] xData = { 0, 1, 2, 3, 4, 5, 6, 7 };
            double[] yData = { 1, 3, 5, 7, 9, 7, 5, 3 };
            //Excel polyfit provides us with :
            //y = -0.4762x2 + 3.7143x + 0.3333
            //which gives us values of y as being:
            //0, 0.3333,  1, 3.5714,   2, 5.8571,  3, 7.1904,  4, 7.5713,  
            //5, 6.9998,  6, 5.4759,   7, 2.9996
            
            //which in turn gives us deviations of:
            //0, -0.6667, 1, 0.5714,  2, 0.8571,  3, 0.1904,  4, -1.4287,  5, -0.0002,  6, 0.4759,  7, -0.0004
            
            PolyFit fitData = Alg_PolyFit.PolynomialFit(xData, yData, 2);
            // These coefficients correlate with a 2nd order trendline in Excel.
            Assert.AreEqual(0.333, Math.Round(fitData.Coeffs[0], 3));
            Assert.AreEqual(3.714, Math.Round(fitData.Coeffs[1], 3));
            Assert.AreEqual(-0.476, Math.Round(fitData.Coeffs[2], 3));
            
            Alg_DevFromPolyFit.DevFromPolyFitResults myResults =  Alg_DevFromPolyFit.DevFromPolyFit(xData, yData, 0, 7, 2);
            
            //using the values we calculated above (in Excel)
            Assert.AreEqual(Math.Round(myResults.MaxAbsDeviationFromPolyFitValue, 3), 1.429);
            Assert.AreEqual(Math.Round(myResults.MaxNegDevFromPolyFitValue, 3), -1.429);
            Assert.AreEqual(Math.Round(myResults.MaxPosDevFromPolyFitValue, 3), 0.857);
            Assert.AreEqual(myResults.ToIndexValueUsed, 7);
            Assert.AreEqual(myResults.FromIndexValueUsed, 0);
        
        }

        public void TestDevFromPolyFit2()
        {

            double[] xData = { 0, 1, 2, 3, 4, 5, 6, 7 };
            double[] yData = { 1, 3, 5, 7, 9, 7, 5, 3 };
            //Excel polyfit provides us with :
            //y = -0.4762x2 + 3.7143x + 0.3333
            //which gives us values of y as being:
            //0, 0.3333,  1, 3.5714,   2, 5.8571,  3, 7.1904,  4, 7.5713,  
            //5, 6.9998,  6, 5.4759,   7, 2.9996

            //which in turn gives us deviations of:
            //0, -0.6667, 1, 0.5714,  2, 0.8571,  3, 0.1904,  4, -1.4287,  5, -0.0002,  6, 0.4759,  7, -0.0004

            PolyFit fitData = Alg_PolyFit.PolynomialFit(xData, yData, 2);
            // These coefficients correlate with a 2nd order trendline in Excel.
            Assert.AreEqual(0.333, Math.Round(fitData.Coeffs[0], 3));
            Assert.AreEqual(3.714, Math.Round(fitData.Coeffs[1], 3));
            Assert.AreEqual(-0.476, Math.Round(fitData.Coeffs[2], 3));

            Alg_DevFromPolyFit.DevFromPolyFitResults myResults = Alg_DevFromPolyFit.DevFromPolyFit(xData, yData, 2);

            //using the values we calculated above (in Excel)
            Assert.AreEqual(Math.Round(myResults.MaxAbsDeviationFromPolyFitValue, 3), 1.429);
            Assert.AreEqual(Math.Round(myResults.MaxNegDevFromPolyFitValue, 3), -1.429);
            Assert.AreEqual(Math.Round(myResults.MaxPosDevFromPolyFitValue, 3), 0.857);
            Assert.AreEqual(myResults.ToIndexValueUsed, 7);
            Assert.AreEqual(myResults.FromIndexValueUsed, 0);

        }



        [Test]
        [ExpectedException(typeof(AlgorithmException))]
        public void TestDevFromPolyFit3()
        {
            //null data
            double[] xData =null;
            double[] yData = { 1, 3, 5, 7, 9, 7, 5, 3 };

            Alg_DevFromPolyFit.DevFromPolyFitResults myResults = Alg_DevFromPolyFit.DevFromPolyFit(xData, yData, 2);

        }

        [Test]
        [ExpectedException(typeof(AlgorithmException))]
        public void TestDevFromPolyFit4()
        {
            //null data
            double[] xData = { 1, 3, 5, 7, 9, 7, 3 }; 
            double[] yData = { 1, 3, 5, 7, 9, 7, 5, 3 };

            PolyFit fitData = Alg_PolyFit.PolynomialFit(xData, yData, 2);
        }


        [Test]
        [ExpectedException(typeof(AlgorithmException))]
        public void TestDevFromPolyFit5()
        {

            double[] xData = { 0, 1, 2, 3, 4, 5, 6, 7 };
            double[] yData = { 1, 3, 5, 7, 9, 7, 5, 3 };
           
            //start bigger than stop index
            Alg_DevFromPolyFit.DevFromPolyFitResults myResults = Alg_DevFromPolyFit.DevFromPolyFit(xData, yData, 1, 0, 2);
        }

        [Test]
        [ExpectedException(typeof(AlgorithmException))]
        public void TestDevFromPolyFit6()
        {

            double[] xData = { 0, 1, 2, 3, 4, 5, 6, 7 };
            double[] yData = { 1, 3, 5, 7, 9, 7, 5, 3 };

            //stop bigger than array size
            Alg_DevFromPolyFit.DevFromPolyFitResults myResults = Alg_DevFromPolyFit.DevFromPolyFit(xData, yData, 0,8, 2);
        }

        public void TestDevFromPolyFit7()
        {

            double[] xData = { 0, 1, 2, 3, 4, 5, 6, 7 };
            double[] yData = { 1, 3, 5, 7, 9, 7, 5, 3 };
            //Excel polyfit provides us with :
            //y = -0.4762x2 + 3.7143x + 0.3333
            //which gives us values of y as being:
            //0, 0.3333,  1, 3.5714,   2, 5.8571,  3, 7.1904,  4, 7.5713,  
            //5, 6.9998,  6, 5.4759,   7, 2.9996

            //which in turn gives us deviations of:
            //0, -0.6667, 1, 0.5714,  2, 0.8571,  3, 0.1904,  4, -1.4287,  5, -0.0002,  6, 0.4759,  7, -0.0004

            PolyFit fitData = Alg_PolyFit.PolynomialFit(xData, yData, 2);
            // These coefficients correlate with a 2nd order trendline in Excel.
            Assert.AreEqual(0.333, Math.Round(fitData.Coeffs[0], 3));
            Assert.AreEqual(3.714, Math.Round(fitData.Coeffs[1], 3));
            Assert.AreEqual(-0.476, Math.Round(fitData.Coeffs[2], 3));

            Alg_DevFromPolyFit.DevFromPolyFitResults myResults = Alg_DevFromPolyFit.DevFromPolyFit(xData, yData, 9);


            //using 9th order polynomial
            //using the values we calculated above (in Excel)
            Assert.AreEqual(Math.Round(myResults.MaxAbsDeviationFromPolyFitValue, 6), 0);
            Assert.AreEqual(Math.Round(myResults.MaxNegDevFromPolyFitValue, 6), -0);
            Assert.AreEqual(Math.Round(myResults.MaxPosDevFromPolyFitValue, 6), 0);
            

        }



    }
}
