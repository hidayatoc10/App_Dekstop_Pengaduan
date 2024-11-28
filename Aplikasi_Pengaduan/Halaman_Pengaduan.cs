using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using Mysqlx.Expr;
using Guna.UI2.WinForms;
using System.IO;

namespace Aplikasi_Pengaduan
{
    public partial class Halaman_Pengaduan : Form
    {
        public SqlConnection koneksi;
        public Halaman_Pengaduan()
        {
            InitializeComponent();
            koneksi = new SqlConnection("Data Source=HIDAYATULLAH;Initial Catalog=db_pengaduan;Integrated Security=True;");
        }
        private void TampilkanData()
        {
            string query = "SELECT id, nama, lokasi, keterangan, foto, status, tanggal FROM tb_pengaduan WHERE status = 'Proses' OR status = 'Menunggu'";
            SqlCommand cmd = new SqlCommand(query, koneksi);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            foreach (DataRow row in dataTable.Rows)
            {
                string nama = row["nama"].ToString();
                string keterangan = row["keterangan"].ToString();
                row["nama"] = MaskNama(nama);
                row["keterangan"] = MaskKeterangan(keterangan);
            }

            DataGrid_laporan.DataSource = dataTable;
        }

        private string MaskNama(string nama)
        {
            if (nama.Length <= 3)
            {
                return nama;
            }
            else
            {
                string namaSamaran = nama.Substring(0, 1) + new string('*', nama.Length - 2) + nama.Substring(nama.Length - 1);
                return namaSamaran;
            }
        }

        private string MaskKeterangan(string keterangan)
        {
            if (keterangan.Length <= 3)
            {
                return keterangan;
            }
            else
            {
                string namaSamaran = keterangan.Substring(0, 1) + new string('*', keterangan.Length - 2) + keterangan.Substring(keterangan.Length - 1);
                return namaSamaran;
            }
        }

        private void TampilkanDataSelesai()
        {
            string query = "SELECT id, nama, lokasi, keterangan, foto, status, tanggal FROM tb_pengaduan WHERE status = 'Selesai' ";
            SqlCommand cmd = new SqlCommand(query, koneksi);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            foreach (DataRow row in dataTable.Rows)
            {
                string nama = row["nama"].ToString();
                row["nama"] = MaskNama(nama);
            }
            foreach (DataRow row in dataTable.Rows)
            {
                if (row["foto"] != DBNull.Value)
                {
                    byte[] imgData = (byte[])row["foto"];
                    Image originalImage;
                    using (MemoryStream ms = new MemoryStream(imgData))
                    {
                        originalImage = Image.FromStream(ms);
                    }
                    Image resizedImage = ResizeImage(originalImage, 50, 50);
                    row["foto"] = ImageToByteArray(resizedImage);
                }
            }

            DataGrid_history.DataSource = dataTable;
        }

        private void DataGrid_laporan_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void tabel_laporan_Click(object sender, EventArgs e)
        {
            DataGrid_history.Visible = false;
        }

        private void tabel_history_Click(object sender, EventArgs e)
        {
            DataGrid_laporan.Visible = false;
        }

        private void DataGrid_history_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            DataGrid_history.Visible = false;
            DataGrid_laporan.Visible = true;

        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            DataGrid_laporan.Visible = false;
            DataGrid_history.Visible = true;

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void tambah_Click(object sender, EventArgs e)
        {
            string namaValue = nama.Text.Trim();
            string lokasiValue = lokasi.Text.Trim();
            string keteranganValue = keterangan.Text.Trim();
            if (string.IsNullOrEmpty(namaValue) || string.IsNullOrEmpty(lokasiValue) || string.IsNullOrEmpty(keteranganValue) || pictureBox1.Image == null)
            {
                MessageBox.Show("Harap isi semua informasi.");
                return;
            }
            try
            {
                koneksi.Open();
                string query = "INSERT INTO tb_pengaduan (nama, lokasi, keterangan, foto, status) VALUES (@nama, @lokasi, @keterangan, @foto, 'Menunggu')";
                SqlCommand cmd = new SqlCommand(query, koneksi);
                cmd.Parameters.AddWithValue("@nama", namaValue);
                cmd.Parameters.AddWithValue("@lokasi", lokasiValue);
                cmd.Parameters.AddWithValue("@keterangan", keteranganValue);
                byte[] imageBytes = ImageToByteArray(gambar2.Image);
                cmd.Parameters.AddWithValue("@foto", imageBytes);
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Data berhasil ditambahkan.", "Sucses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    nama.Text = "";
                    lokasi.Text = "";
                    gambar2.Visible = false;
                    keterangan.Text = "";
                    TampilkanData();
                    TampilkanDataSelesai();
                }
                else
                {
                    MessageBox.Show("Gagal menambahkan data.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan: " + ex.Message);
            }
            finally
            {
                koneksi.Close();
            }
        }

        private byte[] ImageToByteArray(Image image)
        {
            Image resizedImage = ResizeImage(image, 50, 50);
            using (var ms = new System.IO.MemoryStream())
            {
                resizedImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }

        private Image ResizeImage(Image image, int width, int height)
        {
            Bitmap resizedImage = new Bitmap(width, height);
            using (Graphics graphics = Graphics.FromImage(resizedImage))
            {
                graphics.DrawImage(image, 0, 0, width, height);
            }

            return resizedImage;
        }


        private void guna2Button2_Click(object sender, EventArgs e)
        {
            gambar2.Visible = false;
            nama.Text = "";
            lokasi.Text = "";
            keterangan.Text = "";
        }

        private void upload_foto_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "File Gambar (*.jpg;*.jpeg;*.gif;*.bmp;*.png)|*.jpg;*.jpeg;*.gif;*.bmp;*.png";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string filePath = openFileDialog.FileName;
                    gambar2.Image = new Bitmap(filePath);

                    MessageBox.Show("Foto berhasil diunggah!", "Sucses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Terjadi kesalahan:", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void nama_TextChanged(object sender, EventArgs e)
        {

        }

        private void lokasi_TextChanged(object sender, EventArgs e)
        {

        }

        private void keterangan_TextChanged(object sender, EventArgs e)
        {

        }
        private void SearchLaporan(string keyword)
        {
            string query = "SELECT * FROM tb_pengaduan WHERE (id LIKE @keyword OR nama LIKE @keyword OR lokasi LIKE @keyword OR keterangan LIKE @keyword OR status LIKE @keyword)";
            SqlCommand cmd = new SqlCommand(query, koneksi);
            cmd.Parameters.AddWithValue("@keyword", "%" + keyword + "%");
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            DataGrid_laporan.DataSource = dataTable;
        }
        private void SearchHistori(string keyword)
        {
            string query = "SELECT * FROM tb_pengaduan WHERE (id LIKE @keyword OR nama LIKE @keyword OR lokasi LIKE @keyword OR keterangan LIKE @keyword OR status LIKE @keyword)";
            SqlCommand cmd = new SqlCommand(query, koneksi);
            cmd.Parameters.AddWithValue("@keyword", "%" + keyword + "%");
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            DataGrid_history.DataSource = dataTable;
        }
        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {
            string searchText = guna2TextBox1.Text.Trim();
            SearchLaporan(searchText);
            SearchHistori(searchText);

        }

        private void Halaman_Pengaduan_Load(object sender, EventArgs e)
        {
            TampilkanData();
            TampilkanDataSelesai();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void gambar2_Click(object sender, EventArgs e)
        {

        }

        private void guna2ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void keterangan_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void login_Click(object sender, EventArgs e)
        {
            this.Hide();
            Login login = new Login();
            login.Show();
        }

        private void guna2HtmlLabel3_Click(object sender, EventArgs e)
        {

        }
    }
}
