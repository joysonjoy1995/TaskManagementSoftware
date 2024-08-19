using TaskManagementSoftware.Models;

namespace TaskManagementSoftware.Helpers
{
   
    public static class UnitConversionHelper
    {
        private static readonly Dictionary<(Unit, Unit), Func<double, double>> ConversionFactors = new Dictionary<(Unit, Unit), Func<double, double>>
    {
        {(Unit.Liter, Unit.Milliliter), value => value * 1000},
        {(Unit.Milliliter, Unit.Liter), value => value / 1000},
      
    };

        public static bool CanConvert(Unit fromUnit, Unit toUnit)
        {

            return ConversionFactors.ContainsKey((fromUnit, toUnit)) || ConversionFactors.ContainsKey((toUnit, fromUnit));
        }

        public static double Convert(Unit fromUnit, Unit toUnit, double amount)
        {
            if (fromUnit == toUnit)
                return amount;

            if (ConversionFactors.TryGetValue((fromUnit, toUnit), out var conversion))
                return conversion(amount);

            if (ConversionFactors.TryGetValue((toUnit, fromUnit), out var reverseConversion))
                return reverseConversion(amount);

            throw new InvalidOperationException($"Conversion from {fromUnit} to {toUnit} is not supported.");
        }
    }

}
