// [Copyright]
//
// Bookham Test Engine Algorithms
// Bookham.TestLibrary.Algorithms
//
// Alg_ArrayFunctions.cs
//
// Author: Mark Fullalove
// Design: From Bookham "PcLeslie" software

using System;
using System.Collections.Generic;
using System.Text;

namespace TestSystem.TestLibrary.Algorithms
{
    /// <summary>
    /// Provides methods for manipulating arrays.
    /// </summary>
    public sealed class Alg_ArrayFunctions
    {
        /// <summary>
        /// Empty private constructor
        /// </summary>
        private Alg_ArrayFunctions()
        {
        }

        /// <summary>
        /// Add a value to each element of the array.
        /// </summary>
        /// <param name="arrayIn">An array of doubles</param>
        /// <param name="valueToAdd">A value to be added</param>
        /// <returns>A new array in which 'valueToAdd' has been added to each element</returns>
        public static double[] AddToEachArrayElement(double[] arrayIn, double valueToAdd)
        {
            if (arrayIn == null)
            {
                throw new AlgorithmException("AddToEachArrayElement : arrayIn is null");
            }
            double[] resultArray = new double[arrayIn.Length];

            for (int i = 0; i < arrayIn.Length; i++)
            {
                resultArray[i] = arrayIn[i] + valueToAdd;
            }
            return resultArray;
        }

        /// <summary>
        /// Subtract a value from each element of the array.
        /// </summary>
        /// <param name="arrayIn">An array of doubles</param>
        /// <param name="valueToSubtract">A value to be subtracted</param>
        /// <returns>A new array in which 'valueToSubtract' has been subtracted from each element</returns>
        public static double[] SubtractFromEachArrayElement(double[] arrayIn, double valueToSubtract)
        {
            if (arrayIn == null)
            {
                throw new AlgorithmException("SubtractFromEachArrayElement : arrayIn is null");
            }
            return AddToEachArrayElement(arrayIn, -valueToSubtract);
        }

        /// <summary>
        /// Multiply each element of the array by a value.
        /// </summary>
        /// <param name="arrayIn">An array of doubles</param>
        /// <param name="valueToMultiplyBy">A value to be multiply by</param>
        /// <returns>A new array in which each element is the product of the original element and 'valueToMultiplyBy'</returns>
        public static double[] MultiplyEachArrayElement(double[] arrayIn, double valueToMultiplyBy)
        {
            if (arrayIn == null)
            {
                throw new AlgorithmException("MultiplyEachArrayElement : arrayIn is null");
            }

            double[] resultArray = new double[arrayIn.Length];

            for (int i = 0; i < arrayIn.Length; i++)
            {
                resultArray[i] = arrayIn[i] * valueToMultiplyBy;
            }
            return resultArray;
        }

        /// <summary>
        /// Divide each element of the array by a value.
        /// </summary>
        /// <param name="arrayIn">An array of doubles</param>
        /// <param name="valueToDivideBy">A non-zero value to divide by</param>
        /// <returns>A new array in which each element is the dividend of the original element and the divisor 'valueToDivideBy'</returns>
        public static double[] DivideEachArrayElement(double[] arrayIn, double valueToDivideBy)
        {
            // PRECONDITIONS
            if (arrayIn == null)
            {
                throw new AlgorithmException("DivideEachArrayElement : arrayIn is null");
            }
            if (valueToDivideBy == 0)
            {
                throw new AlgorithmException("Alg_ArrayFunctions.divideEachArrayElement : Cannot divide by 0");
            }

            double[] resultArray = new double[arrayIn.Length];

            for (int i = 0; i < arrayIn.Length; i++)
            {
                resultArray[i] = arrayIn[i] / valueToDivideBy;
            }
            return resultArray;
        }

        /// <summary>
        /// Reverses the sequence of elements of an array without altering the original data
        /// </summary>
        /// <param name="arrayIn">Input Array</param>
        /// <returns>A copy of the original array with all elements in reverse order</returns>
        public static double[] ReverseArray(double[] arrayIn)
        {
            if (arrayIn == null)
            {
                throw new AlgorithmException("ReverseArray : arrayIn is null");
            }
            double[] returnArray = (double[])arrayIn.Clone();
            Array.Reverse(returnArray);
            return returnArray;
        }

        /// <summary>
        /// Sums each element of a pair of arrays of matching size
        /// </summary>
        /// <param name="array1">First array</param>
        /// <param name="array2">Second array</param>
        /// <returns>A new array where each element of the sum of the corresponding elements of the two input arrays</returns>
        public static double[] AddArrays(double[] array1, double[] array2)
        {
            // PRECONDITIONS
            if (array1 == null || array2 == null)
            {
                throw new AlgorithmException("Alg_ArrayFunctions.addArrays : neither array may be null");
            }
            if (array1.Length != array2.Length)
            {
                throw new AlgorithmException("Alg_ArrayFunctions.addArrays : The arrays must be of the same size. Array 1 has " + array1.Length + " elements and Array 2 has " + array2.Length + " elements");
            }

            double[] resultArray = new double[array1.Length];

            for (int i = 0; i < array1.Length; i++)
            {
                resultArray[i] = array1[i] + array2[i];
            }
            return resultArray;
        }

        /// <summary>
        /// Subtracts each element of the second array from the matching element of the first array
        /// </summary>
        /// <param name="array1">First array</param>
        /// <param name="array2">Array of values to be subtracted</param>
        /// <returns>A new array where each element of the difference between the corresponding elements of the two input arrays</returns>
        public static double[] SubtractArrays(double[] array1, double[] array2)
        {
            // PRECONDITIONS
            if (array1 == null || array2 == null)
            {
                throw new AlgorithmException("Alg_ArrayFunctions.JoinArrays : Neither array may be null");
            }
            if (array1.Length != array2.Length)
            {
                throw new AlgorithmException("Alg_ArrayFunctions.subtractArrays : The arrays must be of the same size. Array 1 has " + array1.Length + " elements and Array 2 has " + array2.Length + " elements");
            }

            double[] resultArray = new double[array1.Length];

            for (int i = 0; i < array1.Length; i++)
            {
                resultArray[i] = array1[i] - array2[i];
            }
            return resultArray;
        }

        /// <summary>
        /// Appends two arrays.
        /// </summary>
        /// <param name="array1">Start array</param>
        /// <param name="array2">Array to append</param>
        /// <returns>An array of consisting of all the elements of array2 appended to array1.</returns>
        public static double[] JoinArrays(double[] array1, double[] array2)
        {
            if (array1 == null || array2 == null )
            {
                throw new AlgorithmException("Alg_ArrayFunctions.JoinArrays : Neither array may be null");
            }
            double[] bigArray = new double[array1.Length + array2.Length];

            array1.CopyTo(bigArray,0);
            array2.CopyTo(bigArray,array1.Length);

            return bigArray;
        }

        /// <summary>
        /// Appends two arrays.
        /// </summary>
        /// <param name="array1">Start array</param>
        /// <param name="array2">Array to append</param>
        /// <returns>An array of consisting of all the elements of array2 appended to array1.</returns>
        public static int[] JoinArrays(int[] array1, int[] array2)
        {
            if (array1 == null || array2 == null)
            {
                throw new AlgorithmException("JoinArrays: Neither array may be null");
            }

            int[] bigArray = new int[array1.Length + array2.Length];

            array1.CopyTo(bigArray, 0);
            array2.CopyTo(bigArray, array1.Length);

            return bigArray;
        }

        /// <summary>
        /// Extracts the subset of data between two specified indeces.
        /// </summary>
        /// <param name="inputArray">Array to extract data from</param>
        /// <param name="fromIndex">Starting index</param>
        /// <param name="toIndex">Stop index</param>
        /// <returns>A copy of the array containing data between fromIndex and toIndex</returns>
        public static double[] ExtractSubArray(double[] inputArray, int fromIndex, int toIndex)
        {
            // PRECONDITIONS
            if ( fromIndex < 0 || fromIndex >= inputArray.Length )
            {
                throw new AlgorithmException("Alg_ArrayFunctions.extractSubArray : fromIndex is out of bounds. Array has " + inputArray.Length + " elements, fromIndex is " + fromIndex);
            }
            if ( toIndex < 0 || toIndex >= inputArray.Length )
            {
                throw new AlgorithmException("Alg_ArrayFunctions.extractSubArray : toIndex is out of bounds. Array has " + inputArray.Length + " elements, toIndex is " + toIndex);
            }
            if ( fromIndex > toIndex )
            {
                throw new AlgorithmException("Alg_ArrayFunctions.extractSubArray : fromIndex (" + fromIndex + ") is greater then toIndex (" + toIndex + ")");
            }
            if (inputArray.Length == 0)
            {
                throw new AlgorithmException("Alg_ArrayFunctions.extractSubArray : inputArray is empty");
            }

            double[] subsetArray = new double[toIndex - fromIndex + 1];

            for (int i = fromIndex; i <= toIndex; i++)
            {
                subsetArray[i-fromIndex] = inputArray[i];
            }
            return subsetArray;
            }

        /// <summary>
        /// Extracts the subset of data between two specified indeces.
        /// </summary>
        /// <param name="inputArray">Array to extract data from</param>
        /// <param name="fromIndex">Starting index</param>
        /// <param name="toIndex">Stop index</param>
        /// <returns>A copy of the array containing data between fromIndex and toIndex</returns>
        public static byte[] ExtractSubArray(byte[] inputArray, int fromIndex, int toIndex)
        {
            // PRECONDITIONS
            if ( fromIndex < 0 || fromIndex >= inputArray.Length )
            {
                throw new AlgorithmException("Alg_ArrayFunctions.extractSubArray : fromIndex is out of bounds. Array has " + inputArray.Length + " elements, fromIndex is " + fromIndex);
            }
            if ( toIndex < 0 || toIndex >= inputArray.Length )
            {
                throw new AlgorithmException("Alg_ArrayFunctions.extractSubArray : toIndex is out of bounds. Array has " + inputArray.Length + " elements, toIndex is " + toIndex);
            }
            if ( fromIndex > toIndex )
            {
                throw new AlgorithmException("Alg_ArrayFunctions.extractSubArray : fromIndex (" + fromIndex + ") is greater then toIndex (" + toIndex + ")");
            }
            if (inputArray.Length == 0)
            {
                throw new AlgorithmException("Alg_ArrayFunctions.extractSubArray : inputArray is empty");
            }

            byte[] subsetArray = new byte[toIndex - fromIndex + 1];

            for (int i = fromIndex; i <= toIndex; i++)
            {
                subsetArray[i-fromIndex] = inputArray[i];
            }
            return subsetArray;
        }


        /// <summary>
        /// Finds the index of the element that has a value closest to valueToFind
        /// </summary>
        /// <param name="inputArray">Set of data to search</param>
        /// <param name="valueToFind">Value to be found</param>
        /// <returns>Index of the element having a value closest to 'valueToFind'</returns>
        public static int FindIndexOfNearestElement(double[] inputArray, double valueToFind)
        {
            // PRECONDITIONS
            if (inputArray == null)
            {
                throw new AlgorithmException("Alg_ArrayFunctions.findIndexOfNearestElement : inputArray is null");
            }
            if (inputArray.Length == 0)
            {
                throw new AlgorithmException("Alg_ArrayFunctions.findIndexOfNearestElement : inputArray is empty");
            }

            double smallestDifference = double.MaxValue;
            int nearestElement = 0;

            for (int i = 0; i < inputArray.Length; i++)
            {
                double difference = Math.Abs(inputArray[i] - valueToFind);
                if (difference < smallestDifference)
                {
                    smallestDifference = difference;
                    nearestElement = i;
                }
            }

            return nearestElement;
        }

        /// <summary>
        /// Removes any data points outside the range specified by minimumValue and maximumValue.
        /// </summary>
        /// <param name="minimumValue">Minimum value</param>
        /// <param name="maximumValue">Maximum value</param>
        /// <param name="rawArray">Array of data</param>
        /// <param name="mode">Specify whether to use data within or outside the min and max values</param>
        /// <returns>The subset of data matching the above criteria</returns>
        public static double[] GetSubArrayByValueRange(double minimumValue, double maximumValue, double[] rawArray, BoundaryCondition mode)
        {
            double[] subsetArray;

            switch (mode)
            {
                case BoundaryCondition.OutsideMinMaxBoundary:
                    subsetArray = Array.FindAll(rawArray, delegate(double value) { return value < minimumValue || value > maximumValue; });
                    break;
                
                case BoundaryCondition.InsideMinMaxBoundary:
                default:
                    subsetArray = Array.FindAll(rawArray, delegate(double value) { return value >= minimumValue && value <= maximumValue; });
                    break;
            }

            return subsetArray;
        }
        
        /// <summary>
        /// Defines how to search for values
        /// </summary>
        public enum BoundaryCondition
        {
            /// <summary>
            /// Less than or equal to the boundary conditions
            /// </summary>
            InsideMinMaxBoundary,
            /// <summary>
            /// Greater than the boundary conditions
            /// </summary>
            OutsideMinMaxBoundary
        }
    }
}
