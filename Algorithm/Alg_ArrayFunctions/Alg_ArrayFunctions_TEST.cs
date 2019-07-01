// [Copyright]
//
// Bookham Test Engine Algorithms
// Bookham.TestLibrary.Algorithms
//
// Alg_ArrayFunctions_TEST.cs
//
// Author: Mark Fullalove
// Coverage : nCover reports 100% (excluding private ctor)

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
    /// Test harness for Alg_ArrayFunctions
    /// </summary>
    [TestFixture]
    public class Alg_ArrayFunctions_TEST
    {
        [Test]
        public void Valid_Subtract()
        {
            double[] originalData = { 0, 1, 2, 3, 4, 5, 6, 7 };
            double valueToSubtract = 7;
            double[] smallerData = Alg_ArrayFunctions.SubtractFromEachArrayElement(originalData, valueToSubtract);
            Assert.AreEqual(smallerData[0], originalData[0] - valueToSubtract);
            Assert.AreEqual(smallerData[originalData.Length-1], originalData[originalData.Length-1] - valueToSubtract);
            Assert.AreEqual(smallerData.Length, originalData.Length);
        }

        [Test]
        public void EmptyArray_Subtract()
        {
            double[] originalData = { };
            double valueToSubtract = 7;
            double[] smallerData = Alg_ArrayFunctions.SubtractFromEachArrayElement(originalData, valueToSubtract);
            Assert.AreEqual(smallerData.Length, originalData.Length);
        }

        [Test]
        public void Valid_Add()
        {
            double[] originalData = { 0, 1, 2, 3, 4, 5, 6, 7 };
            double valueToAdd = 7;
            double[] smallerData = Alg_ArrayFunctions.AddToEachArrayElement(originalData, valueToAdd);
            Assert.AreEqual(smallerData[0], originalData[0] + valueToAdd);
            Assert.AreEqual(smallerData[originalData.Length - 1], originalData[originalData.Length - 1] + valueToAdd);
            Assert.AreEqual(smallerData.Length, originalData.Length);
        }

        [Test]
        public void EmptyArray_Add()
        {
            double[] originalData = { };
            double valueToAdd = 7;
            double[] smallerData = Alg_ArrayFunctions.AddToEachArrayElement(originalData, valueToAdd);
            Assert.AreEqual(smallerData.Length, originalData.Length);
        }

        [Test]
        public void Valid_Multiply()
        {
            double[] originalData = { 0, 1, 2, 3, 4, 5, 6, 7 };
            double valueToMultiply = 7;
            double[] smallerData = Alg_ArrayFunctions.MultiplyEachArrayElement(originalData, valueToMultiply);
            Assert.AreEqual(smallerData[0], originalData[0] * valueToMultiply);
            Assert.AreEqual(smallerData[originalData.Length - 1], originalData[originalData.Length - 1] * valueToMultiply);
            Assert.AreEqual(smallerData.Length, originalData.Length);
        }

        [Test]
        public void MaxVal_Multiply()
        {
            double[] originalData = { 0, 1, 2, 3, 4, 5, 6, 7 };
            double valueToMultiply = Double.MaxValue;
            double[] smallerData = Alg_ArrayFunctions.MultiplyEachArrayElement(originalData, valueToMultiply);
            Assert.AreEqual(smallerData[0], originalData[0] * valueToMultiply);
            Assert.AreEqual(smallerData[originalData.Length - 1], originalData[originalData.Length - 1] * valueToMultiply);
            Assert.AreEqual(smallerData.Length, originalData.Length);
        }

        [Test]
        public void PositiveInfinity_Multiply()
        {
            double[] originalData = { 0, 1, 2, 3, 4, 5, 6, 7 };
            double valueToMultiply = Double.PositiveInfinity;
            double[] smallerData = Alg_ArrayFunctions.MultiplyEachArrayElement(originalData, valueToMultiply);
            Assert.AreEqual(smallerData[0], originalData[0] * valueToMultiply);
            Assert.AreEqual(smallerData[originalData.Length - 1], originalData[originalData.Length - 1] * valueToMultiply);
            Assert.AreEqual(smallerData.Length, originalData.Length);
        }

        [Test]
        public void EmptyArray_Multiply()
        {
            double[] originalData = { };
            double valueToMultiply = 7;
            double[] smallerData = Alg_ArrayFunctions.MultiplyEachArrayElement(originalData, valueToMultiply);
            Assert.AreEqual(smallerData.Length, originalData.Length);
        }

        [Test]
        public void Valid_Divide()
        {
            double[] originalData = { 0, 1, 2, 3, 4, 5, 6, 7 };
            double valueToDivide = 7;
            double[] smallerData = Alg_ArrayFunctions.DivideEachArrayElement(originalData, valueToDivide);
            Assert.AreEqual(smallerData[0], originalData[0] / valueToDivide);
            Assert.AreEqual(smallerData[originalData.Length - 1], originalData[originalData.Length - 1] / valueToDivide);
            Assert.AreEqual(smallerData.Length, originalData.Length);
        }

        [Test]
        public void NegativeInfinity_Divide()
        {
            double[] originalData = { 0, 1, 2, 3, 4, 5, 6, 7 };
            double valueToDivide = Double.NegativeInfinity;
            double[] smallerData = Alg_ArrayFunctions.DivideEachArrayElement(originalData, valueToDivide);
            Assert.AreEqual(smallerData[0], originalData[0] / valueToDivide);
            Assert.AreEqual(smallerData[originalData.Length - 1], originalData[originalData.Length - 1] / valueToDivide);
            Assert.AreEqual(smallerData.Length, originalData.Length);
        }

        [Test]
        public void NaN_Divide()
        {
            double[] originalData = { 0, 1, 2, 3, 4, 5, 6, 7 };
            double valueToDivide = Double.NaN;
            double[] smallerData = Alg_ArrayFunctions.DivideEachArrayElement(originalData, valueToDivide);
            Assert.AreEqual(smallerData[0], originalData[0] / valueToDivide);
            Assert.AreEqual(smallerData[originalData.Length - 1], originalData[originalData.Length - 1] / valueToDivide);
            Assert.AreEqual(smallerData.Length, originalData.Length);
        }

        [Test]
        public void EmptyArray_Divide()
        {
            double[] originalData = { };
            double valueToDivide = 7;
            double[] smallerData = Alg_ArrayFunctions.DivideEachArrayElement(originalData, valueToDivide);
            Assert.AreEqual(smallerData.Length, originalData.Length);
        }

        [Test]
        [ExpectedException(typeof(AlgorithmException))]
        public void DivisionByZero_Divide()
        {
            double[] originalData = { 0, 1, 2, 3, 4, 5, 6, 7 };
            double valueToDivide = 0;
            double[] smallerData = Alg_ArrayFunctions.DivideEachArrayElement(originalData, valueToDivide);
        }

        [Test]
        public void Valid_Reverse()
        {
            double[] originalData = { 0, 1, 2, 3, 4, 5, 6, 7 };
            double[] reversedData = Alg_ArrayFunctions.ReverseArray(originalData);
            Assert.AreEqual(reversedData[0], originalData[originalData.Length-1]);
            Assert.AreEqual(originalData[0], reversedData[originalData.Length-1]);
        }

        [Test]
        public void Valid_AddArrays()
        {
            double[] array1 = { 20, 13, 24, 35, 46, 57, 68, 7000 };
            double[] array2 = { 80, 1, 2, 3, 4, 5, 6, 700 };
            double[] sumOfData = Alg_ArrayFunctions.AddArrays(array1,array2);
            Assert.AreEqual(sumOfData[0], array1[0] + array2[0]);
            Assert.AreEqual(sumOfData[array1.Length - 1], array1[array1.Length - 1] + array2[array1.Length - 1]);
        }

        [Test]
        [ExpectedException(typeof(AlgorithmException))]
        public void DifferentSized_AddArrays()
        {
            double[] array1 = { 20, 13, 24, 35, 46, 57, 68, 7000 };
            double[] array2 = { 80, 1, 2, 3, 4, 5, 6 };
            double[] sumOfData = Alg_ArrayFunctions.AddArrays(array1, array2);
        }

        [Test]
        public void Valid_SubtractArrays()
        {
            double[] array1 = { 20, 13, 24, 35, 46, 57, 68, 7000 };
            double[] array2 = { 80, 1, 2, 3, 4, 5, 6, 700 };
            double[] differenceOfData = Alg_ArrayFunctions.SubtractArrays(array1, array2);
            Assert.AreEqual(differenceOfData[0], array1[0] - array2[0]);
            Assert.AreEqual(differenceOfData[array1.Length - 1], array1[array1.Length - 1] - array2[array1.Length - 1]);
        }

        [Test]
        [ExpectedException(typeof(AlgorithmException))]
        public void DifferentSized_SubtractArrays()
        {
            double[] array1 = { 20, 13, 24, 35, 46, 57, 68, 7000 };
            double[] array2 = { 80, 1, 2, 3, 4, 5, 6 };
            double[] sumOfData = Alg_ArrayFunctions.SubtractArrays(array1, array2);
        }

        [Test]
        public void Valid_JoinArrays()
        {
            double[] array1 = { 20, 13, 24, 35, 46, 57, 68, 7000 };
            double[] array2 = { 80, 1, 2, 3, 4, 5, 6, 700 };
            double[] bigArray = Alg_ArrayFunctions.JoinArrays(array1, array2);
            Assert.AreEqual(bigArray[0], array1[0]);
            Assert.AreEqual(bigArray[array1.Length - 1], array1[array1.Length - 1]);
            Assert.AreEqual(bigArray[array1.Length], array2[0]);
            Assert.AreEqual(bigArray[bigArray.Length-1], array2[array2.Length-1]);
        }

        [Test]
        public void FirstEmpty_JoinArrays()
        {
            double[] array1 = { };
            double[] array2 = { 80, 1, 2, 3, 4, 5, 6, 700 };
            double[] bigArray = Alg_ArrayFunctions.JoinArrays(array1, array2);
            Assert.AreEqual(bigArray[array1.Length], array2[0]);
            Assert.AreEqual(bigArray[bigArray.Length - 1], array2[array2.Length - 1]);
        }

        [Test]
        public void SecondUnassigned_JoinArrays()
        {
            double[] array1 = { 80, 1, 2, 3, 4, 5, 6, 700 };
            double[] array2 = new double[5];
            double[] bigArray = Alg_ArrayFunctions.JoinArrays(array1, array2);
            Assert.AreEqual(bigArray[array1.Length], array2[0]);
            Assert.AreEqual(bigArray[bigArray.Length - 1], array2[array2.Length - 1]);
        }

        [Test]
        public void Valid_ExtractSubset()
        {
            double[] bigArray = { 80, 1, 2, 3, 23, 4, 5, 99, 6, 700 };
            int stop = 5;
            int start = 3;
            double[] subsetArray = Alg_ArrayFunctions.ExtractSubArray(bigArray, start, stop);
            Assert.AreEqual(bigArray[start], subsetArray[0]);
            Assert.AreEqual(bigArray[start + 1], subsetArray[1]);
            Assert.AreEqual(bigArray[start + 2], subsetArray[2]);
            Assert.AreEqual(subsetArray.Length, stop - start + 1);
        }

        [Test]
        [ExpectedException(typeof(AlgorithmException))]
        public void Empty_ExtractSubset()
        {
            double[] bigArray = { };
            int stop = 0;
            int start = 0;
            double[] subsetArray = Alg_ArrayFunctions.ExtractSubArray(bigArray, start, stop);
        }

        [Test]
        public void FromEqualsTo_ExtractSubset()
        {
            double[] bigArray = { 80, 1, 2, 3, 23, 4, 5, 99, 6, 700 };
            int stop = 5;
            int start = 5;
            double[] subsetArray = Alg_ArrayFunctions.ExtractSubArray(bigArray, start, stop);
            Assert.AreEqual(bigArray[start], subsetArray[0]);
            Assert.AreEqual(subsetArray.Length, stop - start + 1);
        }

        [Test]
        [ExpectedException(typeof(AlgorithmException))]
        public void FromGreaterThanTo_ExtractSubset()
        {
            double[] bigArray = { 80, 1, 2, 3, 23, 4, 5, 99, 6, 700 };
            int stop = 3;
            int start = 5;
            double[] subsetArray = Alg_ArrayFunctions.ExtractSubArray(bigArray, start, stop);
        }
        
        [Test]
        [ExpectedException(typeof(AlgorithmException))]
        public void FromNegative_ExtractSubset()
        {
            double[] bigArray = { 80, 1, 2, 3, 23, 4, 5, 99, 6, 700 };
            int stop = 3;
            int start = -5;
            double[] subsetArray = Alg_ArrayFunctions.ExtractSubArray(bigArray, start, stop);
        }

        [Test]
        [ExpectedException(typeof(AlgorithmException))]
        public void ToNegative_ExtractSubset()
        {
            double[] bigArray = { 80, 1, 2, 3, 23, 4, 5, 99, 6, 700 };
            int stop = -3;
            int start = 5;
            double[] subsetArray = Alg_ArrayFunctions.ExtractSubArray(bigArray, start, stop);
        }

        [Test]
        [ExpectedException(typeof(AlgorithmException))]
        public void stopMaxInt_ExtractSubset()
        {
            double[] bigArray = { 80, 1, 2, 3, 23, 4, 5, 99, 6, 700 };
            int stop = int.MaxValue;
            int start = 5;
            double[] subsetArray = Alg_ArrayFunctions.ExtractSubArray(bigArray, start, stop);
        }

        [Test]
        [ExpectedException(typeof(AlgorithmException))]
        public void stopMinInt_ExtractSubset()
        {
            double[] bigArray = { 80, 1, 2, 3, 23, 4, 5, 99, 6, 700 };
            int stop = int.MinValue;
            int start = 5;
            double[] subsetArray = Alg_ArrayFunctions.ExtractSubArray(bigArray, start, stop);
        }

        [Test]
        public void Valid_findIndexOfNearestElement()
        {
            double[] dataArray = { 80, 1, 2, 3, 23, 4, 5, 99, 6, 700 };
            int location = Alg_ArrayFunctions.FindIndexOfNearestElement(dataArray, 4.4);
            Assert.AreEqual(location, 5);
        }

        [Test]
        [ExpectedException(typeof(AlgorithmException))]
        public void EmptyArray_findIndexOfNearestElement()
        {
            double[] dataArray = { };
            int location = Alg_ArrayFunctions.FindIndexOfNearestElement(dataArray, 4.4);
        }

        [Test]
        public void ValidWithinRange_getSubArrayByValueRange()
        {
            double[] dataArray = { -1, 0, 1, 2, 3, 4, 5, 99, 6, 700,  };
            double minVal = 2;
            double maxVal = 99;
            double[] data = Alg_ArrayFunctions.GetSubArrayByValueRange(minVal, maxVal, dataArray, Alg_ArrayFunctions.BoundaryCondition.InsideMinMaxBoundary);
            Assert.AreEqual(data.Length, 6);
        }

        [Test]
        public void ValidWithinRangeDesc_getSubArrayByValueRange()
        {
            double[] dataArray = { 10, 9, 8, 7, 6, 5, 700 ,99, 4, 3, 2, 1, 0, -1 };
            double minVal = 2;
            double maxVal = 99;
            double[] data = Alg_ArrayFunctions.GetSubArrayByValueRange(minVal, maxVal, dataArray, Alg_ArrayFunctions.BoundaryCondition.InsideMinMaxBoundary);
            Assert.AreEqual(data.Length, 10);
        }

        [Test]
        public void ValidOutsideRange_getSubArrayByValueRange()
        {
            double[] dataArray = { -1, 0, 1, 2, 3, 4, 5, 99, 6, 700, };
            double minVal = 2;
            double maxVal = 99;
            double[] data = Alg_ArrayFunctions.GetSubArrayByValueRange(minVal, maxVal, dataArray, Alg_ArrayFunctions.BoundaryCondition.OutsideMinMaxBoundary);
            Assert.AreEqual(data.Length, 4);
        }

        [Test]
        public void ValidOutsideRangeDesc_getSubArrayByValueRange()
        {
            double[] dataArray = { 10, 9, 8, 7, 6, 5, 700, 99, 4, 3, 2, 1, 0, -1 };
            double minVal = 2;
            double maxVal = 99;
            double[] data = Alg_ArrayFunctions.GetSubArrayByValueRange(minVal, maxVal, dataArray, Alg_ArrayFunctions.BoundaryCondition.OutsideMinMaxBoundary);
            Assert.AreEqual(data.Length, 4);
        }

        [Test]
        public void NoData_getSubArrayByValueRange()
        {
            double[] dataArray = { };
            double minVal = 2;
            double maxVal = 99;
            double[] data = Alg_ArrayFunctions.GetSubArrayByValueRange(minVal, maxVal, dataArray, Alg_ArrayFunctions.BoundaryCondition.OutsideMinMaxBoundary);
            Assert.AreEqual(data.Length, 0);
        }

    }
}
