namespace Test_colloquio
{
    // -------------------------------------------------------
    // Ogni classe statica corrisponde a una sezione del file
    // config.ini con prefisso "Predefiniti_".
    // IniLoader le popola via reflection: aggiungere nuove
    // sezioni significa solo aggiungere nuove classi qui e
    // le voci corrispondenti nel file .ini, senza toccare
    // il codice di caricamento.
    // -------------------------------------------------------

    /// <summary>[Database] - connessione SQL Server</summary>
    public static class Predefiniti_Database
    {
        // Valore di default: LocalDB, usabile senza installazione completa.
        // Sovrascrivibile nel file config.ini sotto [Database].
        public static string ConnectionString { get; set; } =
            @"Server=localhost\SQLEXPRESS;Database=Test_colloquio;Trusted_Connection=True;TrustServerCertificate=True;";
    }

    /// <summary>[Ricerca] - ricerca pre-impostata all'avvio</summary>
    public static class Predefiniti_Ricerca
    {
        /// <summary>Stringa di ricerca da applicare all'avvio (vuota = nessuna)</summary>
        public static string RicercaPredefinita     { get; set; } = "";

        /// <summary>Campo su cui cercare: CodiceMinisteriale | CodiceInterno | DescrizioneEsame</summary>
        public static string TipoRicercaPredefinita { get; set; } = "DescrizioneEsame";
    }
}
