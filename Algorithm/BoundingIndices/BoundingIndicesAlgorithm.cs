// [Copyright]
//
// Bookham Test Engine Library
//
// BoundingIndicesAlgorith.cs
//
// Author: Paul.Worsey, March 2006
// Design: As per Alg_BoundingIndicesAlgorithm DD

using System;


namespace TestSystem.TestLibrary.Algorithms
{
	
	/// <summary>
	/// Class containing the FindBoundingIndices algorithm
	/// </summary>
	public sealed class BoundingIndicesAlgorithm
	{
        /// <summary>
        /// Private Constructor protects class
        /// </summary>
        private BoundingIndicesAlgorithm()
        {
  
        }

        /// <summary>
        /// Given an array of values x, and a search value a, this algorithm tries to find
        /// the first pair of array values that lie either side of the search value.
        /// The indices of these array elements are reported as boundingIndices.IndexOfArrayValueLessThan and
        /// boundingIndices.IndexOfArrayValueGreaterThan.
        /// If the search value is less than every value in the array,
        /// or greater than every value in the array, the algorithm throws AlgorithmException.
        /// If the search value is precisely equal to one of the array elements,
        /// both boundingIndices.IndexOfArrayValueLessThan and boundingIndices.IndexOfArrayValueGreaterThan are assigned the
        /// index of that array element.
        /// Note: Due to finding first pair of array values this only works for single-value data.
        /// </summary>
        /// <param name="array">The array of values to search within</param>
        /// <param name="val">The value to search for</param>
        /// <returns>Bounding indices if the seach value was within the range of array values, otherwise throws AlgorithmException</returns>
       
        static public BoundingIndices Calculate( double[] array, double val )
		{
			//
			// PRECONDITIONS
			//
			if( array == null ) 
			{
				throw new AlgorithmException( "Null input array" );
			}
			if( array.Length < 2 )
			{
				throw new AlgorithmException( "Input array must have 2 or more elements" );
			}
			
			
			//
			// Find value in array
			// Loop from the start of the array until the last point but one
			//
			bool found = false;
			int indexOfArrayValueLessThan = 0;
			int indexOfArrayValueGreaterThan = 0;
			
			for( int i = 0; i < array.Length-1; i++ )
            {
                if (array[i] == val) // Using equality of doubles
                {
                    indexOfArrayValueLessThan = i;
                    indexOfArrayValueGreaterThan = i;
                    found = true;
                    break;
                }

                if (array[i] < val && val < array[i + 1])	// +ve value e.g.  10 <  15 <  20		
                {
                    indexOfArrayValueLessThan = i;
                    indexOfArrayValueGreaterThan = i + 1;
                    found = true;
                    break;
                }
                else if (array[i] > val && val > array[i + 1])	// -ve value e.g. -10 > -15 > -20
                {
                    indexOfArrayValueLessThan = i + 1;
                    indexOfArrayValueGreaterThan = i;
                    found = true;
                    break;
                }

                // Check the last array point
                if (i == array.Length - 2)
                {
                    if (array[i + 1] == val)
                    {
                        indexOfArrayValueLessThan = i + 1;
                        indexOfArrayValueGreaterThan = i + 1;
                        found = true;
                        break;
                    }
                }
            }


			//
			// POST CONDITIONS
			//
			if( !found )
			{
				throw new AlgorithmException( "Value not found within specified array" );
			}
			if( indexOfArrayValueLessThan > array.Length-1 || indexOfArrayValueGreaterThan > array.Length-1 )
			{
                throw new AlgorithmException("Internal error - output indices out of bounds");
			}



			return new BoundingIndices( indexOfArrayValueLessThan, indexOfArrayValueGreaterThan );
		}

	}
}
