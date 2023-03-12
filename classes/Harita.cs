using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ScotRez.UI;
namespace ScotRez.Library
{
    class Harita
    {
        private readonly int _satirMax;
        private readonly int _sutunMax;
        private readonly int _hucreKapasiteMax;
        private Hucre[,] _hucreler;
        public Harita(int satirMax, int sutunMax, int hucreKapasiteMax)
        {
            _sutunMax = sutunMax;
            _satirMax = satirMax;
            _hucreKapasiteMax = hucreKapasiteMax;
            _hucreler = new Hucre[satirMax, sutunMax];
            Setup(hucreKapasiteMax); // initialize
        }
        private void Setup(int hucreKapasiteMax)
        {
            for (int i = 0; i < _satirMax; i++)
                for (int j = 0; j < _sutunMax; j++)
                    _hucreler[i, j] = new Hucre(hucreKapasiteMax);
        }

        public bool AracAl(out Scooter arac, Konum konum)
        {
            arac = _hucreler[konum.Satir, konum.Sutun].AracCikar();
            return arac is not null ? true : false;
        }

        public bool AracYerlestir(Scooter arac, Konum konum)
        {
            if (_hucreler[konum.Satir, konum.Sutun].AracEkle(arac))
            {
                if (Program.testing) Console.WriteLine($"HARITA :{arac.Kod} yerleştirildi. [{konum.Satir}:{konum.Sutun}]");
                return true;
            }
            return false;
        }
        public void Goster(HaritaGosterim tip)
        {
            string baslik = "\nHarita ======";

            if (tip == HaritaGosterim.Sayisal) baslik += "\n(araç sayıları)";
            if (tip == HaritaGosterim.KapasiteDurum) baslik += "\n(kapasite) \n(x : araç iade yapılamaz)";

            Console.WriteLine(baslik);
            Console.WriteLine("   \tA\tB\tC\t");
            Console.WriteLine("----------------------------------");

            for (int i = 0; i < _satirMax; i++)
            {
                Console.Write($"{i + 1} |\t");
                for (int j = 0; j < _sutunMax; j++)
                {

                    if (tip == HaritaGosterim.Sayisal)
                    {
                        int sayi = _hucreler[i, j].AracSayisi;
                        Console.BackgroundColor = sayi == _hucreKapasiteMax ? ConsoleColor.Red : ConsoleColor.Black;
                        Console.Write($" {sayi} \t");
                        Console.BackgroundColor = ConsoleColor.Black;
                    }

                    if (tip == HaritaGosterim.KapasiteDurum)
                    {
                        if (_hucreler[i, j].HucreKapasiteDurum == HucreDurum.Dolu)
                            Console.Write($" X \t");
                        else
                            Console.Write($"   \t");
                    }
                }
                Console.WriteLine();
            }
        }
        public void Doldur(ReadOnlyCollection<Scooter> araclar)
        {
            if (araclar.Count > _satirMax * _sutunMax * _hucreKapasiteMax)
            {
                Console.WriteLine("Araç sayısı harita kapasitesini aşıyor");
                return;
            }
            int yerlestirilen = 0;
            foreach (var arac in araclar)
            {
                if (arac.Durum == ScooterDurum.Kullanilabilir)
                {
                    bool yerlestirildi = false;
                    while (!yerlestirildi)
                    {
                        var r = new Random();
                        var konum = new Konum(r.Next(_satirMax), r.Next(_sutunMax));// Konumu rastlantısal ürettik.
                        if (AracYerlestir(arac, konum))
                        {
                            yerlestirilen++;
                            yerlestirildi = true;
                        }
                    }
                }
            }

            if (Program.testing)
            {
                Console.WriteLine($"{yerlestirilen} araç yerleştirildi.");
                Yardimcilar.DevamPrompt();
            }
        }
        // Testing

        /// <summary>
        /// Tüm hücreleri max kapasite doldurur
        /// </summary>
        /// <param name="hucreKapasite"></param>
        public void TestDoldur(int hucreKapasite)
        {
            AracModel[] modeller = {
                AracModel.X,
                AracModel.Y
             };

            int sayac = 0;
            for (int i = 0; i < _satirMax; i++)
            {
                for (int j = 0; j < _sutunMax; j++)
                {
                    for (int k = 0; k < hucreKapasite; k++)
                    {
                        if (_hucreler[i, j].HucreKapasiteDurum == HucreDurum.YerVar)
                        {
                            var arac = new Scooter(modeller[k % modeller.Length], (i + j));
                            if (_hucreler[i, j].AracEkle(arac))
                            {
                                Console.WriteLine($"{arac.Kod} yerleşti.");
                                sayac++;
                            }
                        }
                    }
                }
            }
            if (Program.testing) Console.WriteLine(sayac + " arac yerleştririldi");
        }
    }

}