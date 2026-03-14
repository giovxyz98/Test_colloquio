namespace Test_colloquio
{
    public class Ambulatorio
    {
        public int    Id      { get; set; }
        public string Nome    { get; set; }
        public string Piano   { get; set; }
        public string Interno { get; set; }
        public bool   Attivo  { get; set; } = true;
        public override string ToString() => Nome;
    }

    public class ParteCorpo
    {
        public int    Id   { get; set; }
        public string Nome { get; set; }
        public override string ToString() => Nome;
    }

    public class Esame
    {
        public int    Id                 { get; set; }
        public string CodiceMinisteriale { get; set; }
        public string CodiceInterno      { get; set; }
        public string DescrizioneEsame   { get; set; }
        public int    IdParteCorpo       { get; set; }
        public int?   DurataMinuti       { get; set; }
        public bool   Attivo             { get; set; } = true;
        public override string ToString() => DescrizioneEsame;
    }

    public class EsameAmbulatorio
    {
        public int IdEsame       { get; set; }
        public int IdAmbulatorio { get; set; }
    }

}
