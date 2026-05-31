using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using PubsProject.Data;

namespace PubsProject.Forms
{
    public class FrmAuthors : Form
    {
        private DataGridView dgv;
        private TextBox txtAuId, txtLname, txtFname, txtPhone, txtAddress, txtCity, txtState, txtZip;
        private CheckBox chkContract;
        private Button btnNuevo, btnGuardar, btnEliminar, btnBuscar, btnLimpiar;
        private Label lblTitulo;
        private Panel panelForm, panelGrid, panelHeader;
        private bool esNuevo = false;

        public FrmAuthors()
        {
            InitializeComponent();
            CargarDatos();
        }

        private void InitializeComponent()
        {
            this.Text = "Gestión de Autores";
            this.Size = new Size(950, 620);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.WhiteSmoke;

            // Header
            panelHeader = new Panel();
            panelHeader.BackColor = Color.FromArgb(0, 102, 153);
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Height = 50;
            lblTitulo = new Label();
            lblTitulo.Text = "👤 Gestión de Autores - Base de Datos Pubs";
            lblTitulo.ForeColor = Color.White;
            lblTitulo.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            lblTitulo.Dock = DockStyle.Fill;
            lblTitulo.TextAlign = ContentAlignment.MiddleLeft;
            lblTitulo.Padding = new Padding(10, 0, 0, 0);
            panelHeader.Controls.Add(lblTitulo);
            this.Controls.Add(panelHeader);

            // Panel formulario (izquierda)
            panelForm = new Panel();
            panelForm.Location = new Point(10, 60);
            panelForm.Size = new Size(300, 500);
            panelForm.BackColor = Color.White;
            panelForm.BorderStyle = BorderStyle.FixedSingle;
            panelForm.Padding = new Padding(10);

            int y = 15;
            AgregarCampo(panelForm, "ID Autor (###-##-####):", ref txtAuId, ref y, false);
            AgregarCampo(panelForm, "Apellido:", ref txtLname, ref y);
            AgregarCampo(panelForm, "Nombre:", ref txtFname, ref y);
            AgregarCampo(panelForm, "Teléfono:", ref txtPhone, ref y);
            AgregarCampo(panelForm, "Dirección:", ref txtAddress, ref y);
            AgregarCampo(panelForm, "Ciudad:", ref txtCity, ref y);
            AgregarCampo(panelForm, "Estado:", ref txtState, ref y);
            AgregarCampo(panelForm, "ZIP:", ref txtZip, ref y);

            Label lblContract = new Label();
            lblContract.Text = "Contrato:";
            lblContract.Font = new Font("Segoe UI", 9);
            lblContract.Location = new Point(10, y);
            lblContract.AutoSize = true;
            panelForm.Controls.Add(lblContract);
            y += 20;

            chkContract = new CheckBox();
            chkContract.Text = "Tiene contrato activo";
            chkContract.Font = new Font("Segoe UI", 9);
            chkContract.Location = new Point(10, y);
            chkContract.AutoSize = true;
            panelForm.Controls.Add(chkContract);
            y += 35;

            // Botones CRUD
            btnNuevo = CrearBoton("➕ Nuevo", Color.FromArgb(40, 167, 69), new Point(10, y));
            btnGuardar = CrearBoton("💾 Guardar", Color.FromArgb(0, 102, 153), new Point(160, y));
            y += 45;
            btnEliminar = CrearBoton("🗑️ Eliminar", Color.FromArgb(220, 53, 69), new Point(10, y));
            btnLimpiar = CrearBoton("🔄 Limpiar", Color.FromArgb(108, 117, 125), new Point(160, y));
            y += 45;
            btnBuscar = CrearBoton("🔍 Buscar todos", Color.FromArgb(255, 140, 0), new Point(10, y));
            btnBuscar.Width = 270;

            btnNuevo.Click += BtnNuevo_Click;
            btnGuardar.Click += BtnGuardar_Click;
            btnEliminar.Click += BtnEliminar_Click;
            btnLimpiar.Click += (s, e) => LimpiarFormulario();
            btnBuscar.Click += (s, e) => CargarDatos();

            panelForm.Controls.AddRange(new Control[] { btnNuevo, btnGuardar, btnEliminar, btnLimpiar, btnBuscar });
            this.Controls.Add(panelForm);

            // DataGridView
            panelGrid = new Panel();
            panelGrid.Location = new Point(320, 60);
            panelGrid.Size = new Size(610, 500);
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
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 102, 153);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgv.EnableHeadersVisualStyles = false;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 248, 255);
            dgv.CellClick += Dgv_CellClick;
            panelGrid.Controls.Add(dgv);
        }

        private void AgregarCampo(Panel panel, string label, ref TextBox txt, ref int y, bool enabled = true)
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
            txt.Size = new Size(270, 25);
            txt.Enabled = enabled;
            txt.BackColor = enabled ? Color.White : Color.FromArgb(240, 240, 240);
            panel.Controls.Add(txt);
            y += 32;
        }

        private Button CrearBoton(string texto, Color color, Point location)
        {
            Button btn = new Button();
            btn.Text = texto;
            btn.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btn.BackColor = color;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Location = location;
            btn.Size = new Size(130, 35);
            return btn;
        }

        private void CargarDatos()
        {
            try
            {
                DataTable dt = ConexionDB.EjecutarConsulta("SELECT * FROM authors ORDER BY au_lname");
                dgv.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgv.Rows[e.RowIndex];
                txtAuId.Text = row.Cells["au_id"]?.Value?.ToString() ?? "";
                txtLname.Text = row.Cells["au_lname"]?.Value?.ToString() ?? "";
                txtFname.Text = row.Cells["au_fname"]?.Value?.ToString() ?? "";
                txtPhone.Text = row.Cells["phone"]?.Value?.ToString() ?? "";
                txtAddress.Text = row.Cells["address"]?.Value?.ToString() ?? "";
                txtCity.Text = row.Cells["city"]?.Value?.ToString() ?? "";
                txtState.Text = row.Cells["state"]?.Value?.ToString() ?? "";
                txtZip.Text = row.Cells["zip"]?.Value?.ToString() ?? "";
                chkContract.Checked = Convert.ToBoolean(row.Cells["contract"]?.Value ?? false);
                esNuevo = false;
                txtAuId.Enabled = false;
            }
        }

        private void BtnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            esNuevo = true;
            txtAuId.Enabled = true;
            txtAuId.Focus();
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAuId.Text) || string.IsNullOrWhiteSpace(txtLname.Text))
            {
                MessageBox.Show("ID y Apellido son obligatorios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                SqlParameter[] parametros = {
                    new SqlParameter("@au_id", txtAuId.Text.Trim()),
                    new SqlParameter("@au_lname", txtLname.Text.Trim()),
                    new SqlParameter("@au_fname", txtFname.Text.Trim()),
                    new SqlParameter("@phone", string.IsNullOrWhiteSpace(txtPhone.Text) ? "UNKNOWN" : txtPhone.Text.Trim()),
                    new SqlParameter("@address", string.IsNullOrWhiteSpace(txtAddress.Text) ? (object)DBNull.Value : txtAddress.Text.Trim()),
                    new SqlParameter("@city", string.IsNullOrWhiteSpace(txtCity.Text) ? (object)DBNull.Value : txtCity.Text.Trim()),
                    new SqlParameter("@state", string.IsNullOrWhiteSpace(txtState.Text) ? (object)DBNull.Value : txtState.Text.Trim()),
                    new SqlParameter("@zip", string.IsNullOrWhiteSpace(txtZip.Text) ? (object)DBNull.Value : txtZip.Text.Trim()),
                    new SqlParameter("@contract", chkContract.Checked ? 1 : 0)
                };

                string query;
                if (esNuevo)
                    query = "INSERT INTO authors VALUES (@au_id, @au_lname, @au_fname, @phone, @address, @city, @state, @zip, @contract)";
                else
                    query = "UPDATE authors SET au_lname=@au_lname, au_fname=@au_fname, phone=@phone, address=@address, city=@city, state=@state, zip=@zip, contract=@contract WHERE au_id=@au_id";

                ConexionDB.EjecutarComando(query, parametros);
                MessageBox.Show(esNuevo ? "Autor registrado exitosamente." : "Autor actualizado exitosamente.",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarDatos();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAuId.Text))
            {
                MessageBox.Show("Seleccione un autor para eliminar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show($"¿Eliminar al autor {txtFname.Text} {txtLname.Text}?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    ConexionDB.EjecutarComando("DELETE FROM authors WHERE au_id = @au_id",
                        new[] { new SqlParameter("@au_id", txtAuId.Text) });
                    MessageBox.Show("Autor eliminado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarDatos();
                    LimpiarFormulario();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al eliminar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LimpiarFormulario()
        {
            txtAuId.Text = txtLname.Text = txtFname.Text = txtPhone.Text =
            txtAddress.Text = txtCity.Text = txtState.Text = txtZip.Text = "";
            chkContract.Checked = false;
            esNuevo = false;
            txtAuId.Enabled = true;
        }
    }
}
