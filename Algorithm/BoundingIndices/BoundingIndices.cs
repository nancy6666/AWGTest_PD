// [Copyright]
//
// Bookham Test Engine Library
//
// BoundingIndices.cs
//
// Author: Paul.Worsey, March 2006
// Design: As per Alg_BoundingIndicesAlgorithm DD

using System;



namespace TestSystem.TestLibrary.Algorithms
{
	/// <summary>
	/// A simple struct, defining the array indicies bounding a search value.
	/// </summary>
    public struct BoundingIndices
	{

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="indexOfArrayValueLessThan">Index of closet array element, less than the search value</param>
        /// <param name="indexOfArrayValueGreaterThan">Index of closet array element, greater than the search value</param>
		public BoundingIndices( int indexOfArrayValueLessThan, int indexOfArrayValueGreaterThan )
		{
            if (indexOfArrayValueLessThan < 0 || indexOfArrayValueGreaterThan < 0)
                throw new AlgorithmException("Array indices cannot be less than 0");

            if ( System.Math.Abs(indexOfArrayValueLessThan-indexOfArrayValueGreaterThan) > 1 )
                throw new AlgorithmException("Bounding indices must be equal or adjacent");


			this.IndexOfArrayValueLessThan = indexOfArrayValueLessThan;
			this.IndexOfArrayValueGreaterThan = indexOfArrayValueGreaterThan;
		}

        /// <summary>Index of closet array element, less than the search value</summary>
		public readonly int IndexOfArrayValueLessThan;
        /// <summary>Index of closet array element, greater than the search value</summary>
		public readonly int IndexOfArrayValueGreaterThan;
	}
}
