using System;
using System.Diagnostics;
using NUnit.Framework;


namespace TestSystem.TestLibrary.Algorithms
{
	/// <summary>
	/// Test harness for LinearInterpolate
	/// </summary>
	[TestFixture]
	public class LinearInterpolateTEST
	{

        /// <exclude/>
        [Test]
 		[ExpectedException (typeof(AlgorithmException))]
        public void x1_equals_x2_fail()
        {
 			double y = LinearInterpolateAlgorithm.Calculate( 10, 10, 1, 2, 15 );
        }

        /// <exclude/>
        [Test]
		public void xInc_yInc_Ok()
		{	
			double y = LinearInterpolateAlgorithm.Calculate( 10, 20, 1, 2, 15 );
			Debug.Assert( y == 1.5 );

			y = LinearInterpolateAlgorithm.Calculate( 10, 20, -1, -2, 15 );
			Debug.Assert( y == -1.5 );

			y = LinearInterpolateAlgorithm.Calculate( -10, -20, 1, 2, -15 );
			Debug.Assert( y == 1.5 );

			y = LinearInterpolateAlgorithm.Calculate( -10, -20, -1, -2, -15 );
			Debug.Assert( y == -1.5 );
		}

        /// <exclude/>
        [Test]
        public void xInc_yDec_Ok()
		{
			double y = LinearInterpolateAlgorithm.Calculate( 10, 20, 2, 1, 15 );
			Debug.Assert( y == 1.5 );

			y = LinearInterpolateAlgorithm.Calculate( 10, 20, -2, -1, 15 );
			Debug.Assert( y == -1.5 );

			y = LinearInterpolateAlgorithm.Calculate( -10, -20, 2, 1, -15 );
			Debug.Assert( y == 1.5 );

			y = LinearInterpolateAlgorithm.Calculate( -10, -20, -2, -1, -15 );
			Debug.Assert( y == -1.5 );
		}
	}
}
