using System;
using System.Drawing;
using System.Windows.Forms;

namespace PubsProject.Forms
{
    public class FrmPrincipal : Form
    {
        private MenuStrip menuStrip;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel lblStatus;

        public FrmPrincipal()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Sistema Pubs - Panel Principal";
            this.Size = new Size(1100, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.IsMdiContainer = true;
            this.BackColor = Color.FromArgb(240, 244, 248);

            // Menu Strip
            menuStrip = new MenuStrip();
            menuStrip.BackColor = Color.FromArgb(0, 102, 153);
            menuStrip.ForeColor = Color.White;
            menuStrip.Font = new Font("Segoe UI", 10);

            // Menú Catálogos
            ToolStripMenuItem mnuCatalogos = CrearMenu("📋 Catálogos");
            mnuCatalogos.DropDownItems.Add(CrearItem("👤 Autores", (s, e) => AbrirForm(new FrmAuthors())));
            mnuCatalogos.DropDownItems.Add(CrearItem("🏢 Editoriales", (s, e) => AbrirForm(new FrmPublishers())));
            mnuCatalogos.DropDownItems.Add(CrearItem("📖 Títulos", (s, e) => AbrirForm(new FrmTitles())));
            mnuCatalogos.DropDownItems.Add(CrearItem("🏪 Tiendas", (s, e) => AbrirForm(new FrmStores())));
            mnuCatalogos.DropDownItems.Add(CrearItem("💼 Puestos", (s, e) => AbrirForm(new FrmJobs())));

            // Menú Operaciones
            ToolStripMenuItem mnuOperaciones = CrearMenu("⚙️ Operaciones");
            mnuOperaciones.DropDownItems.Add(CrearItem("🔗 Autor-Título", (s, e) => AbrirForm(new FrmTitleAuthor())));
            mnuOperaciones.DropDownItems.Add(CrearItem("💰 Ventas", (s, e) => AbrirForm(new FrmSales())));
            mnuOperaciones.DropDownItems.Add(CrearItem("📊 Regalías", (s, e) => AbrirForm(new FrmRoySched())));
            mnuOperaciones.DropDownItems.Add(CrearItem("🏷️ Descuentos", (s, e) => AbrirForm(new FrmDiscounts())));

            // Menú Personal
            ToolStripMenuItem mnuPersonal = CrearMenu("👥 Personal");
            mnuPersonal.DropDownItems.Add(CrearItem("👨‍💼 Empleados", (s, e) => AbrirForm(new FrmEmployee())));
            mnuPersonal.DropDownItems.Add(CrearItem("🖼️ Info Editorial", (s, e) => AbrirForm(new FrmPubInfo())));

            // Menú Salir
            ToolStripMenuItem mnuSalir = CrearMenu("🚪 Salir");
            mnuSalir.Click += (s, e) => { this.Close(); };

            menuStrip.Items.AddRange(new ToolStripItem[] {
                mnuCatalogos, mnuOperaciones, mnuPersonal, mnuSalir
            });

            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);

            // Status strip
            statusStrip = new StatusStrip();
            statusStrip.BackColor = Color.FromArgb(0, 102, 153);
            lblStatus = new ToolStripStatusLabel();
            lblStatus.Text = $"  ✅ Sistema Pubs | Usuario: admin | {DateTime.Now:dd/MM/yyyy HH:mm}";
            lblStatus.ForeColor = Color.White;
            lblStatus.Font = new Font("Segoe UI", 9);
            statusStrip.Items.Add(lblStatus);
            this.Controls.Add(statusStrip);
        }

        private ToolStripMenuItem CrearMenu(string texto)
        {
            var item = new ToolStripMenuItem(texto);
            item.ForeColor = Color.White;
            item.Font = new Font("Segoe UI", 10);
            return item;
        }

        private ToolStripMenuItem CrearItem(string texto, EventHandler handler)
        {
            var item = new ToolStripMenuItem(texto);
            item.Click += handler;
            return item;
        }

        private void AbrirForm(Form form)
        {
            form.MdiParent = this;
            form.Show();
        }
    }
}
