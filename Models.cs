namespace Test_colloquio
{
    public class Ambulatorio
    {
        public int    Id   { get; set; }
        public string Nome { get; set; }
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
        public int    Id                  { get; set; }
        public string CodiceMinisteriale  { get; set; }
        public string CodiceInterno       { get; set; }
        public string DescrizioneEsame    { get; set; }
        public int    IdParteCorpo        { get; set; }

        // Il ListBox mostra la descrizione; le altre proprietà
        // vengono usate nella griglia dopo la conferma.
        public override string ToString() => DescrizioneEsame;
    }
}
