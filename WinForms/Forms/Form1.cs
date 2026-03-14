using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Test_colloquio
{
    public class Form1 : Form
    {
        private readonly Repository _repo;
        private List<int>? _filtroEsameIds = null;
        private bool _aggiornando = false;

        private ListBox      lstAmbulatori = null!;
        private ListBox      lstPartiCorpo = null!;
        private ListBox      lstEsami      = null!;
        private TextBox      txtRicerca    = null!;
        private RadioButton  rdoCodMin     = null!;
        private RadioButton  rdoCodInt     = null!;
        private RadioButton  rdoDesc       = null!;
        private Button       btnCerca      = null!;
        private Button       btnVediTutti  = null!;
        private DataGridView grid          = null!;
        private Button       btnConferma   = null!;
        private Button       btnRimuovi    = null!;
        private Button       btnSu         = null!;
        private Button       btnGiu        = null!;

        public Form1()
        {
            _repo = new Repository(Predefiniti_Database.ConnectionString);
            BuildUI();
            CaricaAmbulatori();
            ApplicaRicercaPredefinita();
        }

        private void BuildUI()
        {
            Text          = "Selezione Esami";
            Size          = new Size(1150, 780);
            MinimumSize   = new Size(950, 650);
            StartPosition = FormStartPosition.CenterScreen;

            var pnlGrid    = BuildGridPanel();
            var pnlTre     = BuildTrePannelli();
            var pnlRicerca = BuildRicercaPanel();

            Controls.Add(pnlGrid);
            Controls.Add(pnlTre);
            Controls.Add(pnlRicerca);
        }

        private GroupBox BuildRicercaPanel()
        {
            var grp = new GroupBox
            {
                Text    = "Ricerca",
                Dock    = DockStyle.Top,
                Height  = 68,
                Padding = new Padding(8, 16, 8, 4)
            };

            txtRicerca = new TextBox { Left = 8, Top = 26, Width = 260 };
            txtRicerca.KeyPress += (s, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter) { e.Handled = true; EseguiRicerca(); }
            };

            rdoCodMin = new RadioButton { Text = "Cod. Ministeriale", Left = 278, Top = 26, AutoSize = true };
            rdoCodInt = new RadioButton { Text = "Cod. Interno",       Left = 410, Top = 26, AutoSize = true };
            rdoDesc   = new RadioButton { Text = "Descrizione",         Left = 520, Top = 26, AutoSize = true, Checked = true };

            btnCerca     = new Button { Text = "Cerca",     Left = 638, Top = 23, Width = 85, Height = 26 };
            btnVediTutti = new Button { Text = "Vedi tutti", Left = 730, Top = 23, Width = 85, Height = 26 };

            btnCerca.Click     += (s, e) => EseguiRicerca();
            btnVediTutti.Click += (s, e) => ResetRicerca();

            grp.Controls.AddRange([txtRicerca, rdoCodMin, rdoCodInt, rdoDesc, btnCerca, btnVediTutti]);
            return grp;
        }

        private Panel BuildTrePannelli()
        {
            var pnl = new Panel { Dock = DockStyle.Top, Height = 330, Padding = new Padding(6) };

            var grpAmb = new GroupBox { Text = "Ambulatori", Left = 6, Top = 6, Width = 220, Height = 318 };
            lstAmbulatori = new ListBox { Dock = DockStyle.Fill };
            lstAmbulatori.SelectedIndexChanged += OnAmbulatoriChanged;
            grpAmb.Controls.Add(lstAmbulatori);

            var grpPC = new GroupBox { Text = "Parti del corpo", Left = 232, Top = 6, Width = 220, Height = 318 };
            lstPartiCorpo = new ListBox { Dock = DockStyle.Fill };
            lstPartiCorpo.SelectedIndexChanged += OnPartiCorpoChanged;
            grpPC.Controls.Add(lstPartiCorpo);

            var grpEsami = new GroupBox { Text = "Esami", Left = 458, Top = 6, Width = 360, Height = 318 };
            lstEsami = new ListBox { Dock = DockStyle.Fill };
            grpEsami.Controls.Add(lstEsami);

            btnConferma = new Button
            {
                Text   = "Conferma scelta ▶",
                Left   = 830,
                Top    = 140,
                Width  = 160,
                Height = 36,
                Font   = new Font(Font, FontStyle.Bold)
            };
            btnConferma.Click += OnConfermaClick;

            pnl.Controls.AddRange([grpAmb, grpPC, grpEsami, btnConferma]);
            return pnl;
        }

        private GroupBox BuildGridPanel()
        {
            var grp = new GroupBox
            {
                Text    = "Esami selezionati",
                Dock    = DockStyle.Fill,
                Padding = new Padding(6)
            };

            var pnlBtn = new Panel { Dock = DockStyle.Right, Width = 110, Padding = new Padding(4) };

            btnRimuovi = new Button { Text = "❌ Rimuovi", Dock = DockStyle.Top, Height = 34 };
            btnSu      = new Button { Text = "▲ Su",       Dock = DockStyle.Top, Height = 34 };
            btnGiu     = new Button { Text = "▼ Giù",      Dock = DockStyle.Top, Height = 34 };

            btnRimuovi.Click += OnRimuoviClick;
            btnSu.Click      += OnSuClick;
            btnGiu.Click     += OnGiuClick;

            pnlBtn.Controls.Add(btnGiu);
            pnlBtn.Controls.Add(btnSu);
            pnlBtn.Controls.Add(btnRimuovi);

            grid = new DataGridView
            {
                Dock                  = DockStyle.Fill,
                AllowUserToAddRows    = false,
                AllowUserToDeleteRows = false,
                ReadOnly              = true,
                SelectionMode         = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode   = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible     = false,
                BackgroundColor       = SystemColors.Window,
                BorderStyle           = BorderStyle.None,
                MultiSelect           = false
            };
            grid.Columns.Add("CodiceMinisteriale", "Cod. Ministeriale");
            grid.Columns.Add("CodiceInterno",      "Cod. Interno");
            grid.Columns.Add("DescrizioneEsame",   "Descrizione Esame");
            grid.Columns.Add("DurataMinuti",        "Durata (min)");
            grid.Columns["CodiceMinisteriale"].FillWeight = 18;
            grid.Columns["CodiceInterno"].FillWeight      = 18;
            grid.Columns["DescrizioneEsame"].FillWeight   = 52;
            grid.Columns["DurataMinuti"].FillWeight       = 12;

            grp.Controls.Add(grid);
            grp.Controls.Add(pnlBtn);
            return grp;
        }

        private void CaricaAmbulatori()
        {
            _aggiornando = true;
            lstAmbulatori.Items.Clear();

            try
            {
                var items = _repo.GetAmbulatori(_filtroEsameIds);
                foreach (var a in items) lstAmbulatori.Items.Add(a);
            }
            catch (Exception ex)
            {
                MostraErroreDb(ex);
            }

            _aggiornando = false;

            if (lstAmbulatori.Items.Count > 0)
                lstAmbulatori.SelectedIndex = 0;
            else
                CaricaPartiCorpo();
        }

        private void CaricaPartiCorpo()
        {
            _aggiornando = true;
            lstPartiCorpo.Items.Clear();

            if (lstAmbulatori.SelectedItem is Ambulatorio amb)
            {
                try
                {
                    var items = _repo.GetPartiCorpo(amb.Id, _filtroEsameIds);
                    foreach (var pc in items) lstPartiCorpo.Items.Add(pc);
                }
                catch (Exception ex)
                {
                    MostraErroreDb(ex);
                }
            }

            _aggiornando = false;

            if (lstPartiCorpo.Items.Count > 0)
                lstPartiCorpo.SelectedIndex = 0;
            else
                CaricaEsami();
        }

        private void CaricaEsami()
        {
            lstEsami.Items.Clear();

            if (lstAmbulatori.SelectedItem is not Ambulatorio amb) return;
            if (lstPartiCorpo.SelectedItem is not ParteCorpo pc)   return;

            try
            {
                var items = _repo.GetEsami(amb.Id, pc.Id, _filtroEsameIds);
                foreach (var e in items) lstEsami.Items.Add(e);
            }
            catch (Exception ex)
            {
                MostraErroreDb(ex);
            }

            if (lstEsami.Items.Count > 0)
                lstEsami.SelectedIndex = 0;
        }

        private void OnAmbulatoriChanged(object? sender, EventArgs e)
        {
            if (_aggiornando) return;
            CaricaPartiCorpo();
        }

        private void OnPartiCorpoChanged(object? sender, EventArgs e)
        {
            if (_aggiornando) return;
            CaricaEsami();
        }

        private void EseguiRicerca()
        {
            string testo = txtRicerca.Text.Trim();
            if (string.IsNullOrEmpty(testo)) { ResetRicerca(); return; }

            string campo = rdoCodMin.Checked ? "CodiceMinisteriale"
                         : rdoCodInt.Checked ? "CodiceInterno"
                                             : "DescrizioneEsame";

            try
            {
                var risultati = _repo.SearchEsami(campo, testo);
                _filtroEsameIds = [.. risultati.Select(r => r.Id)];
            }
            catch (Exception ex)
            {
                MostraErroreDb(ex);
                return;
            }

            CaricaAmbulatori();

            if (_filtroEsameIds.Count == 0)
            {
                MessageBox.Show(
                    "Nessun risultato trovato.",
                    "Ricerca",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void ResetRicerca()
        {
            _filtroEsameIds = null;
            txtRicerca.Clear();
            CaricaAmbulatori();
        }

        private void ApplicaRicercaPredefinita()
        {
            switch (Predefiniti_Ricerca.TipoRicercaPredefinita)
            {
                case "CodiceMinisteriale": rdoCodMin.Checked = true; break;
                case "CodiceInterno":      rdoCodInt.Checked = true; break;
                default:                   rdoDesc.Checked   = true; break;
            }

            string testo = Predefiniti_Ricerca.RicercaPredefinita;
            if (string.IsNullOrWhiteSpace(testo)) return;

            txtRicerca.Text = testo;
            EseguiRicerca();
        }

        private void OnConfermaClick(object? sender, EventArgs e)
        {
            if (lstEsami.SelectedItem is not Esame esame)
            {
                MessageBox.Show(
                    "Seleziona un esame dalla lista prima di confermare.",
                    "Attenzione",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            grid.Rows.Add(
                esame.CodiceMinisteriale,
                esame.CodiceInterno,
                esame.DescrizioneEsame,
                esame.DurataMinuti.HasValue ? (object)esame.DurataMinuti.Value : "");

            grid.ClearSelection();
            grid.Rows[^1].Selected = true;
        }

        private void OnRimuoviClick(object? sender, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0) return;

            int idx = grid.SelectedRows[0].Index;
            grid.Rows.RemoveAt(idx);

            if (grid.Rows.Count > 0)
                grid.Rows[Math.Min(idx, grid.Rows.Count - 1)].Selected = true;
        }

        private void OnSuClick(object? sender, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0) return;
            int idx = grid.SelectedRows[0].Index;
            if (idx == 0) return;
            SwapRows(idx, idx - 1);
            grid.Rows[idx - 1].Selected = true;
        }

        private void OnGiuClick(object? sender, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0) return;
            int idx = grid.SelectedRows[0].Index;
            if (idx >= grid.Rows.Count - 1) return;
            SwapRows(idx, idx + 1);
            grid.Rows[idx + 1].Selected = true;
        }

        private void SwapRows(int a, int b)
        {
            for (int c = 0; c < grid.Columns.Count; c++)
            {
                (grid.Rows[a].Cells[c].Value, grid.Rows[b].Cells[c].Value) =
                    (grid.Rows[b].Cells[c].Value, grid.Rows[a].Cells[c].Value);
            }
        }

        private static void MostraErroreDb(Exception ex)
        {
            MessageBox.Show(
                $"Errore di accesso al database:\n\n{ex.Message}",
                "Errore DB",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
