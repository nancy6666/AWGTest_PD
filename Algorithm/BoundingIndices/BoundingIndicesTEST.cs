using System;
using System.Diagnostics;
using NUnit.Framework;


namespace TestSystem.TestLibrary.Algorithms
{
	/// <summary>
	/// Test Harness for BoundingIndices
	/// </summary>
	[TestFixture]
	public class BoundingIndicesTEST
	{

		//
		// ERROR CONDITIONS
		//
        /// <exclude/>
        [Test]
		[ExpectedException (typeof(AlgorithmException))]
		public void NullArray()
		{
			double[] nullArray = null;
			boundingIndices = BoundingIndicesAlgorithm.Calculate( nullArray, 4 );
			Debug.Assert( false );
		}

        /// <exclude/>
        [Test]
		[ExpectedException (typeof(AlgorithmException))]
		public void EmptyArray()
		{
			double[] emptyArray = {};
			boundingIndices = BoundingIndicesAlgorithm.Calculate( emptyArray, 4 );
			Debug.Assert( false );
		}

        /// <exclude/>
        [Test]
		[ExpectedException (typeof(AlgorithmException))]
		public void SearchValueOutsideArrayRange()
		{
			boundingIndices = BoundingIndicesAlgorithm.Calculate( incPosdataArray, 1000 );
			Debug.Assert( boundingIndices.IndexOfArrayValueLessThan == 0 );
			Debug.Assert( boundingIndices.IndexOfArrayValueGreaterThan == 0 );
		}



		//
		// AVAILABLE OPERATIONS
		//

        /// <exclude/>
        [Test]
		public void SearchValueWithinArray()
		{
			// inc +ve data
			boundingIndices = BoundingIndicesAlgorithm.Calculate( incPosdataArray, 20 );
 
			Debug.Assert( boundingIndices.IndexOfArrayValueLessThan == 1 );
			Debug.Assert( boundingIndices.IndexOfArrayValueGreaterThan == 1 );
			// inc -ve data
			boundingIndices = BoundingIndicesAlgorithm.Calculate( incNegDataArray, -20 );
			Debug.Assert( boundingIndices.IndexOfArrayValueLessThan == 5 );
			Debug.Assert( boundingIndices.IndexOfArrayValueGreaterThan == 5 );		
		}

        /// <exclude/>
        [Test]
		public void SearchValueWithinArray_ManyContigousValuesTheSame()
		{
			// inc +ve data
			boundingIndices = BoundingIndicesAlgorithm.Calculate( incPosdataArray, 40 );
			Debug.Assert( boundingIndices.IndexOfArrayValueLessThan == 3 );
			Debug.Assert( boundingIndices.IndexOfArrayValueGreaterThan == 3 );
			// inc -ve data
			boundingIndices = BoundingIndicesAlgorithm.Calculate( incNegDataArray, -40 );
			Debug.Assert( boundingIndices.IndexOfArrayValueLessThan == 1 );
			Debug.Assert( boundingIndices.IndexOfArrayValueGreaterThan == 1 );
		}

        /// <exclude/>
        [Test]
		public void SearchValueAtStartOfArray()
		{
			// inc +ve data
			boundingIndices = BoundingIndicesAlgorithm.Calculate( incPosdataArray, 10 );
			Debug.Assert( boundingIndices.IndexOfArrayValueLessThan == 0 );
			Debug.Assert( boundingIndices.IndexOfArrayValueGreaterThan == 0 );
			// inc -ve data
			boundingIndices = BoundingIndicesAlgorithm.Calculate( incNegDataArray, -50 );
			Debug.Assert( boundingIndices.IndexOfArrayValueLessThan == 0 );
			Debug.Assert( boundingIndices.IndexOfArrayValueGreaterThan == 0 );
		}

        /// <exclude/>
        [Test]
		public void SearchValueAtEndOfArray()
		{
			// +ve data
			boundingIndices = BoundingIndicesAlgorithm.Calculate( incPosdataArray, 50 );
			Debug.Assert( boundingIndices.IndexOfArrayValueLessThan == 6 );
			Debug.Assert( boundingIndices.IndexOfArrayValueGreaterThan == 6 );
			// inc -ve data
			boundingIndices = BoundingIndicesAlgorithm.Calculate( incNegDataArray, -10 );
			Debug.Assert( boundingIndices.IndexOfArrayValueLessThan == 6 );
			Debug.Assert( boundingIndices.IndexOfArrayValueGreaterThan == 6 );
		}

        /// <exclude/>
        [Test]
		public void SearchValueBoundedByArrayData()
		{
			// inc +ve data
			boundingIndices = BoundingIndicesAlgorithm.Calculate( incPosdataArray, 25 );
			Debug.Assert( boundingIndices.IndexOfArrayValueLessThan == 1 );
			Debug.Assert( boundingIndices.IndexOfArrayValueGreaterThan == 2 );
			// inc -ve data
			boundingIndices = BoundingIndicesAlgorithm.Calculate( incNegDataArray, -25 );
			Debug.Assert( boundingIndices.IndexOfArrayValueLessThan == 4 );
			Debug.Assert( boundingIndices.IndexOfArrayValueGreaterThan == 5 );
			// dec +ve data
			boundingIndices = BoundingIndicesAlgorithm.Calculate( decPosdataArray, 25 );
			Debug.Assert( boundingIndices.IndexOfArrayValueLessThan == 5 );
			Debug.Assert( boundingIndices.IndexOfArrayValueGreaterThan == 4 );
			// dec -ve data
			boundingIndices = BoundingIndicesAlgorithm.Calculate( decNegDataArray, -25 );
            //Debug.Assert( boundingIndices != null );
			Debug.Assert( boundingIndices.IndexOfArrayValueLessThan == 2 );
			Debug.Assert( boundingIndices.IndexOfArrayValueGreaterThan == 1 );
		}

        /// <exclude/>
        [Test]
		public void SearchValueBoundedByArrayData_AtArrayStart()
		{
			// inc +ve data
			boundingIndices = BoundingIndicesAlgorithm.Calculate( incPosdataArray, 15 );
            //Debug.Assert( boundingIndices != null );
			Debug.Assert( boundingIndices.IndexOfArrayValueLessThan == 0 );
			Debug.Assert( boundingIndices.IndexOfArrayValueGreaterThan == 1 );
			// inc -ve data
			boundingIndices = BoundingIndicesAlgorithm.Calculate( incNegDataArray, -45 );
            //Debug.Assert( boundingIndices != null );
			Debug.Assert( boundingIndices.IndexOfArrayValueLessThan == 0 );
			Debug.Assert( boundingIndices.IndexOfArrayValueGreaterThan == 1 );
		}

        /// <exclude/>
        [Test]
		public void SearchValueBoundedByArrayData_AtArrayEnd()
		{
			// inc +ve data
			boundingIndices = BoundingIndicesAlgorithm.Calculate( incPosdataArray, 45 );
			Debug.Assert( boundingIndices.IndexOfArrayValueLessThan == 5 );
			Debug.Assert( boundingIndices.IndexOfArrayValueGreaterThan == 6 );
			// inc -ve data
			boundingIndices = BoundingIndicesAlgorithm.Calculate( incNegDataArray, -15 );
			Debug.Assert( boundingIndices.IndexOfArrayValueLessThan == 5 );
			Debug.Assert( boundingIndices.IndexOfArrayValueGreaterThan == 6 );
		}




		#region Private Data
		private double[] indexArray =			{   0,   1,   2,   3,   4,   5,   6 };
		private double[] incPosdataArray =		{  10,  20,  30,  40,  40,  40,  50 };
		private double[] decPosdataArray =		{  50,  40,  40,  40,  30,  20,  10 };
		private double[] incNegDataArray =		{ -50, -40, -40, -40, -30, -20, -10 };
		private double[] decNegDataArray =		{ -10, -20, -30, -40, -40, -40, -50 };

		private BoundingIndices boundingIndices; //= null
		#endregion
	}
}
