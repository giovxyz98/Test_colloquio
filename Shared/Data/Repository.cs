using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;

namespace Test_colloquio
{
    /// <summary>
    /// Dati mock per sviluppo UI senza database.
    /// Quando il DB è pronto, sostituire questa classe con Repository.cs
    /// mantenendo le stesse firme dei metodi.
    /// </summary>
    public class Repository
    {
        private readonly string _connectionString;
        // ----------------------------------------------------------------
        // Dati statici di esempio
        // ----------------------------------------------------------------

        private static readonly List<Ambulatorio> _ambulatori = new List<Ambulatorio>
        {
            new Ambulatorio { Id = 1, Nome = "Radiologia"           },
            new Ambulatorio { Id = 2, Nome = "Tac1"                 },
            new Ambulatorio { Id = 3, Nome = "Tac2"                 },
            new Ambulatorio { Id = 4, Nome = "Risonanza"            },
            new Ambulatorio { Id = 5, Nome = "EcografiaPrivitera"   },
            new Ambulatorio { Id = 6, Nome = "EcografiaMassimino"   },
            new Ambulatorio { Id = 7, Nome = "EcografiaDoppler"     },
        };

        private static readonly List<ParteCorpo> _partiCorpo = new List<ParteCorpo>
        {
            new ParteCorpo { Id = 1, Nome = "Testa"           },
            new ParteCorpo { Id = 2, Nome = "Arti Superiori"  },
            new ParteCorpo { Id = 3, Nome = "Addome"          },
            new ParteCorpo { Id = 4, Nome = "Torace"          },
        };

        private static readonly List<Esame> _esami = new List<Esame>
        {
            new Esame { Id = 1,  CodiceMinisteriale = "RX0001", CodiceInterno = "INT001", DescrizioneEsame = "RX mano Dx",              IdParteCorpo = 2 },
            new Esame { Id = 2,  CodiceMinisteriale = "RX0002", CodiceInterno = "INT002", DescrizioneEsame = "RX spalla Sx",            IdParteCorpo = 2 },
            new Esame { Id = 3,  CodiceMinisteriale = "RMN001", CodiceInterno = "INT003", DescrizioneEsame = "RMN cranio",              IdParteCorpo = 1 },
            new Esame { Id = 4,  CodiceMinisteriale = "RMN002", CodiceInterno = "INT004", DescrizioneEsame = "RMN rachide cervicale",   IdParteCorpo = 1 },
            new Esame { Id = 5,  CodiceMinisteriale = "ECO001", CodiceInterno = "INT005", DescrizioneEsame = "Eco Addome completo",     IdParteCorpo = 3 },
            new Esame { Id = 6,  CodiceMinisteriale = "ECO002", CodiceInterno = "INT006", DescrizioneEsame = "Eco fegato",              IdParteCorpo = 3 },
            new Esame { Id = 7,  CodiceMinisteriale = "TAC001", CodiceInterno = "INT007", DescrizioneEsame = "TAC addome con mdc",      IdParteCorpo = 3 },
            new Esame { Id = 8,  CodiceMinisteriale = "RX0003", CodiceInterno = "INT008", DescrizioneEsame = "RX torace",               IdParteCorpo = 4 },
            new Esame { Id = 9,  CodiceMinisteriale = "TAC002", CodiceInterno = "INT009", DescrizioneEsame = "TAC torace ad alta ris.", IdParteCorpo = 4 },
            new Esame { Id = 10, CodiceMinisteriale = "ECO003", CodiceInterno = "INT010", DescrizioneEsame = "Eco Doppler arti sup.",   IdParteCorpo = 2 },
        };

        // Relazione molti-a-molti Esame <-> Ambulatorio
        private static readonly List<(int IdEsame, int IdAmb)> _associazioni = new List<(int, int)>
        {
            (1,  1),  // RX mano Dx           -> Radiologia
            (2,  1),  // RX spalla Sx         -> Radiologia
            (3,  4),  // RMN cranio           -> Risonanza
            (4,  4),  // RMN rachide cerv.    -> Risonanza
            (5,  5),  // Eco Addome completo  -> EcografiaPrivitera
            (5,  6),  // Eco Addome completo  -> EcografiaMassimino
            (6,  5),  // Eco fegato           -> EcografiaPrivitera
            (6,  7),  // Eco fegato           -> EcografiaDoppler
            (7,  2),  // TAC addome           -> Tac1
            (7,  3),  // TAC addome           -> Tac2
            (8,  1),  // RX torace            -> Radiologia
            (9,  2),  // TAC torace           -> Tac1
            (9,  3),  // TAC torace           -> Tac2
            (10, 7),  // Eco Doppler arti sup -> EcografiaDoppler
        };

        // ----------------------------------------------------------------
        // Metodi pubblici — stesse firme della versione con DB
        // ----------------------------------------------------------------

        public Repository(string connectionString) { _connectionString = connectionString; }

        public List<Ambulatorio> GetAmbulatori(List<int> filtroEsameIds = null)
        {
            var esameIds = FiltroOTutti(filtroEsameIds);

            var ambIds = _associazioni
                .Where(a => esameIds.Contains(a.IdEsame))
                .Select(a => a.IdAmb)
                .Distinct();

            return _ambulatori
                .Where(a => ambIds.Contains(a.Id))
                .OrderBy(a => a.Nome)
                .ToList();
        }

        public List<ParteCorpo> GetPartiCorpo(int idAmbulatorio, List<int> filtroEsameIds = null)
        {
            var esameIds = FiltroOTutti(filtroEsameIds);

            var esameIdsInAmb = _associazioni
                .Where(a => a.IdAmb == idAmbulatorio && esameIds.Contains(a.IdEsame))
                .Select(a => a.IdEsame);

            var pcIds = _esami
                .Where(e => esameIdsInAmb.Contains(e.Id))
                .Select(e => e.IdParteCorpo)
                .Distinct();

            return _partiCorpo
                .Where(pc => pcIds.Contains(pc.Id))
                .OrderBy(pc => pc.Nome)
                .ToList();
        }

        public List<Esame> GetEsami(int idAmbulatorio, int idParteCorpo, List<int> filtroEsameIds = null)
        {
            var esameIds = FiltroOTutti(filtroEsameIds);

            var esameIdsInAmb = _associazioni
                .Where(a => a.IdAmb == idAmbulatorio)
                .Select(a => a.IdEsame);

            return _esami
                .Where(e => esameIdsInAmb.Contains(e.Id)
                         && e.IdParteCorpo == idParteCorpo
                         && esameIds.Contains(e.Id))
                .OrderBy(e => e.DescrizioneEsame)
                .ToList();
        }

        public List<Esame> SearchEsami(string campo, string testo)
        {
            string t = testo.ToLowerInvariant();

            return _esami.Where(e =>
            {
                switch (campo)
                {
                    case "CodiceMinisteriale": return e.CodiceMinisteriale.ToLower().Contains(t);
                    case "CodiceInterno":      return e.CodiceInterno.ToLower().Contains(t);
                    default:                   return e.DescrizioneEsame.ToLower().Contains(t);
                }
            })
            .OrderBy(e => e.DescrizioneEsame)
            .ToList();
        }

        // ----------------------------------------------------------------
        // Scrittura DB
        // ----------------------------------------------------------------

        /// <summary>
        /// Inserisce un nuovo esame nella tabella Esami e restituisce l'Id generato.
        /// </summary>
        public int AddEsame(Esame esame)
        {
            const string sql = @"
                INSERT INTO Esami (CodiceMinisteriale, CodiceInterno, DescrizioneEsame, IdParteCorpo, DurataMinuti, Attivo)
                VALUES (@CodMin, @CodInt, @Descr, @IdPC, @Durata, @Attivo);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@CodMin",  esame.CodiceMinisteriale);
            cmd.Parameters.AddWithValue("@CodInt",  esame.CodiceInterno);
            cmd.Parameters.AddWithValue("@Descr",   esame.DescrizioneEsame);
            cmd.Parameters.AddWithValue("@IdPC",    esame.IdParteCorpo);
            cmd.Parameters.AddWithValue("@Durata",  (object)esame.DurataMinuti ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Attivo",  esame.Attivo);

            esame.Id = (int)cmd.ExecuteScalar();
            return esame.Id;
        }

        // ----------------------------------------------------------------
        // Helper
        // ----------------------------------------------------------------

        private IEnumerable<int> FiltroOTutti(List<int> filtro)
        {
            return filtro ?? _esami.Select(e => e.Id).ToList();
        }
    }
}
