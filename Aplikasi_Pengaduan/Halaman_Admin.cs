using iTextSharp.text.pdf;
using iTextSharp.text;
using Org.BouncyCastle.Bcpg.Sig;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aplikasi_Pengaduan
{
    public partial class Halaman_Admin : Form
    {
        public SqlConnection koneksi;
        public Halaman_Admin()
        {
            InitializeComponent();
            koneksi = new SqlConnection("Data Source=HIDAYATULLAH;Initial Catalog=db_pengaduan;Integrated Security=True;");

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void login_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Anda yakin ingin logout?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (result == DialogResult.Yes)
            {
                Halaman_Pengaduan loginForm = new Halaman_Pengaduan();
                loginForm.Show();
                this.Hide();
            }
        }
        private void TampilkanDataSelesai()
        {
            string query = "SELECT id, nama, lokasi, keterangan, foto, status, tanggal FROM tb_pengaduan WHERE status = 'Selesai'";
            SqlCommand cmd = new SqlCommand(query, koneksi);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            foreach (DataRow row in dataTable.Rows)
            {
                if (row["foto"] != DBNull.Value)
                {
                    byte[] imgData = (byte[])row["foto"];
                    System.Drawing.Image originalImage;
                    using (MemoryStream ms = new MemoryStream(imgData))
                    {
                        originalImage = System.Drawing.Image.FromStream(ms);
                    }
                    System.Drawing.Image resizedImage = ResizeImage(originalImage, 50, 50);
                    row["foto"] = ImageToByteArray(resizedImage);
                }
            }
            DataGrid_history.DataSource = dataTable;
        }

        private byte[] ImageToByteArray(System.Drawing.Image image)
        {
            System.Drawing.Image resizedImage = ResizeImage(image, 50, 50);
            using (var ms = new MemoryStream())
            {
                resizedImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }

        private System.Drawing.Image ResizeImage(System.Drawing.Image image, int width, int height)
        {
            Bitmap resizedImage = new Bitmap(width, height);
            using (Graphics graphics = Graphics.FromImage(resizedImage))
            {
                graphics.DrawImage(image, 0, 0, width, height);
            }
            return resizedImage;
        }

        private void TampilkanData()
        {
            string query = "SELECT id, nama, lokasi, keterangan, foto, status, tanggal FROM tb_pengaduan WHERE status = 'Proses' OR status = 'Menunggu'";
            SqlCommand cmd = new SqlCommand(query, koneksi);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            DataGrid_laporan.DataSource = dataTable;
        }
        private void keterangan_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void guna2HtmlLabel5_Click(object sender, EventArgs e)
        {

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

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            DataGrid_laporan.Visible = false;
            DataGrid_history.Visible = true;
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            DataGrid_history.Visible = false;
            DataGrid_laporan.Visible = true;
        }

        private void DataGrid_history_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == DataGrid_history.Columns["nama"].Index && e.RowIndex != -1)
            {
                // Tampilkan dialog konfirmasi
                DialogResult result = MessageBox.Show("Apakah Anda yakin ingin menghapus data ini?", "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                // Jika pengguna mengonfirmasi untuk menghapus
                if (result == DialogResult.Yes)
                {
                    // Ambil id dari baris yang akan dihapus
                    int id = Convert.ToInt32(DataGrid_history.Rows[e.RowIndex].Cells["id"].Value);

                    // Hapus data dari database
                    HapusDataDariDatabase(id);

                    // Hapus baris dari DataGridView Histori
                    DataGrid_history.Rows.RemoveAt(e.RowIndex);
                }
            }
        }
        private void HapusDataDariDatabase(int id)
        {
            try
            {
                koneksi.Open();

                string query = "DELETE FROM tb_pengaduan WHERE id = @id";
                SqlCommand cmd = new SqlCommand(query, koneksi);
                cmd.Parameters.AddWithValue("@id", id);
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Data berhasil dihapus dari database.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Gagal menghapus data dari database.", "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                koneksi.Close();
            }
        }
        private void DataGrid_laporan_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == DataGrid_laporan.Columns["status"].Index && e.RowIndex != -1)
            {
                DialogResult result = MessageBox.Show("Apakah Anda Ingin Mengubah Status?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (result == DialogResult.Yes)
                {
                    int id = Convert.ToInt32(DataGrid_laporan.Rows[e.RowIndex].Cells["id"].Value);
                    string query = "UPDATE tb_pengaduan SET status = 'Proses' WHERE id = @id";
                    SqlCommand cmd = new SqlCommand(query, koneksi);
                    cmd.Parameters.AddWithValue("@id", id);

                    koneksi.Open();
                    cmd.ExecuteNonQuery();
                    koneksi.Close();
                    MessageBox.Show("Status berhasil Diubah.", "Sucses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    TampilkanData();
                }
            }

            if (e.ColumnIndex == DataGrid_laporan.Columns["id"].Index && e.RowIndex != -1)
            {
                int id = Convert.ToInt32(DataGrid_laporan.Rows[e.RowIndex].Cells["id"].Value);
                string query = "SELECT keterangan, foto FROM tb_pengaduan WHERE id = @id";
                SqlCommand cmd = new SqlCommand(query, koneksi);
                cmd.Parameters.AddWithValue("@id", id);

                koneksi.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    keterangan.Text = reader["keterangan"].ToString();

                    if (reader["foto"] != DBNull.Value)
                    {
                        byte[] imgData = (byte[])reader["foto"];
                        MemoryStream ms = new MemoryStream(imgData);
                        // Gunakan System.Drawing.Image untuk menghindari ambiguitas
                        gambar2.Image = System.Drawing.Image.FromStream(ms);
                        gambar2.Visible = true;
                    }
                    else
                    {
                        gambar2.Visible = false;
                    }
                }

                reader.Close();
                koneksi.Close();
            }
        }


        private void guna2Button2_Click(object sender, EventArgs e)
        {
            gambar2.Visible = false;
            keterangan.Text = "";
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

        private void edit_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(DataGrid_laporan.CurrentRow.Cells["id"].Value);
            string query = "UPDATE tb_pengaduan SET keterangan = @keterangan, foto = @foto, status = 'Selesai' WHERE id = @id";
            SqlCommand cmd = new SqlCommand(query, koneksi);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@keterangan", keterangan.Text);
            if (gambar2.Image != null)
            {
                MemoryStream ms = new MemoryStream();
                gambar2.Image.Save(ms, gambar2.Image.RawFormat);
                byte[] imgData = ms.ToArray();
                cmd.Parameters.AddWithValue("@foto", imgData);
            }
            else
            {
                cmd.Parameters.AddWithValue("@foto", DBNull.Value);
            }

            koneksi.Open();
            cmd.ExecuteNonQuery();
            koneksi.Close();

            MessageBox.Show("Data berhasil diperbarui.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
            gambar2.Visible = false;
            keterangan.Text = "";
            TampilkanData();
            TampilkanDataSelesai();
        }

        private void te_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel2_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel10_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel9_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2HtmlLabel6_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void gambar2_Click(object sender, EventArgs e)
        {

        }

        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Halaman_Admin_Load(object sender, EventArgs e)
        {
            guna2ComboBox1.Items.Add("1 Bulan");
            guna2ComboBox1.Items.Add("2 Bulan");
            guna2ComboBox1.Items.Add("3 Bulan");
            guna2ComboBox1.SelectedIndex = 0;

            TampilkanData();
            TampilkanDataSelesai();
        }
        private void rekap_Click(object sender, EventArgs e)
        {
            string selectedDuration = guna2ComboBox1.SelectedItem.ToString();
            int months = 1;
            if (selectedDuration == "2 Bulan") months = 2;
            else if (selectedDuration == "3 Bulan") months = 3;

            DateTime startDate = DateTime.Now.AddMonths(-months);

            string query = "SELECT status, COUNT(*) AS jumlah FROM tb_pengaduan WHERE tanggal >= @startDate GROUP BY status";
            SqlCommand cmd = new SqlCommand(query, koneksi);
            cmd.Parameters.AddWithValue("@startDate", startDate);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF file (*.pdf)|*.pdf";
            saveFileDialog.FileName = "Rekap_Pengaduan.pdf";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 10f);
                    PdfWriter.GetInstance(pdfDoc, new FileStream(saveFileDialog.FileName, FileMode.Create));
                    pdfDoc.Open();

                    pdfDoc.Add(new Paragraph("Rekap Pengaduan"));
                    pdfDoc.Add(new Paragraph("Durasi: " + selectedDuration));
                    pdfDoc.Add(new Paragraph("\n"));

                    foreach (DataRow row in dataTable.Rows)
                    {
                        string status = row["status"].ToString();
                        string jumlah = row["jumlah"].ToString();
                        pdfDoc.Add(new Paragraph($"{status}: {jumlah} pengaduan"));
                    }

                    pdfDoc.Close();
                    MessageBox.Show("PDF berhasil dibuat!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal membuat PDF: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void guna2ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

    }
}
