using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;

namespace Test_colloquio
{
    public class Repository
    {
        private readonly string _connectionString;

        public Repository(string connectionString) { _connectionString = connectionString; }

        public List<Ambulatorio> GetAmbulatori(List<int>? filtroEsameIds = null)
        {
            string filtro = FiltroInClause(filtroEsameIds, "ea.IdEsame");
            string sql = $@"
                SELECT DISTINCT a.Id, a.Nome, a.Piano, a.Interno, a.Attivo
                FROM Ambulatori a
                INNER JOIN EsamiAmbulatoriMtoM ea ON ea.IdAmbulatorio = a.Id
                WHERE a.Attivo = 1
                {filtro}
                ORDER BY a.Nome";

            var result = new List<Ambulatorio>();
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            AddFiltroParams(cmd, filtroEsameIds);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new Ambulatorio
                {
                    Id      = reader.GetInt32(0),
                    Nome    = reader.GetString(1),
                    Piano   = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Interno = reader.IsDBNull(3) ? null : reader.GetString(3),
                    Attivo  = reader.GetBoolean(4),
                });
            }
            return result;
        }

        public List<ParteCorpo> GetPartiCorpo(int idAmbulatorio, List<int>? filtroEsameIds = null)
        {
            string filtro = FiltroInClause(filtroEsameIds, "ea.IdEsame");
            string sql = $@"
                SELECT DISTINCT pc.Id, pc.Nome
                FROM PartiCorpo pc
                INNER JOIN Esami e ON e.IdParteCorpo = pc.Id
                INNER JOIN EsamiAmbulatoriMtoM ea ON ea.IdEsame = e.Id
                WHERE ea.IdAmbulatorio = @IdAmbulatorio
                  AND e.Attivo = 1
                {filtro}
                ORDER BY pc.Nome";

            var result = new List<ParteCorpo>();
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@IdAmbulatorio", idAmbulatorio);
            AddFiltroParams(cmd, filtroEsameIds);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new ParteCorpo
                {
                    Id   = reader.GetInt32(0),
                    Nome = reader.GetString(1),
                });
            }
            return result;
        }

        public List<Esame> GetEsami(int idAmbulatorio, int idParteCorpo, List<int>? filtroEsameIds = null)
        {
            string filtro = FiltroInClause(filtroEsameIds, "e.Id");
            string sql = $@"
                SELECT e.Id, e.CodiceMinisteriale, e.CodiceInterno, e.DescrizioneEsame,
                       e.IdParteCorpo, e.DurataMinuti, e.Attivo
                FROM Esami e
                INNER JOIN EsamiAmbulatoriMtoM ea ON ea.IdEsame = e.Id
                WHERE ea.IdAmbulatorio = @IdAmbulatorio
                  AND e.IdParteCorpo   = @IdParteCorpo
                  AND e.Attivo = 1
                {filtro}
                ORDER BY e.DescrizioneEsame";

            var result = new List<Esame>();
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@IdAmbulatorio", idAmbulatorio);
            cmd.Parameters.AddWithValue("@IdParteCorpo",  idParteCorpo);
            AddFiltroParams(cmd, filtroEsameIds);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                result.Add(ReadEsame(reader));
            return result;
        }

        public List<Esame> SearchEsami(string campo, string testo)
        {
            string colonna = campo switch
            {
                "CodiceMinisteriale" => "e.CodiceMinisteriale",
                "CodiceInterno"      => "e.CodiceInterno",
                _                    => "e.DescrizioneEsame",
            };

            string sql = $@"
                SELECT e.Id, e.CodiceMinisteriale, e.CodiceInterno, e.DescrizioneEsame,
                       e.IdParteCorpo, e.DurataMinuti, e.Attivo
                FROM Esami e
                WHERE e.Attivo = 1
                  AND {colonna} LIKE @Testo COLLATE Latin1_General_CI_AI
                ORDER BY e.DescrizioneEsame";

            var result = new List<Esame>();
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Testo", $"%{testo}%");
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                result.Add(ReadEsame(reader));
            return result;
        }

        private static Esame ReadEsame(SqlDataReader r) => new Esame
        {
            Id                 = r.GetInt32(0),
            CodiceMinisteriale = r.GetString(1),
            CodiceInterno      = r.GetString(2),
            DescrizioneEsame   = r.GetString(3),
            IdParteCorpo       = r.GetInt32(4),
            DurataMinuti       = r.IsDBNull(5) ? (int?)null : r.GetInt32(5),
            Attivo             = r.GetBoolean(6),
        };

        private static string FiltroInClause(List<int> ids, string colonna)
        {
            if (ids == null || ids.Count == 0) return "";
            var nomi = ids.Select((_, i) => $"@fId{i}");
            return $"AND {colonna} IN ({string.Join(", ", nomi)})";
        }

        private static void AddFiltroParams(SqlCommand cmd, List<int> ids)
        {
            if (ids == null) return;
            for (int i = 0; i < ids.Count; i++)
                cmd.Parameters.AddWithValue($"@fId{i}", ids[i]);
        }
    }
}
