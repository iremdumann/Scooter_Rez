using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using ScotRez.Library;
using ScotRez.UI;

namespace ScotRez.Data
{
    class Veritabani
    {
        public int KiralamaSayisi { get { return _kiralamaKayitlar.Count; } }
        public ReadOnlyCollection<Scooter> Araclar
        {
            get
            {
                return _araclar.AsReadOnly();
            }
        }
        public IEnumerable<KiralamaKayit> KiralamaKayitlar
        {
            get
            {
                return _kiralamaKayitlar.AsReadOnly();
            }
        }

        private List<Scooter> _araclar = new List<Scooter>();
        private List<KiralamaKayit> _kiralamaKayitlar = new List<KiralamaKayit>();
        private readonly string _dosyaAdKiralamaKayit;

        public Veritabani(string dosyaAdKiralamaKayit)
        {
            _dosyaAdKiralamaKayit = dosyaAdKiralamaKayit;
            KiralamaKayitlarYukle();
        }

        public void GuncelleKiralamaKayitlar()
        {
            DosyaKaydetKiralamaKayit();
        }

        public void KiralamaKayitlarYukle()
        {
            DosyaOkuKiralamaKayit();
        }

        public void KiralamaKayitEkle(KiralamaKayit kayit)
        {
            _kiralamaKayitlar.Add(kayit);
            DosyaKaydetKiralamaKayit();
        }

        public void KiralamaKayitGuncelle(KiralamaKayit kayit)
        {
            kayit.Bitis = DateTime.Now;
            kayit.Durum = KiralamaKayitDurum.Kapali;

            // ücretlendirme
            TimeSpan sure = kayit.Bitis.Subtract(kayit.Baslangic);
            kayit.Ucret = (decimal)sure.TotalSeconds * kayit.Arac.BirimKiralamaUcreti;

            // UI ?!
            Console.Write($"Toplam Kiralama ücreti : {kayit.Ucret.ToString("#.#")}\t ");
            Console.WriteLine($"Kiralama süresi : {sure.TotalSeconds.ToString("#.#")}");
            // 
            DosyaKaydetKiralamaKayit();
        }

        /// <summary>
        /// İade işleminde kullanılıyor
        /// </summary>
        /// <param name="kod"></param>
        /// <param name="kayit"></param>
        /// <returns></returns>
        public bool KayitKontrol(string kod, out KiralamaKayit kayit)
        {
            foreach (var k in _kiralamaKayitlar)
            {
                if (k.Arac.Kod == kod && k.Durum == KiralamaKayitDurum.Acik)
                {
                    kayit = k;
                    return true;
                }
            }
            kayit = null;
            return false;
        }

        public void AracStokOlustur(int aracsayisi)
        {
            AracModel[] modeller = {
                AracModel.X,
                AracModel.Y,
             };

            for (int i = 0; i < aracsayisi; i++)
            {
                int modelIndex = i % 2 == 0 ? 0 : 1;
                Scooter scooter = new Scooter(modeller[modelIndex], i + 1);
                _araclar.Add(scooter);

                if (KayitKontrol(scooter.Kod, out KiralamaKayit kayit))
                {
                    // Araç kirada 
                    scooter.Durum = ScooterDurum.Kullanimda;
                }
            }

            //Development
            if (Program.testing)
                foreach (var s in _araclar)
                {
                    Console.WriteLine($"PROGRAM: {s.Kod} oluşturuldu. [{s.Durum}]");
                    Yardimcilar.DevamPrompt();
                }
        }




        private void DosyaKaydetKiralamaKayit()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                IncludeFields = true
            };
            string kayitlarSeri = JsonSerializer.Serialize(_kiralamaKayitlar, options);
            File.WriteAllText(_dosyaAdKiralamaKayit, kayitlarSeri);
            if (Program.testing) Console.WriteLine("Kiralama Dosya kaydedildi");
        }

        private void DosyaOkuKiralamaKayit()
        {
            try
            {
                string kayitlar = File.ReadAllText(_dosyaAdKiralamaKayit);

                var options = new JsonSerializerOptions
                {
                    IncludeFields = true
                };
                _kiralamaKayitlar = JsonSerializer.Deserialize<List<KiralamaKayit>>(kayitlar, options);
            }
            catch (FileNotFoundException)
            {

                Console.WriteLine("VERİTABANI: Kiralama kayit dosya bulunamadı!");
            }

            // if (Program.testing) Console.WriteLine("Kiralama Dosya okundu");
        }

    }
}
