﻿namespace RMU.Shanten
{
    public static class ShantenFormula
    {
        public static int CalculateShanten(int _groups, int _pairs, int _taatsu, int _uniqueTerminals)
        {
            int _standardShanten = CalculateStandardShanten(_groups, _pairs, _taatsu);
            int _sevenPairsShanten = CalculateSevenPairsShanten(_pairs);
            int _thirteenOrphansShanten = CalculateThirteenOrphansShanten(_uniqueTerminals);
            return Globals.Functions.MinOfThree(_standardShanten, _sevenPairsShanten, _thirteenOrphansShanten);   
        }

        public static int CalculateStandardShanten(int _groups, int _pairs, int _taatsu)
        {
            int _standardShanten = (8 - (2 * _groups) - _pairs - _taatsu);
            if(_standardShanten == -1 && _pairs == 0) //Prevents the treating of a hand with four melds and an incomplete sequence as complete
            {
                return 0;
            }
            return _standardShanten;
        }

        public static int CalculateSevenPairsShanten(int _pairs)
        {
            return 6 - _pairs;
        }

        public static int CalculateThirteenOrphansShanten(int _uniqueTerminals)
        {
            return 13 - _uniqueTerminals; //TODO: Make sure this logic works in the typical case where a hand is tenpai with only 12 unique terminals
        }
    }
}
