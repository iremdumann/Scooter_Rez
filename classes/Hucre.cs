using System.Collections.Generic;

namespace ScotRez.Library
{
    class Hucre
    {
        public static int AracKapasite { get; private set; }
        public Hucre(int aracKapasite) => AracKapasite = aracKapasite;
        private Queue<Scooter> _araclar = new Queue<Scooter>();
        public HucreDurum HucreKapasiteDurum
        {
            get => _araclar.Count == AracKapasite ? HucreDurum.Dolu : HucreDurum.YerVar;
        }
        public int AracSayisi { get => _araclar.Count; }
        public bool AracEkle(Scooter arac)
        {
            if (HucreKapasiteDurum == HucreDurum.Dolu) //İş Mantığı
                return false;

            _araclar.Enqueue(arac);
            return true;
        }

        public Scooter AracCikar()
        {
            if (_araclar.TryDequeue(out Scooter arac))
            {
                return arac;
            }
            return null;
            //   return _araclar.TryDequeue(out arac) ? arac : null;


        }

    }

}