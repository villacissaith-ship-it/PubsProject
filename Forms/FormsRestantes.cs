// =====================================================================
// FORMULARIOS RESTANTES DE LA BASE DE DATOS PUBS
// Cada clase sigue el mismo patrón CRUD que FrmAuthors y FrmPublishers
// =====================================================================

using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using PubsProject.Data;

namespace PubsProject.Forms
{
    // ==================== TIENDAS (stores) ====================
    public class FrmStores : Form
    {
        private DataGridView dgv;
        private TextBox txtStorId, txtStorName, txtStorAddress, txtCity, txtState, txtZip;
        private Button btnNuevo, btnGuardar, btnEliminar, btnLimpiar;
        private bool esNuevo = false;

        public FrmStores()
        {
            this.Text = "Gestión de Tiendas";
            this.Size = new Size(950, 560);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.WhiteSmoke;
            ConstruirUI(Color.FromArgb(25, 135, 84), "🏪 Gestión de Tiendas");
            CargarDatos();
        }

        private void ConstruirUI(Color color, string titulo)
        {
            Panel panelHeader = new Panel();
            panelHeader.BackColor = color; panelHeader.Dock = DockStyle.Top; panelHeader.Height = 50;
            Label lbl = new Label();
            lbl.Text = titulo; lbl.ForeColor = Color.White; lbl.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            lbl.Dock = DockStyle.Fill; lbl.TextAlign = ContentAlignment.MiddleLeft; lbl.Padding = new Padding(10, 0, 0, 0);
            panelHeader.Controls.Add(lbl); this.Controls.Add(panelHeader);

            Panel panelForm = new Panel();
            panelForm.Location = new Point(10, 60); panelForm.Size = new Size(290, 440);
            panelForm.BackColor = Color.White; panelForm.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(panelForm);

            int y = 15;
            CrearCampo(panelForm, "ID Tienda:", ref txtStorId, ref y);
            CrearCampo(panelForm, "Nombre Tienda:", ref txtStorName, ref y);
            CrearCampo(panelForm, "Dirección:", ref txtStorAddress, ref y);
            CrearCampo(panelForm, "Ciudad:", ref txtCity, ref y);
            CrearCampo(panelForm, "Estado:", ref txtState, ref y);
            CrearCampo(panelForm, "ZIP:", ref txtZip, ref y);

            btnNuevo = Boton("➕ Nuevo", Color.FromArgb(40, 167, 69), new Point(10, y));
            btnGuardar = Boton("💾 Guardar", color, new Point(150, y)); y += 45;
            btnEliminar = Boton("🗑️ Eliminar", Color.FromArgb(220, 53, 69), new Point(10, y));
            btnLimpiar = Boton("🔄 Limpiar", Color.FromArgb(108, 117, 125), new Point(150, y));
            btnNuevo.Click += (s, e) => { LimpiarForm(); esNuevo = true; txtStorId.Enabled = true; };
            btnGuardar.Click += Guardar; btnEliminar.Click += Eliminar; btnLimpiar.Click += (s, e) => LimpiarForm();
            panelForm.Controls.AddRange(new Control[] { btnNuevo, btnGuardar, btnEliminar, btnLimpiar });

            Panel panelGrid = new Panel();
            panelGrid.Location = new Point(310, 60); panelGrid.Size = new Size(620, 480); this.Controls.Add(panelGrid);
            dgv = ConstruirGrid(color); dgv.CellClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    var r = dgv.Rows[e.RowIndex];
                    txtStorId.Text = r.Cells["stor_id"]?.Value?.ToString() ?? "";
                    txtStorName.Text = r.Cells["stor_name"]?.Value?.ToString() ?? "";
                    txtStorAddress.Text = r.Cells["stor_address"]?.Value?.ToString() ?? "";
                    txtCity.Text = r.Cells["city"]?.Value?.ToString() ?? "";
                    txtState.Text = r.Cells["state"]?.Value?.ToString() ?? "";
                    txtZip.Text = r.Cells["zip"]?.Value?.ToString() ?? "";
                    esNuevo = false; txtStorId.Enabled = false;
                }
            };
            panelGrid.Controls.Add(dgv);
        }

        private void CargarDatos()
        {
            try { dgv.DataSource = ConexionDB.EjecutarConsulta("SELECT * FROM stores ORDER BY stor_name"); }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void Guardar(object s, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtStorId.Text)) { MessageBox.Show("El ID es obligatorio."); return; }
            try
            {
                SqlParameter[] p = {
                    new SqlParameter("@stor_id", txtStorId.Text.Trim()),
                    new SqlParameter("@stor_name", (object)txtStorName.Text.Trim() ?? DBNull.Value),
                    new SqlParameter("@stor_address", (object)txtStorAddress.Text.Trim() ?? DBNull.Value),
                    new SqlParameter("@city", (object)txtCity.Text.Trim() ?? DBNull.Value),
                    new SqlParameter("@state", (object)txtState.Text.Trim() ?? DBNull.Value),
                    new SqlParameter("@zip", (object)txtZip.Text.Trim() ?? DBNull.Value)
                };
                string q = esNuevo
                    ? "INSERT INTO stores VALUES (@stor_id, @stor_name, @stor_address, @city, @state, @zip)"
                    : "UPDATE stores SET stor_name=@stor_name, stor_address=@stor_address, city=@city, state=@state, zip=@zip WHERE stor_id=@stor_id";
                ConexionDB.EjecutarComando(q, p);
                MessageBox.Show("Guardado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarDatos(); LimpiarForm();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void Eliminar(object s, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtStorId.Text)) { MessageBox.Show("Seleccione un registro."); return; }
            if (MessageBox.Show("¿Eliminar esta tienda?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    ConexionDB.EjecutarComando("DELETE FROM stores WHERE stor_id=@stor_id", new[] { new SqlParameter("@stor_id", txtStorId.Text) });
                    MessageBox.Show("Eliminado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarDatos(); LimpiarForm();
                }
                catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
            }
        }

        private void LimpiarForm()
        {
            txtStorId.Text = txtStorName.Text = txtStorAddress.Text = txtCity.Text = txtState.Text = txtZip.Text = "";
            esNuevo = false; txtStorId.Enabled = true;
        }

        private void CrearCampo(Panel panel, string lbl, ref TextBox txt, ref int y)
        {
            Label l = new Label(); l.Text = lbl; l.Font = new Font("Segoe UI", 9); l.Location = new Point(10, y); l.AutoSize = true;
            panel.Controls.Add(l); y += 18;
            txt = new TextBox(); txt.Font = new Font("Segoe UI", 9); txt.Location = new Point(10, y); txt.Size = new Size(260, 25);
            panel.Controls.Add(txt); y += 32;
        }

        private DataGridView ConstruirGrid(Color headerColor)
        {
            DataGridView g = new DataGridView();
            g.Dock = DockStyle.Fill; g.BackgroundColor = Color.White;
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            g.MultiSelect = false; g.ReadOnly = true; g.AllowUserToAddRows = false; g.RowHeadersVisible = false;
            g.Font = new Font("Segoe UI", 9);
            g.ColumnHeadersDefaultCellStyle.BackColor = headerColor;
            g.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            g.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            g.EnableHeadersVisualStyles = false;
            return g;
        }

        private Button Boton(string texto, Color color, Point loc)
        {
            Button b = new Button(); b.Text = texto; b.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            b.BackColor = color; b.ForeColor = Color.White; b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0; b.Location = loc; b.Size = new Size(125, 35);
            return b;
        }
    }


    // ==================== PUESTOS (jobs) ====================
    public class FrmJobs : Form
    {
        private DataGridView dgv;
        private TextBox txtJobId, txtJobDesc, txtMinLvl, txtMaxLvl;
        private Button btnNuevo, btnGuardar, btnEliminar, btnLimpiar;
        private bool esNuevo = false;

        public FrmJobs()
        {
            this.Text = "Gestión de Puestos";
            this.Size = new Size(900, 520);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.WhiteSmoke;
            BuildUI();
            CargarDatos();
        }

        private void BuildUI()
        {
            Color color = Color.FromArgb(255, 140, 0);
            Panel ph = new Panel(); ph.BackColor = color; ph.Dock = DockStyle.Top; ph.Height = 50;
            Label lt = new Label(); lt.Text = "💼 Gestión de Puestos de Trabajo"; lt.ForeColor = Color.White;
            lt.Font = new Font("Segoe UI", 13, FontStyle.Bold); lt.Dock = DockStyle.Fill;
            lt.TextAlign = ContentAlignment.MiddleLeft; lt.Padding = new Padding(10, 0, 0, 0);
            ph.Controls.Add(lt); this.Controls.Add(ph);

            Panel pf = new Panel(); pf.Location = new Point(10, 60); pf.Size = new Size(290, 380);
            pf.BackColor = Color.White; pf.BorderStyle = BorderStyle.FixedSingle; this.Controls.Add(pf);

            int y = 15;
            // job_id es IDENTITY, solo lectura
            Label lId = new Label(); lId.Text = "ID (Auto):"; lId.Font = new Font("Segoe UI", 9);
            lId.Location = new Point(10, y); lId.AutoSize = true; pf.Controls.Add(lId); y += 18;
            txtJobId = new TextBox(); txtJobId.Font = new Font("Segoe UI", 9); txtJobId.Location = new Point(10, y);
            txtJobId.Size = new Size(260, 25); txtJobId.Enabled = false; txtJobId.BackColor = Color.FromArgb(240, 240, 240);
            pf.Controls.Add(txtJobId); y += 32;

            CrearCampo(pf, "Descripción del Puesto:", ref txtJobDesc, ref y);
            CrearCampo(pf, "Nivel Mínimo (10-250):", ref txtMinLvl, ref y);
            CrearCampo(pf, "Nivel Máximo (10-250):", ref txtMaxLvl, ref y);
            y += 10;

            btnNuevo = Boton("➕ Nuevo", Color.FromArgb(40, 167, 69), new Point(10, y));
            btnGuardar = Boton("💾 Guardar", color, new Point(150, y)); y += 45;
            btnEliminar = Boton("🗑️ Eliminar", Color.FromArgb(220, 53, 69), new Point(10, y));
            btnLimpiar = Boton("🔄 Limpiar", Color.FromArgb(108, 117, 125), new Point(150, y));
            btnNuevo.Click += (s, e) => { LimpiarForm(); esNuevo = true; };
            btnGuardar.Click += Guardar; btnEliminar.Click += Eliminar; btnLimpiar.Click += (s, e) => LimpiarForm();
            pf.Controls.AddRange(new Control[] { btnNuevo, btnGuardar, btnEliminar, btnLimpiar });

            Panel pg = new Panel(); pg.Location = new Point(310, 60); pg.Size = new Size(570, 430); this.Controls.Add(pg);
            dgv = new DataGridView(); dgv.Dock = DockStyle.Fill; dgv.BackgroundColor = Color.White;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect; dgv.MultiSelect = false;
            dgv.ReadOnly = true; dgv.AllowUserToAddRows = false; dgv.RowHeadersVisible = false;
            dgv.Font = new Font("Segoe UI", 9);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = color;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgv.EnableHeadersVisualStyles = false;
            dgv.CellClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    var r = dgv.Rows[e.RowIndex];
                    txtJobId.Text = r.Cells["job_id"]?.Value?.ToString() ?? "";
                    txtJobDesc.Text = r.Cells["job_desc"]?.Value?.ToString() ?? "";
                    txtMinLvl.Text = r.Cells["min_lvl"]?.Value?.ToString() ?? "";
                    txtMaxLvl.Text = r.Cells["max_lvl"]?.Value?.ToString() ?? "";
                    esNuevo = false;
                }
            };
            pg.Controls.Add(dgv);
        }

        private void CargarDatos()
        {
            try { dgv.DataSource = ConexionDB.EjecutarConsulta("SELECT * FROM jobs ORDER BY job_desc"); }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void Guardar(object s, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtJobDesc.Text)) { MessageBox.Show("La descripción es obligatoria."); return; }
            try
            {
                int minLvl = int.Parse(txtMinLvl.Text);
                int maxLvl = int.Parse(txtMaxLvl.Text);
                if (minLvl < 10 || maxLvl > 250) { MessageBox.Show("Niveles fuera de rango (10-250)."); return; }

                SqlParameter[] p = {
                    new SqlParameter("@job_desc", txtJobDesc.Text.Trim()),
                    new SqlParameter("@min_lvl", minLvl),
                    new SqlParameter("@max_lvl", maxLvl)
                };
                if (esNuevo)
                    ConexionDB.EjecutarComando("INSERT INTO jobs (job_desc, min_lvl, max_lvl) VALUES (@job_desc, @min_lvl, @max_lvl)", p);
                else
                {
                    p = new SqlParameter[] {
                        new SqlParameter("@job_id", int.Parse(txtJobId.Text)),
                        new SqlParameter("@job_desc", txtJobDesc.Text.Trim()),
                        new SqlParameter("@min_lvl", minLvl),
                        new SqlParameter("@max_lvl", maxLvl)
                    };
                    ConexionDB.EjecutarComando("UPDATE jobs SET job_desc=@job_desc, min_lvl=@min_lvl, max_lvl=@max_lvl WHERE job_id=@job_id", p);
                }
                MessageBox.Show("Guardado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarDatos(); LimpiarForm();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void Eliminar(object s, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtJobId.Text)) { MessageBox.Show("Seleccione un registro."); return; }
            if (MessageBox.Show("¿Eliminar este puesto?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    ConexionDB.EjecutarComando("DELETE FROM jobs WHERE job_id=@job_id", new[] { new SqlParameter("@job_id", int.Parse(txtJobId.Text)) });
                    MessageBox.Show("Eliminado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarDatos(); LimpiarForm();
                }
                catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
            }
        }

        private void LimpiarForm()
        {
            txtJobId.Text = txtJobDesc.Text = txtMinLvl.Text = txtMaxLvl.Text = "";
            esNuevo = false;
        }

        private void CrearCampo(Panel panel, string lbl, ref TextBox txt, ref int y)
        {
            Label l = new Label(); l.Text = lbl; l.Font = new Font("Segoe UI", 9); l.Location = new Point(10, y); l.AutoSize = true;
            panel.Controls.Add(l); y += 18;
            txt = new TextBox(); txt.Font = new Font("Segoe UI", 9); txt.Location = new Point(10, y); txt.Size = new Size(260, 25);
            panel.Controls.Add(txt); y += 32;
        }

        private Button Boton(string texto, Color color, Point loc)
        {
            Button b = new Button(); b.Text = texto; b.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            b.BackColor = color; b.ForeColor = Color.White; b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0; b.Location = loc; b.Size = new Size(125, 35);
            return b;
        }
    }


    // ==================== EMPLEADOS (employee) ====================
    public class FrmEmployee : Form
    {
        private DataGridView dgv;
        private TextBox txtEmpId, txtFname, txtMinit, txtLname, txtJobId, txtJobLvl, txtPubId;
        private DateTimePicker dtpHireDate;
        private Button btnNuevo, btnGuardar, btnEliminar, btnLimpiar;
        private bool esNuevo = false;

        public FrmEmployee()
        {
            this.Text = "Gestión de Empleados";
            this.Size = new Size(1000, 580);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.WhiteSmoke;
            BuildUI();
            CargarDatos();
        }

        private void BuildUI()
        {
            Color color = Color.FromArgb(111, 66, 193);
            Panel ph = new Panel(); ph.BackColor = color; ph.Dock = DockStyle.Top; ph.Height = 50;
            Label lt = new Label(); lt.Text = "👨‍💼 Gestión de Empleados"; lt.ForeColor = Color.White;
            lt.Font = new Font("Segoe UI", 13, FontStyle.Bold); lt.Dock = DockStyle.Fill;
            lt.TextAlign = ContentAlignment.MiddleLeft; lt.Padding = new Padding(10, 0, 0, 0);
            ph.Controls.Add(lt); this.Controls.Add(ph);

            Panel pf = new Panel(); pf.Location = new Point(10, 60); pf.Size = new Size(300, 490);
            pf.BackColor = Color.White; pf.BorderStyle = BorderStyle.FixedSingle; this.Controls.Add(pf);

            int y = 10;
            CrearCampo(pf, "ID Empleado (9 chars):", ref txtEmpId, ref y);
            CrearCampo(pf, "Nombre:", ref txtFname, ref y);
            CrearCampo(pf, "Inicial:", ref txtMinit, ref y);
            CrearCampo(pf, "Apellido:", ref txtLname, ref y);
            CrearCampo(pf, "ID Puesto:", ref txtJobId, ref y);
            CrearCampo(pf, "Nivel Puesto:", ref txtJobLvl, ref y);
            CrearCampo(pf, "ID Editorial:", ref txtPubId, ref y);

            Label lDate = new Label(); lDate.Text = "Fecha Contratación:"; lDate.Font = new Font("Segoe UI", 9);
            lDate.Location = new Point(10, y); lDate.AutoSize = true; pf.Controls.Add(lDate); y += 18;
            dtpHireDate = new DateTimePicker(); dtpHireDate.Font = new Font("Segoe UI", 9);
            dtpHireDate.Location = new Point(10, y); dtpHireDate.Size = new Size(270, 25);
            dtpHireDate.Format = DateTimePickerFormat.Short; pf.Controls.Add(dtpHireDate); y += 38;

            btnNuevo = Boton("➕ Nuevo", Color.FromArgb(40, 167, 69), new Point(10, y));
            btnGuardar = Boton("💾 Guardar", color, new Point(155, y)); y += 45;
            btnEliminar = Boton("🗑️ Eliminar", Color.FromArgb(220, 53, 69), new Point(10, y));
            btnLimpiar = Boton("🔄 Limpiar", Color.FromArgb(108, 117, 125), new Point(155, y));
            btnNuevo.Click += (s, e) => { LimpiarForm(); esNuevo = true; txtEmpId.Enabled = true; };
            btnGuardar.Click += Guardar; btnEliminar.Click += Eliminar; btnLimpiar.Click += (s, e) => LimpiarForm();
            pf.Controls.AddRange(new Control[] { btnNuevo, btnGuardar, btnEliminar, btnLimpiar });

            Panel pg = new Panel(); pg.Location = new Point(320, 60); pg.Size = new Size(660, 490); this.Controls.Add(pg);
            dgv = new DataGridView(); dgv.Dock = DockStyle.Fill; dgv.BackgroundColor = Color.White;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect; dgv.MultiSelect = false;
            dgv.ReadOnly = true; dgv.AllowUserToAddRows = false; dgv.RowHeadersVisible = false;
            dgv.Font = new Font("Segoe UI", 9);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = color;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgv.EnableHeadersVisualStyles = false;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 245, 255);
            dgv.CellClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    var r = dgv.Rows[e.RowIndex];
                    txtEmpId.Text = r.Cells["emp_id"]?.Value?.ToString() ?? "";
                    txtFname.Text = r.Cells["fname"]?.Value?.ToString() ?? "";
                    txtMinit.Text = r.Cells["minit"]?.Value?.ToString() ?? "";
                    txtLname.Text = r.Cells["lname"]?.Value?.ToString() ?? "";
                    txtJobId.Text = r.Cells["job_id"]?.Value?.ToString() ?? "";
                    txtJobLvl.Text = r.Cells["job_lvl"]?.Value?.ToString() ?? "";
                    txtPubId.Text = r.Cells["pub_id"]?.Value?.ToString() ?? "";
                    if (r.Cells["hire_date"]?.Value != null && r.Cells["hire_date"].Value != DBNull.Value)
                        dtpHireDate.Value = Convert.ToDateTime(r.Cells["hire_date"].Value);
                    esNuevo = false; txtEmpId.Enabled = false;
                }
            };
            pg.Controls.Add(dgv);
        }

        private void CargarDatos()
        {
            try { dgv.DataSource = ConexionDB.EjecutarConsulta("SELECT * FROM employee ORDER BY lname"); }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void Guardar(object s, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmpId.Text) || string.IsNullOrWhiteSpace(txtLname.Text))
            { MessageBox.Show("ID y Apellido son obligatorios."); return; }
            try
            {
                SqlParameter[] p = {
                    new SqlParameter("@emp_id", txtEmpId.Text.Trim()),
                    new SqlParameter("@fname", txtFname.Text.Trim()),
                    new SqlParameter("@minit", string.IsNullOrWhiteSpace(txtMinit.Text) ? (object)DBNull.Value : txtMinit.Text.Trim()),
                    new SqlParameter("@lname", txtLname.Text.Trim()),
                    new SqlParameter("@job_id", string.IsNullOrWhiteSpace(txtJobId.Text) ? 1 : int.Parse(txtJobId.Text)),
                    new SqlParameter("@job_lvl", string.IsNullOrWhiteSpace(txtJobLvl.Text) ? 10 : int.Parse(txtJobLvl.Text)),
                    new SqlParameter("@pub_id", string.IsNullOrWhiteSpace(txtPubId.Text) ? "9952" : txtPubId.Text.Trim()),
                    new SqlParameter("@hire_date", dtpHireDate.Value)
                };
                string q = esNuevo
                    ? "INSERT INTO employee VALUES (@emp_id, @fname, @minit, @lname, @job_id, @job_lvl, @pub_id, @hire_date)"
                    : "UPDATE employee SET fname=@fname, minit=@minit, lname=@lname, job_id=@job_id, job_lvl=@job_lvl, pub_id=@pub_id, hire_date=@hire_date WHERE emp_id=@emp_id";
                ConexionDB.EjecutarComando(q, p);
                MessageBox.Show("Guardado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarDatos(); LimpiarForm();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void Eliminar(object s, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmpId.Text)) { MessageBox.Show("Seleccione un registro."); return; }
            if (MessageBox.Show("¿Eliminar este empleado?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    ConexionDB.EjecutarComando("DELETE FROM employee WHERE emp_id=@emp_id", new[] { new SqlParameter("@emp_id", txtEmpId.Text) });
                    MessageBox.Show("Eliminado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarDatos(); LimpiarForm();
                }
                catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
            }
        }

        private void LimpiarForm()
        {
            txtEmpId.Text = txtFname.Text = txtMinit.Text = txtLname.Text =
            txtJobId.Text = txtJobLvl.Text = txtPubId.Text = "";
            dtpHireDate.Value = DateTime.Now;
            esNuevo = false; txtEmpId.Enabled = true;
        }

        private void CrearCampo(Panel panel, string lbl, ref TextBox txt, ref int y)
        {
            Label l = new Label(); l.Text = lbl; l.Font = new Font("Segoe UI", 9); l.Location = new Point(10, y); l.AutoSize = true;
            panel.Controls.Add(l); y += 18;
            txt = new TextBox(); txt.Font = new Font("Segoe UI", 9); txt.Location = new Point(10, y); txt.Size = new Size(270, 25);
            panel.Controls.Add(txt); y += 30;
        }

        private Button Boton(string texto, Color color, Point loc)
        {
            Button b = new Button(); b.Text = texto; b.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            b.BackColor = color; b.ForeColor = Color.White; b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0; b.Location = loc; b.Size = new Size(130, 35);
            return b;
        }
    }


    // ==================== AUTOR-TÍTULO (titleauthor) ====================
    public class FrmTitleAuthor : Form
    {
        private DataGridView dgv;
        private TextBox txtAuId, txtTitleId, txtAuOrd, txtRoyaltyPer;
        private Button btnNuevo, btnGuardar, btnEliminar, btnLimpiar;
        private bool esNuevo = false;

        public FrmTitleAuthor()
        {
            this.Text = "Relación Autor-Título";
            this.Size = new Size(900, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.WhiteSmoke;
            BuildUI();
            CargarDatos();
        }

        private void BuildUI()
        {
            Color color = Color.FromArgb(220, 53, 69);
            Panel ph = new Panel(); ph.BackColor = color; ph.Dock = DockStyle.Top; ph.Height = 50;
            Label lt = new Label(); lt.Text = "🔗 Relación Autor - Título"; lt.ForeColor = Color.White;
            lt.Font = new Font("Segoe UI", 13, FontStyle.Bold); lt.Dock = DockStyle.Fill;
            lt.TextAlign = ContentAlignment.MiddleLeft; lt.Padding = new Padding(10, 0, 0, 0);
            ph.Controls.Add(lt); this.Controls.Add(ph);

            Panel pf = new Panel(); pf.Location = new Point(10, 60); pf.Size = new Size(280, 360);
            pf.BackColor = Color.White; pf.BorderStyle = BorderStyle.FixedSingle; this.Controls.Add(pf);

            int y = 15;
            CrearCampo(pf, "ID Autor:", ref txtAuId, ref y);
            CrearCampo(pf, "ID Título:", ref txtTitleId, ref y);
            CrearCampo(pf, "Orden Autor:", ref txtAuOrd, ref y);
            CrearCampo(pf, "% Regalía:", ref txtRoyaltyPer, ref y);
            y += 10;

            btnNuevo = Boton("➕ Nuevo", Color.FromArgb(40, 167, 69), new Point(10, y));
            btnGuardar = Boton("💾 Guardar", color, new Point(150, y)); y += 45;
            btnEliminar = Boton("🗑️ Eliminar", Color.FromArgb(108, 117, 125), new Point(10, y));
            btnLimpiar = Boton("🔄 Limpiar", Color.FromArgb(108, 117, 125), new Point(150, y));
            btnNuevo.Click += (s, e) => { LimpiarForm(); esNuevo = true; };
            btnGuardar.Click += Guardar; btnEliminar.Click += Eliminar; btnLimpiar.Click += (s, e) => LimpiarForm();
            pf.Controls.AddRange(new Control[] { btnNuevo, btnGuardar, btnEliminar, btnLimpiar });

            Panel pg = new Panel(); pg.Location = new Point(300, 60); pg.Size = new Size(585, 420); this.Controls.Add(pg);
            dgv = new DataGridView(); dgv.Dock = DockStyle.Fill; dgv.BackgroundColor = Color.White;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect; dgv.MultiSelect = false;
            dgv.ReadOnly = true; dgv.AllowUserToAddRows = false; dgv.RowHeadersVisible = false;
            dgv.Font = new Font("Segoe UI", 9);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = color;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgv.EnableHeadersVisualStyles = false;
            dgv.CellClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    var r = dgv.Rows[e.RowIndex];
                    txtAuId.Text = r.Cells["au_id"]?.Value?.ToString() ?? "";
                    txtTitleId.Text = r.Cells["title_id"]?.Value?.ToString() ?? "";
                    txtAuOrd.Text = r.Cells["au_ord"]?.Value?.ToString() ?? "";
                    txtRoyaltyPer.Text = r.Cells["royaltyper"]?.Value?.ToString() ?? "";
                    esNuevo = false;
                }
            };
            pg.Controls.Add(dgv);
        }

        private void CargarDatos()
        {
            try { dgv.DataSource = ConexionDB.EjecutarConsulta("SELECT * FROM titleauthor"); }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void Guardar(object s, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAuId.Text) || string.IsNullOrWhiteSpace(txtTitleId.Text))
            { MessageBox.Show("ID Autor e ID Título son obligatorios."); return; }
            try
            {
                SqlParameter[] p = {
                    new SqlParameter("@au_id", txtAuId.Text.Trim()),
                    new SqlParameter("@title_id", txtTitleId.Text.Trim()),
                    new SqlParameter("@au_ord", string.IsNullOrWhiteSpace(txtAuOrd.Text) ? (object)DBNull.Value : byte.Parse(txtAuOrd.Text)),
                    new SqlParameter("@royaltyper", string.IsNullOrWhiteSpace(txtRoyaltyPer.Text) ? (object)DBNull.Value : int.Parse(txtRoyaltyPer.Text))
                };
                string q = esNuevo
                    ? "INSERT INTO titleauthor VALUES (@au_id, @title_id, @au_ord, @royaltyper)"
                    : "UPDATE titleauthor SET au_ord=@au_ord, royaltyper=@royaltyper WHERE au_id=@au_id AND title_id=@title_id";
                ConexionDB.EjecutarComando(q, p);
                MessageBox.Show("Guardado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarDatos(); LimpiarForm();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void Eliminar(object s, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAuId.Text)) { MessageBox.Show("Seleccione un registro."); return; }
            if (MessageBox.Show("¿Eliminar este registro?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    SqlParameter[] p = { new SqlParameter("@au_id", txtAuId.Text), new SqlParameter("@title_id", txtTitleId.Text) };
                    ConexionDB.EjecutarComando("DELETE FROM titleauthor WHERE au_id=@au_id AND title_id=@title_id", p);
                    MessageBox.Show("Eliminado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarDatos(); LimpiarForm();
                }
                catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
            }
        }

        private void LimpiarForm() { txtAuId.Text = txtTitleId.Text = txtAuOrd.Text = txtRoyaltyPer.Text = ""; esNuevo = false; }
        private void CrearCampo(Panel panel, string lbl, ref TextBox txt, ref int y)
        {
            Label l = new Label(); l.Text = lbl; l.Font = new Font("Segoe UI", 9); l.Location = new Point(10, y); l.AutoSize = true;
            panel.Controls.Add(l); y += 18;
            txt = new TextBox(); txt.Font = new Font("Segoe UI", 9); txt.Location = new Point(10, y); txt.Size = new Size(250, 25);
            panel.Controls.Add(txt); y += 32;
        }
        private Button Boton(string texto, Color color, Point loc)
        {
            Button b = new Button(); b.Text = texto; b.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            b.BackColor = color; b.ForeColor = Color.White; b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0; b.Location = loc; b.Size = new Size(125, 35);
            return b;
        }
    }


    // ==================== VENTAS (sales) ====================
    public class FrmSales : Form
    {
        private DataGridView dgv;
        private TextBox txtStorId, txtOrdNum, txtQty, txtPayterms, txtTitleId;
        private DateTimePicker dtpOrdDate;
        private Button btnNuevo, btnGuardar, btnEliminar, btnLimpiar;
        private bool esNuevo = false;

        public FrmSales()
        {
            this.Text = "Gestión de Ventas";
            this.Size = new Size(1000, 540);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.WhiteSmoke;
            BuildUI();
            CargarDatos();
        }

        private void BuildUI()
        {
            Color color = Color.FromArgb(32, 178, 170);
            Panel ph = new Panel(); ph.BackColor = color; ph.Dock = DockStyle.Top; ph.Height = 50;
            Label lt = new Label(); lt.Text = "💰 Gestión de Ventas"; lt.ForeColor = Color.White;
            lt.Font = new Font("Segoe UI", 13, FontStyle.Bold); lt.Dock = DockStyle.Fill;
            lt.TextAlign = ContentAlignment.MiddleLeft; lt.Padding = new Padding(10, 0, 0, 0);
            ph.Controls.Add(lt); this.Controls.Add(ph);

            Panel pf = new Panel(); pf.Location = new Point(10, 60); pf.Size = new Size(290, 430);
            pf.BackColor = Color.White; pf.BorderStyle = BorderStyle.FixedSingle; this.Controls.Add(pf);

            int y = 15;
            CrearCampo(pf, "ID Tienda:", ref txtStorId, ref y);
            CrearCampo(pf, "N° Orden:", ref txtOrdNum, ref y);

            Label lDate = new Label(); lDate.Text = "Fecha Orden:"; lDate.Font = new Font("Segoe UI", 9);
            lDate.Location = new Point(10, y); lDate.AutoSize = true; pf.Controls.Add(lDate); y += 18;
            dtpOrdDate = new DateTimePicker(); dtpOrdDate.Font = new Font("Segoe UI", 9);
            dtpOrdDate.Location = new Point(10, y); dtpOrdDate.Size = new Size(260, 25);
            dtpOrdDate.Format = DateTimePickerFormat.Short; pf.Controls.Add(dtpOrdDate); y += 35;

            CrearCampo(pf, "Cantidad:", ref txtQty, ref y);
            CrearCampo(pf, "Términos Pago:", ref txtPayterms, ref y);
            CrearCampo(pf, "ID Título:", ref txtTitleId, ref y);
            y += 5;

            btnNuevo = Boton("➕ Nuevo", Color.FromArgb(40, 167, 69), new Point(10, y));
            btnGuardar = Boton("💾 Guardar", color, new Point(150, y)); y += 45;
            btnEliminar = Boton("🗑️ Eliminar", Color.FromArgb(220, 53, 69), new Point(10, y));
            btnLimpiar = Boton("🔄 Limpiar", Color.FromArgb(108, 117, 125), new Point(150, y));
            btnNuevo.Click += (s, e) => { LimpiarForm(); esNuevo = true; };
            btnGuardar.Click += Guardar; btnEliminar.Click += Eliminar; btnLimpiar.Click += (s, e) => LimpiarForm();
            pf.Controls.AddRange(new Control[] { btnNuevo, btnGuardar, btnEliminar, btnLimpiar });

            Panel pg = new Panel(); pg.Location = new Point(310, 60); pg.Size = new Size(670, 460); this.Controls.Add(pg);
            dgv = new DataGridView(); dgv.Dock = DockStyle.Fill; dgv.BackgroundColor = Color.White;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect; dgv.MultiSelect = false;
            dgv.ReadOnly = true; dgv.AllowUserToAddRows = false; dgv.RowHeadersVisible = false;
            dgv.Font = new Font("Segoe UI", 9);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = color;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgv.EnableHeadersVisualStyles = false;
            dgv.CellClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    var r = dgv.Rows[e.RowIndex];
                    txtStorId.Text = r.Cells["stor_id"]?.Value?.ToString() ?? "";
                    txtOrdNum.Text = r.Cells["ord_num"]?.Value?.ToString() ?? "";
                    txtQty.Text = r.Cells["qty"]?.Value?.ToString() ?? "";
                    txtPayterms.Text = r.Cells["payterms"]?.Value?.ToString() ?? "";
                    txtTitleId.Text = r.Cells["title_id"]?.Value?.ToString() ?? "";
                    if (r.Cells["ord_date"]?.Value != null && r.Cells["ord_date"].Value != DBNull.Value)
                        dtpOrdDate.Value = Convert.ToDateTime(r.Cells["ord_date"].Value);
                    esNuevo = false;
                }
            };
            pg.Controls.Add(dgv);
        }

        private void CargarDatos()
        {
            try { dgv.DataSource = ConexionDB.EjecutarConsulta("SELECT * FROM sales ORDER BY ord_date DESC"); }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void Guardar(object s, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtStorId.Text) || string.IsNullOrWhiteSpace(txtOrdNum.Text))
            { MessageBox.Show("ID Tienda y N° Orden son obligatorios."); return; }
            try
            {
                SqlParameter[] p = {
                    new SqlParameter("@stor_id", txtStorId.Text.Trim()),
                    new SqlParameter("@ord_num", txtOrdNum.Text.Trim()),
                    new SqlParameter("@ord_date", dtpOrdDate.Value),
                    new SqlParameter("@qty", short.Parse(txtQty.Text)),
                    new SqlParameter("@payterms", txtPayterms.Text.Trim()),
                    new SqlParameter("@title_id", txtTitleId.Text.Trim())
                };
                string q = esNuevo
                    ? "INSERT INTO sales VALUES (@stor_id, @ord_num, @ord_date, @qty, @payterms, @title_id)"
                    : "UPDATE sales SET ord_date=@ord_date, qty=@qty, payterms=@payterms, title_id=@title_id WHERE stor_id=@stor_id AND ord_num=@ord_num AND title_id=@title_id";
                ConexionDB.EjecutarComando(q, p);
                MessageBox.Show("Guardado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarDatos(); LimpiarForm();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void Eliminar(object s, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtStorId.Text)) { MessageBox.Show("Seleccione un registro."); return; }
            if (MessageBox.Show("¿Eliminar esta venta?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    SqlParameter[] p = {
                        new SqlParameter("@stor_id", txtStorId.Text),
                        new SqlParameter("@ord_num", txtOrdNum.Text),
                        new SqlParameter("@title_id", txtTitleId.Text)
                    };
                    ConexionDB.EjecutarComando("DELETE FROM sales WHERE stor_id=@stor_id AND ord_num=@ord_num AND title_id=@title_id", p);
                    MessageBox.Show("Eliminado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarDatos(); LimpiarForm();
                }
                catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
            }
        }

        private void LimpiarForm()
        {
            txtStorId.Text = txtOrdNum.Text = txtQty.Text = txtPayterms.Text = txtTitleId.Text = "";
            dtpOrdDate.Value = DateTime.Now; esNuevo = false;
        }
        private void CrearCampo(Panel panel, string lbl, ref TextBox txt, ref int y)
        {
            Label l = new Label(); l.Text = lbl; l.Font = new Font("Segoe UI", 9); l.Location = new Point(10, y); l.AutoSize = true;
            panel.Controls.Add(l); y += 18;
            txt = new TextBox(); txt.Font = new Font("Segoe UI", 9); txt.Location = new Point(10, y); txt.Size = new Size(260, 25);
            panel.Controls.Add(txt); y += 32;
        }
        private Button Boton(string texto, Color color, Point loc)
        {
            Button b = new Button(); b.Text = texto; b.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            b.BackColor = color; b.ForeColor = Color.White; b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0; b.Location = loc; b.Size = new Size(125, 35);
            return b;
        }
    }


    // ==================== REGALÍAS (roysched) ====================
    public class FrmRoySched : Form
    {
        private DataGridView dgv;
        private TextBox txtTitleId, txtLorange, txtHirange, txtRoyalty;
        private Button btnNuevo, btnGuardar, btnEliminar, btnLimpiar;
        private bool esNuevo = false;

        public FrmRoySched()
        {
            this.Text = "Gestión de Regalías";
            this.Size = new Size(850, 480);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.WhiteSmoke;
            BuildUI();
            CargarDatos();
        }

        private void BuildUI()
        {
            Color color = Color.FromArgb(139, 69, 19);
            Panel ph = new Panel(); ph.BackColor = color; ph.Dock = DockStyle.Top; ph.Height = 50;
            Label lt = new Label(); lt.Text = "📊 Gestión de Regalías"; lt.ForeColor = Color.White;
            lt.Font = new Font("Segoe UI", 13, FontStyle.Bold); lt.Dock = DockStyle.Fill;
            lt.TextAlign = ContentAlignment.MiddleLeft; lt.Padding = new Padding(10, 0, 0, 0);
            ph.Controls.Add(lt); this.Controls.Add(ph);

            Panel pf = new Panel(); pf.Location = new Point(10, 60); pf.Size = new Size(280, 360);
            pf.BackColor = Color.White; pf.BorderStyle = BorderStyle.FixedSingle; this.Controls.Add(pf);

            int y = 15;
            CrearCampo(pf, "ID Título:", ref txtTitleId, ref y);
            CrearCampo(pf, "Rango Mínimo:", ref txtLorange, ref y);
            CrearCampo(pf, "Rango Máximo:", ref txtHirange, ref y);
            CrearCampo(pf, "Regalía (%):", ref txtRoyalty, ref y);
            y += 10;

            btnNuevo = Boton("➕ Nuevo", Color.FromArgb(40, 167, 69), new Point(10, y));
            btnGuardar = Boton("💾 Guardar", color, new Point(150, y)); y += 45;
            btnEliminar = Boton("🗑️ Eliminar", Color.FromArgb(220, 53, 69), new Point(10, y));
            btnLimpiar = Boton("🔄 Limpiar", Color.FromArgb(108, 117, 125), new Point(150, y));
            btnNuevo.Click += (s, e) => { LimpiarForm(); esNuevo = true; };
            btnGuardar.Click += Guardar; btnEliminar.Click += Eliminar; btnLimpiar.Click += (s, e) => LimpiarForm();
            pf.Controls.AddRange(new Control[] { btnNuevo, btnGuardar, btnEliminar, btnLimpiar });

            Panel pg = new Panel(); pg.Location = new Point(300, 60); pg.Size = new Size(535, 400); this.Controls.Add(pg);
            dgv = new DataGridView(); dgv.Dock = DockStyle.Fill; dgv.BackgroundColor = Color.White;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect; dgv.MultiSelect = false;
            dgv.ReadOnly = true; dgv.AllowUserToAddRows = false; dgv.RowHeadersVisible = false;
            dgv.Font = new Font("Segoe UI", 9);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = color;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgv.EnableHeadersVisualStyles = false;
            dgv.CellClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    var r = dgv.Rows[e.RowIndex];
                    txtTitleId.Text = r.Cells["title_id"]?.Value?.ToString() ?? "";
                    txtLorange.Text = r.Cells["lorange"]?.Value?.ToString() ?? "";
                    txtHirange.Text = r.Cells["hirange"]?.Value?.ToString() ?? "";
                    txtRoyalty.Text = r.Cells["royalty"]?.Value?.ToString() ?? "";
                    esNuevo = false;
                }
            };
            pg.Controls.Add(dgv);
        }

        private void CargarDatos()
        {
            try { dgv.DataSource = ConexionDB.EjecutarConsulta("SELECT * FROM roysched ORDER BY title_id"); }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void Guardar(object s, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitleId.Text)) { MessageBox.Show("ID Título es obligatorio."); return; }
            try
            {
                SqlParameter[] p = {
                    new SqlParameter("@title_id", txtTitleId.Text.Trim()),
                    new SqlParameter("@lorange", string.IsNullOrWhiteSpace(txtLorange.Text) ? (object)DBNull.Value : int.Parse(txtLorange.Text)),
                    new SqlParameter("@hirange", string.IsNullOrWhiteSpace(txtHirange.Text) ? (object)DBNull.Value : int.Parse(txtHirange.Text)),
                    new SqlParameter("@royalty", string.IsNullOrWhiteSpace(txtRoyalty.Text) ? (object)DBNull.Value : int.Parse(txtRoyalty.Text))
                };
                string q = esNuevo
                    ? "INSERT INTO roysched VALUES (@title_id, @lorange, @hirange, @royalty)"
                    : "UPDATE roysched SET lorange=@lorange, hirange=@hirange, royalty=@royalty WHERE title_id=@title_id AND lorange=@lorange";
                ConexionDB.EjecutarComando(q, p);
                MessageBox.Show("Guardado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarDatos(); LimpiarForm();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void Eliminar(object s, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitleId.Text)) { MessageBox.Show("Seleccione un registro."); return; }
            if (MessageBox.Show("¿Eliminar?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    SqlParameter[] p = { new SqlParameter("@title_id", txtTitleId.Text), new SqlParameter("@lorange", int.Parse(txtLorange.Text)) };
                    ConexionDB.EjecutarComando("DELETE FROM roysched WHERE title_id=@title_id AND lorange=@lorange", p);
                    MessageBox.Show("Eliminado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarDatos(); LimpiarForm();
                }
                catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
            }
        }

        private void LimpiarForm() { txtTitleId.Text = txtLorange.Text = txtHirange.Text = txtRoyalty.Text = ""; esNuevo = false; }
        private void CrearCampo(Panel panel, string lbl, ref TextBox txt, ref int y)
        {
            Label l = new Label(); l.Text = lbl; l.Font = new Font("Segoe UI", 9); l.Location = new Point(10, y); l.AutoSize = true;
            panel.Controls.Add(l); y += 18;
            txt = new TextBox(); txt.Font = new Font("Segoe UI", 9); txt.Location = new Point(10, y); txt.Size = new Size(250, 25);
            panel.Controls.Add(txt); y += 32;
        }
        private Button Boton(string texto, Color color, Point loc)
        {
            Button b = new Button(); b.Text = texto; b.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            b.BackColor = color; b.ForeColor = Color.White; b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0; b.Location = loc; b.Size = new Size(125, 35);
            return b;
        }
    }


    // ==================== DESCUENTOS (discounts) ====================
    public class FrmDiscounts : Form
    {
        private DataGridView dgv;
        private TextBox txtDiscounttype, txtStorId, txtLowqty, txtHighqty, txtDiscount;
        private Button btnNuevo, btnGuardar, btnEliminar, btnLimpiar;
        private bool esNuevo = false;

        public FrmDiscounts()
        {
            this.Text = "Gestión de Descuentos";
            this.Size = new Size(900, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.WhiteSmoke;
            BuildUI();
            CargarDatos();
        }

        private void BuildUI()
        {
            Color color = Color.FromArgb(230, 100, 20);
            Panel ph = new Panel(); ph.BackColor = color; ph.Dock = DockStyle.Top; ph.Height = 50;
            Label lt = new Label(); lt.Text = "🏷️ Gestión de Descuentos"; lt.ForeColor = Color.White;
            lt.Font = new Font("Segoe UI", 13, FontStyle.Bold); lt.Dock = DockStyle.Fill;
            lt.TextAlign = ContentAlignment.MiddleLeft; lt.Padding = new Padding(10, 0, 0, 0);
            ph.Controls.Add(lt); this.Controls.Add(ph);

            Panel pf = new Panel(); pf.Location = new Point(10, 60); pf.Size = new Size(285, 390);
            pf.BackColor = Color.White; pf.BorderStyle = BorderStyle.FixedSingle; this.Controls.Add(pf);

            int y = 15;
            CrearCampo(pf, "Tipo Descuento:", ref txtDiscounttype, ref y);
            CrearCampo(pf, "ID Tienda (opcional):", ref txtStorId, ref y);
            CrearCampo(pf, "Cant. Mínima:", ref txtLowqty, ref y);
            CrearCampo(pf, "Cant. Máxima:", ref txtHighqty, ref y);
            CrearCampo(pf, "Descuento (%):", ref txtDiscount, ref y);
            y += 10;

            btnNuevo = Boton("➕ Nuevo", Color.FromArgb(40, 167, 69), new Point(10, y));
            btnGuardar = Boton("💾 Guardar", color, new Point(150, y)); y += 45;
            btnEliminar = Boton("🗑️ Eliminar", Color.FromArgb(220, 53, 69), new Point(10, y));
            btnLimpiar = Boton("🔄 Limpiar", Color.FromArgb(108, 117, 125), new Point(150, y));
            btnNuevo.Click += (s, e) => { LimpiarForm(); esNuevo = true; };
            btnGuardar.Click += Guardar; btnEliminar.Click += Eliminar; btnLimpiar.Click += (s, e) => LimpiarForm();
            pf.Controls.AddRange(new Control[] { btnNuevo, btnGuardar, btnEliminar, btnLimpiar });

            Panel pg = new Panel(); pg.Location = new Point(305, 60); pg.Size = new Size(575, 420); this.Controls.Add(pg);
            dgv = new DataGridView(); dgv.Dock = DockStyle.Fill; dgv.BackgroundColor = Color.White;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect; dgv.MultiSelect = false;
            dgv.ReadOnly = true; dgv.AllowUserToAddRows = false; dgv.RowHeadersVisible = false;
            dgv.Font = new Font("Segoe UI", 9);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = color;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgv.EnableHeadersVisualStyles = false;
            dgv.CellClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    var r = dgv.Rows[e.RowIndex];
                    txtDiscounttype.Text = r.Cells["discounttype"]?.Value?.ToString() ?? "";
                    txtStorId.Text = r.Cells["stor_id"]?.Value?.ToString() ?? "";
                    txtLowqty.Text = r.Cells["lowqty"]?.Value?.ToString() ?? "";
                    txtHighqty.Text = r.Cells["highqty"]?.Value?.ToString() ?? "";
                    txtDiscount.Text = r.Cells["discount"]?.Value?.ToString() ?? "";
                    esNuevo = false;
                }
            };
            pg.Controls.Add(dgv);
        }

        private void CargarDatos()
        {
            try { dgv.DataSource = ConexionDB.EjecutarConsulta("SELECT * FROM discounts ORDER BY discounttype"); }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void Guardar(object s, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDiscounttype.Text)) { MessageBox.Show("Tipo de descuento es obligatorio."); return; }
            try
            {
                SqlParameter[] p = {
                    new SqlParameter("@discounttype", txtDiscounttype.Text.Trim()),
                    new SqlParameter("@stor_id", string.IsNullOrWhiteSpace(txtStorId.Text) ? (object)DBNull.Value : txtStorId.Text.Trim()),
                    new SqlParameter("@lowqty", string.IsNullOrWhiteSpace(txtLowqty.Text) ? (object)DBNull.Value : short.Parse(txtLowqty.Text)),
                    new SqlParameter("@highqty", string.IsNullOrWhiteSpace(txtHighqty.Text) ? (object)DBNull.Value : short.Parse(txtHighqty.Text)),
                    new SqlParameter("@discount", decimal.Parse(txtDiscount.Text))
                };
                string q = esNuevo
                    ? "INSERT INTO discounts VALUES (@discounttype, @stor_id, @lowqty, @highqty, @discount)"
                    : "UPDATE discounts SET stor_id=@stor_id, lowqty=@lowqty, highqty=@highqty, discount=@discount WHERE discounttype=@discounttype";
                ConexionDB.EjecutarComando(q, p);
                MessageBox.Show("Guardado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarDatos(); LimpiarForm();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void Eliminar(object s, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDiscounttype.Text)) { MessageBox.Show("Seleccione un registro."); return; }
            if (MessageBox.Show("¿Eliminar?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    ConexionDB.EjecutarComando("DELETE FROM discounts WHERE discounttype=@discounttype",
                        new[] { new SqlParameter("@discounttype", txtDiscounttype.Text) });
                    MessageBox.Show("Eliminado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarDatos(); LimpiarForm();
                }
                catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
            }
        }

        private void LimpiarForm() { txtDiscounttype.Text = txtStorId.Text = txtLowqty.Text = txtHighqty.Text = txtDiscount.Text = ""; esNuevo = false; }
        private void CrearCampo(Panel panel, string lbl, ref TextBox txt, ref int y)
        {
            Label l = new Label(); l.Text = lbl; l.Font = new Font("Segoe UI", 9); l.Location = new Point(10, y); l.AutoSize = true;
            panel.Controls.Add(l); y += 18;
            txt = new TextBox(); txt.Font = new Font("Segoe UI", 9); txt.Location = new Point(10, y); txt.Size = new Size(255, 25);
            panel.Controls.Add(txt); y += 32;
        }
        private Button Boton(string texto, Color color, Point loc)
        {
            Button b = new Button(); b.Text = texto; b.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            b.BackColor = color; b.ForeColor = Color.White; b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0; b.Location = loc; b.Size = new Size(125, 35);
            return b;
        }
    }


    // ==================== INFO EDITORIAL (pub_info) ====================
    public class FrmPubInfo : Form
    {
        private DataGridView dgv;
        private TextBox txtPubId, txtPrInfo;
        private Button btnNuevo, btnGuardar, btnEliminar, btnLimpiar;
        private bool esNuevo = false;

        public FrmPubInfo()
        {
            this.Text = "Info Editorial";
            this.Size = new Size(900, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.WhiteSmoke;
            BuildUI();
            CargarDatos();
        }

        private void BuildUI()
        {
            Color color = Color.FromArgb(70, 130, 180);
            Panel ph = new Panel(); ph.BackColor = color; ph.Dock = DockStyle.Top; ph.Height = 50;
            Label lt = new Label(); lt.Text = "🖼️ Info y Descripción de Editoriales"; lt.ForeColor = Color.White;
            lt.Font = new Font("Segoe UI", 13, FontStyle.Bold); lt.Dock = DockStyle.Fill;
            lt.TextAlign = ContentAlignment.MiddleLeft; lt.Padding = new Padding(10, 0, 0, 0);
            ph.Controls.Add(lt); this.Controls.Add(ph);

            Panel pf = new Panel(); pf.Location = new Point(10, 60); pf.Size = new Size(285, 390);
            pf.BackColor = Color.White; pf.BorderStyle = BorderStyle.FixedSingle; this.Controls.Add(pf);

            int y = 15;
            Label lId = new Label(); lId.Text = "ID Editorial:"; lId.Font = new Font("Segoe UI", 9);
            lId.Location = new Point(10, y); lId.AutoSize = true; pf.Controls.Add(lId); y += 18;
            txtPubId = new TextBox(); txtPubId.Font = new Font("Segoe UI", 9); txtPubId.Location = new Point(10, y);
            txtPubId.Size = new Size(255, 25); pf.Controls.Add(txtPubId); y += 32;

            Label lInfo = new Label(); lInfo.Text = "Descripción (pr_info):"; lInfo.Font = new Font("Segoe UI", 9);
            lInfo.Location = new Point(10, y); lInfo.AutoSize = true; pf.Controls.Add(lInfo); y += 18;
            txtPrInfo = new TextBox(); txtPrInfo.Font = new Font("Segoe UI", 9); txtPrInfo.Location = new Point(10, y);
            txtPrInfo.Size = new Size(255, 100); txtPrInfo.Multiline = true; pf.Controls.Add(txtPrInfo); y += 110;

            btnNuevo = Boton("➕ Nuevo", Color.FromArgb(40, 167, 69), new Point(10, y));
            btnGuardar = Boton("💾 Guardar", color, new Point(150, y)); y += 45;
            btnEliminar = Boton("🗑️ Eliminar", Color.FromArgb(220, 53, 69), new Point(10, y));
            btnLimpiar = Boton("🔄 Limpiar", Color.FromArgb(108, 117, 125), new Point(150, y));
            btnNuevo.Click += (s, e) => { LimpiarForm(); esNuevo = true; txtPubId.Enabled = true; };
            btnGuardar.Click += Guardar; btnEliminar.Click += Eliminar; btnLimpiar.Click += (s, e) => LimpiarForm();
            pf.Controls.AddRange(new Control[] { btnNuevo, btnGuardar, btnEliminar, btnLimpiar });

            Panel pg = new Panel(); pg.Location = new Point(305, 60); pg.Size = new Size(575, 420); this.Controls.Add(pg);
            dgv = new DataGridView(); dgv.Dock = DockStyle.Fill; dgv.BackgroundColor = Color.White;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect; dgv.MultiSelect = false;
            dgv.ReadOnly = true; dgv.AllowUserToAddRows = false; dgv.RowHeadersVisible = false;
            dgv.Font = new Font("Segoe UI", 9);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = color;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgv.EnableHeadersVisualStyles = false;
            dgv.CellClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    var r = dgv.Rows[e.RowIndex];
                    txtPubId.Text = r.Cells["pub_id"]?.Value?.ToString() ?? "";
                    txtPrInfo.Text = r.Cells["pr_info"]?.Value?.ToString() ?? "";
                    esNuevo = false; txtPubId.Enabled = false;
                }
            };
            pg.Controls.Add(dgv);
        }

        private void CargarDatos()
        {
            try { dgv.DataSource = ConexionDB.EjecutarConsulta("SELECT pub_id, CAST(pr_info AS varchar(200)) AS pr_info FROM pub_info"); }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void Guardar(object s, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPubId.Text)) { MessageBox.Show("El ID es obligatorio."); return; }
            try
            {
                SqlParameter[] p = {
                    new SqlParameter("@pub_id", txtPubId.Text.Trim()),
                    new SqlParameter("@pr_info", string.IsNullOrWhiteSpace(txtPrInfo.Text) ? (object)DBNull.Value : txtPrInfo.Text.Trim())
                };
                string q = esNuevo
                    ? "INSERT INTO pub_info (pub_id, pr_info) VALUES (@pub_id, @pr_info)"
                    : "UPDATE pub_info SET pr_info=@pr_info WHERE pub_id=@pub_id";
                ConexionDB.EjecutarComando(q, p);
                MessageBox.Show("Guardado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarDatos(); LimpiarForm();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void Eliminar(object s, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPubId.Text)) { MessageBox.Show("Seleccione un registro."); return; }
            if (MessageBox.Show("¿Eliminar?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    ConexionDB.EjecutarComando("DELETE FROM pub_info WHERE pub_id=@pub_id", new[] { new SqlParameter("@pub_id", txtPubId.Text) });
                    MessageBox.Show("Eliminado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarDatos(); LimpiarForm();
                }
                catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
            }
        }

        private void LimpiarForm() { txtPubId.Text = txtPrInfo.Text = ""; esNuevo = false; txtPubId.Enabled = true; }
        private Button Boton(string texto, Color color, Point loc)
        {
            Button b = new Button(); b.Text = texto; b.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            b.BackColor = color; b.ForeColor = Color.White; b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0; b.Location = loc; b.Size = new Size(125, 35);
            return b;
        }
    }
}
