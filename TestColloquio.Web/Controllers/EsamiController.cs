using Microsoft.AspNetCore.Mvc;
using Test_colloquio;

namespace TestColloquio.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EsamiController : ControllerBase
    {
        private readonly Repository _repo;

        public EsamiController(Repository repo)
        {
            _repo = repo;
        }

        // GET /api/esami/ambulatori
        // Restituisce tutti gli ambulatori, filtrati per ricerca se attiva
        [HttpGet("ambulatori")]
        public ActionResult<IEnumerable<Ambulatorio>> GetAmbulatori(
            [FromQuery] string? cerca = null,
            [FromQuery] string? campo = null)
        {
            var filtroIds = RisolviFlitro(cerca, campo);
            return Ok(_repo.GetAmbulatori(filtroIds));
        }

        // GET /api/esami/partiCorpo?ambulatorioId=1
        // Restituisce le parti del corpo legate agli esami dell'ambulatorio selezionato
        [HttpGet("partiCorpo")]
        public ActionResult<IEnumerable<ParteCorpo>> GetPartiCorpo(
            [FromQuery] int ambulatorioId,
            [FromQuery] string? cerca = null,
            [FromQuery] string? campo = null)
        {
            var filtroIds = RisolviFlitro(cerca, campo);
            return Ok(_repo.GetPartiCorpo(ambulatorioId, filtroIds));
        }

        // GET /api/esami?ambulatorioId=1&parteCorpoId=2
        // Restituisce gli esami per ambulatorio e parte del corpo, con ricerca opzionale
        [HttpGet]
        public ActionResult<IEnumerable<Esame>> GetEsami(
            [FromQuery] int ambulatorioId,
            [FromQuery] int parteCorpoId,
            [FromQuery] string? cerca = null,
            [FromQuery] string? campo = null)
        {
            var filtroIds = RisolviFlitro(cerca, campo);
            return Ok(_repo.GetEsami(ambulatorioId, parteCorpoId, filtroIds));
        }

        // Risolve il filtro di ricerca: usa i parametri passati oppure i default da Predefiniti_Ricerca
        private List<int>? RisolviFlitro(string? cerca, string? campo)
        {
            string testoRicerca = !string.IsNullOrWhiteSpace(cerca)
                ? cerca
                : Predefiniti_Ricerca.RicercaPredefinita;

            if (string.IsNullOrWhiteSpace(testoRicerca))
                return null;

            string campoRicerca = !string.IsNullOrWhiteSpace(campo)
                ? campo
                : Predefiniti_Ricerca.TipoRicercaPredefinita;

            var risultati = _repo.SearchEsami(campoRicerca, testoRicerca);
            return risultati.Select(e => e.Id).ToList();
        }
    }
}
