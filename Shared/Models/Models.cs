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

    public class Paziente
    {
        public int      Id            { get; set; }
        public string   Nome          { get; set; }
        public string   Cognome       { get; set; }
        public string   CodiceFiscale { get; set; }
        public DateTime? DataNascita  { get; set; }
        public string   Sesso         { get; set; }
        public string   Telefono      { get; set; }
        public string   Email         { get; set; }
        public string   Indirizzo     { get; set; }
        public string   Note          { get; set; }
        public override string ToString() => $"{Cognome} {Nome}";
    }

    public class Prenotazione
    {
        public int      Id            { get; set; }
        public int      IdPaziente    { get; set; }
        public int      IdEsame       { get; set; }
        public int      IdAmbulatorio { get; set; }
        public DateTime DataOra       { get; set; }
        public string   Stato         { get; set; } = "Prenotato";
        public string   Note          { get; set; }
        public DateTime DataCreazione { get; set; }
        public DateTime? DataModifica { get; set; }
    }
}
