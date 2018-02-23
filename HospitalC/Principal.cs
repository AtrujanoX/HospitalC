using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.WebSockets;
using MySql.Data.MySqlClient;

namespace Hospital {
    public partial class FormPrincipal : Form {

        MySqlConnectionStringBuilder connString = new MySqlConnectionStringBuilder();
        MySqlConnection conn;
        string nombre, a_paterno, a_materno, llamada;

        public FormPrincipal() {
            InitializeComponent();
        }

        private string b = "";
        private int numCama = Properties.Settings.Default.numCama;

        private int idPaciente;

        private void timer1_Tick(object sender, EventArgs e) {
            int hh = DateTime.Now.Hour % 12;
            int pm = DateTime.Now.Hour > 12 ? 1 : 0;
            int mm = DateTime.Now.Minute;
            int ss = DateTime.Now.Second;
            DayOfWeek ww = DateTime.Now.DayOfWeek;
            string diaSem = "";
            int DD = DateTime.Now.Day;
            int MM = DateTime.Now.Month;
            string mes = "";
            int AA = DateTime.Now.Year;
            switch (ww) {
                case DayOfWeek.Monday: diaSem = "LUN"; break;
                case DayOfWeek.Tuesday: diaSem = "MAR"; break;
                case DayOfWeek.Wednesday: diaSem = "MIÉ"; break;
                case DayOfWeek.Thursday: diaSem = "JUE"; break;
                case DayOfWeek.Friday: diaSem = "VIE"; break;
                case DayOfWeek.Saturday: diaSem = "SÁB"; break;
                case DayOfWeek.Sunday: diaSem = "DOM"; break;
            }
            switch (MM) {
                case 1: mes = "ENE"; break;
                case 2: mes = "FEB"; break;
                case 3: mes = "MAR"; break;
                case 4: mes = "ABR"; break;
                case 5: mes = "MAY"; break;
                case 6: mes = "JUN"; break;
                case 7: mes = "JUL"; break;
                case 8: mes = "AGO"; break;
                case 9: mes = "SEP"; break;
                case 10: mes = "OCT"; break;
                case 11: mes = "NOV"; break;
                case 12: mes = "DIC"; break;

            }

            labelHora.Text = (hh < 10 && hh != 0 ? "0" : "") + (hh == 0 ? 12 : hh) + ":" + (mm < 10 ? "0" : "") + mm + (pm == 1 ? " p. m." : " a. m.");
            labelFecha.Text = diaSem + "., " + mes + ". " + DD + ", " + AA;
        }

        private void buttonAlerta_Click(object sender, EventArgs e) {
            buttonAlerta.BackgroundImage = Properties.Resources.btnAlerta_s;
            buttonEnfermera.BackgroundImage = Properties.Resources.btnEnfermera;
            DBQuery("UPDATE paciente SET llamada='1' WHERE id_paciente='" + idPaciente + "'");
            buttonCancelar.Visible = true;
        }
        private void buttonEnfermera_Click(object sender, EventArgs e) {
            buttonAlerta.BackgroundImage = Properties.Resources.btnAlerta;
            buttonEnfermera.BackgroundImage = Properties.Resources.btnEnfermera_s;
            DBQuery("UPDATE paciente SET llamada='2' WHERE id_paciente='" + idPaciente + "'");
            buttonCancelar.Visible = true;
        }
        private void buttonCancelar_Click(object sender, EventArgs e) {
            buttonAlerta.BackgroundImage = Properties.Resources.btnAlerta;
            buttonEnfermera.BackgroundImage = Properties.Resources.btnEnfermera;
            DBQuery("UPDATE paciente SET llamada='3' WHERE id_paciente='" + idPaciente + "'");
            buttonCancelar.Visible = false;
        }

        private void cambiarSelectMenu() {

        }

        private void buttonMenu_Click(object sender, EventArgs e) {
            Button v = (Button)sender;
            buttonCocina.BackgroundImage = Properties.Resources.btnCocina;
            buttonCunero.BackgroundImage = Properties.Resources.btnCunero;
            buttonMantto.BackgroundImage = Properties.Resources.btnMantto;
            buttonMedicamentos.BackgroundImage = Properties.Resources.btnMedicamentos;
            buttonMedico.BackgroundImage = Properties.Resources.btnMedico;
            switch (v.Name.ToString()) {
                case "buttonHome":
                    //buttonHome.BackgroundImage = Properties.Resources.btnCocina_s;
                    panelPrincipal.Visible = true;
                    panelCocina.Visible = panelCunero.Visible = panelMantto.Visible = panelMedicamentos.Visible = panelMedico.Visible = false;
                    break;
                case "buttonCocina":
                    buttonCocina.BackgroundImage = Properties.Resources.btnCocina_s;
                    panelCocina.Visible = true;
                    panelPrincipal.Visible = panelCunero.Visible = panelMantto.Visible = panelMedicamentos.Visible = panelMedico.Visible = false;
                    panelCocina_setup();
                    break;
                case "buttonCunero":
                    buttonCunero.BackgroundImage = Properties.Resources.btnCunero_s;
                    panelPrincipal.Visible = panelCocina.Visible = panelMantto.Visible = panelMedicamentos.Visible = panelMedico.Visible = false;
                    panelCunero.Visible = true;
                    break;
                case "buttonMantto":
                    buttonMantto.BackgroundImage = Properties.Resources.btnMantto_s;
                    if (!panelMantto.Visible) {
                        panelMantenimiento_pin.Visible = !panelMantenimiento_pin.Visible;
                    }
                    break;
                case "buttonMedicamentos":
                    buttonMedicamentos.BackgroundImage = Properties.Resources.btnMedicamentos_s;
                    panelPrincipal.Visible = panelCocina.Visible = panelCunero.Visible = panelMantto.Visible = panelMedico.Visible = false;
                    panelMedicamentos.Visible = false;
                    break;
                case "buttonMedico":
                    buttonMedico.BackgroundImage = Properties.Resources.btnMedico_s;
                    panelPrincipal.Visible = panelCocina.Visible = panelCunero.Visible = panelMantto.Visible = panelMedicamentos.Visible = false;
                    panelMedico.Visible = true;
                    break;
            }

        }


        //PANEL PIN
        private void buttonBorrarPin_Click(object sender, EventArgs e) {
            b = "";
        }
        private void buttonAceptarPin_Click(object sender, EventArgs e) {
            if (b == Properties.Settings.Default.pinSeguridad) {
                panelMantto.Visible = true;
                panelPrincipal.Visible = panelCocina.Visible = panelCunero.Visible = panelMedicamentos.Visible = panelMedico.Visible = false;
            }
            else {
                panelPrincipal.Visible = true;
                buttonCocina.BackgroundImage = Properties.Resources.btnCocina;
                buttonCunero.BackgroundImage = Properties.Resources.btnCunero;
                buttonMantto.BackgroundImage = Properties.Resources.btnMantto;
                buttonMedicamentos.BackgroundImage = Properties.Resources.btnMedicamentos;
                buttonMedico.BackgroundImage = Properties.Resources.btnMedico;
            }
            panelMantenimiento_pin.Visible = false;
            b = "";
        }
        private void btnNumero_Click(object sender, EventArgs e) {
            Button a = (Button)sender;
            b += a.Text;
        }

        private void numCamaChange_Click(object sender, EventArgs e) {
            if (((Button)sender).Text == "<") {
                if (numCama != 1) {
                    numCama--;
                }
            }
            else {
                numCama++;
            }
            labelNumCama.Text = numCama.ToString();
            Properties.Settings.Default.numCama = numCama;
            Properties.Settings.Default.Save();
            FormPrincipal_Refresh(null, null);
        }

        private void panelCocina_setup() {
            conn.Open();
            string query = "SELECT * FROM cocina WHERE id_paciente=" + idPaciente;
            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            flowLayoutPanel1.Controls.Clear();
            flowLayoutPanel2.Controls.Clear();
            flowLayoutPanel3.Controls.Clear();

            while (dataReader.Read()) {
                RadioButton c1 = new RadioButton();
                switch (int.Parse(dataReader["tipo_comida"].ToString())) {
                    case 0:
                        c1.Parent = flowLayoutPanel1;

                        break;
                    case 1:
                        c1.Parent = flowLayoutPanel2;
                        break;
                    case 2:
                        c1.Parent = flowLayoutPanel3;
                        break;
                }
                c1.Font = new Font("Microsoft Sans Serif", 10f);
                c1.ForeColor = SystemColors.Control;
                c1.Text = dataReader["menu"].ToString();
            }

            dataReader.Close();
            conn.Close();
        }

        private void label4_Click(object sender, EventArgs e) {

        }

        private List<List<string>> readData(string query) {
            MySqlConnection conn = new MySqlConnection(connString.ConnectionString);
            conn.Open();
            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            cmd = new MySqlCommand(query, conn);
            dataReader = cmd.ExecuteReader();
            List<List<string>> r = new List<List<string>>();
            int j = 0;
            while (dataReader.Read()) {
                r.Add(new List<string>());
                for (int i = 0; i < dataReader.FieldCount; i++) {
                    r[j].Add(dataReader[i].ToString());
                }

            }
            dataReader.Close();
            conn.Close();
            return r;
        }

        private void FormPrincipal_Refresh(object sender, EventArgs e) {
            labelNumCama.Text = numCama.ToString();
            //BUSCANDO ID_PACIENTE EN ID_CAMA
            connString.Server = "www.madraccoonstudio.com";
            connString.Password = "Lordsith78";
            connString.Database = "madracco_medico";
            connString.UserID = "madracco_admin";
            conn = new MySqlConnection(connString.ConnectionString);
            conn.Open();
            string query = "SELECT id_paciente FROM cama WHERE id_cama=" + numCama;
            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            dataReader.Read();
            if (dataReader.HasRows)
                idPaciente = int.Parse(dataReader["id_paciente"].ToString());
            else idPaciente = 0;
            dataReader.Close();
            //LEYENDO DATOS DE ID_PACIENTE
            query = "SELECT * FROM paciente WHERE id_paciente=" + idPaciente;
            cmd = new MySqlCommand(query, conn);
            dataReader = cmd.ExecuteReader();
            dataReader.Read();
            if (dataReader.HasRows) {
                nombre = dataReader["nombre"].ToString();
                a_paterno = dataReader["a_paterno"].ToString();
                a_materno = dataReader["a_materno"].ToString();
                labelNombre.Text = nombre + " " + a_paterno + " " + a_materno;
            }
            else labelNombre.Text = "Paciente no asignado";
            //labelDebug.Text = id_paciente + "";
            dataReader.Close();
            conn.Close();
        }

        async void DBQuery(string query) {
            conn.Open();
            MySqlCommand cmd = new MySqlCommand(query, conn);
            int x = await cmd.ExecuteNonQueryAsync();
            conn.Close();
        }
    }
}
