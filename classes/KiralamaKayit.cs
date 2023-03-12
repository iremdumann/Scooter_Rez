using System;


namespace ScotRez.Library
{
    class KiralamaKayit
    {
        private static int sayac;

        public int Id { get; set; }
        public DateTime Baslangic { get; set; }
        public DateTime Bitis { get; set; }
        public Scooter Arac { get; set; }
        public decimal Ucret { get; set; }
        public KiralamaKayitDurum Durum { get; set; }

        public KiralamaKayit()
        {
            sayac++;
            Id = sayac;
            Durum = KiralamaKayitDurum.Acik;
        }

        public override string ToString()
        {
            return $"Id:{Id} | Araç:{Arac.Kod} | Ucret:{Ucret.ToString()} | Durum: {Durum}";
        }

    }

}
