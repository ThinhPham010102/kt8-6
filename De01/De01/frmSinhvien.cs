using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace De01
{
    public partial class Form1 : Form
    {
        private string connectionString = "Server=LAPTOP-JMNTL6SS\\SQLEXPRESS;Database=QuanlySV;Trusted_Connection=True;";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadData();
            SetControlState(false);
        }

        private void LoadData()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Sinhvien", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                lvSinhvien.Items.Clear();
                foreach (DataRow row in dt.Rows)
                {
                    ListViewItem item = new ListViewItem(row["MaSV"].ToString());
                    item.SubItems.Add(row["HotenSV"].ToString());
                    item.SubItems.Add(Convert.ToDateTime(row["Ngaysinh"]).ToString("dd/MM/yyyy"));
                    item.SubItems.Add(row["MaLop"].ToString());
                    lvSinhvien.Items.Add(item);
                }

                da = new SqlDataAdapter("SELECT * FROM Lop", conn);
                DataTable dtLop = new DataTable();
                da.Fill(dtLop);
                cboLop.DataSource = dtLop;
                cboLop.DisplayMember = "TenLop";
                cboLop.ValueMember = "MaLop";
            }
        }
        private void SetControlState(bool editing)
        {
            txtMaSV.Enabled = editing;
            txtHotenSV.Enabled = editing;
            dtNgaysinh.Enabled = editing;
            cboLop.Enabled = editing;

            btThem.Enabled = !editing;
            btSua.Enabled = !editing && lvSinhvien.SelectedItems.Count > 0;
            btXoa.Enabled = !editing && lvSinhvien.SelectedItems.Count > 0;
            btLuu.Enabled = editing;
            btKhong.Enabled = editing;
        }

     

        private void ClearControls()
        {
            txtMaSV.Clear();
            txtHotenSV.Clear();
            dtNgaysinh.Value = DateTime.Now;
            cboLop.SelectedIndex = -1;
        }

       

     

     

     

        private void lvSinhvien_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (lvSinhvien.SelectedItems.Count > 0)
            {
                ListViewItem item = lvSinhvien.SelectedItems[0];
                txtMaSV.Text = item.SubItems[0].Text;
                txtHotenSV.Text = item.SubItems[1].Text;
                dtNgaysinh.Value = DateTime.Parse(item.SubItems[2].Text);
                cboLop.SelectedValue = item.SubItems[3].Text;

                btSua.Enabled = true;
                btXoa.Enabled = true;
            }
            else
            {
                btSua.Enabled = false;
                btXoa.Enabled = false;
            }
        }

        private void btThem_Click_1(object sender, EventArgs e)
        {
          
                ClearControls();
               SetControlState(true);
            
        }

        private void btSua_Click_1(object sender, EventArgs e)
        {
            
                SetControlState(true);
            
        }

        private void btXoa_Click_1(object sender, EventArgs e)
        {
           
                if (MessageBox.Show("Are you sure you want to delete this record?", "Confirm Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        string query = "DELETE FROM Sinhvien WHERE MaSV = @MaSV";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@MaSV", txtMaSV.Text);

                        cmd.ExecuteNonQuery();
                    }

                    LoadData();
                    SetControlState(false);
                }
        }

        private void btLuu_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaSV.Text) || string.IsNullOrEmpty(txtHotenSV.Text) || cboLop.SelectedValue == null)
            {
                MessageBox.Show("Please fill all fields before saving.");
                return;
            }

            // Truncate txtMaSV to fit the database field size (assuming MaSV is 6 characters)
            string truncatedMaSV = txtMaSV.Text.Length > 6 ? txtMaSV.Text.Substring(0, 6) : txtMaSV.Text;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Kiểm tra xem MaSV đã tồn tại chưa
                string checkQuery = "SELECT COUNT(*) FROM Sinhvien WHERE MaSV = @MaSV";
                SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@MaSV", truncatedMaSV);
                int count = (int)checkCmd.ExecuteScalar();

                if (count > 0)
                {
                    MessageBox.Show("MaSV already exists. Please enter a unique MaSV.");
                    return;
                }

                // Thêm bản ghi mới nếu MaSV không tồn tại
                string query = "INSERT INTO Sinhvien (MaSV, HotenSV, Ngaysinh, MaLop) VALUES (@MaSV, @HotenSV, @Ngaysinh, @MaLop)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaSV", truncatedMaSV);
                cmd.Parameters.AddWithValue("@HotenSV", txtHotenSV.Text);
                cmd.Parameters.AddWithValue("@Ngaysinh", dtNgaysinh.Value);
                cmd.Parameters.AddWithValue("@MaLop", cboLop.SelectedValue.ToString());

                cmd.ExecuteNonQuery();
            }

            LoadData();
            SetControlState(false);
        }
        private void btKhong_Click_1(object sender, EventArgs e)
        {
          
                SetControlState(false);
            
        }

        private void btThoat_Click(object sender, EventArgs e)
        {
            // Hiển thị hộp thoại xác nhận khi người dùng nhấn nút Thoát
            DialogResult result = MessageBox.Show("Bạn có muốn thoát không?", "Xác nhận thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Nếu người dùng chọn Yes, đóng ứng dụng
                Application.Exit();
            }
            // Nếu người dùng chọn No, không làm gì cả (ứng dụng sẽ tiếp tục chạy)
        }
    }
    }

