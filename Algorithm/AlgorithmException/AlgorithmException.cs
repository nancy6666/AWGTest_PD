

using System;

namespace TestSystem.TestLibrary.Algorithms
{
	/// <summary>
	/// Exception Class for Algorithms
	/// </summary>
	public class AlgorithmException : System.Exception
	{

		/// <summary>
		/// Prevent use of this default constructor
		/// </summary>
		private AlgorithmException() : base()
		{
		}

		/// <summary>
		/// Exception class for use by algorithm developers
		/// </summary>
		/// <param name="message">Description</param>
		public AlgorithmException( string message ) : base( message )
		{
		}

		/// <summary>
		/// Exception class for use by algorithm developers
		/// </summary>
		/// <param name="message">Description</param>
		/// <param name="e">Inner exception</param>
		public AlgorithmException( string message, Exception e ) : base( message, e )
		{
		}
	}
}
