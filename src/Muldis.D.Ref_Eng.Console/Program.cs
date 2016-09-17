[assembly: System.CLSCompliant(true)]

namespace Muldis.D.Ref_Eng.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            System.Console.WriteLine("Muldis.D.Ref_Eng.Console Main starting.");
            API.Machine x = new API.Machine();
            System.Console.WriteLine("Muldis.D.Ref_Eng.Console Main ending.");
        }
    }
}
