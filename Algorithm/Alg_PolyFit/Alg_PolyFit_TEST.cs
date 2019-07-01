// [Copyright]
//
// Bookham Test Engine Algorithms
// Bookham.TestLibrary.Algorithms
//
// Alg_PolyFit_TEST.cs
//
// Author: Mark Fullalove


using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using TestSystem.TestLibrary.Algorithms;

// Disable missing XML comment warnings for this
#pragma warning disable 1591

namespace TestSystem.TestLibrary.Algorithms
{
    /// <summary>
	/// Test harness for Alg_Differential
	/// </summary>
	[TestFixture]
    public class Alg_PolyFit_TEST
    {
        [Test]
        public void Valid_Fit()
        {
            double[] xData = { 0, 1, 2, 3, 4, 5, 6, 7 };
            double[] yData = { 1, 3, 5, 7, 9, 7, 5, 3 };
            PolyFit fitData = Alg_PolyFit.PolynomialFit(xData, yData, 2);
            // These coefficients correlate with a 2nd order trendline in Excel.
            Assert.AreEqual(0.333, Math.Round(fitData.Coeffs[0], 3));
            Assert.AreEqual(3.714, Math.Round(fitData.Coeffs[1], 3));
            Assert.AreEqual(-0.476, Math.Round(fitData.Coeffs[2], 3));
        }

        [Test]
        [ExpectedException(typeof(AlgorithmException))]
        public void RidiculouslyHighOrder_Fit()
        {
            double[] xData = { 0, 1, 2, 3, 4, 5, 6, 7 };
            double[] yData = { 1, 3, 5, 7, 9, 7, 5, 3 };
            PolyFit fitData = Alg_PolyFit.PolynomialFit(xData, yData, 500);
        }

        [Test]
        [ExpectedException(typeof(AlgorithmException))]
        public void MismatchXY_Diff()
        {
            double[] xData = { 0, 1, 2, 3, 4, 5, 6 };
            double[] yData = { 1, 3, 5, 7, 9, 7, 5, 3 };
            PolyFit fitData = Alg_PolyFit.PolynomialFit(xData, yData, 2);
        }

        [Test]
        [ExpectedException(typeof(AlgorithmException))]
        public void NoX_Diff()
        {
            double[] xData = new double[0];
            double[] yData = { 1, 3, 5, 7, 9, 7, 5, 3 };
            PolyFit fitData = Alg_PolyFit.PolynomialFit(xData, yData, 3);
        }

        [Test]
        [ExpectedException(typeof(AlgorithmException))]
        public void NoY_Diff()
        {
            double[] yData = new double[0];
            double[] xData = { 1, 3, 5, 7, 9, 7, 5, 3 };
            PolyFit fitData = Alg_PolyFit.PolynomialFit(xData, yData, 3);
        }

        [Test]
        public void Valid_IndexArgsFit()
        {
            double[] xData = { 0, 1, 2, 3, 4, 5, 6, 7 };
            double[] yData = { -1, 3, 5, 7, 9, 11, 13, 21 };
            PolyFit fitData = Alg_PolyFit.PolynomialFit_indexArgs(xData, yData, 3, 6, 2);
            Assert.AreEqual(1, Math.Round(fitData.Coeffs[0], 3));
            Assert.AreEqual(2, Math.Round(fitData.Coeffs[1], 3));
            Assert.AreEqual(0, Math.Round(fitData.Coeffs[2], 3));
        }
        
        [Test]
        [ExpectedException(typeof(AlgorithmException))]
        public void FromGreaterThanTo_IndexArgsFit()
        {
            double[] xData = { 0, 1, 2, 3, 4, 5, 6, 7 };
            double[] yData = { -1, 3, 5, 7, 9, 11, 13, 21 };
            PolyFit fitData = Alg_PolyFit.PolynomialFit_indexArgs(xData, yData, 7, 2, 2);
        }

        [Test]
        [ExpectedException(typeof(AlgorithmException))]
        public void FromGreaterThanUbound_IndexArgsFit()
        {
            double[] xData = { 0, 1, 2, 3, 4, 5, 6, 7 };
            double[] yData = { -1, 3, 5, 7, 9, 11, 13, 21 };
            PolyFit fitData = Alg_PolyFit.PolynomialFit_indexArgs(xData, yData, 70, 2, 2);
        }

        [Test]
        [ExpectedException(typeof(AlgorithmException))]
        public void ToGreaterThanUbound_IndexArgsFit()
        {
            double[] xData = { 0, 1, 2, 3, 4, 5, 6, 7 };
            double[] yData = { -1, 3, 5, 7, 9, 11, 13, 21 };
            PolyFit fitData = Alg_PolyFit.PolynomialFit_indexArgs(xData, yData, 2, 20, 2);
        }

        [Test]
        public void Valid_XrangeFit()
        {
            double[] xData = { 0, 1, 2, 3, 4, 5, 6, 7 };
            double[] yData = { -1, 3, 5, 7, 9, 11, 13, 21 };
            PolyFit fitData = Alg_PolyFit.PolynomialFit_XvalueArgs(xData, yData, 2, 5, 2);
            Assert.AreEqual(1, Math.Round(fitData.Coeffs[0], 3));
            Assert.AreEqual(2, Math.Round(fitData.Coeffs[1], 3));
            Assert.AreEqual(0, Math.Round(fitData.Coeffs[2], 3));
        }

        [Test]
        [ExpectedException(typeof(AlgorithmException))]
        public void FromOutOfBounds_XrangeFit()
        {
            double[] xData = { 0, 1, 2, 3, 4, 5, 6, 7 };
            double[] yData = { -1, 3, 5, 7, 9, 11, 13, 21 };
            PolyFit fitData = Alg_PolyFit.PolynomialFit_XvalueArgs(xData, yData, 200, 5, 2);
        }

        [Test]
        [ExpectedException(typeof(AlgorithmException))]
        public void ToOutOfBounds_XrangeFit()
        {
            double[] xData = { 0, 1, 2, 3, 4, 5, 6, 7 };
            double[] yData = { -1, 3, 5, 7, 9, 11, 13, 21 };
            PolyFit fitData = Alg_PolyFit.PolynomialFit_XvalueArgs(xData, yData, 2, -500, 2);
        }

        [Test]
        public void Valid_YrangeFit()
        {
            double[] xData = { 0, 1, 2, 3, 4, 5, 6, 7 };
            double[] yData = { -1, 3, 5, 7, 9, 11, 13, 21 };
            PolyFit fitData = Alg_PolyFit.PolynomialFit_YvalueArgs(xData, yData, 3, 11, 2);
            Assert.AreEqual(1, Math.Round(fitData.Coeffs[0], 3));
            Assert.AreEqual(2, Math.Round(fitData.Coeffs[1], 3));
            Assert.AreEqual(0, Math.Round(fitData.Coeffs[2], 3));
        }

        [Test]
        [ExpectedException(typeof(AlgorithmException))]
        public void FromOutOfBounds_YrangeFit()
        {
            double[] xData = { 0, 1, 2, 3, 4, 5, 6, 7 };
            double[] yData = { -1, 3, 5, 7, 9, 11, 13, 21 };
            PolyFit fitData = Alg_PolyFit.PolynomialFit_YvalueArgs(xData, yData, 200, 5, 2);
        }

        [Test]
        [ExpectedException(typeof(AlgorithmException))]
        public void ToOutOfBounds_YrangeFit()
        {
            double[] xData = { 0, 1, 2, 3, 4, 5, 6, 7 };
            double[] yData = { -1, 3, 5, 7, 9, 11, 13, 21 };
            PolyFit fitData = Alg_PolyFit.PolynomialFit_YvalueArgs(xData, yData, 2, -500, 2);
        }
    }
}