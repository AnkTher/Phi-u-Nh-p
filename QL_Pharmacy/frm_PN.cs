using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QL_Pharmacy
{
    public partial class frm_PN : Form
    {//khai bao toan bo
        SqlConnection conn = new SqlConnection();
        SqlDataAdapter da = new SqlDataAdapter();
        SqlDataAdapter daT = new SqlDataAdapter();
        SqlDataAdapter daCT = new SqlDataAdapter();
        SqlDataAdapter dancc = new SqlDataAdapter();
        SqlCommand cmd = new SqlCommand();
        DataTable dt = new DataTable();
        DataTable dtT = new DataTable();
        DataTable dtCT = new DataTable();
        DataTable comdt = new DataTable();
       
        string sql, constr;
        public frm_PN()
        {
            InitializeComponent();
        }
       
        private void frm_PN_Load(object sender, EventArgs e)
        {// thiet lap ket noi voi csdl
            constr = "Data Source=DESKTOP-7NII7JG\\MSQL;Initial Catalog=\"QL NHA THUOC\";Integrated Security=True;Encrypt=False";
            conn.ConnectionString = constr;
            conn.Open();
            sql = "select * from dbo.NhapThuoc order by MaPhieuNhap";
            da = new SqlDataAdapter(sql, conn);
            da.Fill(dt);
            grdData.DataSource = dt;
            grdData.Refresh();
            NapCT();

        }
        private void grdData_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            NapCT();
            LoadDataToGridView();
          
        }
        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnhead_Click(object sender, EventArgs e)
        {
            grdData.CurrentCell = grdData[0, 0];
            NapCT();
        }

        private void btnfirst_Click(object sender, EventArgs e)
        {
            int i = grdData.CurrentRow.Index;
            if (i > 0)
            {
                grdData.CurrentCell = grdData[0, i - 1];
                NapCT();
            }
        }

        private void btnnext_Click(object sender, EventArgs e)
        {

            int i = grdData.CurrentRow.Index;
            if (i < grdData.RowCount - 1)
            {
                grdData.CurrentCell = grdData[0, i + 1];
                NapCT();
            }
        }

        private void btnlast_Click(object sender, EventArgs e)
        {


            grdData.CurrentCell = grdData[0, grdData.RowCount - 1];
            NapCT();

        }

        private void btnexit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void NapCT()
        {
              int i = grdData.CurrentRow.Index;
            txtmaPN.Text = grdData.Rows[i].Cells["MaPhieuNhap"].Value?.ToString();
            txtngaynhap.Text = grdData.Rows[i].Cells["NgayLap"].Value?.ToString();
            txttenthukhonhap.Text = grdData.Rows[i].Cells["thukhonhap"].Value?.ToString();
            txttenncc.Text = grdData.Rows[i].Cells["tenncc"].Value?.ToString();
            txttongtien.Text = grdData.Rows[i].Cells["TongTien"].Value?.ToString();
            
        }

        private void comTentruong_SelectedIndexChanged(object sender, EventArgs e)
        {
            //tim theo ma hang lay vao combobox
            if (comTentruong.Text != "tenncc")
            {
                sql = "select distinct MaPhieuNhap, " + comTentruong.Text + " from dbo.NhapThuoc WHERE '"+ comTentruong.Text +"' IS NOT NULL AND '" + comTentruong.Text + "' <> ''";
                da = new SqlDataAdapter(sql, conn);
                comdt.Clear();
                da.Fill(comdt);
                comGT.DataSource = comdt;
                comGT.DisplayMember = comTentruong.Text;
            }
            else
            {
                sql = "select distinct MaPhieuNhap, tenncc from dbo.NhapThuoc WHERE tenncc IS NOT NULL AND tenncc <> ''";
                da = new SqlDataAdapter(sql, conn);
                comdt.Clear();
                da.Fill(comdt);
                comGT.DataSource = comdt;
                comGT.DisplayMember = "tenncc";
                comGT.ValueMember = "tenncc";
            }
            if (comTentruong.SelectedItem != null)
            {
                string selectedField = comTentruong.SelectedItem.ToString();

                // Xử lý để lấy các giá trị khác nhau thuộc trường đã chọn
                sql = $"SELECT DISTINCT {selectedField} FROM dbo.NhapThuoc WHERE {selectedField} IS NOT NULL AND {selectedField} <> ''"; // Sử dụng SELECT DISTINCT
                da = new SqlDataAdapter(sql, conn);
                comdt.Clear();
                da.Fill(comdt);

                // Cập nhật nguồn dữ liệu cho comGT
                comGT.DataSource = comdt;
                comGT.DisplayMember = selectedField; // Hiển thị giá trị của trường đã chọn
                comGT.ValueMember = selectedField; // Giá trị tương ứng
            }
  
        }

        private void comGT_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void btnadd_Click(object sender, EventArgs e)
        {
            txtmaPN.Text = " ";
            txtngaynhap.Text = " ";
            txttenncc.Text = " ";
            txttenthukhonhap.Text = " ";
            txttongtien.Text = " ";
            txttenthukhonhap.Focus();
            // Ẩn txttenncc và hiện comncc
            txttenncc.Visible = false;
            comncc.Visible = true;
            DataTable dtcomncc = new DataTable();
            sql = "select distinct tenncc from dbo.ncc ";
            dancc = new SqlDataAdapter(sql, conn);
            dancc.Fill(dtcomncc);

            // Gán dữ liệu từ DataTable vào ComboBox comncc
            comncc.DataSource = dtcomncc;
            comncc.DisplayMember = "tenncc";
            comncc.ValueMember = "tenncc";
            try
            {
                using (SqlConnection conn = new SqlConnection(constr))
                {
                    conn.Open();

                    // Gọi Stored Procedure để chèn bản ghi mới và kích hoạt trigger
                    using (SqlCommand cmdInsert = new SqlCommand("sp_InsertNhapThuoc", conn))
                    {
                        cmdInsert.CommandType = CommandType.StoredProcedure;
                        cmdInsert.ExecuteNonQuery();
                    }

                    // Lấy bản ghi mới nhất
                    string sqlGetNewRecord = "SELECT TOP 1 MaPhieuNhap, NgayLap FROM NhapThuoc ORDER BY MaPhieuNhap DESC";
                    using (SqlCommand cmdGetNewRecord = new SqlCommand(sqlGetNewRecord, conn))
                    {
                        using (SqlDataReader reader = cmdGetNewRecord.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtmaPN.Text = reader["MaPhieuNhap"].ToString();
                                txtngaynhap.Text = reader["NgayLap"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message);
            }
        }

        private void btnupdate_Click(object sender, EventArgs e)
        {
            sql = "UPDATE NhapThuoc SET thukhonhap = N'" + txttenthukhonhap.Text + "' ,tenncc= N'" + comncc.SelectedValue.ToString() + "' WHERE MaPhieuNhap = '" + txtmaPN.Text + "'";
            cmd = new SqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
            MessageBox.Show("Đã thêm mới thành công!");
            Naplai();
            comncc.Visible = false;
            txttenncc.Visible = true;
        }
        private void frm_PN_FormClosing(object sender, FormClosingEventArgs e)
        {
            string sql = "DELETE FROM NhapThuoc WHERE MaPhieuNhap = '" + txtmaPN.Text + "'";

            using (SqlConnection conn = new SqlConnection(constr))
            {
                try
                {
                    conn.Open(); // Mở kết nối
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.ExecuteNonQuery(); // Thực hiện câu lệnh SQL
                        MessageBox.Show("Bản ghi đã bị xóa."); // Thông báo nếu xóa thành công
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Đã xảy ra lỗi khi xóa bản ghi: " + ex.Message);
                }
            }

            // Xác nhận người dùng có muốn đóng form không
            if (MessageBox.Show("Bạn có chắc chắn muốn đóng form?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                e.Cancel = true; // Hủy bỏ việc đóng form nếu người dùng chọn No
            }
        }
        private void comncc_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void btnrefresh_Click(object sender, EventArgs e)
        {
            sql = "select * from dbo.NhapThuoc order by MaPhieuNhap";
            da = new SqlDataAdapter(sql, conn);
            dt.Clear();
            da.Fill(dt);
            grdData.DataSource = dt;
            grdData.Refresh();
            NapCT();
        }

        private void btnfilter_Click(object sender, EventArgs e)
        {
            sql = "select * from dbo.NhapThuoc Where " + comTentruong.Text + "=N'" + comGT.Text + "'";
            da = new SqlDataAdapter(sql, conn);
            dt.Clear();
            da.Fill(dt);
            grdData.DataSource = dt;
            grdData.Refresh();
            NapCT();




        }

        private void btndel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn xóa bản ghi hiện thời?","Xác nhận yêu cầu xóa"
                ,MessageBoxButtons.YesNo,MessageBoxIcon.Question)==DialogResult.Yes)
            {
                sql = "delete from dbo.ChiTietPhieuNhap where MaPhieuNhap='" + txtmaPN.Text + "'";
                sql = "delete from dbo.NhapThuoc where MaPhieuNhap='" + txtmaPN.Text + "'";
               
                cmd =new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                Naplai();
            }    
        }

        private void btnedit_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Hãy thực hiện sửa nội dung dữ liệu trên ô lưới, kết thúc bằng việc cập nhật.");
        
            if (grdData.CurrentRow != null) // Kiểm tra nếu có bản ghi đang được chọn
            {
                    // Thiết lập txtmaPN và txtngaynhap không cho phép chỉnh sửa
                    txtmaPN.ReadOnly = true;
                    txtngaynhap.ReadOnly = true;

                    // Lấy thông tin từ bản ghi đang chọn
                    int i = grdData.CurrentRow.Index;
                txtmaPN.Text = grdData.Rows[i].Cells["MaPhieuNhap"].Value.ToString();
                txtngaynhap.Text = grdData.Rows[i].Cells["NgayLap"].Value.ToString();
                txttenthukhonhap.Text = grdData.Rows[i].Cells["thukhonhap"].Value.ToString();
                txttenncc.Text = grdData.Rows[i].Cells["tenncc"].Value.ToString();
                txttongtien.Text = grdData.Rows[i].Cells["TongTien"].Value.ToString();

                
                // Ẩn txttenncc và hiển thị comncc
                txttenncc.Visible = false;
                comncc.Visible = true;

                // Đổ dữ liệu từ bảng `ncc` vào ComboBox comncc
                DataTable dtcomncc = new DataTable();
                sql = "SELECT DISTINCT tenncc FROM dbo.ncc";
                dancc = new SqlDataAdapter(sql, conn);
                dancc.Fill(dtcomncc);

                comncc.DataSource = dtcomncc;
                comncc.DisplayMember = "tenncc";
                comncc.ValueMember = "tenncc";

                // Thiết lập giá trị của ComboBox comncc theo giá trị đang có trong txttenncc
                comncc.SelectedValue = txttenncc.Text;
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một bản ghi để chỉnh sửa.");
            }
        
}

        private void btnCTPN_Click(object sender, EventArgs e)
        {
            frm_CTPhieuNhap FormCT = new frm_CTPhieuNhap();
            FormCT.Show();

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void txtngaynhap_TextChanged(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void txttongtien_TextChanged(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {
   
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {
            LoadData();  
        }
        public void NapT()
        {
            
        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void Naplai()
        {
            sql = "select * from dbo.NhapThuoc order by MaPhieuNhap";
            da = new SqlDataAdapter(sql, conn);
            dt.Clear();
            da.Fill(dt);
            grdData.DataSource = dt;
            grdData.Refresh();
            NapCT();
        }
        private void LoadData()
        {
            try
            {
                string sql = "SELECT MaThuoc, TenThuoc, tenloaithuoc, dvcoso, hangsx FROM dbo.QL_Thuoc ORDER BY MaThuoc";
                SqlDataAdapter daT = new SqlDataAdapter(sql, conn);
                DataTable dtT = new DataTable();
                daT.Fill(dtT);
                grdT.DataSource = dtT;
                grdT.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void panCTPN_Paint(object sender, PaintEventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void grdT_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void grdCTNhap_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            NapCTPN();
        }

        private void grdT_SelectionChanged(object sender, EventArgs e)
        {
           
        }
        private void LoadDataToGridView()
        {
            // Kiểm tra nếu có hàng nào được chọn trong grdData
            if (grdData.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một phiếu nhập.");
                return;
            }

            // Lấy MaPhieuNhap từ dòng được chọn trong grdData
            string maPhieuNhap = grdData.SelectedRows[0].Cells["MaPhieuNhap"].Value.ToString();

            // Chuỗi kết nối đến cơ sở dữ liệu
            string constr = "Data Source=DESKTOP-7NII7JG\\MSQL;Initial Catalog=\"QL NHA THUOC\";Integrated Security=True;Encrypt=False";
            using (SqlConnection conn = new SqlConnection(constr))
            {
                // Câu lệnh SQL để lấy tất cả các trường trừ MaPhieuNhap từ bảng ChiTietPhieuNhap
                string sql = "SELECT SoLo, MaThuoc, NgaySanXuat, NgayHetHan, DonViNhap, slDonViNhap, GiaNhap, " +
                    "(slDonViNhap * GiaNhap) AS ThanhTien " + // Tính giá trị ThanhTien
                             "FROM ChiTietPhieuNhap " +
                             "WHERE MaPhieuNhap = @MaPhieuNhap";

                SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);
                adapter.SelectCommand.Parameters.AddWithValue("@MaPhieuNhap", maPhieuNhap);
                DataTable dtCT = new DataTable();

                try
                {
                    // Mở kết nối và đổ dữ liệu vào DataTable
                    conn.Open();
                    adapter.Fill(dtCT);

                    if (dtCT.Rows.Count == 0)
                    {
                        MessageBox.Show("Không tìm thấy dữ liệu chi tiết cho phiếu nhập được chọn.");
                    }

                    // Đổ dữ liệu vào grdCTNhap
                    grdCTNhap.DataSource = dtCT;
                    grdCTNhap.Refresh();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }
        private void grdCTNhap_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            NapCTPN();
        }

        private void grdCTNhap_MouseEnter(object sender, EventArgs e)
        {
            
        }

        private void grdCTNhap_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            NapCTPN();
        }
        // Biến để lưu chỉ số dòng hiện tại của `DataGridView` phiếu nhập
        private int selectedRowIndex = -1;
        private string maPN, ngayNhap, tenThuKhoNhap, tenNCC, tongTien;
        // Cờ để tránh gọi lại đệ quy
        private bool isHandlingSelection = false;
        private void grdData_CurrentCellChanged(object sender, EventArgs e)
        {
            
        }

        // Cờ để kiểm soát khi nào dòng hiện tại bị khóa
        private bool isLocked = false;
        private void button5_Click(object sender, EventArgs e)
        {
            txtMaThuoc.Text = " ";
            txtSoLo.Text = " ";
            txtNgaySanXuat.Text = " ";
            txtNgayHetHan.Text = " ";
            txtDonViNhap.Text = " ";
            txtslDonViNhap.Text = " ";
            txtGiaNhap.Text = " ";
            txtThanhTien.Text = " ";
            // Lưu giá trị hiện tại của các TextBox để giữ nguyên khi khóa dòng
            maPN = txtmaPN.Text;
            ngayNhap = txtngaynhap.Text;
            tenThuKhoNhap = txttenthukhonhap.Text;
            tenNCC = txttenncc.Text;
            tongTien = txttongtien.Text;
            // Bước 2: Đặt chế độ ReadOnly cho GridView phiếu nhập
            txtmaPN.ReadOnly = true;
            txtngaynhap.ReadOnly = true;
            txttenthukhonhap.ReadOnly = true;
            txttenncc.ReadOnly = true;
            txttongtien.ReadOnly = true;
            grdData.ReadOnly = true;
            // Lấy mã thuốc từ `grdT` và điền vào `txtMaThuoc`
            if (grdT.SelectedRows.Count > 0)
            {
                txtMaThuoc.Text = grdT.SelectedRows[0].Cells["MaThuoc"].Value.ToString();
            }

            // Lưu chỉ số dòng hiện tại trong `DataGridView` phiếu nhập
            if (grdData.SelectedRows.Count > 0)
            {
                selectedRowIndex = grdData.SelectedRows[0].Index;
            }
            // Đặt cờ khóa dòng để ngăn chọn dòng khác
            isLocked = true;

            // Đăng ký sự kiện để khóa việc chọn dòng khác trong `grdData`
            grdData.SelectionChanged += grdData_SelectionChanged;
        }
        private void grdData_SelectionChanged(object sender, EventArgs e)
        {
            
        }

        // Hàm để hủy bỏ khóa chọn dòng khác (nếu cần)
        private void UnlockRowSelection()
        {
            // Xóa cờ khóa dòng và sự kiện SelectionChanged
            isLocked = false;
            grdData.SelectionChanged -= grdData_SelectionChanged;
        }

        public void NapCTPN()
        {
            int i = grdCTNhap.CurrentRow.Index;
            txtMaThuoc.Text = grdCTNhap.Rows[i].Cells["MaThuoc"].Value?.ToString();
            txtSoLo.Text = grdCTNhap.Rows[i].Cells["SoLo"].Value?.ToString();
            txtNgaySanXuat.Text = grdCTNhap.Rows[i].Cells["NgaySanXuat"].Value?.ToString();
            txtNgayHetHan.Text = grdCTNhap.Rows[i].Cells["NgayHetHan"].Value?.ToString();
            txtDonViNhap.Text = grdCTNhap.Rows[i].Cells["DonViNhap"].Value?.ToString();
            txtslDonViNhap.Text = grdCTNhap.Rows[i].Cells["slDonViNhap"].Value?.ToString();
            txtGiaNhap.Text = grdCTNhap.Rows[i].Cells["GiaNhap"].Value?.ToString();
            txtThanhTien.Text = grdCTNhap.Rows[i].Cells["ThanhTien"].Value?.ToString();

        }

    }
}
