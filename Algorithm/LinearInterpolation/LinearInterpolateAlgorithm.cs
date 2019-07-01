

using System;


namespace TestSystem.TestLibrary.Algorithms

{
	/// <summary>
	/// Class containing LinearInterpolation algorithm
	/// </summary>
	public sealed class LinearInterpolateAlgorithm
	{
        /// <summary>
        /// Private Constructor protects class
        /// </summary>
        private LinearInterpolateAlgorithm()
        {

        }

		/// <summary>
		/// Perform linear interpolation on the specified values
		/// </summary>
		/// <param name="x1">x1</param>
		/// <param name="x2">x2</param>
		/// <param name="y1">y1</param>
		/// <param name="y2">y2</param>
		/// <param name="xValue">x value to interpolate for</param>
		/// <returns>Interpolated y value</returns>
        /// 

		public static double Calculate( double x1, double x2, double y1, double y2, double xValue )
		{
			// PRECONDITION's
			if( x1 == x2 )
			{
				throw new AlgorithmException( "x1 == x2" );
			}	

			double gradient = (y2-y1) / (x2-x1);
			double interpolatedYvalue = gradient*(xValue-x1) + y1;

			return interpolatedYvalue;
		}
	}
}
