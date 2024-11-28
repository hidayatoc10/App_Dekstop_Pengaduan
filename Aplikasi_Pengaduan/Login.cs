using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aplikasi_Pengaduan
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void username_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if ((username.Text == "admin") && (password.Text == "Telkomdso123"))
            {
                MessageBox.Show("Login Berhasil, Selamat Datang ADMIN!", "Sucses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                username.Text = "";
                password.Text = "";
                this.Hide();
                Halaman_Admin HalamanAdmin = new Halaman_Admin();
                HalamanAdmin.Show();
            }
            else
            {
                MessageBox.Show("Opss Login Gagal, Coba Ulangi", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void reset_Click(object sender, EventArgs e)
        {
            this.Hide();
            Halaman_Pengaduan Halaman_Pengaduan = new Halaman_Pengaduan();
            Halaman_Pengaduan.Show();
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void password_TextChanged(object sender, EventArgs e)
        {

        }

        private void show_CheckedChanged(object sender, EventArgs e)
        {
            if (show.Checked == true)
            {
                password.UseSystemPasswordChar = false;

            }
            else
            {
                password.UseSystemPasswordChar = true;
            }
        }
    }
}
