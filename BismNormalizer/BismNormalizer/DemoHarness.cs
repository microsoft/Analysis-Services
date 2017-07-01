

#if DEBUG

using BismNormalizer.TabularCompare;
using BismNormalizer.TabularCompare.Core;
using System.Diagnostics;

namespace BismNormalizer
{
    public static class DemoHarness
    {
        public static void Main()
        {
            using (Comparison c = ComparisonFactory.CreateComparison("C:\\TabularCompare1.bsmn"))
            {
                c.Connect();
                c.CompareTabularModels();
                //c.ComparisonObjects

                c.ValidationMessage += HandleValidationMessage;
                c.ValidateSelection();
                c.Update();

                c.Disconnect();
            }
        }

        private static void HandleValidationMessage(object sender, ValidationMessageEventArgs e)
        {
            Debug.WriteLine(e.Message);
        }
    }
}

#endif

