namespace ScotRez.Library
{
    class Scooter
    {
        public AracModel Model { get; private set; }
        public int SeriNo { get; private set; }
        public int BirimKiralamaUcreti
        {
            get => (int)Model;
        }
        public string Kod
        {
            get
            {
                return $"{Model.ToString()}{SeriNo.ToString().PadLeft(2, '0')}"; //Kiralama sistemine bağlayan koddur. Model+SeriNo birleşimidir.
            }
        }
        public ScooterDurum Durum { get; set; }
        public Scooter(AracModel model, int seriNo)
        {
            Model = model;
            SeriNo = seriNo;
            // BirimKiralamaUcreti = (int)Model;
            Durum = ScooterDurum.Kullanilabilir;
        }
    }

}