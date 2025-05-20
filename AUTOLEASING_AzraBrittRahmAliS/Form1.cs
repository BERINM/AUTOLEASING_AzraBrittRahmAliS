using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AUTOLEASING_AzraBrittRahmAliS
{
    public partial class Form1 : Form
    {
      
        public Form1()
        {
            InitializeComponent();
        }
      
        private void Form1_Load(object sender, EventArgs e)
        {
            tabControl1.Appearance = TabAppearance.FlatButtons;

            tabControl1.ItemSize = new Size(0, 1);

            tabControl1.SizeMode = TabSizeMode.Fixed;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 2;
        }

        private void buttonSTARTJetzt_REGRI_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
        }

        private void buttonLOGIN_Click(object sender, EventArgs e)
        {

            string email = textBoxloginEMAIL.Text;
            string password = textBoxLOGINpasswort.Text;
            if (string.IsNullOrWhiteSpace(textBoxloginEMAIL.Text) || string.IsNullOrWhiteSpace(textBoxLOGINpasswort.Text))
            {
                MessageBox.Show("Bitte geben Sie Email und Passwort ein", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string connectionString = "Server=localhost;Database=Autoleasing_MySQLABRA;Uid=root;Pwd=123Schule123-;";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT K_ID FROM Kunde WHERE Email = @Email AND Passwort = @Passwort";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Email", textBoxloginEMAIL.Text);
                    cmd.Parameters.AddWithValue("@Passwort", textBoxLOGINpasswort.Text);
                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        int kundenId = Convert.ToInt32(result);
                        tabControl1.SelectedTab = tabPage4;
                        LoadCustomerData(kundenId);
                    }
                    else
                    {
                        MessageBox.Show("Falsche E-Mail oder Passwort!", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler bei der Verbindung: " + ex.Message, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string email = textBoxloginEMAIL.Text;
            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Bitte geben Sie zuerst eine Email-Adresse ein.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!CheckEmailExists(email))
            {
                MessageBox.Show("Diese Email-Adresse ist nicht registriert.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // UI anpassen
            textBoxloginEMAIL.Enabled = false;
            textBoxLOGINpasswort.Visible = false;
            buttonLOGIN.Visible = false;
            linkLabel1.Visible = false;
            label5passwortlogin.Visible = false;
            label6emaillogin.Visible = false;
            textBoxNewPassword.Visible = false;
            textBoxConfirmPassword.Visible = false;
            textBoxloginEMAIL.Visible = false;

            labelVerification.Visible = true;
            textBoxVerificationCode.Visible = true;
            buttonVerifyCode.Visible = true;

            GenerateVerificationCode();

        }
        private bool ContainsNumbers(string input) => System.Text.RegularExpressions.Regex.IsMatch(input, @"\d");
        private bool IsNumeric(string input) => System.Text.RegularExpressions.Regex.IsMatch(input, @"^[0-9 +]+$");

        private void button2REG_Click(object sender, EventArgs e)
        {
            if (!ValidateRegistrationData()) return;

            DateTime geburtsdatum = dateTimePickerGeburtsdatumREG.Value;
            if (DateTime.Now.Year - geburtsdatum.Year < 18 ||
                (DateTime.Now.Year - geburtsdatum.Year == 18 && DateTime.Now.DayOfYear < geburtsdatum.DayOfYear))
            {
                MessageBox.Show("Sie müssen mindestens 18 Jahre alt sein.", "Altersbeschränkung", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string connectionString = "Server=localhost;Database=Autoleasing_MySQLABRA;Uid=root;Pwd=123Schule123-;";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"INSERT INTO Kunde (Vorname, Nachname, Adresse, Email, Geburtsdatum, Geschlecht, Telefonnummer, Passwort) 
                                    VALUES (@Vorname, @Nachname, @Adresse, @Email, @Geburtsdatum, @Geschlecht, @Telefonnummer, @Passwort)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Vorname", textBoxVorNREG.Text);
                    cmd.Parameters.AddWithValue("@Nachname", textBoxNachNREG.Text);
                    cmd.Parameters.AddWithValue("@Adresse", textBoxAdresseREG.Text);
                    cmd.Parameters.AddWithValue("@Email", textBoxEMAIlREG.Text);
                    cmd.Parameters.AddWithValue("@Geburtsdatum", geburtsdatum);
                    cmd.Parameters.AddWithValue("@Geschlecht", GetSelectedGender());
                    cmd.Parameters.AddWithValue("@Telefonnummer", textBoxTelefonummerREG.Text);
                    cmd.Parameters.AddWithValue("@Passwort", textBox3PasswortEREG.Text);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        cmd.CommandText = "SELECT LAST_INSERT_ID()";
                        int kundenId = Convert.ToInt32(cmd.ExecuteScalar());
                        tabControl1.SelectedTab = tabPage4;
                        LoadCustomerData(kundenId);
                    }
                }
                catch (MySqlException ex)
                {
                    if (ex.Number == 1062)
                    {
                        MessageBox.Show("Diese Email-Adresse ist bereits registriert.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show("Fehler bei der Registrierung: " + ex.Message, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private bool ValidateRegistrationData()
        {
            if (string.IsNullOrWhiteSpace(textBoxVorNREG.Text) ||
                string.IsNullOrWhiteSpace(textBoxNachNREG.Text) ||
                string.IsNullOrWhiteSpace(textBoxAdresseREG.Text) ||
                string.IsNullOrWhiteSpace(textBoxEMAIlREG.Text) ||
                string.IsNullOrWhiteSpace(textBox3PasswortEREG.Text))
            {
                MessageBox.Show("Bitte füllen Sie alle Pflichtfelder aus.", "Fehlende Daten", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (ContainsNumbers(textBoxVorNREG.Text) || ContainsNumbers(textBoxNachNREG.Text))
            {
                MessageBox.Show("Vor- und Nachname dürfen keine Zahlen enthalten.", "Ungültige Eingabe", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            string adresse = textBoxAdresseREG.Text;
            if (!adresse.Contains(" ") || !adresse.Contains(","))
        {
            tabControl1.SelectedIndex = 3;
        }

            if (!textBoxEMAIlREG.Text.Contains("@") || !textBoxEMAIlREG.Text.Contains("."))
            {
                MessageBox.Show("Geben Sie eine gültige E-Mail-Adresse ein.", "Ungültige E-Mail", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!IsNumeric(textBoxTelefonummerREG.Text))
        {
            tabControl1.SelectedIndex = 1;
        }
        private void button4_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 3;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (radioButtonMaennlichREG.Checked) return "m";
            if (radioButtonWeiblichREG.Checked) return "w";
            if (radioButtonDIversREG.Checked) return "d";
            return "";
        }




        private bool CheckEmailExists(string email)
        {
            string connectionString = "Server=localhost;Database=Autoleasing_MySQLABRA;Uid=root;Pwd=123Schule123-;";
            using (var conn = new MySqlConnection(connectionString))
        {
                conn.Open();
                var cmd = new MySqlCommand("SELECT COUNT(*) FROM Kunde WHERE Email = @Email", conn);
                cmd.Parameters.AddWithValue("@Email", email);
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }

        private void button3GEN_Click(object sender, EventArgs e)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            Random random = new Random();
            char[] code = new char[6];
            for (int i = 0; i < 6; i++)
                code[i] = digits[random.Next(digits.Length)];
            verificationCode = new string(code);
            MessageBox.Show($"Ihr Verifikationscode: {verificationCode}", "Sicherheitscode", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void buttonVerifyCode_Click(object sender, EventArgs e)
        {
            if (textBoxVerificationCode.Text == verificationCode)
            {
                labelVerification.Visible = false;
                textBoxVerificationCode.Visible = false;
                buttonVerifyCode.Visible = false;
                textBoxLOGINpasswort.Clear();
                textBoxNewPassword.Clear();
                labelNewPassword.Visible = true;
                textBoxNewPassword.Visible = true;
                labelConfirmPassword.Visible = true;
                textBoxConfirmPassword.Visible = true;
                buttonUpdatePassword.Visible = true;
                textBoxNewPassword.Focus();
            }
            else
            {
                MessageBox.Show("Ungültiger Code. Bitte versuchen Sie es erneut.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void buttonUpdatePassword_Click(object sender, EventArgs e)
        {
            if (textBoxNewPassword.Text.Length != 8)
            {
                MessageBox.Show("Das Passwort muss genau 8 Zeichen lang sein.", "Ungültiges Passwort", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (textBoxNewPassword.Text != textBoxConfirmPassword.Text)
            {
                MessageBox.Show("Die Passwörter stimmen nicht überein.", "Passwortfehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                string email = textBoxloginEMAIL.Text;
                string newPassword = textBoxNewPassword.Text;

                UpdatePasswordInDatabase(email, newPassword);
                MessageBox.Show("Passwort wurde erfolgreich aktualisiert!", "Erfolg", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ResetLoginForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Aktualisieren des Passworts: " + ex.Message, "Datenbankfehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private void UpdatePasswordInDatabase(string email, string newPassword)
        {
            try
            {
                // Passwort hashen
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

                string connectionString = "Server=localhost;Database=Autoleasing_MySQLABRA;Uid=root;Pwd=123Schule123-;";
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "UPDATE Kunde SET Passwort = @Password WHERE Email = @Email";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Password", hashedPassword);
                        cmd.Parameters.AddWithValue("@Email", email);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            MessageBox.Show("Kein Benutzer mit dieser E-Mail gefunden.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            MessageBox.Show("Passwort erfolgreich geändert!", "Erfolg", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Ändern des Passworts: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ResetLoginForm()
        {
            // Alle Felder zurücksetzen
            textBoxloginEMAIL.Clear();
            textBoxLOGINpasswort.Clear();
            textBoxVerificationCode.Clear();
            textBoxNewPassword.Clear();
            textBoxConfirmPassword.Clear();
            verificationCode = string.Empty;

            // Ursprüngliche Login-UI wiederherstellen
           

            // Passwort-Reset-Elemente ausblenden
            labelVerification.Visible = false;
            textBoxVerificationCode.Visible = false;
            buttonVerifyCode.Visible = false;
            labelNewPassword.Visible = false;
            textBoxNewPassword.Visible = false;
            labelConfirmPassword.Visible = false;
            textBoxConfirmPassword.Visible = false;
            buttonUpdatePassword.Visible = false;
            textBoxloginEMAIL.Visible = true;
            textBoxLOGINpasswort.Visible = true;
        }
        private void LoadCustomerData(int kundenId)
        {
            string connectionString = "Server=localhost;Database=Autoleasing_MySQLABRA;Uid=root;Pwd=123Schule123-;";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Kunde WHERE K_ID = @K_ID";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@K_ID", kundenId);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        labelKundenID.Text = $"Kunden-ID: {kundenId}";
                        richTextBox1.Text = $@"Kundendaten:
Vorname: {reader["Vorname"]}
Nachname: {reader["Nachname"]}
Geburtsdatum: {Convert.ToDateTime(reader["Geburtsdatum"]).ToShortDateString()}
Geschlecht: {GetGeschlechtText(reader["Geschlecht"].ToString())}
Adresse: {reader["Adresse"]}
E-Mail: {reader["Email"]}
Telefon: {reader["Telefonnummer"]}";

                        textBoxVORNACCOUNT.Text = reader["Vorname"].ToString();
                        textBoxNACHNACCOUNT.Text = reader["Nachname"].ToString();
                        textBoxADRESSEACCOUNT.Text = reader["Adresse"].ToString();
                        textBoxEMAILACCOUNT.Text = reader["Email"].ToString();
                        textBoxTELEFONACCOUNT.Text = reader["Telefonnummer"].ToString();
                        textBoxPASSWORTACCOUNT.Text = reader["Passwort"].ToString();

                        if (DateTime.TryParse(reader["Geburtsdatum"].ToString(), out DateTime geburtsdatum))
                            dateTimePickerACCOUNT.Value = geburtsdatum;

                        string geschlecht = reader["Geschlecht"].ToString().ToLower();
                        radioButtonMaennlichACC.Checked = geschlecht == "m";
                        radioButtonWeiblichACC.Checked = geschlecht == "w";
                        radioButtonDiversACC.Checked = geschlecht == "d";
                    }
                }
            }
         }
        private string GetGeschlechtText(string geschlecht)
        {
            switch (geschlecht.ToLower())
            {
                case "m": return "Männlich";
                case "w": return "Weiblich";
                case "d": return "Divers";
                default: return "Unbekannt";
            }
        }

       
        private void buttonTogglePassword_Click(object sender, EventArgs e)
        {
            textBoxNewPassword.PasswordChar = textBoxLOGINpasswort.PasswordChar == '•' ? '\0' : '•';
            textBoxConfirmPassword.PasswordChar = textBoxConfirmPassword.PasswordChar == '•' ? '\0' : '•';
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            
        }

            for (int i = 0; i < 8; i++)
            {
                chars[i] = validChars[random.Next(validChars.Length)];
            }

        }

        }
    }
}
