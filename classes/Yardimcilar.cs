using System;

namespace ScotRez.Library
{
    class Yardimcilar
    {
        public static void DevamPrompt()
        {
            Console.WriteLine("\nDevam için herhangi bir tuşa basınız.");
            Console.ReadKey();
        }
        public static Konum KonumAl()
        {
            Console.WriteLine("Konum Bilgileri giriniz.");
            Console.Write("Satır :");
            int satir = Convert.ToInt32(Console.ReadLine()) - 1;
            Console.Write("Sutun :");
            int sutun = Convert.ToInt32(Console.ReadLine()) - 1;

            return new Konum(satir, sutun);
        }
    }
}