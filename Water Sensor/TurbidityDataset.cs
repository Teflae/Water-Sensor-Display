using System;

namespace Water_Sensor
{
    class TurbidityDataset
    {
        //Static constants, for reference
        public static double DiffuseMaxRange = 20;

        //Data
        public string RawString;
        public int[] RawData;
        public int TimeStamp;

        // To get the entire picture around turbidity:
        //   'Absorb' values are from the sensor directly pointed by the laser,
        //            measuring the absorption of light by the water (how black it is).
        //   'Diffuse' values are from the sensor hidden from the laser
        //             measuring the diffusion of light by the water (how cloudy it is).
        // To account for ambient light:
        //   'Control' values are taken when the laser is off.
        //   'Laser' values taken when the laser is on. 
        public double ControlAbsorb, ControlDiffuse, LaserAbsorb, LaserDiffuse, Absorb, Diffuse;
        public TurbidityDataset(string raw, string[] spl)
        {
            //Save Raw Format for Debugging
            RawString = raw;

            // Convert Data from string to integers
            string[] StrData = spl;
            int length = StrData.Length;
            RawData = new int[length];
            for (int i = 0; i < length - 1; i++)
            {
                RawData[i] = Convert.ToInt32(StrData[i]);
                i++;
            }

            //Set TimeStamp to beginning of measurements
            TimeStamp = RawData[0];

            //Raw variables representing Control-Absorb, Control-Diffuse, Laser-Absorb, Laser-Diffuse
            int ca = 0;
            int cd = 0;
            int la = 0;
            int ld = 0;

            // For loop to average out control values. Only last 5 readings are taken due to the laser's capacitance
            for (int i = 25; i < 50; i += 5)
            {
                ca += RawData[i + 3];
                cd += RawData[i + 4];
            }

            // For loop to average out control values. Only last 8 readings are taken due to the laser's capacitance
            for (int i = 60; i < 100; i += 5)
            {
                la += RawData[i + 3];
                ld += RawData[i + 4];
            }

            //Averaging
            ca /= 5;
            cd /= 5;
            la /= 8;
            ld /= 8;

            //Converting to Lux approximation for correct calculations
            ControlAbsorb = TurValueToLux(ca);
            ControlDiffuse = TurValueToLux(cd);
            LaserAbsorb = TurValueToLux(la);
            LaserDiffuse = TurValueToLux(ld);

            //Ambient Light Subtraction
            Absorb = LaserAbsorb - ControlAbsorb;
            Diffuse = LaserDiffuse - ControlDiffuse;
        }
        // Function to convert arduino analog readings from log, allowing data to be converted to Lux approximation. 

        private static double AnalogueToDigitalScale = 5.0 / 1023.0;
        public static double x = 12000000.0;
        public static double b = -1.4;

        private static double TurValueToLux(int Analogue)
        {
            double RVoltage = Analogue * AnalogueToDigitalScale;
            double LDRVoltage = 5 - RVoltage;
            double LDRResistance = LDRVoltage / RVoltage * 10000;
            return x * Math.Pow(LDRResistance, b);
        }
    }
}
