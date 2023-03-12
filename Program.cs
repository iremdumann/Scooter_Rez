using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using ScotRez.Data;
using ScotRez.Library;

namespace ScotRez.UI
{
    class Program
    {
        public static bool testing = true; // development option

        static int aracStokSayisi = 6;
        static int haritaEn = 3;
        static int haritaBoy = 3;
        static int hucreAracLimit = 3;

        static Veritabani db;
        static Harita harita;

        private static void Main(string[] args)
        {
            db = new Veritabani("KiralamaKayitlar.json");
            db.AracStokOlustur(aracStokSayisi);
            SetupHarita(haritaEn, haritaBoy, hucreAracLimit);
            Menu(harita);
            // db.GuncelleKiralamaKayitlar();
        }


        #region Islemler

        private static void Menu(Harita harita)
        {
            bool devam = true;
            while (devam)
            {
                // Console.Clear();
                Console.WriteLine("Seçim?");
                Console.WriteLine("(1) Kiralama");
                Console.WriteLine("(2) İade");
                Console.WriteLine("(3) Harita (kapasite durum)");
                Console.WriteLine("(4) Harita (araç sayılar)");
                Console.WriteLine("(5) Kiralama Kayıtları [ADMIN]");
                Console.WriteLine("(x) Çıkış");

                string giris = Console.ReadLine();
                switch (giris)
                {
                    case "1":
                        Kiralama(harita);
                        break;
                    case "2":
                        Iade(harita);
                        break;
                    case "3":
                        HaritaGoruntuleme(harita, HaritaGosterim.KapasiteDurum);
                        break;
                    case "4":
                        HaritaGoruntuleme(harita, HaritaGosterim.Sayisal);
                        break;
                    case "5":
                        KiralamaKayitlariRaporlama();
                        break;
                    case "6":
                    case "7":
                    case "8":
                        Console.WriteLine("Geçersiz secim");
                        break;
                    case "x":
                        devam = false;
                        break;
                    default:
                        Console.WriteLine("Geçersiz secim");
                        Thread.Sleep(1000);
                        break;
                }
            }
        }

        private static void Kiralama(Harita harita)
        {
            Console.Clear();
            Console.Write("\n==== Kiralama ==== ");
            Scooter secilenArac;
            harita.Goster(HaritaGosterim.Sayisal);
            if (harita.AracAl(out secilenArac, Yardimcilar.KonumAl()))
            {
                Console.WriteLine(secilenArac.Kod + " aracı seçildi");

                foreach (var a in db.Araclar)
                {
                    if (a.Kod == secilenArac.Kod)
                    {
                        a.Durum = ScooterDurum.Kullanimda;

                        // Kiralama kaydı oluştur
                        KiralamaKayit kayit = new KiralamaKayit();
                        kayit.Baslangic = DateTime.Now;
                        kayit.Arac = a;
                        db.KiralamaKayitEkle(kayit);

                        Console.WriteLine($"{a.Kod} aracı kiralandı.{DateTime.Now}");
                        Yardimcilar.DevamPrompt();
                    }
                }
            }
            else
            {
                Console.WriteLine("Bu konumda araç bulunamadı.");
                Yardimcilar.DevamPrompt();
            }
        }

        private static void Iade(Harita harita)
        {
            Console.Clear();
            Console.WriteLine("\n==== İade ==== ");
            Console.WriteLine("İade işlemi için araç kodu giriniz.");

            KiralamaKayit kayit;
            Scooter arac;

            string kod = Console.ReadLine();

            // Kontroller
            if (!db.KayitKontrol(kod, out kayit))
            {
                Console.WriteLine("Bu araca ilişkin kiralama kaydı yok.");
                Yardimcilar.DevamPrompt();
                return;
            }

            if (!AracKodKontrol(kod, out arac))
            {
                Console.WriteLine("Geçersiz kod!");
                Yardimcilar.DevamPrompt();
                return;
            }
            harita.Goster(HaritaGosterim.KapasiteDurum);
            var konum = Yardimcilar.KonumAl();

            if (!harita.AracYerlestir(arac, konum))
            {
                Console.WriteLine("Bu konumda iade kabul edilemiyor. Başka bir konum deneyin.");
                Yardimcilar.DevamPrompt();
                return;
            }

            // iade işlemleri
            // araç veri yapısı güncelle
            foreach (var a in db.Araclar)
            {
                if (a.Kod == arac.Kod)
                {
                    // Arac durum güncelle
                    a.Durum = ScooterDurum.Kullanilabilir;
                    Console.WriteLine($"{a.Kod} aracı iade edildi.{DateTime.Now}");
                }
            }
            db.KiralamaKayitGuncelle(kayit);
            Yardimcilar.DevamPrompt();
        }

        private static void HaritaGoruntuleme(Harita harita, HaritaGosterim tip)
        {
            Console.Clear();
            harita.Goster(tip);
            Yardimcilar.DevamPrompt();
        }

        private static void KiralamaKayitlariRaporlama()
        {
            Console.WriteLine("==== Kiralama Kayıtları ====");
            if (db.KiralamaSayisi == 0)
                Console.WriteLine("kayıt yok");
            else
                foreach (var kayit in db.KiralamaKayitlar)
                    Console.WriteLine(kayit);
            Yardimcilar.DevamPrompt();
        }

        #endregion

        #region Kontrol Ediciler
        /// <summary>
        /// İade işleminde kullanılıyor
        /// </summary>
        /// <param name="kod"></param>
        /// <param name="arac"></param>
        /// <returns></returns>
        private static bool AracKodKontrol(string kod, out Scooter arac)
        {
            foreach (var a in db.Araclar)
            {
                if (a.Kod == kod)
                {
                    arac = a;
                    return true;
                }
            }
            arac = null;
            return false;
        }



        #endregion


        /// <summary>
        /// Haritayı "araclar"la doldurur
        /// </summary>
        /// <param name="haritaSatir"></param>
        /// <param name="haritaSutun"></param>
        /// <param name="hucreAracLimit"></param>
        private static void SetupHarita(int haritaSatir, int haritaSutun, int hucreAracLimit)
        {
            harita = new Harita(haritaSatir, haritaSutun, hucreAracLimit);
            harita.Doldur(db.Araclar);

            // harita.TestDoldur(hucreAracLimit);
        }
    }
}
