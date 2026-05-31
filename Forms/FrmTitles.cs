using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using PubsProject.Data;

namespace PubsProject.Forms
{
    public class FrmTitles : Form
    {
        private DataGridView dgv;
        private TextBox txtTitleId, txtTitle, txtType, txtPubId, txtPrice, txtAdvance, txtRoyalty, txtYtdSales, txtNotes;
        private DateTimePicker dtpPubDate;
        private Button btnNuevo, btnGuardar, btnEliminar, btnLimpiar;
        private bool esNuevo = false;

        public FrmTitles()
        {
            InitializeComponent();
            CargarDatos();
        }

        private void InitializeComponent()
        {
            this.Text = "Gestión de Títulos";
            this.Size = new Size(1000, 640);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.WhiteSmoke;

            Panel panelHeader = new Panel();
            panelHeader.BackColor = Color.FromArgb(102, 16, 242);
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Height = 50;
            Label lbl = new Label();
            lbl.Text = "📖 Gestión de Títulos / Libros";
            lbl.ForeColor = Color.White;
            lbl.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            lbl.Dock = DockStyle.Fill;
            lbl.TextAlign = ContentAlignment.MiddleLeft;
            lbl.Padding = new Padding(10, 0, 0, 0);
            panelHeader.Controls.Add(lbl);
            this.Controls.Add(panelHeader);

            Panel panelForm = new Panel();
            panelForm.Location = new Point(10, 60);
            panelForm.Size = new Size(310, 540);
            panelForm.BackColor = Color.White;
            panelForm.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(panelForm);

            int y = 10;
            AgregarCampo(panelForm, "ID Título:", ref txtTitleId, ref y);
            AgregarCampo(panelForm, "Título:", ref txtTitle, ref y);
            AgregarCampo(panelForm, "Tipo:", ref txtType, ref y);
            AgregarCampo(panelForm, "ID Editorial:", ref txtPubId, ref y);
            AgregarCampo(panelForm, "Precio:", ref txtPrice, ref y);
            AgregarCampo(panelForm, "Anticipo:", ref txtAdvance, ref y);
            AgregarCampo(panelForm, "Regalía (%):", ref txtRoyalty, ref y);
            AgregarCampo(panelForm, "Ventas Año:", ref txtYtdSales, ref y);
            AgregarCampo(panelForm, "Notas:", ref txtNotes, ref y);

            Label lblDate = new Label();
            lblDate.Text = "Fecha Publicación:";
            lblDate.Font = new Font("Segoe UI", 9);
            lblDate.Location = new Point(10, y);
            lblDate.AutoSize = true;
            panelForm.Controls.Add(lblDate);
            y += 18;

            dtpPubDate = new DateTimePicker();
            dtpPubDate.Font = new Font("Segoe UI", 9);
            dtpPubDate.Location = new Point(10, y);
            dtpPubDate.Size = new Size(280, 25);
            dtpPubDate.Format = DateTimePickerFormat.Short;
            panelForm.Controls.Add(dtpPubDate);
            y += 38;

            btnNuevo = CrearBoton("➕ Nuevo", Color.FromArgb(40, 167, 69), new Point(10, y));
            btnGuardar = CrearBoton("💾 Guardar", Color.FromArgb(102, 16, 242), new Point(155, y)); y += 45;
            btnEliminar = CrearBoton("🗑️ Eliminar", Color.FromArgb(220, 53, 69), new Point(10, y));
            btnLimpiar = CrearBoton("🔄 Limpiar", Color.FromArgb(108, 117, 125), new Point(155, y));

            btnNuevo.Click += (s, e) => { LimpiarFormulario(); esNuevo = true; txtTitleId.Enabled = true; };
            btnGuardar.Click += BtnGuardar_Click;
            btnEliminar.Click += BtnEliminar_Click;
            btnLimpiar.Click += (s, e) => LimpiarFormulario();
            panelForm.Controls.AddRange(new Control[] { btnNuevo, btnGuardar, btnEliminar, btnLimpiar });

            Panel panelGrid = new Panel();
            panelGrid.Location = new Point(330, 60);
            panelGrid.Size = new Size(650, 540);
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
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(102, 16, 242);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgv.EnableHeadersVisualStyles = false;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 240, 255);
            dgv.CellClick += Dgv_CellClick;
            panelGrid.Controls.Add(dgv);
        }

        private void AgregarCampo(Panel panel, string label, ref TextBox txt, ref int y)
        {
            Label lbl = new Label();
            lbl.Text = label; lbl.Font = new Font("Segoe UI", 9);
            lbl.Location = new Point(10, y); lbl.AutoSize = true;
            panel.Controls.Add(lbl); y += 18;
            txt = new TextBox();
            txt.Font = new Font("Segoe UI", 9);
            txt.Location = new Point(10, y); txt.Size = new Size(280, 25);
            panel.Controls.Add(txt); y += 30;
        }

        private Button CrearBoton(string texto, Color color, Point location)
        {
            Button btn = new Button();
            btn.Text = texto; btn.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btn.BackColor = color; btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat; btn.FlatAppearance.BorderSize = 0;
            btn.Location = location; btn.Size = new Size(130, 35);
            return btn;
        }

        private void CargarDatos()
        {
            try { dgv.DataSource = ConexionDB.EjecutarConsulta("SELECT * FROM titles ORDER BY title"); }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dgv.Rows[e.RowIndex];
                txtTitleId.Text = row.Cells["title_id"]?.Value?.ToString() ?? "";
                txtTitle.Text = row.Cells["title"]?.Value?.ToString() ?? "";
                txtType.Text = row.Cells["type"]?.Value?.ToString() ?? "";
                txtPubId.Text = row.Cells["pub_id"]?.Value?.ToString() ?? "";
                txtPrice.Text = row.Cells["price"]?.Value?.ToString() ?? "";
                txtAdvance.Text = row.Cells["advance"]?.Value?.ToString() ?? "";
                txtRoyalty.Text = row.Cells["royalty"]?.Value?.ToString() ?? "";
                txtYtdSales.Text = row.Cells["ytd_sales"]?.Value?.ToString() ?? "";
                txtNotes.Text = row.Cells["notes"]?.Value?.ToString() ?? "";
                if (row.Cells["pubdate"]?.Value != null && row.Cells["pubdate"].Value != DBNull.Value)
                    dtpPubDate.Value = Convert.ToDateTime(row.Cells["pubdate"].Value);
                esNuevo = false;
                txtTitleId.Enabled = false;
            }
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitleId.Text) || string.IsNullOrWhiteSpace(txtTitle.Text))
            { MessageBox.Show("ID y Título son obligatorios."); return; }
            try
            {
                decimal? price = string.IsNullOrWhiteSpace(txtPrice.Text) ? (decimal?)null : decimal.Parse(txtPrice.Text);
                decimal? advance = string.IsNullOrWhiteSpace(txtAdvance.Text) ? (decimal?)null : decimal.Parse(txtAdvance.Text);
                int? royalty = string.IsNullOrWhiteSpace(txtRoyalty.Text) ? (int?)null : int.Parse(txtRoyalty.Text);
                int? ytd = string.IsNullOrWhiteSpace(txtYtdSales.Text) ? (int?)null : int.Parse(txtYtdSales.Text);

                SqlParameter[] p = {
                    new SqlParameter("@title_id", txtTitleId.Text.Trim()),
                    new SqlParameter("@title", txtTitle.Text.Trim()),
                    new SqlParameter("@type", string.IsNullOrWhiteSpace(txtType.Text) ? "UNDECIDED" : txtType.Text.Trim()),
                    new SqlParameter("@pub_id", string.IsNullOrWhiteSpace(txtPubId.Text) ? (object)DBNull.Value : txtPubId.Text.Trim()),
                    new SqlParameter("@price", price.HasValue ? (object)price.Value : DBNull.Value),
                    new SqlParameter("@advance", advance.HasValue ? (object)advance.Value : DBNull.Value),
                    new SqlParameter("@royalty", royalty.HasValue ? (object)royalty.Value : DBNull.Value),
                    new SqlParameter("@ytd_sales", ytd.HasValue ? (object)ytd.Value : DBNull.Value),
                    new SqlParameter("@notes", string.IsNullOrWhiteSpace(txtNotes.Text) ? (object)DBNull.Value : txtNotes.Text.Trim()),
                    new SqlParameter("@pubdate", dtpPubDate.Value)
                };
                string q = esNuevo
                    ? "INSERT INTO titles VALUES (@title_id, @title, @type, @pub_id, @price, @advance, @royalty, @ytd_sales, @notes, @pubdate)"
                    : "UPDATE titles SET title=@title, type=@type, pub_id=@pub_id, price=@price, advance=@advance, royalty=@royalty, ytd_sales=@ytd_sales, notes=@notes, pubdate=@pubdate WHERE title_id=@title_id";
                ConexionDB.EjecutarComando(q, p);
                MessageBox.Show("Guardado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarDatos(); LimpiarFormulario();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitleId.Text)) { MessageBox.Show("Seleccione un registro."); return; }
            if (MessageBox.Show("¿Eliminar este título?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    ConexionDB.EjecutarComando("DELETE FROM titles WHERE title_id=@title_id",
                        new[] { new SqlParameter("@title_id", txtTitleId.Text) });
                    MessageBox.Show("Eliminado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarDatos(); LimpiarFormulario();
                }
                catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
            }
        }

        private void LimpiarFormulario()
        {
            txtTitleId.Text = txtTitle.Text = txtType.Text = txtPubId.Text =
            txtPrice.Text = txtAdvance.Text = txtRoyalty.Text = txtYtdSales.Text = txtNotes.Text = "";
            dtpPubDate.Value = DateTime.Now;
            esNuevo = false; txtTitleId.Enabled = true;
        }
    }
}
