using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QL_Pharmacy
{
    
    public partial class frm_main : Form
    {
        public frm_main()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;  // Mở tràn màn hình
        }

        private void quảnLýDanhMụcNhàCungCấpToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void frm_main_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }

        private void danhMụcThuốcToolStripMenuItem_Click(object sender, EventArgs e)
        {
        
        }

        private void nhậpKhoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frm_PN f=new frm_PN();
            f.Show();
        }
    }
}
