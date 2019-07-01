

using System;
using System.Collections.Generic;
using System.Text;

namespace TestSystem.TestLibrary.Algorithms
{
    /// <summary>
    /// Class to convert between deciBels and linear units
    /// </summary>
    public sealed class Alg_PowConvert_dB
    {
        /// <summary>
        /// Private constructor
        /// </summary>
        private Alg_PowConvert_dB() { }

        /// <summary>
        /// Convert dBm to mW
        /// </summary>
        /// <param name="power_dBm">Power value in dBm to be converted to mW</param>
        /// <returns>Value in mW</returns>
        public static double Convert_dBmtomW(double power_dBm)
        {
            // Conversion 
            return Math.Pow(10.0, power_dBm / 10.0);
        }

        /// <summary>
        /// Convert each element of an array from dBm to mW
        /// </summary>
        /// <param name="power_dBm">Power value in dBm to be converted to mW</param>
        /// <returns>Value in mW</returns>
        public static double[] Convert_dBmtomW(double[] power_dBm)
        {
            int arraySize = power_dBm.Length;
            if (arraySize == 0)
            {
                throw new AlgorithmException("Input array is empty");
            }

            double[] power_mW = new double[arraySize];
            for (int i = 0; i < arraySize; i++)
            {
                power_mW[i] =  Convert_dBmtomW( power_dBm[i] );
            }
            return power_mW;
        }

        /// <summary>
        /// Convert mW to dBm.
        /// Note that negative or zero values will return -10000 dBm value.
        /// This is clearly not physically possible, so you can use this to
        /// trap for this condition if required.
        /// </summary>
        /// <param name="power_mW">Power value in mW to be converted to dBm</param>
        /// <returns>Value in dBm</returns>
        public static double Convert_mWtodBm(double power_mW)
        {
            // Conversion
            // check for negative power
            if (power_mW <= 0.0) return -10000.0;
            return 10.0 * Math.Log10(power_mW);
        }

        /// <summary>
        /// Convert mW to dBm for each element of an array.
        /// Note that negative or zero values will return -10000 dBm value.
        /// This is clearly not physically possible, so you can use this to
        /// trap for this condition if required.
        /// </summary>
        /// <param name="power_mW">Power value in mW to be converted to dBm</param>
        /// <returns>Value in dBm</returns>
        public static double[] Convert_mWtodBm(double[] power_mW)
        {
            int arraySize = power_mW.Length;
            if (arraySize == 0)
            {
                throw new AlgorithmException("Input array is empty");
            }

            double[] power_dBm = new double[arraySize];
            for (int i = 0; i < arraySize; i++)
            {
                power_dBm[i] = Convert_mWtodBm( power_mW[i] );
            }
            return power_dBm;
        }     
    }
}
