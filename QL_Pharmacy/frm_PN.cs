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
        SqlDataAdapter daDV = new SqlDataAdapter();
        SqlCommand cmd = new SqlCommand();
        DataTable dt = new DataTable();
        DataTable dtT = new DataTable();
        DataTable dtCT = new DataTable();
        DataTable comdt = new DataTable();
        bool maT = false;
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
            txtmaPN.ReadOnly = true;
            txtngaynhap.ReadOnly = true;
            txttenncc.ReadOnly = true;
            txttenthukhonhap.ReadOnly = true;
            txttongtien.ReadOnly = true;
            txttenthukhonhap.ReadOnly = true;
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
                sql = "select distinct MaPhieuNhap, " + comTentruong.Text + " from dbo.NhapThuoc WHERE '" + comTentruong.Text + "' IS NOT NULL AND '" + comTentruong.Text + "' <> ''";
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


        private void btnadd_Click(object sender, EventArgs e)
        {
            flag = "thêm";
            txtmaPN.Text = " ";
            txtngaynhap.Text = " ";
            txttenncc.Text = " ";
            txttenthukhonhap.Text = " ";
            txttongtien.Text = " ";
            txttenthukhonhap.Focus();
            txttenncc.ReadOnly = false;
            txttenthukhonhap.ReadOnly = false;
            txttongtien.ReadOnly = true;
            txttenthukhonhap.ReadOnly = false;
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
            using (SqlConnection conn = new SqlConnection(constr))
            {
                try
                {
                    conn.Open(); // Mở kết nối
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
            cmd.ExecuteNonQuery();
                        if (flag == "sửa")
                        { MessageBox.Show("Đã cập nhật thành công!"); }
                        if (flag == "thêm")
                        { MessageBox.Show("Đã thêm mới thành công!"); }
            Naplai();
            txttenncc.ReadOnly = true;
            txttenthukhonhap.ReadOnly = true;
            txttongtien.ReadOnly = false;
            //ẩn combo box hiện text box
            comncc.Visible = false;
            txttenncc.Visible = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Đã xảy ra lỗi khi thay đổi: " + ex.Message);
                }
            }
          
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

        //test
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
            if (MessageBox.Show("Bạn có chắc chắn muốn xóa bản ghi hiện thời?", "Xác nhận yêu cầu xóa"
                , MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                sql = "delete from dbo.ChiTietPhieuNhap where MaPhieuNhap='" + txtmaPN.Text + "'";
                sql = "delete from dbo.NhapThuoc where MaPhieuNhap='" + txtmaPN.Text + "'";

                cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                Naplai();
            }
        }

        private void btnedit_Click(object sender, EventArgs e)
        {
            flag = "sửa";
            MessageBox.Show("Hãy thực hiện sửa nội dung dữ liệu trên ô lưới, kết thúc bằng việc cập nhật.");

            if (grdData.CurrentRow != null) // Kiểm tra nếu có bản ghi đang được chọn
            {
                // Thiết lập txtmaPN và txtngaynhap không cho phép chỉnh sửa
                txtmaPN.ReadOnly = true;
                txtngaynhap.ReadOnly = true;

                txttenncc.ReadOnly = false;
                txttenthukhonhap.ReadOnly = false;

                txttenthukhonhap.ReadOnly = false;

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


        private void panel4_Paint(object sender, PaintEventArgs e)
        {
            LoadData();
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
        private void NapLaiCT()
        {
            // Câu lệnh SQL để lấy dữ liệu
            string sql = "SELECT SoLo, MaThuoc, NgaySanXuat, NgayHetHan, DonViNhap, slDonViNhap, GiaNhap, " +
                         "(slDonViNhap * GiaNhap) AS ThanhTien " + // Tính giá trị ThanhTien
                         "FROM ChiTietPhieuNhap " +
                         "WHERE MaPhieuNhap = @MaPhieuNhap";

            // Tạo SqlDataAdapter và thiết lập tham số
            SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);

            // Chuyển đổi MaPhieuNhap thành chuỗi và thiết lập nó vào tham số
            adapter.SelectCommand.Parameters.AddWithValue("@MaPhieuNhap", txtmaPN.Text.ToString());

            // Tạo DataTable và làm mới dữ liệu
          
            dtCT.Clear();

            // Thực hiện lệnh và đổ dữ liệu vào DataTable
            adapter.Fill(dtCT);

            // Gán dữ liệu vào DataGridView và làm mới
            grdCTNhap.DataSource = dtCT;
            grdCTNhap.Refresh();
            NapCTPN();
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





        private void grdT_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (maT)
            {
                // Lấy mã thuốc từ `grdT` và điền vào `txtMaThuoc`

                txtMaThuoc.Text = grdT.SelectedCells[0].Value.ToString();
                //

            }
        }



        private void grdCTNhap_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            NapCTPN();
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
            string maPhieuNhap = grdData.SelectedRows[0].Cells["MaPhieuNhap"].Value?.ToString();

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


        private void grdCTNhap_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            NapCTPN();
        }
        // Biến để lưu chỉ số dòng hiện tại của `DataGridView` phiếu nhập
        private int selectedRowIndex = -1;
        private string maPN, ngayNhap, tenThuKhoNhap, tenNCC, tongTien;
        // Cờ để tránh gọi lại đệ quy
        private bool isHandlingSelection = false;

        private void comDV_DropDown(object sender, EventArgs e)
        {

            // Gán dữ liệu từ DataTable vào ComboBox comDV
            DataTable dtcomDV = new DataTable();
            string sql = $"SELECT ql.dvcoso AS DonVi FROM dbo.QL_Thuoc ql WHERE ql.MaThuoc = '{txtMaThuoc.Text}' UNION SELECT qd.DonViQuyDoi AS DonVi FROM dbo.QuyDoiDonVi qd WHERE qd.MaThuoc = '{txtMaThuoc.Text}'";
            daDV = new SqlDataAdapter(sql, conn);
            dtcomDV.Clear();
            daDV.Fill(dtcomDV);

            // Kiểm tra nếu có dữ liệu
            if (dtcomDV.Rows.Count > 0)
            {
                // Gán dữ liệu vào ComboBox
                comDV.DataSource = dtcomDV;
                comDV.DisplayMember = "DonVi";
                comDV.ValueMember = "DonVi";
            }
            else
            {
                MessageBox.Show("Không tìm thấy đơn vị nào cho mã thuốc này.");
                comDV.DataSource = null;
            }
        }

        private void btnudateCT_Click(object sender, EventArgs e)
        {
            dtNSX.Visible = false;
            dtNHH.Visible = false;
            txtNgaySanXuat.Visible = true;
            txtNgayHetHan.Visible = true;
            txtMaThuoc.ReadOnly = true;
            txtSoLo.ReadOnly = true;
            txtNgaySanXuat.ReadOnly = true;
            txtNgayHetHan.ReadOnly = true;
            txtDonViNhap.ReadOnly = true;
            txtslDonViNhap.ReadOnly = true;
            txtGiaNhap.ReadOnly = true;
            // Hiện TextBox và ẩn ComboBox
            txtDonViNhap.Visible = true;
            comDV.Visible = false;
            // Kiểm tra các trường dữ liệu đầu vào có rỗng không
            if (string.IsNullOrEmpty(txtmaPN.Text) ||
                string.IsNullOrEmpty(txtMaThuoc.Text) ||
                string.IsNullOrEmpty(txtSoLo.Text) ||
                comDV.SelectedValue == null ||
                string.IsNullOrEmpty(txtslDonViNhap.Text) ||
                string.IsNullOrEmpty(txtGiaNhap.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin.");
                return;
            }
            
                // Kiểm tra sự trùng lặp của mã thuốc, số lô và ngày hết hạn
                string checkDateSql = "SELECT COUNT(1) FROM dbo.ChiTietPhieuNhap WHERE MaThuoc = @MaThuoc AND SoLo = @SoLo AND NgayHetHan <> @NgayHetHan";

            using (SqlCommand checkDateCmd = new SqlCommand(checkDateSql, conn))
            {
                checkDateCmd.Parameters.AddWithValue("@MaThuoc", txtMaThuoc.Text);
                checkDateCmd.Parameters.AddWithValue("@SoLo", txtSoLo.Text);
                checkDateCmd.Parameters.AddWithValue("@NgayHetHan", dtNHH.Value.ToString("yyyy-MM-dd"));

                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                int dateConflict = (int)checkDateCmd.ExecuteScalar();

                if (dateConflict > 0)
                {
                    MessageBox.Show("Không thể có ngày hết hạn khác nhau cho cùng số lô thuốc của mã thuốc này!", "Lỗi ngày hết hạn", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            if (flag == "add")
            {
                // Kiểm tra nếu giá trị đã tồn tại
                string checkSql = "SELECT COUNT(1) FROM dbo.ChiTietPhieuNhap WHERE MaPhieuNhap = @MaPhieuNhap AND MaThuoc = @MaThuoc AND SoLo = @SoLo";

            using (SqlCommand checkCmd = new SqlCommand(checkSql, conn))
            {
                checkCmd.Parameters.AddWithValue("@MaPhieuNhap", txtmaPN.Text);
                checkCmd.Parameters.AddWithValue("@MaThuoc", txtMaThuoc.Text);
                checkCmd.Parameters.AddWithValue("@SoLo", txtSoLo.Text);

                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                int exists = (int)checkCmd.ExecuteScalar();

                // Nếu bản ghi đã tồn tại, hiển thị thông báo lỗi và dừng lệnh chèn
                if (exists > 0)
                {
                    MessageBox.Show("Lỗi: Giá trị đã tồn tại trong Chi tiết phiếu nhập!", "Lỗi trùng dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            

                string sql = "INSERT INTO dbo.ChiTietPhieuNhap(MaPhieuNhap, MaThuoc, SoLo, NgaySanXuat, NgayHetHan, DonViNhap, slDonViNhap, GiaNhap) " +
                         "VALUES (@MaPhieuNhap, @MaThuoc, @SoLo, @NgaySanXuat, @NgayHetHan, @DonViNhap, @slDonViNhap, @GiaNhap)";

                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    // Gán giá trị cho các tham số
                    cmd.Parameters.AddWithValue("@MaPhieuNhap", txtmaPN.Text);
                    cmd.Parameters.AddWithValue("@MaThuoc", txtMaThuoc.Text);
                    cmd.Parameters.AddWithValue("@SoLo", txtSoLo.Text);

                    // Sử dụng SelectedValue của ComboBox comDV
                    cmd.Parameters.AddWithValue("@DonViNhap", comDV.SelectedValue?.ToString());

                    // Lấy giá trị ngày từ DateTimePicker và chuyển đổi sang định dạng yyyy-MM-dd
                    cmd.Parameters.AddWithValue("@NgaySanXuat", dtNSX.Value.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@NgayHetHan", dtNHH.Value.ToString("yyyy-MM-dd"));

                    // Chuyển đổi slDonViNhap sang kiểu Int
                    int slDonViNhap = int.TryParse(txtslDonViNhap.Text, out slDonViNhap) ? slDonViNhap : 0;
                    cmd.Parameters.AddWithValue("@slDonViNhap", slDonViNhap);

                    // Chuyển đổi GiaNhap sang kiểu decimal
                    decimal giaNhap = decimal.TryParse(txtGiaNhap.Text, out giaNhap) ? giaNhap : 0;
                    cmd.Parameters.AddWithValue("@GiaNhap", giaNhap);

                    // Thực thi câu lệnh và kiểm tra số dòng được chèn
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Thêm chi tiết hóa đơn thành công vào cơ sở dữ liệu!");
                        Naplai();
                        NapLaiCT();
                    }
                    else
                    {
                        MessageBox.Show("Không thể thêm chi tiết hóa đơn vào cơ sở dữ liệu.");
                    }
                }
            }
            else if (flag == "edit")
            {
                // Thực hiện lệnh UPDATE
                string updateSql = "UPDATE dbo.ChiTietPhieuNhap SET NgaySanXuat = @NgaySanXuat, NgayHetHan = @NgayHetHan, DonViNhap = @DonViNhap, slDonViNhap = @slDonViNhap, GiaNhap = @GiaNhap " +
                                   "WHERE MaPhieuNhap = @MaPhieuNhap AND MaThuoc = @MaThuoc AND SoLo = @SoLo";

                using (SqlCommand cmd = new SqlCommand(updateSql, conn))
                {
                    cmd.Parameters.AddWithValue("@MaPhieuNhap", txtmaPN.Text);
                    cmd.Parameters.AddWithValue("@MaThuoc", txtMaThuoc.Text);
                    cmd.Parameters.AddWithValue("@SoLo", txtSoLo.Text);
                    cmd.Parameters.AddWithValue("@DonViNhap", comDV.SelectedValue?.ToString());
                    cmd.Parameters.AddWithValue("@NgaySanXuat", dtNSX.Value.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@NgayHetHan", dtNHH.Value.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@slDonViNhap", int.TryParse(txtslDonViNhap.Text, out int slDonViNhap) ? slDonViNhap : 0);
                    cmd.Parameters.AddWithValue("@GiaNhap", decimal.TryParse(txtGiaNhap.Text, out decimal giaNhap) ? giaNhap : 0);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Cập nhật chi tiết phiếu nhập thành công!");
                        Naplai();
                        NapLaiCT();
                        grdData.Enabled = true;
                        grdCTNhap.Enabled = true;
                    }
                    else
                    {
                        MessageBox.Show("Không thể cập nhật chi tiết phiếu nhập.");
                    }
                }
            }

            // Đóng kết nối sau khi hoàn tất
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }

        private void dtNHH_ValueChanged(object sender, EventArgs e)
        {
            // Lấy giá trị hiện tại từ các TextBox
            string maThuoc = txtMaThuoc.Text;
            string soLo = txtSoLo.Text;
            DateTime ngayHetHanMoi = dtNHH.Value;

            // Kiểm tra các ô MaThuoc và SoLo đã có giá trị hay chưa
            if (!string.IsNullOrEmpty(maThuoc) && !string.IsNullOrEmpty(soLo))
            {
                // Câu truy vấn để kiểm tra sự tồn tại của bản ghi với MaThuoc và SoLo nhưng có NgayHetHan khác nhau
                string sql = "SELECT COUNT(*) FROM ChiTietPhieuNhap " +
                             "WHERE MaThuoc = @MaThuoc AND SoLo = @SoLo AND NgayHetHan != @NgayHetHan";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    // Thêm tham số cho câu truy vấn
                    cmd.Parameters.AddWithValue("@MaThuoc", maThuoc);
                    cmd.Parameters.AddWithValue("@SoLo", soLo);
                    cmd.Parameters.AddWithValue("@NgayHetHan", ngayHetHanMoi);

                    // Mở kết nối nếu nó chưa mở
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }

                    // Thực thi câu lệnh và lấy kết quả
                    int count = (int)cmd.ExecuteScalar();

                    // Kiểm tra nếu tồn tại bản ghi không thỏa mãn điều kiện
                    if (count > 0)
                    {
                        // Hiển thị thông báo lỗi
                        MessageBox.Show("Không thể có ngày hết hạn khác nhau cho cùng số lô thuốc của mã thuốc này!",
                                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        // Đặt lại DateTimePicker để người dùng chọn lại
                        dtNHH.Focus();

                        // Thoát khỏi sự kiện mà không thực hiện các bước tiếp theo
                        return;
                    }
                }
            }
        }

        private void txtslDonViNhap_Leave(object sender, EventArgs e)
        {
            // Kiểm tra xem các ô textbox có trống hay không
            if (!string.IsNullOrEmpty(txtslDonViNhap.Text) && !string.IsNullOrEmpty(txtGiaNhap.Text))
            {
                // Chuyển đổi giá trị của các ô thành kiểu số
                if (decimal.TryParse(txtslDonViNhap.Text, out decimal slDonViNhap) && decimal.TryParse(txtGiaNhap.Text, out decimal giaNhap))
                {
                    // Tính và điền vào ô txtThanhTien
                    decimal thanhTien = slDonViNhap * giaNhap;
                    txtThanhTien.Text = thanhTien.ToString("0.00"); // Định dạng thành số thập phân 2 chữ số
                }
                else
                {
                    MessageBox.Show("Vui lòng nhập đúng định dạng số vào các ô Số lượng và Giá nhập.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // Nếu có ô trống, bạn có thể đặt giá trị mặc định cho txtThanhTien hoặc không làm gì
                txtThanhTien.Clear(); // Xóa ô txtThanhTien nếu có ô trống
            }
        }
        private string flag = "";

        private void btneditCT_Click(object sender, EventArgs e)
        {
            flag = "edit";
            if (grdCTNhap.CurrentRow != null)
            {
                grdData.Enabled = false;
                grdCTNhap.Enabled = false;
                grdT.Enabled = false;
                dtNSX.Visible = true;
                dtNHH.Visible = true;
                txtNgaySanXuat.Visible = false;
                txtNgayHetHan.Visible = false;
                txtMaThuoc.ReadOnly = true;
                txtSoLo.ReadOnly = true;
                txtNgaySanXuat.ReadOnly = false;
                txtNgayHetHan.ReadOnly = false;

                txtslDonViNhap.ReadOnly = false;
                txtGiaNhap.ReadOnly = false;
                // Lấy thông tin từ các bảng ghi đang chọn
                txtThanhTien.ReadOnly = true;
                int i = grdCTNhap.CurrentRow.Index;
                txtMaThuoc.Text = grdCTNhap.Rows[i].Cells["MaThuoc"].Value?.ToString();
                txtSoLo.Text = grdCTNhap.Rows[i].Cells["SoLo"].Value?.ToString();
                txtNgaySanXuat.Text = grdCTNhap.Rows[i].Cells["NgaySanXuat"].Value?.ToString();
                txtNgayHetHan.Text = grdCTNhap.Rows[i].Cells["NgayHetHan"].Value?.ToString();
                txtDonViNhap.Text = grdCTNhap.Rows[i].Cells["DonViNhap"].Value?.ToString();
                txtslDonViNhap.Text = grdCTNhap.Rows[i].Cells["slDonViNhap"].Value?.ToString();
                txtGiaNhap.Text = grdCTNhap.Rows[i].Cells["GiaNhap"].Value?.ToString();
                txtThanhTien.Text = grdCTNhap.Rows[i].Cells["ThanhTien"].Value?.ToString();
                // Ẩn TextBox và hiển thị ComboBox
                txtDonViNhap.Visible = false;
                comDV.Visible = true;
                // Gán dữ liệu từ DataTable vào ComboBox comDV
                DataTable dtcomDV = new DataTable();
                string sql = $"SELECT ql.dvcoso AS DonVi FROM dbo.QL_Thuoc ql WHERE ql.MaThuoc = '{txtMaThuoc.Text}' UNION SELECT qd.DonViQuyDoi AS DonVi FROM dbo.QuyDoiDonVi qd WHERE qd.MaThuoc = '{txtMaThuoc.Text}'";
                daDV = new SqlDataAdapter(sql, conn);
                dtcomDV.Clear();
                daDV.Fill(dtcomDV);

                // Kiểm tra nếu có dữ liệu
                if (dtcomDV.Rows.Count > 0)
                {
                    // Gán dữ liệu vào ComboBox
                    comDV.DataSource = dtcomDV;
                    comDV.DisplayMember = "DonVi";
                    comDV.ValueMember = "DonVi";
                }
                // Gán giá trị từ TextBox cho ComboBox
                {
                    comDV.SelectedValue = txtDonViNhap.Text;
                }
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
                maT = true;
                // Ẩn TextBox và hiển thị ComboBox
                txtDonViNhap.Visible = false;
                comDV.Visible = true;
                // Gán giá trị từ TextBox cho ComboBox
                {
                    comDV.SelectedValue = txtDonViNhap.Text;
                }
                // Cấu hình định dạng hiển thị của DateTimePicker
                dtNSX.Format = DateTimePickerFormat.Custom;
                dtNSX.CustomFormat = "dd-MM-yyyy";
                dtNHH.Format = DateTimePickerFormat.Custom;
                dtNHH.CustomFormat = "dd-MM-yyyy";

                // Gán giá trị từ TextBox cho DateTimePicker
                DateTime ngaySanXuat;
                DateTime ngayHetHan;

                // Thử chuyển đổi giá trị từ TextBox và kiểm tra xem có phải ngày hợp lệ không
                if (DateTime.TryParse(txtNgaySanXuat.Text, out ngaySanXuat))
                {
                    dtNSX.Value = ngaySanXuat;
                }
                else
                {
                    MessageBox.Show("Giá trị Ngày Sản Xuất không hợp lệ.", "Lỗi định dạng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (DateTime.TryParse(txtNgayHetHan.Text, out ngayHetHan))
                {
                    dtNHH.Value = ngayHetHan;
                }
                else
                {
                    MessageBox.Show("Giá trị Ngày Hết Hạn không hợp lệ.", "Lỗi định dạng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                {
                    MessageBox.Show("Vui lòng chọn một bảng ghi để chỉnh sửa.");
                }
            }
            // Kiểm tra kết nối
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
        }


        // Cờ để kiểm soát khi nào dòng hiện tại bị khóa
        private bool isLocked = false;
        private void button5_Click(object sender, EventArgs e)
        {
            flag = "add";
            dtNSX.Visible = true;
            dtNHH.Visible = true;
            txtNgaySanXuat.Visible = false;
            txtNgayHetHan.Visible = false;
            txtMaThuoc.Text = " ";
            txtSoLo.Text = " ";
           
            txtDonViNhap.Text = " ";
            txtslDonViNhap.Text = " ";
            txtGiaNhap.Text = " ";
            txtThanhTien.Text = " ";
            txtMaThuoc.ReadOnly = false;
            txtSoLo.ReadOnly = false;
            txtNgaySanXuat.ReadOnly = false;
            txtNgayHetHan.ReadOnly = false;
            txtDonViNhap.ReadOnly = false;
            txtslDonViNhap.ReadOnly = false;
            txtGiaNhap.ReadOnly = false;
           
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
            maT = true;
            // Ẩn TextBox và hiển thị ComboBox
            txtDonViNhap.Visible = false;
            comDV.Visible = true;

            // Kiểm tra kết nối
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            // Lấy mã thuốc từ `grdT` và điền vào `txtMaThuoc`
            //if (grdT.SelectedRows.Count > 0)
            //{
            //    txtMaThuoc.Text = grdT.SelectedRows[0].Cells["ID"].Value?.ToString();
            //}
            // Kiểm tra nếu txtMaThuoc.Text có giá trị
            if (string.IsNullOrEmpty(txtMaThuoc.Text))
            {
                MessageBox.Show("Vui lòng nhập mã thuốc.");
                return;
            }

            //// Gán dữ liệu từ DataTable vào ComboBox comDV
            //DataTable dtcomDV = new DataTable();
            //string sql = $"SELECT ql.dvcoso AS DonVi FROM dbo.QL_Thuoc ql WHERE ql.MaThuoc = '%{txtMaThuoc.Text}%' UNION SELECT qd.DonViQuyDoi AS DonVi FROM dbo.QuyDoiDonVi qd WHERE qd.MaThuoc = '%{txtMaThuoc.Text }%'";
            //daDV = new SqlDataAdapter(sql, conn);
            //daDV.Fill(dtcomDV);

            //// Kiểm tra nếu có dữ liệu
            //if (dtcomDV.Rows.Count > 0)
            //{
            //    // Gán dữ liệu vào ComboBox
            //    comDV.DataSource = dtcomDV;
            //    comDV.DisplayMember = "DonVi";
            //    comDV.ValueMember = "DonVi";
            //}
            //else
            //{
            //    MessageBox.Show("Không tìm thấy đơn vị nào cho mã thuốc này.");
            //    comDV.DataSource = null;
            //}
           

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
            txtMaThuoc.ReadOnly = true;
            txtSoLo.ReadOnly = true;
            txtNgaySanXuat.ReadOnly = true;
            txtNgayHetHan.ReadOnly = true;
            txtDonViNhap.ReadOnly = true;
            txtslDonViNhap.ReadOnly = true;
            txtGiaNhap.ReadOnly = true;
            txtThanhTien.ReadOnly = true;
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
