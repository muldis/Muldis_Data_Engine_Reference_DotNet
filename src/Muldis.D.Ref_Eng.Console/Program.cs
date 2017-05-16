using System;

[assembly: CLSCompliant(true)]

namespace Muldis.D.Ref_Eng.Console
{
    public class Program
    {
        public static void Main(String[] args)
        {
            System.Console.WriteLine("Muldis.D.Ref_Eng.Console Main starting.");
            Muldis.D.Ref_Eng.Machine x = new Muldis.D.Ref_Eng.Machine();
            System.Console.WriteLine("Muldis.D.Ref_Eng.Console Main ending.");
        }
    }
}
