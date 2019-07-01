namespace SG_Filter_Demo.RAW
{
    public class Point
    {
        public Point(){}
        public Point(double Wavelength, double Intensity)
        {
            this.Wavelength = Wavelength;
            this.Intensity = Intensity;
        }

        public double Wavelength { get; set; }
        public double Intensity { get; set; }

        public override string ToString()
        {
            return $"{Wavelength}, {Intensity}";
        }
    }
}
