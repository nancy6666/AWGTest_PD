using System;
using System.Collections.Generic;
using System.Text;
using TestSystem.TestLibrary.Algorithms;

namespace TestSystem.TestLibrary.Algorithms
{
    /// <summary>
    /// Class to calculate the deviation from a Polynomial Fit of some data
    /// </summary>
    public sealed class Alg_DevFromPolyFit
    {
        /// <summary>
        /// Empty private constructor.
        /// </summary>
        private Alg_DevFromPolyFit()
        {
        }

        /// <summary>
        /// Function to calculate the deviation from a PolyFit fit, with array index arguments being provided
        /// </summary>
        /// <param name="x">Our array of x values</param>
        /// <param name="y">Our array of y values</param>
        /// <param name="fromIndex">The array index indicating the START of the range we wish to calculate deviation from a PolyFit fit from</param>
        /// <param name="toIndex">The array index indicating the END of the range we wish to calculate deviation from a PolyFit fit from</param>
        /// <param name="polyFitOrder">eg 2nd order polyfit, value = 2</param>
        /// <returns>A devFromPolyFitResults structure</returns>
        public static DevFromPolyFitResults DevFromPolyFit( double[] x, double[] y, int fromIndex, int toIndex, int polyFitOrder )
        {
            if (x == null || y == null)
            {
                throw new AlgorithmException("devFromPolyFit - input array is null");
            }

            double xlength = x.Length;
            double ylength = y.Length;



            if (xlength != ylength)
            {
                throw new AlgorithmException("devFromPolyFit - input arrays must be the same size");
            }
            if (xlength < 3)
            {
                throw new AlgorithmException("devFromPolyFit - need at least 3 data points");
            }

            if (toIndex >= x.Length)
            {
                throw new AlgorithmException("devFromPolyFit - cannot have toindex bigger than array size");
            }

            if (fromIndex >= toIndex)
            {
                throw new AlgorithmException("devFromPolyFit - cannot have fromIndex bigger than toIndex");
            }

            //do a 'nn' Order polyfit on the array data over the range stipulated

            PolyFit myPolyFit = Alg_PolyFit.PolynomialFit_indexArgs(x, y, fromIndex, toIndex, polyFitOrder);

            //create a results structure
            DevFromPolyFitResults myResultsStruct = new DevFromPolyFitResults();
            
            //populate the fromIndex and ToIndex as used, (important to feed this back as the Alg user may have originally stipulated only
            //x values, not indexes, and i only wish to have 1 distinct results structure in place for all devFromPolyFit_xxx functions)
            myResultsStruct.FromIndexValueUsed = fromIndex;
            myResultsStruct.ToIndexValueUsed = toIndex;
            myResultsStruct.PolyFittedYAxisData = myPolyFit.FittedYArray;
            
            //create an array to hold the calculated deviation from PolyFit values
            double[] deviationFromFittedYArray = new double[x.Length];

            // Populate the whole of the fitted line array
	        for(  int i = 0; i < x.Length; i++ )
	        {
                //seems a good time to calculate the deviation from the fitted slope as well
                deviationFromFittedYArray[i] = myResultsStruct.PolyFittedYAxisData[i] - y[i];
            }

            //need to cut this 'deviation' array down to the correct range we wish to examine for max, min values
            deviationFromFittedYArray = Alg_ArrayFunctions.ExtractSubArray(deviationFromFittedYArray,fromIndex,toIndex);


            myResultsStruct.MaxPosDevFromPolyFitValue = Alg_PointSearch.FindMaxValueInArray(deviationFromFittedYArray);
            myResultsStruct.MaxNegDevFromPolyFitValue = Alg_PointSearch.FindMinValueInArray(deviationFromFittedYArray);
            
            //need to establish the abs maximum of the 2 values now
            double a = Math.Abs( myResultsStruct.MaxPosDevFromPolyFitValue );
            double b = Math.Abs( myResultsStruct.MaxNegDevFromPolyFitValue );


            myResultsStruct.MaxAbsDeviationFromPolyFitValue = (a > b) ? a : b;

            return (myResultsStruct);
            
        }


        /// <summary>
        /// Function to calculate the deviation from a PolyFit fit, using the full array 
        /// </summary>
        /// <param name="x">Our array of x values</param>
        /// <param name="y">Our array of y values</param>
        /// <param name="polyFitOrder">eg 2nd order polyfit, value = 2</param>
        /// <returns>A devFromPolyFitResults structure</returns>
        public static DevFromPolyFitResults DevFromPolyFit(double[] x, double[] y, int polyFitOrder)
        {
            if (x == null || y == null)
            {
                throw new AlgorithmException("devFromPolyFit - input array is null");
            }
            int xlength = x.Length-1;
            return (Alg_DevFromPolyFit.DevFromPolyFit(x, y, 0, xlength, polyFitOrder));

        }

        /// <summary>
        /// Function to calculate the deviation from a PolyFit fit, with x values being provided for FROM and TO, and this function calculating the array index's to use
        /// </summary>
        /// <param name="x">Our array of x values</param>
        /// <param name="y">Our array of y values</param>
        /// <param name="fitFromNearestXvalue">Function will search for the x value nearest to that specified and will use the index of this as the fromIndex array value</param>
        /// <param name="fitToNearestXvalue">Function will search for the x value nearest to that specified and will use the index of this as the toIndex array value</param>
        /// <param name="polyFitOrder">eg 2nd order polyfit, value = 2</param>
        /// <returns>devFromPolyFitResults structure</returns>
        public static DevFromPolyFitResults DevFromPolyFit_xValues( double[] x, double[] y, double fitFromNearestXvalue, double fitToNearestXvalue, int polyFitOrder)
        {
            if (x == null || y == null)
            {
                throw new AlgorithmException("devFromPolyFit - input array is null");
            }

            int fromIndex = Alg_ArrayFunctions.FindIndexOfNearestElement(x, fitFromNearestXvalue);
            int toIndex = Alg_ArrayFunctions.FindIndexOfNearestElement(x, fitToNearestXvalue);

            return(DevFromPolyFit( x, y, fromIndex, toIndex, polyFitOrder )); 
        
        }

        /// <summary>
        /// A structure to encapsulate the results we will get back from our Alg_DevFromPolyFit functions
        /// </summary>
        public struct DevFromPolyFitResults
        {
            /// <summary>
            /// This is our original y axis data, that has now been adjusted based on results
            /// from using Alg_PolyFit.  Note that the WHOLE y axis array is adjusted, not just the subArray as stipulated
            /// with From and To index values
            /// </summary>
            public double[] PolyFittedYAxisData;
            /// <summary>
            /// The maximum positive deviation from the adjusted PolyFit fitted value.  
            /// This is calculated using only the values within the subArray as stipulated
            /// with From and To index values.
            /// </summary>
            public double MaxPosDevFromPolyFitValue;
            /// <summary>
            /// The maximum negative deviation from the adjusted PolyFit fitted value.  
            /// This is calculated using only the values within the subArray as stipulated
            /// with From and To index values.
            /// </summary>
            public double MaxNegDevFromPolyFitValue;
            /// <summary>
            /// The 'absolute' maximum deviation from the adjusted PolyFit fitted value.  
            /// This is calculated using only the values within the subArray as stipulated
            /// with From and To index values.
            /// </summary>
            public double MaxAbsDeviationFromPolyFitValue;
            /// <summary>
            /// The 'from' array index value we have used for our calculations
            /// </summary>
            public int FromIndexValueUsed;
            /// <summary>
            /// The 'to' array index value we have used for our calculations 
            /// </summary>
            public int ToIndexValueUsed;
        }

    }
}
