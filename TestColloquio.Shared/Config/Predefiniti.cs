namespace Test_colloquio
{
    public static class Predefiniti_Database
    {
        public static string ConnectionString { get; set; } =
            @"Server=localhost\SQLEXPRESS;Database=Test_colloquio;Trusted_Connection=True;TrustServerCertificate=True;";
    }

    public static class Predefiniti_Ricerca
    {
        public static string RicercaPredefinita     { get; set; } = "";
        public static string TipoRicercaPredefinita { get; set; } = "DescrizioneEsame";
    }
}
