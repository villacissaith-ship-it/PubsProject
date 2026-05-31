using System;
using System.Drawing;
using System.Windows.Forms;
using PubsProject.Data;

namespace PubsProject.Forms
{
    public class FrmLogin : Form
    {
        private int intentos = 0;
        private const int MAX_INTENTOS = 3;

        private Label lblTitulo;
        private Label lblUsuario;
        private Label lblClave;
        private Label lblIntentos;
        private TextBox txtUsuario;
        private TextBox txtClave;
        private Button btnIngresar;
        private Button btnCancelar;
        private Panel panelHeader;

        public FrmLogin()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Sistema Pubs - Iniciar Sesión";
            this.Size = new Size(420, 340);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.WhiteSmoke;

            // Header panel
            panelHeader = new Panel();
            panelHeader.BackColor = Color.FromArgb(0, 102, 153);
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Height = 70;

            lblTitulo = new Label();
            lblTitulo.Text = "📚 Sistema Pubs";
            lblTitulo.ForeColor = Color.White;
            lblTitulo.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            lblTitulo.AutoSize = false;
            lblTitulo.Size = new Size(400, 60);
            lblTitulo.TextAlign = ContentAlignment.MiddleCenter;
            lblTitulo.Location = new Point(0, 5);
            panelHeader.Controls.Add(lblTitulo);
            this.Controls.Add(panelHeader);

            // Usuario
            lblUsuario = new Label();
            lblUsuario.Text = "Usuario:";
            lblUsuario.Font = new Font("Segoe UI", 10);
            lblUsuario.Location = new Point(50, 100);
            lblUsuario.AutoSize = true;
            this.Controls.Add(lblUsuario);

            txtUsuario = new TextBox();
            txtUsuario.Font = new Font("Segoe UI", 10);
            txtUsuario.Location = new Point(50, 120);
            txtUsuario.Size = new Size(300, 28);
            this.Controls.Add(txtUsuario);

            // Clave
            lblClave = new Label();
            lblClave.Text = "Contraseña:";
            lblClave.Font = new Font("Segoe UI", 10);
            lblClave.Location = new Point(50, 158);
            lblClave.AutoSize = true;
            this.Controls.Add(lblClave);

            txtClave = new TextBox();
            txtClave.Font = new Font("Segoe UI", 10);
            txtClave.Location = new Point(50, 178);
            txtClave.Size = new Size(300, 28);
            txtClave.PasswordChar = '●';
            this.Controls.Add(txtClave);

            // Label intentos
            lblIntentos = new Label();
            lblIntentos.Text = $"Intentos restantes: {MAX_INTENTOS}";
            lblIntentos.Font = new Font("Segoe UI", 9, FontStyle.Italic);
            lblIntentos.ForeColor = Color.Gray;
            lblIntentos.Location = new Point(50, 215);
            lblIntentos.AutoSize = true;
            this.Controls.Add(lblIntentos);

            // Botón Ingresar
            btnIngresar = new Button();
            btnIngresar.Text = "Ingresar";
            btnIngresar.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnIngresar.BackColor = Color.FromArgb(0, 102, 153);
            btnIngresar.ForeColor = Color.White;
            btnIngresar.FlatStyle = FlatStyle.Flat;
            btnIngresar.FlatAppearance.BorderSize = 0;
            btnIngresar.Location = new Point(50, 248);
            btnIngresar.Size = new Size(140, 38);
            btnIngresar.Click += BtnIngresar_Click;
            this.Controls.Add(btnIngresar);

            // Botón Cancelar
            btnCancelar = new Button();
            btnCancelar.Text = "Cancelar";
            btnCancelar.Font = new Font("Segoe UI", 10);
            btnCancelar.BackColor = Color.FromArgb(220, 53, 69);
            btnCancelar.ForeColor = Color.White;
            btnCancelar.FlatStyle = FlatStyle.Flat;
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Location = new Point(210, 248);
            btnCancelar.Size = new Size(140, 38);
            btnCancelar.Click += (s, e) => Application.Exit();
            this.Controls.Add(btnCancelar);

            this.AcceptButton = btnIngresar;
        }

        private void BtnIngresar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsuario.Text) || string.IsNullOrWhiteSpace(txtClave.Text))
            {
                MessageBox.Show("Por favor ingrese usuario y contraseña.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (ConexionDB.ValidarUsuario(txtUsuario.Text.Trim(), txtClave.Text.Trim()))
            {
                FrmPrincipal frmPrincipal = new FrmPrincipal();
                this.Hide();
                frmPrincipal.ShowDialog();
                this.Close();
            }
            else
            {
                intentos++;
                int restantes = MAX_INTENTOS - intentos;
                lblIntentos.Text = $"Intentos restantes: {restantes}";
                lblIntentos.ForeColor = restantes <= 1 ? Color.Red : Color.OrangeRed;

                if (intentos >= MAX_INTENTOS)
                {
                    MessageBox.Show("Ha superado el número máximo de intentos.\nEl sistema se cerrará.",
                        "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
                else
                {
                    MessageBox.Show($"Usuario o contraseña incorrectos.\nIntentos restantes: {restantes}",
                        "Error de acceso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtClave.Clear();
                    txtClave.Focus();
                }
            }
        }
    }
}
