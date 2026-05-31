using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using PubsProject.Data;

namespace PubsProject.Forms
{
    public class FrmPublishers : Form
    {
        private DataGridView dgv;
        private TextBox txtPubId, txtPubName, txtCity, txtState, txtCountry;
        private Button btnNuevo, btnGuardar, btnEliminar, btnLimpiar;
        private bool esNuevo = false;

        public FrmPublishers()
        {
            InitializeComponent();
            CargarDatos();
        }

        private void InitializeComponent()
        {
            this.Text = "Gestión de Editoriales";
            this.Size = new Size(950, 560);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.WhiteSmoke;

            Panel panelHeader = new Panel();
            panelHeader.BackColor = Color.FromArgb(13, 110, 253);
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Height = 50;
            Label lblTitulo = new Label();
            lblTitulo.Text = "🏢 Gestión de Editoriales";
            lblTitulo.ForeColor = Color.White;
            lblTitulo.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            lblTitulo.Dock = DockStyle.Fill;
            lblTitulo.TextAlign = ContentAlignment.MiddleLeft;
            lblTitulo.Padding = new Padding(10, 0, 0, 0);
            panelHeader.Controls.Add(lblTitulo);
            this.Controls.Add(panelHeader);

            Panel panelForm = new Panel();
            panelForm.Location = new Point(10, 60);
            panelForm.Size = new Size(290, 440);
            panelForm.BackColor = Color.White;
            panelForm.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(panelForm);

            int y = 15;
            AgregarCampo(panelForm, "ID Editorial (4 chars):", ref txtPubId, ref y);
            AgregarCampo(panelForm, "Nombre Editorial:", ref txtPubName, ref y);
            AgregarCampo(panelForm, "Ciudad:", ref txtCity, ref y);
            AgregarCampo(panelForm, "Estado:", ref txtState, ref y);
            AgregarCampo(panelForm, "País:", ref txtCountry, ref y);
            txtCountry.Text = "USA";

            btnNuevo = CrearBoton("➕ Nuevo", Color.FromArgb(40, 167, 69), new Point(10, y));
            btnGuardar = CrearBoton("💾 Guardar", Color.FromArgb(13, 110, 253), new Point(150, y)); y += 45;
            btnEliminar = CrearBoton("🗑️ Eliminar", Color.FromArgb(220, 53, 69), new Point(10, y));
            btnLimpiar = CrearBoton("🔄 Limpiar", Color.FromArgb(108, 117, 125), new Point(150, y));

            btnNuevo.Click += (s, e) => { LimpiarFormulario(); esNuevo = true; txtPubId.Enabled = true; };
            btnGuardar.Click += BtnGuardar_Click;
            btnEliminar.Click += BtnEliminar_Click;
            btnLimpiar.Click += (s, e) => LimpiarFormulario();
            panelForm.Controls.AddRange(new Control[] { btnNuevo, btnGuardar, btnEliminar, btnLimpiar });

            Panel panelGrid = new Panel();
            panelGrid.Location = new Point(310, 60);
            panelGrid.Size = new Size(620, 480);
            this.Controls.Add(panelGrid);

            dgv = new DataGridView();
            dgv.Dock = DockStyle.Fill;
            dgv.BackgroundColor = Color.White;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.RowHeadersVisible = false;
            dgv.Font = new Font("Segoe UI", 9);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(13, 110, 253);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgv.EnableHeadersVisualStyles = false;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(235, 245, 255);
            dgv.CellClick += Dgv_CellClick;
            panelGrid.Controls.Add(dgv);
        }

        private void AgregarCampo(Panel panel, string label, ref TextBox txt, ref int y)
        {
            Label lbl = new Label();
            lbl.Text = label;
            lbl.Font = new Font("Segoe UI", 9);
            lbl.Location = new Point(10, y);
            lbl.AutoSize = true;
            panel.Controls.Add(lbl);
            y += 18;
            txt = new TextBox();
            txt.Font = new Font("Segoe UI", 9);
            txt.Location = new Point(10, y);
            txt.Size = new Size(260, 25);
            panel.Controls.Add(txt);
            y += 32;
        }

        private Button CrearBoton(string texto, Color color, Point location)
        {
            Button btn = new Button();
            btn.Text = texto; btn.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btn.BackColor = color; btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat; btn.FlatAppearance.BorderSize = 0;
            btn.Location = location; btn.Size = new Size(125, 35);
            return btn;
        }

        private void CargarDatos()
        {
            try { dgv.DataSource = ConexionDB.EjecutarConsulta("SELECT * FROM publishers ORDER BY pub_name"); }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dgv.Rows[e.RowIndex];
                txtPubId.Text = row.Cells["pub_id"]?.Value?.ToString() ?? "";
                txtPubName.Text = row.Cells["pub_name"]?.Value?.ToString() ?? "";
                txtCity.Text = row.Cells["city"]?.Value?.ToString() ?? "";
                txtState.Text = row.Cells["state"]?.Value?.ToString() ?? "";
                txtCountry.Text = row.Cells["country"]?.Value?.ToString() ?? "";
                esNuevo = false;
                txtPubId.Enabled = false;
            }
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPubId.Text)) { MessageBox.Show("El ID es obligatorio."); return; }
            try
            {
                SqlParameter[] p = {
                    new SqlParameter("@pub_id", txtPubId.Text.Trim()),
                    new SqlParameter("@pub_name", (object)txtPubName.Text.Trim() ?? DBNull.Value),
                    new SqlParameter("@city", (object)txtCity.Text.Trim() ?? DBNull.Value),
                    new SqlParameter("@state", (object)txtState.Text.Trim() ?? DBNull.Value),
                    new SqlParameter("@country", string.IsNullOrWhiteSpace(txtCountry.Text) ? (object)DBNull.Value : txtCountry.Text.Trim())
                };
                string q = esNuevo
                    ? "INSERT INTO publishers VALUES (@pub_id, @pub_name, @city, @state, @country)"
                    : "UPDATE publishers SET pub_name=@pub_name, city=@city, state=@state, country=@country WHERE pub_id=@pub_id";
                ConexionDB.EjecutarComando(q, p);
                MessageBox.Show("Guardado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarDatos(); LimpiarFormulario();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPubId.Text)) { MessageBox.Show("Seleccione un registro."); return; }
            if (MessageBox.Show("¿Eliminar esta editorial?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    ConexionDB.EjecutarComando("DELETE FROM publishers WHERE pub_id=@pub_id",
                        new[] { new SqlParameter("@pub_id", txtPubId.Text) });
                    MessageBox.Show("Eliminado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarDatos(); LimpiarFormulario();
                }
                catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
            }
        }

        private void LimpiarFormulario()
        {
            txtPubId.Text = txtPubName.Text = txtCity.Text = txtState.Text = "";
            txtCountry.Text = "USA";
            esNuevo = false; txtPubId.Enabled = true;
        }
    }
}
