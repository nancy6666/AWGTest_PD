// [Copyright]
//
// Bookham Test Engine Algorithms
// Bookham.TestLibrary.Algorithms
//
// Alg_PointSearch_TEST.cs
//
// Author: Keith Pillar


using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
// Disable missing XML comment warnings for this
#pragma warning disable 1591

namespace TestSystem.TestLibrary.Algorithms
{
    /// <summary>
    /// Test harness for Alg_PointSearch_TEST
    /// </summary>
    [TestFixture]
    public class Alg_PointSearch_TEST
    {
        [Test]
        public void TestMaxValue1()
        {
            // finding max value in array
            double[] xData = { 0, 1, 2, 3, 4, 5, 6, 7 };
            
            double maxVal = Alg_PointSearch.FindMaxValueInArray(xData);
            Assert.AreEqual(7, maxVal);
        }

        [Test]
        [ExpectedException(typeof(AlgorithmException))]
        public void TestMaxValue2()
        {
            // finding max value in empty array, expect exception
            double[] xData = {};
            
            double maxVal = Alg_PointSearch.FindMaxValueInArray(xData);
                        
        }

        [Test]
        public void TestMaxValue3()
        {
            // finding max value in array with -ve numbers
            double[] xData = { -11, -12, -13, -14, -15, -16, -17 };

            double maxVal = Alg_PointSearch.FindMaxValueInArray(xData);
            Assert.AreEqual(-11, maxVal);
        }

        [Test]
        public void TestMaxValue4()
        {
            // finding index of max value in array with -ve numbers
            double[] xData = { -11, -12, -13, -14, -15, -16, -17 };

            double maxVal = Alg_PointSearch.FindFirstIndexOfMaxValueInArray(xData);
            Assert.AreEqual(0, maxVal);
        }
        [Test]
        public void TestMaxValue5()
        {
            // finding index of max value in array with -ve numbers
            double[] xData = { -11, -12, -13, -14, -15, -16, -17, -11 };

            double maxVal = Alg_PointSearch.FindLastIndexOfMaxValueInArray(xData);
            Assert.AreEqual(7, maxVal);
        }


        [Test]
        public void TestMinValue1()
        {
            // finding min value in array
            double[] xData = { 0, 1, 2, 3, 4, 5, 6, 7 };

            double minVal = Alg_PointSearch.FindMinValueInArray(xData);
            Assert.AreEqual(0, minVal);
        }

        [Test]
        [ExpectedException(typeof(AlgorithmException))]
        public void TestMinValue2()
        {
            // finding min value in empty array, expect exception
            double[] xData = { };
            double minVal = Alg_PointSearch.FindMinValueInArray(xData);
        }

        [Test]
        public void TestMinValue3()
        {
            // finding min value in array of -ve numbers
            double[] xData = { -11, -12, -13, -14, -15, -16, -17 };

            double minVal = Alg_PointSearch.FindMinValueInArray(xData);
            Assert.AreEqual(-17, minVal);
        }

        [Test]
        public void TestMinValue4()
        {
            // finding index of min value in array of -ve numbers
            double[] xData = { -17, -11, -12, -13, -14, -15, -16, -17 };

            double minVal = Alg_PointSearch.FindFirstIndexOfMinValueInArray(xData);
            Assert.AreEqual(0, minVal);
        }

        [Test]
        public void TestMinValue5()
        {
            // finding index of min value in array of -ve numbers
            double[] xData = { -17, -11, -12, -13, -14, -15, -16, -17 };

            double minVal = Alg_PointSearch.FindLastIndexOfMinValueInArray(xData);
            Assert.AreEqual(7, minVal);
        }
        [Test]
        public void TestFindFirstValTest1()
        {
            // finding first matching value in array of -ve numbers
            double[] xData = { -11, -12, -13, -14, -15, -12, -17 };
            Alg_PointSearch.ComparisonOperator cp= Alg_PointSearch.ComparisonOperator.EqualTo;
            
            int dataVal;
            dataVal = Alg_PointSearch.FindFirstValue(xData, cp, -12);
            
            Assert.AreEqual(1, dataVal);
        }

        [Test]
        public void TestFindFirstValTest2()
        {
            // finding first matching value in array, but passing in a number that isn't in the array           
            double[] xData = { -11, -12, -13, -14, -15, -12, -17 };
            Alg_PointSearch.ComparisonOperator cp = Alg_PointSearch.ComparisonOperator.EqualTo;

            int dataVal;
            dataVal = Alg_PointSearch.FindFirstValue(xData, cp, -18);
            Assert.AreEqual(dataVal, -1);
            
        }
        [Test]
        public void TestFindFirstValTest3()
        {
            // finding first matching value in array, using a double,
            //the 12.100000000000001 which it ignores is 15 points precision
            double[] xData = { 11, 12.100000000000001, 13, 14, 15, 12.1, 17 };
            Alg_PointSearch.ComparisonOperator cp = Alg_PointSearch.ComparisonOperator.EqualTo;

            int dataVal;
            dataVal = Alg_PointSearch.FindFirstValue(xData, cp, 12.1);
            
            Assert.AreEqual(5, dataVal);

        }

        [Test]
        public void TestFindFirstValTest4()
        {
            // finding first matching value in array, using a double,
            // the 12.1000000000000001 which it finds instead of correct value
            // is 16 points precision
            double[] xData = { 11, 12.1000000000000001, 13, 14, 15, 12.1, 17 };
            Alg_PointSearch.ComparisonOperator cp = Alg_PointSearch.ComparisonOperator.EqualTo;

            int dataVal;
            dataVal = Alg_PointSearch.FindFirstValue(xData, cp, 12.1);
            
            Assert.AreEqual(1, dataVal);

        }



        [Test]
        public void TestFindLastValTest1()
        {
            // finding Last matching value in array of -ve numbers
            double[] xData = { -11, -12, -13, -14, -15, -12, -17 };
            Alg_PointSearch.ComparisonOperator cp = Alg_PointSearch.ComparisonOperator.EqualTo;

            int dataVal;
            dataVal = Alg_PointSearch.FindLastValue(xData, cp, -12);
            
            Assert.AreEqual(5, dataVal);
        }

        [Test]
        public void TestFindLastValTest2()
        {
            // finding Last matching value in array, but passing in a number that isn't in the array           
            double[] xData = { -11, -12, -13, -14, -15, -12, -17 };
            Alg_PointSearch.ComparisonOperator cp = Alg_PointSearch.ComparisonOperator.EqualTo;

            int dataVal;
            dataVal = Alg_PointSearch.FindLastValue(xData, cp, -18);
            Assert.AreEqual(dataVal, -1);

        }
        [Test]
        public void TestFindLastValTest3()
        {
            // finding Last matching value in array, using a double,
            //the 12.100000000000001 which it ignores is 15 points precision
            double[] xData = { 12.1, 11, 12.100000000000001, 13, 14, 15, 12.1, 17 };
            Alg_PointSearch.ComparisonOperator cp = Alg_PointSearch.ComparisonOperator.EqualTo;

            int dataVal;
            dataVal = Alg_PointSearch.FindLastValue(xData, cp, 12.1);
            
            Assert.AreEqual(6, dataVal);

        }

        [Test]
        public void TestFindLastValTest4()
        {
            // finding Last matching value in array, using a double,
            // the 12.1000000000000001 which it finds instead of correct value
            // is 16 points precision
            double[] xData = { 12.1, 11, 12.1000000000000001, 13, 14, 15, 12.101, 17 };
            Alg_PointSearch.ComparisonOperator cp = Alg_PointSearch.ComparisonOperator.EqualTo;

            int dataVal;
            dataVal = Alg_PointSearch.FindLastValue(xData, cp, 12.1);
            
            Assert.AreEqual(2, dataVal);

        }

        [Test]
        public void TestOfDifferentComparisonOperators1()
        {            
            double[] xData = { 12.1, 11, 12.11, 13, 14, 15, 12.101, 17 };
            //Equals comparator has already been trialled

            //************************* GT **************************
            Alg_PointSearch.ComparisonOperator cp = Alg_PointSearch.ComparisonOperator.GreaterThan;

            int dataVal;

            dataVal = Alg_PointSearch.FindLastValue(xData, cp, 13.01);
            
            Assert.AreEqual(7, dataVal);

            dataVal = Alg_PointSearch.FindFirstValue(xData, cp, 13.01);
            
            Assert.AreEqual(4, dataVal);
            
        }

        [Test]
        public void TestOfDifferentComparisonOperators2()
        {
            double[] xData = { 12.1, 11, 12.11, 13, 14, 15, 12.101, 17 };
            
            //************************* GTE **************************
            Alg_PointSearch.ComparisonOperator cp = Alg_PointSearch.ComparisonOperator.GreaterThanOrEqualTo;

            int dataVal;

            dataVal = Alg_PointSearch.FindLastValue(xData, cp, 13.01);
            
            Assert.AreEqual(7, dataVal);

            dataVal = Alg_PointSearch.FindFirstValue(xData, cp, 13.01);
            
            Assert.AreEqual(4, dataVal);

            dataVal = Alg_PointSearch.FindLastValue(xData, cp, 13.0);
            
            Assert.AreEqual(7, dataVal);

            dataVal = Alg_PointSearch.FindFirstValue(xData, cp, 13.0);
            
            Assert.AreEqual(3, dataVal);

            //************************* LT **************************
            cp = Alg_PointSearch.ComparisonOperator.LessThan;

            dataVal = Alg_PointSearch.FindFirstValue(xData, cp, 12.01);
            
            Assert.AreEqual(1, dataVal);

            dataVal = Alg_PointSearch.FindFirstValue(xData, cp, 13.01);
            
            Assert.AreEqual(0, dataVal);

            dataVal = Alg_PointSearch.FindLastValue(xData, cp, 13.0);
            
            Assert.AreEqual(6, dataVal);

            dataVal = Alg_PointSearch.FindFirstValue(xData, cp, 13.0);
            
            Assert.AreEqual(0, dataVal);

            //************************* LTE **************************
            cp = Alg_PointSearch.ComparisonOperator.LessThanOrEqualTo;

            dataVal = Alg_PointSearch.FindLastValue(xData, cp, 11.1);
            
            Assert.AreEqual(1, dataVal);

            dataVal = Alg_PointSearch.FindFirstValue(xData, cp, 11.0);
            
            Assert.AreEqual(1, dataVal);

            dataVal = Alg_PointSearch.FindLastValue(xData, cp, 17.0);
            
            Assert.AreEqual(7, dataVal);
        }

        
        [Test]
        [ExpectedException(typeof(AlgorithmException))]
        public void TestNullArray()
        {
            // finding Last matching value in array of -ve numbers
            double[] xData =null;
            Alg_PointSearch.ComparisonOperator cp = Alg_PointSearch.ComparisonOperator.EqualTo;

            int dataVal;
            dataVal = Alg_PointSearch.FindLastValue(xData, cp, -12);

            Assert.AreEqual(5, dataVal);
        }
        [Test]
        [ExpectedException(typeof(AlgorithmException))]
        public void TestIndexOutOfBounds1()
        {
            // finding Last matching value in array of -ve numbers
            double[] xData = { 12.1, 11, 12.11, 13, 14, 15, 12.101, 17 }; 
            Alg_PointSearch.ComparisonOperator cp = Alg_PointSearch.ComparisonOperator.EqualTo;

            int dataVal;
            dataVal = Alg_PointSearch.FindLastValue(xData, cp, -12,0,8);

            Assert.AreEqual(5, dataVal);
        }
    
    
    }
}
