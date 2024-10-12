using NTDLS.Helpers;

namespace TestHarness
{
    internal class Program
    {
        static void Main()
        {

            var a = Converters.ConvertToNullable<bool?>("True");
            var b = Converters.ConvertToNullable<bool?>("false");
            var c = Converters.ConvertToNullable<bool?>(null);

        }
    }
}
