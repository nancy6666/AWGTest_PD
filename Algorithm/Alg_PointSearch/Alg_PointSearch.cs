
using System;


namespace TestSystem.TestLibrary.Algorithms
{
    /// <summary>
    /// Provides methods to find specific points in an array of values. 
    /// Accurate 'only' up to 15th decimal place
    /// </summary>
    public sealed class Alg_PointSearch
    {
        /// <summary>
        /// Empty private constructor.
        /// </summary>
        private Alg_PointSearch()
        {
        }
    
        /// <summary>
        /// Function to search an array for a given value in the array, and will return the index of the FIRST matching value 
        /// </summary>
        /// <param name="myOneDArray">The one dimensional array we wish to search</param>
        /// <param name="myComparisonOp">An enum which indicates the valid comparsions you can specify</param>
        /// <param name="mySearchValue">The value you wish to find in the array</param>
        /// <param name="myStartIndex">Starting index point in the array that you wish to commence your search operatons from</param>
        /// <param name="myStopIndex">Last index point in the array that you wish to search up to</param>
        /// <returns>This array index value that matches our query conditions, -1 if not successful </returns>
        public static int FindFirstValue ( double[] myOneDArray,  ComparisonOperator myComparisonOp, double mySearchValue, int myStartIndex, int myStopIndex )
        {
            SetupValues(myOneDArray, myComparisonOp, mySearchValue, myStartIndex, myStopIndex);

            return (Array.FindIndex(myOneDArray, startIndex, count, ComparisonOperation));
        }

        /// <summary>
        /// Function to search an array for a given value in the array, and will return the index of the FIRST matching value 
        /// </summary>
        /// <param name="myOneDArray">The one dimensional array we wish to search</param>
        /// <param name="myComparisonOp">An enum which indicates the valid comparsions you can specify</param>
        /// <param name="mySearchValue">The value you wish to find in the array</param>
        /// <returns>This array index value that matches our query conditions, -1 if not successful </returns>
        public static int FindFirstValue(double[] myOneDArray, ComparisonOperator myComparisonOp, double mySearchValue)
        {
            SetupValues(myOneDArray, myComparisonOp, mySearchValue);
            
            return(Array.FindIndex(myOneDArray, startIndex, count, ComparisonOperation));
        }

        /// <summary>
        /// Function to search an array for a given value in the array, and will return the index of the LAST matching value 
        /// </summary>
        /// <param name="myOneDArray">The one dimensional array we wish to search</param>
        /// <param name="myComparisonOp">An enum which indicates the valid comparsions you can specify</param>
        /// <param name="mySearchValue">The value you wish to find in the array</param>
        /// <param name="myStartIndex">Starting index point in the array that you wish to commence your search operatons from</param>
        /// <param name="myStopIndex">Last index point in the array that you wish to search up to</param>
        /// <returns>This array index value that matches our query conditions, -1 if not successful </returns>
        public static int FindLastValue(double[] myOneDArray, ComparisonOperator myComparisonOp, double mySearchValue, int myStartIndex, int myStopIndex)
        {
            SetupValues(myOneDArray, myComparisonOp, mySearchValue, myStartIndex, myStopIndex);

            return(Array.FindLastIndex(myOneDArray, stopIndex, count, ComparisonOperation));
        }

        /// <summary>
        /// Function to search an array for a given value in the array, and will return the index of the LAST matching value 
        /// </summary>
        /// <param name="myOneDArray">The one dimensional array we wish to search</param>
        /// <param name="myComparisonOp">An enum which indicates the valid comparsions you can specify</param>
        /// <param name="mySearchValue">The value you wish to find in the array</param>
        /// <returns>This array index value that matches our query conditions, -1 if not successful</returns>
        public static int FindLastValue(double[] myOneDArray, ComparisonOperator myComparisonOp, double mySearchValue)
        {

            SetupValues(myOneDArray, myComparisonOp, mySearchValue);

            return(Array.FindLastIndex(myOneDArray, stopIndex, count, ComparisonOperation));
        }

        /// <summary>
        /// Function to find the Minimum value within an one dimensional arrays contents
        /// </summary>
        /// <param name="oneDArray">The one dimensional array we wish to search</param>
        /// <returns>The minimum value within the array</returns>
        public static double FindMinValueInArray(double[] oneDArray)
        {
            if (oneDArray == null)
            {
                throw new AlgorithmException("Alg_PointSearch:  Null array passed in");
            }

            if (oneDArray.Length == 0)
            {
                throw new AlgorithmException("Alg_PointSearch:  Non valid array passed in");
            }
            
            double minValue = oneDArray[0];
            
            foreach (double x in oneDArray)
	        {
		        if (x < minValue)
                {
                    minValue = x;
                }
	        }
            
            return (minValue);
        }

        /// <summary>
        /// Function to find the Minimum value within an one dimensional arrays contents
        /// </summary>
        /// <param name="oneDArray">The one dimensional array we wish to search</param>
        /// <returns>The minimum value within the array</returns>
        public static double FindMinValueInArray(double[] oneDArray,int iStart,int iStop)
        {
            if (oneDArray == null)
            {
                throw new AlgorithmException("Alg_PointSearch:  Null array passed in");
            }

            if (oneDArray.Length == 0)
            {
                throw new AlgorithmException("Alg_PointSearch:  Non valid array passed in");
            }

            double minValue = oneDArray[iStart];
            for (int i = iStart; i <= iStop; i++)
            {
                if (oneDArray[i] < minValue)
                {
                    minValue = oneDArray[i];
                }
            }
            //foreach (double x in oneDArray)
            //{
            //    if (x < minValue)
            //    {
            //        minValue = x;
            //    }
            //}
            return (minValue);
        }

        /// <summary>
        /// Function to find the Maximum value within an one dimensional arrays contents
        /// </summary>
        /// <param name="oneDArray">The one dimensional array we wish to search</param>
        /// <returns>The maximum value within the array</returns>
        public static double FindMaxValueInArray(double[] oneDArray)
        {
            if (oneDArray == null)
            {
                throw new AlgorithmException("Alg_PointSearch:  Null array passed in");
            }

            if (oneDArray.Length == 0)
            {
                throw new AlgorithmException("Alg_PointSearch:  Non valid array passed in");
            }

            double maxValue = oneDArray[0];
            
            foreach (double x in oneDArray)
            {
                if (x > maxValue)
                {
                    maxValue = x;
                }
            }
            return (maxValue);
        }

        /// <summary>
        /// Function to find the Maximum value within an one dimensional arrays contents
        /// </summary>
        /// <param name="oneDArray">The one dimensional array we wish to search</param>
        /// <returns>The maximum value within the array</returns>
        public static double FindMaxValueInArray(double[] oneDArray,int iStart,int iStop)
        {
            if (oneDArray == null)
            {
                throw new AlgorithmException("Alg_PointSearch:  Null array passed in");
            }

            if (oneDArray.Length == 0)
            {
                throw new AlgorithmException("Alg_PointSearch:  Non valid array passed in");
            }

            double maxValue = oneDArray[iStart];
            for (int i = iStart; i<= iStop; i++)
            {
                if (oneDArray[i] > maxValue)
                {
                    maxValue = oneDArray[i];
                }
            }
            //foreach (double x in oneDArray)
            //{
            //    if (x > maxValue)
            //    {
            //        maxValue = x;
            //    }
            //}
            return (maxValue);
        }
        /// <summary>
        /// Function to find the first instance of the Maximum value within an one dimensional arrays contents
        /// </summary>
        /// <param name="oneDArray">The one dimensional array we wish to search</param>
        /// <returns>The index of the first instance of the maximum value within the array </returns>
        public static int FindFirstIndexOfMaxValueInArray(double[] oneDArray)
        {
            ComparisonOperator mycp= ComparisonOperator.EqualTo;
            if (oneDArray == null)
            {
                throw new AlgorithmException("Alg_PointSearch:  Null array passed in");
            }
           
            double maxValue = Alg_PointSearch.FindMaxValueInArray(oneDArray);
            
            return(Alg_PointSearch.FindFirstValue(oneDArray, mycp, maxValue));
            
        }

        /// <summary>
        /// Function to find the Last instance of the Maximum value within an one dimensional arrays contents
        /// </summary>
        /// <param name="oneDArray">The one dimensional array we wish to search</param>
        /// <returns>The index of the last instance of the maximum value within the array </returns>
        public static int FindLastIndexOfMaxValueInArray(double[] oneDArray)
        {
            ComparisonOperator mycp = ComparisonOperator.EqualTo;

            if (oneDArray == null)
            {
                throw new AlgorithmException("Alg_PointSearch:  Null array passed in");
            }

            double maxValue = Alg_PointSearch.FindMaxValueInArray(oneDArray);
            
            return(Alg_PointSearch.FindLastValue(oneDArray, mycp, maxValue));
            
        }

        /// <summary>
        /// Function to find the first instance of the Minimum value within an one dimensional arrays contents
        /// </summary>
        /// <param name="oneDArray">The one dimensional array we wish to search</param>
        /// <returns>The index of the first instance of the Minimum value within the array </returns>
        public static int FindFirstIndexOfMinValueInArray(double[] oneDArray)
        {
            ComparisonOperator mycp = ComparisonOperator.EqualTo;
            if (oneDArray == null)
            {
                throw new AlgorithmException("Alg_PointSearch:  Null array passed in");
            }

            double minValue = Alg_PointSearch.FindMinValueInArray(oneDArray);
            
            return(Alg_PointSearch.FindFirstValue(oneDArray, mycp, minValue));
            
        }

        /// <summary>
        /// Function to find the Last instance of the Minimum value within an one dimensional arrays contents
        /// </summary>
        /// <param name="oneDArray">The one dimensional array we wish to search</param>
        /// <returns>The index of the last instance of the Minimum value within the array </returns>
        public static int FindLastIndexOfMinValueInArray(double[] oneDArray)
        {
            ComparisonOperator mycp = ComparisonOperator.EqualTo;
            if (oneDArray == null)
            {
                throw new AlgorithmException("Alg_PointSearch:  Null array passed in");
            }

            double minValue = Alg_PointSearch.FindMinValueInArray(oneDArray);
            
            return(Alg_PointSearch.FindLastValue(oneDArray, mycp, minValue));
            
        }

        /// <summary>
        /// A public enum to indicate the valid comparison operations that are available
        /// </summary>
        public enum ComparisonOperator
        {	
            /// <summary>Equal To</summary>
            EqualTo,
            /// <summary>Greater Than</summary>
            GreaterThan,
            /// <summary>Greater Than or Equal To</summary>
            GreaterThanOrEqualTo,
            /// <summary>Less Than</summary>
            LessThan,
            /// <summary>Less Than or Equal To</summary>
            LessThanOrEqualTo
        }

        /// <summary>
        /// private function which will check values are within our expected preconditions, and will also keep local module level copies of our key variables
        /// </summary>
        /// <param name="myOneDArray">The one dimensional array we wish to search</param>
        /// <param name="myComparisonOp">An enum which indicates the valid comparsions you can specify</param>
        /// <param name="mySearchValue">The value you wish to find in the array</param>
        /// <param name="myStartIndex">Starting index point in the array that you wish to commence your search operatons from</param>
        /// <param name="myStopIndex">Last index point in the array that you wish to search up to</param>
        private static void SetupValues(double[] myOneDArray, ComparisonOperator myComparisonOp, double mySearchValue, int myStartIndex, int myStopIndex)
        {
            comparisonOp = myComparisonOp;
            searchValue = mySearchValue;
            stopIndex = myStopIndex;
            startIndex = myStartIndex;
            count = myStopIndex - myStartIndex;

            //check for null array
            if (myOneDArray == null) 
            {
                throw new AlgorithmException("Alg_PointSearch:  Null array passed in");
            }

            if (myOneDArray.Length ==0)
            {
                throw new AlgorithmException("Alg_PointSearch:  Non valid array passed in");
            }
            //Let us do some checking of the numbers we have been passed in
            if (stopIndex >= myOneDArray.Length) 
            {
                //stop index is greater than array size...
                throw new AlgorithmException("Alg_PointSearch:  Specified stop index is greater than array size");
            }
            if (startIndex == stopIndex)
            { 
                //we aren't searching any values in our array are we?
                throw new AlgorithmException("Alg_PointSearch:  Specified stop index is the same as start index");
            }
            if (myStartIndex > myStopIndex)
            {
                //they have passed thru incorrect values
                throw new AlgorithmException("Alg_PointSearch:  Specified start index is greater than stop index");
            }
            if (startIndex < 0)
            {
                //the index value for starting cannot be less than 0
                throw new AlgorithmException("Alg_PointSearch:  Specified start index cannot be less than zero");
            }
        }

        /// <summary>
        /// private function which will check values are within our expected preconditions, and will also keep local module level copies of our key variables
        /// </summary>
        /// <param name="myOneDArray">The one dimensional array we wish to search</param>
        /// <param name="myComparisonOp">An enum which indicates the valid comparsions you can specify</param>
        /// <param name="mySearchValue">The value you wish to find in the array</param>
        private static void SetupValues(double[] myOneDArray, ComparisonOperator myComparisonOp, double mySearchValue)
        {
            //check for null array
            if (myOneDArray == null)
            {
                throw new AlgorithmException("Alg_PointSearch:  Null array passed in");
            }

            //Call our other function but pass in the arrays lower and upper bounds in place of start and stop indexes
            SetupValues(myOneDArray,myComparisonOp,mySearchValue,myOneDArray.GetLowerBound(0),myOneDArray.GetUpperBound(0));
        }

        /// <summary>
        /// This function will compare the input value with the search value and will return
        /// true if it meets the specified comparison criteria
        /// nb: Both the search value, and the comparison criteria are module level static variables, and are setup
        /// before this function is ever called.
        /// </summary>
        /// <param name="compValue">The value to do the comparison with</param>
        /// <returns>true if the input value matches the comparison criteria with the search value</returns>
        private static bool ComparisonOperation(double compValue)
        {
            switch (comparisonOp)
            {
                case ComparisonOperator.EqualTo:
                    return(compValue == searchValue);
                    
                case ComparisonOperator.GreaterThan:
                    return (compValue > searchValue);
                    
                case ComparisonOperator.GreaterThanOrEqualTo:
                    return (compValue >= searchValue);
                    
                case ComparisonOperator.LessThan:
                    return (compValue < searchValue);
                    
                case ComparisonOperator.LessThanOrEqualTo:
                    return (compValue <= searchValue);
                    
                default:
                    throw new AlgorithmException("Alg_PointSearch:  Unrecognised comparison operator");
            }
        
        }

        /// <summary>
        /// Local container for the specified comparision operation
        /// </summary>
        private static ComparisonOperator comparisonOp;
        /// <summary>
        /// Local container for the specified searchvalue
        /// </summary>
        private static double searchValue;
        /// <summary>
        /// Local container for the specified start index
        /// </summary>
        private static int startIndex;
        /// <summary>
        /// Local container for the specified stop index
        /// </summary>
        private static int stopIndex;
        /// <summary>
        /// Local container for the count value (stop index - start index)
        /// </summary>
        private static int count;
}

}