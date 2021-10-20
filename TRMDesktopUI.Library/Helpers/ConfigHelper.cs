using System.Configuration;

namespace TRMDesktopUI.Library.Helpers
{
    public class ConfigHelper : IConfigHelper
    {
        public decimal GetTaxRate()
        {
            var rateText = ConfigurationManager.AppSettings["taxRate"];
            var isValidTaxRate = decimal.TryParse(rateText, out var output);

            if (!isValidTaxRate)
            {
                throw new ConfigurationErrorsException("Erro na configuração de imposto.");
            }

            return output;
        }
    }
}