﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace AUTOLEASING_AzraBrittRahmAliS
{
    public partial class Form1 : Form
    {
        private string verificationCode;

        public Form1()
        {
            InitializeComponent();
            InitializeUI();
            LoadFahrzeugeData();
            LoadFahrzeugComboBox();
            dateTimePickerAnfang.MinDate = DateTime.Today;
            dateTimePickerEnde.MinDate = DateTime.Today.AddDays(1);

        }
        //private void LoadFahrzeugComboBox()
        //{
        //    string connectionString = "Server=localhost;Port=3307;Database=Autoleasing_MySQLABRA;Uid=root;Pwd=123Schule123;";
        //    using (MySqlConnection conn = new MySqlConnection(connectionString))
        //    {
        //        try
        //        {
        //            conn.Open();
        //            string query = "SELECT F_ID, CONCAT(Hersteller, ' ', Modell) AS FahrzeugInfo, Bildpfad FROM Fahrzeug";
        //            MySqlCommand cmd = new MySqlCommand(query, conn);
        //            MySqlDataReader reader = cmd.ExecuteReader();

        //            comboboxFahrzeug.Items.Clear();
        //            while (reader.Read())
        //            {
        //                comboboxFahrzeug.Items.Add(new
        //                {
        //                    F_ID = Convert.ToInt32(reader["F_ID"]),
        //                    DisplayText = reader["FahrzeugInfo"].ToString(),
        //                    Bildpfad = reader["Bildpfad"]?.ToString() ?? $"{reader["Hersteller"].ToString().ToLower()}_{reader["Modell"].ToString().ToLower().Replace(" ", "_")}.png"
        //                });
        //            }

        //            comboboxFahrzeug.DisplayMember = "DisplayText";
        //            comboboxFahrzeug.ValueMember = "F_ID";
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show("Fehler beim Laden: " + ex.Message);
        //        }
        //    }

        //}

        private int currentFahrzeugId = -1;
        private void LoadFahrzeugComboBox()
        {
            string connectionString = "Server=localhost;Port=3306;Database=Autoleasing_MySQLABRA;Uid=root;Pwd=123Schule123;";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT F_ID, CONCAT(Hersteller, ' ', Modell) AS FahrzeugInfo FROM Fahrzeug";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    // Leere die TextBox
                    textBox2_Fahrzeuge.Text = "";

                    // Speichere alle Fahrzeuge in einer Liste (optional)
                    var fahrzeuge = new List<(int F_ID, string Info)>();

                    while (reader.Read())
                    {
                        int fahrzeugId = Convert.ToInt32(reader["F_ID"]);
                        string info = reader["FahrzeugInfo"].ToString();
                        fahrzeuge.Add((fahrzeugId, info));
                    }

                    // Zeige z. B. das erste Fahrzeug in der TextBox an
                    if (fahrzeuge.Count > 0)
                    {
                        var first = fahrzeuge[0];
                        currentFahrzeugId = first.F_ID;
                        textBox2_Fahrzeuge.Text = first.Info;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler beim Laden: " + ex.Message);
                }
            }
        }
        private void LoadFahrzeugeData()
        {
            string connectionString = "Server=localhost;Port=3306;Database=Autoleasing_MySQLABRA;Uid=root;Pwd=123Schule123;";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM Fahrzeug";
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridView1.DataSource = dt;

                    // Spaltenreihenfolge und Sichtbarkeit anpassen
                    dataGridView1.Columns["F_ID"].Visible = false;         // Fahrzeug-ID ausblenden
                    dataGridView1.Columns["Bildpfad"].Visible = false;     // Bildpfad ausblenden

                    // Sichtbare Spalten konfigurieren
                    dataGridView1.Columns["Hersteller"].DisplayIndex = 0;
                    dataGridView1.Columns["Modell"].DisplayIndex = 1;
                    dataGridView1.Columns["Farbe"].DisplayIndex = 2;
                    dataGridView1.Columns["Listenpreis"].DisplayIndex = 3;
                    dataGridView1.Columns["Baujahr"].DisplayIndex = 4;
                    dataGridView1.Columns["Leasingkategorie"].DisplayIndex = 5;

                    // Header-Texte und Formatierungen
                    dataGridView1.Columns["Listenpreis"].HeaderText = "Preis (€)";
                    dataGridView1.Columns["Listenpreis"].DefaultCellStyle.Format = "C2";
                    dataGridView1.ReadOnly = true;
                    dataGridView1.AutoResizeColumns();

                    // KEINE automatische Auswahl der ersten Zeile
                    dataGridView1.ClearSelection();  // <-- HIER EINFÜGEN
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler beim Laden der Fahrzeuge: " + ex.Message);
                }
            }
        }


        private void InitializeUI()
        {


            /* tabControl1.Appearance = TabAppearance.FlatButtons;
             tabControl1.ItemSize = new Size(0, 1);
             tabControl1.SizeMode = TabSizeMode.Fixed;*/

            textBoxLOGINpasswort.PasswordChar = '•';
            textBoxNewPassword.PasswordChar = '•';
            textBoxConfirmPassword.PasswordChar = '•';
            textBox3PasswortEREG.PasswordChar = '•';
            textBoxPASSWORTACCOUNT.PasswordChar = '•';






            // Standardmäßig unsichtbare Elemente
            labelVerification.Visible = false;
            textBoxVerificationCode.Visible = false;
            buttonVerifyCode.Visible = false;
            textBoxNewPassword.Visible = false;

            // Nur Einfachauswahl erlauben
            dataGridView1.MultiSelect = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
            textBox2_Fahrzeuge.Text = string.Empty;
        }





        private bool ContainsNumbers(string input) => System.Text.RegularExpressions.Regex.IsMatch(input, @"\d");
        private bool IsNumeric(string input) => System.Text.RegularExpressions.Regex.IsMatch(input, @"^[0-9 +]+$");


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
                MessageBox.Show("Adresse muss Hausnummer, PLZ und Ort enthalten (z.B.: 'Musterstraße 1, 12345 Stadt').", "Ungültige Adresse", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!textBoxEMAIlREG.Text.Contains("@") || !textBoxEMAIlREG.Text.Contains("."))
            {
                MessageBox.Show("Geben Sie eine gültige E-Mail-Adresse ein.", "Ungültige E-Mail", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!IsNumeric(textBoxTelefonummerREG.Text))
            {
                MessageBox.Show("Die Telefonnummer darf nur Zahlen enthalten.", "Ungültige Telefonnummer", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (textBox3PasswortEREG.Text.Length != 8)
            {
                MessageBox.Show("Das Passwort muss genau 8 Zeichen lang sein.", "Ungültiges Passwort", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }
        private string GetSelectedGender()
        {
            if (radioButtonMaennlichREG.Checked) return "m";
            if (radioButtonWeiblichREG.Checked) return "w";
            if (radioButtonDIversREG.Checked) return "d";
            return "";
        }




        private bool CheckEmailExists(string email)
        {
            string connectionString = "Server=localhost;Port=3306;Database=Autoleasing_MySQLABRA;Uid=root;Pwd=123Schule123;";
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT COUNT(*) FROM Kunde WHERE Email = @Email", conn);
                cmd.Parameters.AddWithValue("@Email", email);
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }


        private void GenerateVerificationCode()
        {
            const string digits = "0123456789";
            Random random = new Random();
            char[] code = new char[6];
            for (int i = 0; i < 6; i++)
                code[i] = digits[random.Next(digits.Length)];
            verificationCode = new string(code);
            MessageBox.Show($"Ihr Verifikationscode: {verificationCode}", "Sicherheitscode", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }





        private void UpdatePasswordInDatabase(string email, string newPassword)
        {
            try
            {
                string connectionString = "Server=localhost;Port=3306;Database=Autoleasing_MySQLABRA;Uid=root;Pwd=123Schule123;";
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE Kunde SET Passwort = @Password WHERE Email = @Email";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        // Store plaintext password directly
                        cmd.Parameters.AddWithValue("@Password", newPassword);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ResetLoginForm()
        {
            // Clear all fields
            textBoxloginEMAIL.Clear();
            textBoxLOGINpasswort.Clear();
            textBoxVerificationCode.Clear();
            textBoxNewPassword.Clear();
            textBoxConfirmPassword.Clear();
            verificationCode = string.Empty;


            // Restore original login UI
            textBoxloginEMAIL.Enabled = true;
            textBoxloginEMAIL.Visible = true;
            textBoxLOGINpasswort.Visible = true;
            buttonLOGIN.Visible = true;
            linkLabel1.Visible = true;
            label5passwortlogin.Visible = true;
            label6emaillogin.Visible = true;

            // Hide all password reset elements
            labelVerification.Visible = false;
            textBoxVerificationCode.Visible = false;
            buttonVerifyCode.Visible = false;
            labelNewPassword.Visible = false;
            textBoxNewPassword.Visible = false;
            labelConfirmPassword.Visible = false;
            textBoxConfirmPassword.Visible = false;
            buttonUpdatePassword.Visible = false;
        }

        private void LoadCustomerData(int kundenId)
        {
            string connectionString = "Server=localhost;Port=3306;Database=Autoleasing_MySQLABRA;Uid=root;Pwd=123Schule123;";
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

                        // Deaktiviere alle Felder beim ersten Laden
                        SetEditMode(false);
                        buttonBearbeitenACC.Text = "B E A R B E I T E N"; // Sicherstellen, dass der Button "Bearbeiten" anzeigt
                    }
                }
            }
            vertragkundelabel.Text = ("Kunden-ID: ") + kundenId.ToString();

            vertragkundelabel.BackColor = Color.LightGray; // Optional: Visuelle Markierung
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

        private bool isEditMode = false;



        private void SetEditMode(bool editMode)
        {
            isEditMode = editMode;

            // Aktiviere/Deaktiviere alle Eingabefelder
            textBoxVORNACCOUNT.Enabled = editMode;
            textBoxNACHNACCOUNT.Enabled = editMode;
            textBoxADRESSEACCOUNT.Enabled = editMode;
            textBoxEMAILACCOUNT.Enabled = editMode;
            textBoxTELEFONACCOUNT.Enabled = editMode;
            textBoxPASSWORTACCOUNT.Enabled = editMode;
            dateTimePickerACCOUNT.Enabled = editMode;
            radioButtonMaennlichACC.Enabled = editMode;
            radioButtonWeiblichACC.Enabled = editMode;
            radioButtonDiversACC.Enabled = editMode;

            // RichTextBox bleibt immer aktiv (nur Anzeige)
            richTextBox1.Enabled = true;

            // Passwortzeichen ändern je nach Modus
            textBoxPASSWORTACCOUNT.PasswordChar = editMode ? '\0' : '•';
        }
        private string GetSelectedGenderAccount()
        {
            if (radioButtonMaennlichACC.Checked) return "m";
            if (radioButtonWeiblichACC.Checked) return "w";
            if (radioButtonDiversACC.Checked) return "d";
            return "m"; // Default value if none selected
        }


        private void ResetAllForms()
        {
            // Zurücksetzen der Login-Felder
            textBoxloginEMAIL.Clear();
            textBoxLOGINpasswort.Clear();
            textBoxLOGINpasswort.Visible = true;
            textBoxloginEMAIL.Visible = true;
            label6emaillogin.Visible = true;
            // Zurücksetzen der Registrierungsfelder
            textBoxVorNREG.Clear();
            textBoxNachNREG.Clear();
            textBoxAdresseREG.Clear();
            textBoxEMAIlREG.Clear();
            textBoxTelefonummerREG.Clear();
            textBox3PasswortEREG.Clear();
            dateTimePickerGeburtsdatumREG.Value = DateTime.Now;
            radioButtonMaennlichREG.Checked = true;

            // Zurücksetzen der Account-Felder
            textBoxVORNACCOUNT.Clear();
            textBoxNACHNACCOUNT.Clear();
            textBoxADRESSEACCOUNT.Clear();
            textBoxEMAILACCOUNT.Clear();
            textBoxTELEFONACCOUNT.Clear();
            textBoxPASSWORTACCOUNT.Clear();
            dateTimePickerACCOUNT.Value = DateTime.Now;
            radioButtonMaennlichACC.Checked = true;
            richTextBox1.Clear();
            labelKundenID.Text = "Kunden-ID:";

            // Zurücksetzen des Passwort-Reset-Forms
            textBoxVerificationCode.Clear();
            textBoxNewPassword.Clear();
            textBoxConfirmPassword.Clear();
            verificationCode = string.Empty;

            // UI-Elemente zurücksetzen
            SetEditMode(false);
            buttonBearbeitenACC.Text = "B E A R B E I T E N";

            // Zur Startseite zurückkehren
            tabControl1.SelectedIndex = 0;

            // Alle versteckten Elemente zurücksetzen
            labelVerification.Visible = false;
            textBoxVerificationCode.Visible = false;
            buttonVerifyCode.Visible = false;
            textBoxNewPassword.Visible = false;
            labelNewPassword.Visible = false;
            labelConfirmPassword.Visible = false;
            textBoxConfirmPassword.Visible = false;
            buttonUpdatePassword.Visible = false;

            // Login-Elemente wieder anzeigen
            textBoxLOGINpasswort.Visible = true;
            buttonLOGIN.Visible = true;
            linkLabel1.Visible = true;
            label5passwortlogin.Visible = true;
            textBoxloginEMAIL.Enabled = true;

            //Fahrzeug-Vertrag ELemente zurücksetzen
            this.textBoxmrate.Text = "";
            this.vertragkundelabel.Text = "";
            dateTimePickerAnfang.Value = DateTime.Today;
            dateTimePickerEnde.Value = DateTime.Today.AddMonths(1); // Standardmäßig 1 Monat Laufzeit
            textBox2_Fahrzeuge.Text = string.Empty;
            dataGridView1.ClearSelection();
            pictureBox10.Image = null; // Bild zurücksetzen

        }


        private void buttonSTART_LOGIN_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 2;

        }

        private void buttonSTARTJetzt_REGRI_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;

        }

        private void button2REG_Click(object sender, EventArgs e)
        {

            if (!ValidateRegistrationData()) return;

            // Altersüberprüfung
            DateTime geburtsdatum = dateTimePickerGeburtsdatumREG.Value;
            if (DateTime.Now.Year - geburtsdatum.Year < 18 ||
                (DateTime.Now.Year - geburtsdatum.Year == 18 && DateTime.Now.DayOfYear < geburtsdatum.DayOfYear))
            {
                MessageBox.Show("Sie müssen mindestens 18 Jahre alt sein.", "Altersbeschränkung", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string email = textBoxEMAIlREG.Text;
            string connectionString = "Server=localhost;Port=3306;Database=Autoleasing_MySQLABRA;Uid=root;Pwd=123Schule123;";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Prüfen ob Email bereits existiert
                    string checkEmailQuery = "SELECT COUNT(*) FROM Kunde WHERE Email = @Email";
                    MySqlCommand checkCmd = new MySqlCommand(checkEmailQuery, conn);
                    checkCmd.Parameters.AddWithValue("@Email", email);
                    int emailCount = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (emailCount > 0)
                    {
                        MessageBox.Show("Diese Email-Adresse ist bereits registriert.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Registrierung durchführen
                    string query = @"INSERT INTO Kunde (Vorname, Nachname, Adresse, Email, Geburtsdatum, Geschlecht, Telefonnummer, Passwort) 
         VALUES (@Vorname, @Nachname, @Adresse, @Email, @Geburtsdatum, @Geschlecht, @Telefonnummer, @Passwort)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    // Alle Parameter hinzufügen
                    cmd.Parameters.AddWithValue("@Vorname", textBoxVorNREG.Text);
                    cmd.Parameters.AddWithValue("@Nachname", textBoxNachNREG.Text);
                    cmd.Parameters.AddWithValue("@Adresse", textBoxAdresseREG.Text);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Geburtsdatum", geburtsdatum.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@Geschlecht", GetSelectedGender());
                    cmd.Parameters.AddWithValue("@Telefonnummer", textBoxTelefonummerREG.Text);
                    cmd.Parameters.AddWithValue("@Passwort", textBox3PasswortEREG.Text); // Klartext-Passwort

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
                    MessageBox.Show("Fehler bei der Registrierung: " + ex.Message, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }



        private void buttonLOGIN_Click(object sender, EventArgs e)
        {
            string email = textBoxloginEMAIL.Text;
            string password = textBoxLOGINpasswort.Text;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Bitte geben Sie Email und Passwort ein", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string connectionString = "Server=localhost;Port=3306;Database=Autoleasing_MySQLABRA;Uid=root;Pwd=123Schule123;";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Zuerst prüfen ob Email existiert
                    string checkEmailQuery = "SELECT COUNT(*) FROM Kunde WHERE Email = @Email";
                    MySqlCommand checkCmd = new MySqlCommand(checkEmailQuery, conn);
                    checkCmd.Parameters.AddWithValue("@Email", email);
                    int emailCount = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (emailCount == 0)
                    {
                        MessageBox.Show("Diese Email-Adresse ist nicht registriert.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Dann Passwort prüfen
                    string query = "SELECT K_ID, Passwort FROM Kunde WHERE Email = @Email";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Email", email);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string storedPassword = reader["Passwort"].ToString();

                            if (password == storedPassword)
                            {
                                int kundenId = Convert.ToInt32(reader["K_ID"]);
                                tabControl1.SelectedTab = tabPage4;
                                LoadCustomerData(kundenId);
                            }
                            else
                            {
                                MessageBox.Show("Falsches Passwort!", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
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

            // Hide login elements
            textBoxLOGINpasswort.Visible = false;
            buttonLOGIN.Visible = false;
            linkLabel1.Visible = false;
            label5passwortlogin.Visible = false;
            textBoxNewPassword.Visible = false;
            labelNewPassword.Visible = false;
            textBoxloginEMAIL.Visible = false;
            label6emaillogin.Visible = false;
            // Show verification elements
            labelVerification.Visible = true;
            textBoxVerificationCode.Visible = true;
            buttonVerifyCode.Visible = true;

            GenerateVerificationCode();

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

        private void buttonBearbeitenACC_Click(object sender, EventArgs e)
        {
            if (!isEditMode)
            {
                // Aktiviere Bearbeitungsmodus
                SetEditMode(true);
                buttonBearbeitenACC.Text = "S P E I C H E R N";
            }
            else
            {
                try
                {
                    // Validierung der Eingaben
                    if (string.IsNullOrWhiteSpace(textBoxVORNACCOUNT.Text) ||
                        string.IsNullOrWhiteSpace(textBoxNACHNACCOUNT.Text) ||
                        string.IsNullOrWhiteSpace(textBoxADRESSEACCOUNT.Text) ||
                        string.IsNullOrWhiteSpace(textBoxEMAILACCOUNT.Text) ||
                        string.IsNullOrWhiteSpace(textBoxTELEFONACCOUNT.Text) ||
                        string.IsNullOrWhiteSpace(textBoxPASSWORTACCOUNT.Text))
                    {
                        MessageBox.Show("Bitte füllen Sie alle Felder aus.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (textBoxPASSWORTACCOUNT.Text.Length != 8)
                    {
                        MessageBox.Show("Das Passwort muss genau 8 Zeichen lang sein.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Kunden-ID aus dem Label extrahieren
                    int kundenId = int.Parse(labelKundenID.Text.Replace("Kunden-ID: ", ""));

                    string connectionString = "Server=localhost;Port=3306;Database=Autoleasing_MySQLABRA;Uid=root;Pwd=123Schule123;";
                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();
                        string updateQuery = @"UPDATE Kunde SET 
                            Vorname = @Vorname, 
                            Nachname = @Nachname, 
                            Geschlecht = @Geschlecht, 
                            Geburtsdatum = @Geburtsdatum, 
                            Adresse = @Adresse, 
                            Telefonnummer = @Telefonnummer, 
                            Email = @Email, 
                            Passwort = @Passwort 
                            WHERE K_ID = @K_ID"
                        ;

                        MySqlCommand cmd = new MySqlCommand(updateQuery, conn);
                        cmd.Parameters.AddWithValue("@K_ID", kundenId);
                        cmd.Parameters.AddWithValue("@Vorname", textBoxVORNACCOUNT.Text);
                        cmd.Parameters.AddWithValue("@Nachname", textBoxNACHNACCOUNT.Text);
                        cmd.Parameters.AddWithValue("@Geschlecht", GetSelectedGenderAccount());
                        cmd.Parameters.AddWithValue("@Geburtsdatum", dateTimePickerACCOUNT.Value.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@Adresse", textBoxADRESSEACCOUNT.Text);
                        cmd.Parameters.AddWithValue("@Telefonnummer", textBoxTELEFONACCOUNT.Text);
                        cmd.Parameters.AddWithValue("@Email", textBoxEMAILACCOUNT.Text);
                        cmd.Parameters.AddWithValue("@Passwort", textBoxPASSWORTACCOUNT.Text);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Ihre Daten wurden erfolgreich aktualisiert!", "Erfolg", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            // Daten neu laden und Bearbeitungsmodus beenden
                            LoadCustomerData(kundenId);
                            SetEditMode(false);
                            buttonBearbeitenACC.Text = "B E A R B E I T E N";
                        }
                        else
                        {
                            MessageBox.Show("Keine Änderungen vorgenommen.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler beim Aktualisieren der Daten: " + ex.Message, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonloeschenACC_Click(object sender, EventArgs e)
        {
            // Bestätigungsdialog anzeigen
            if (MessageBox.Show("Möchten Sie Ihr Konto wirklich unwiderruflich löschen?",
                               "Konto löschen",
                               MessageBoxButtons.YesNo,
                               MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    // Kunden-ID aus dem Label extrahieren
                    int kundenId = int.Parse(labelKundenID.Text.Replace("Kunden-ID: ", ""));

                    string connectionString = "Server=localhost;Port=3306;Database=Autoleasing_MySQLABRA;Uid=root;Pwd=123Schule123;";
                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();

                        // 1. Alle abhängigen Verträge zuerst löschen (falls nötig)
                        string deleteVertrageQuery = "DELETE FROM Leasingvertrag WHERE K_ID = @K_ID";
                        MySqlCommand vertragCmd = new MySqlCommand(deleteVertrageQuery, conn);
                        vertragCmd.Parameters.AddWithValue("@K_ID", kundenId);
                        vertragCmd.ExecuteNonQuery();

                        // 2. Kunde löschen
                        string deleteKundeQuery = "DELETE FROM Kunde WHERE K_ID = @K_ID";
                        MySqlCommand kundeCmd = new MySqlCommand(deleteKundeQuery, conn);
                        kundeCmd.Parameters.AddWithValue("@K_ID", kundenId);

                        int rowsAffected = kundeCmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Ihr Konto wurde erfolgreich gelöscht. Vielen Dank für Ihre Nutzung unserer Dienste.",
                                          "Konto gelöscht",
                                          MessageBoxButtons.OK,
                                          MessageBoxIcon.Information);

                            // Zur Startseite zurückkehren
                            tabControl1.SelectedIndex = 0;
                            ResetLoginForm();
                        }
                        else
                        {
                            MessageBox.Show("Löschen fehlgeschlagen. Konto nicht gefunden.",
                                          "Fehler",
                                          MessageBoxButtons.OK,
                                          MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler beim Löschen des Kontos: " + ex.Message,
                                  "Datenbankfehler",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Error);
                }
            }
        }

        private void buttonKAUFENACC_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 4;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ResetAllForms();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ResetAllForms();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ResetAllForms();
        }

        private void buttonVerifyCode_Click(object sender, EventArgs e)
        {
            if (textBoxVerificationCode.Text == verificationCode)
            {
                // Hide verification elements
                labelVerification.Visible = false;
                textBoxVerificationCode.Visible = false;
                buttonVerifyCode.Visible = false;

                // Show password reset elements
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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void labelKundenID_Click(object sender, EventArgs e)
        {

        }
        private int? GetFahrzeugIdFromName(string fahrzeugName)
        {
            string connectionString = "Server=localhost;Port=3306;Database=Autoleasing_MySQLABRA;Uid=root;Pwd=123Schule123;";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT F_ID FROM Fahrzeug WHERE CONCAT(Hersteller, ' ', Modell) LIKE @FahrzeugName LIMIT 1";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@FahrzeugName", $"%{fahrzeugName}%");
                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    return Convert.ToInt32(result);
                }
            }
            return null;
        }

        private void buttonvertrag_Click(object sender, EventArgs e)
        {

            if (!int.TryParse(labelKundenID.Text.Replace("Kunden-ID: ", ""), out int kundenId))
            {
                MessageBox.Show("Ungültige Kunden-ID!", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 2. Validierung: Fahrzeugname wurde eingegeben
            string fahrzeugName = textBox2_Fahrzeuge.Text.Trim();
            if (string.IsNullOrEmpty(fahrzeugName))
            {
                MessageBox.Show("Bitte geben Sie einen Fahrzeugnamen ein!", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3. Finde die F_ID basierend auf dem Namen aus der TextBox
            int? fahrzeugId = GetFahrzeugIdFromName(fahrzeugName);
            if (!fahrzeugId.HasValue)
            {
                MessageBox.Show("Fahrzeug nicht gefunden. Bitte geben Sie einen gültigen Fahrzeugnamen ein.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 4. Berechne monatliche Rate
            decimal rate = BerechneMonatlicheRate(fahrzeugId.Value, dateTimePickerAnfang.Value, dateTimePickerEnde.Value);

            // 5. Anzeige der Rate formatiert als Währung
            textBoxmrate.Text = rate.ToString("C");

            // 6. Speichere den Vertrag in der Datenbank
            string connectionString = "Server=localhost;Port=3306;Database=Autoleasing_MySQLABRA;Uid=root;Pwd=123Schule123;";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"INSERT INTO Leasingvertrag 
                            (K_ID, F_ID, Vertragsbeginn, Vertragsende, monatliche_Rate)
                            VALUES (@K_ID, @F_ID, @Anfang, @Ende, @Rate)";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@K_ID", kundenId);
                    cmd.Parameters.AddWithValue("@F_ID", fahrzeugId.Value);
                    cmd.Parameters.AddWithValue("@Anfang", dateTimePickerAnfang.Value.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@Ende", dateTimePickerEnde.Value.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@Rate", rate);

                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler beim Speichern des Vertrags: " + ex.Message);
                    return;
                }
            }

            // 7. Erfolgsmeldung anzeigen
            MessageBox.Show($"Vertrag erstellt! Monatliche Rate: {rate:C}", "Erfolg", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }



        //private void ErstelleLeasingvertrag()
        //{
        //    if (!int.TryParse(labelKundenID.Text.Replace("Kunden-ID: ", ""), out int kundenId))
        //    {
        //        MessageBox.Show("Ungültige Kunden-ID!", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        return;
        //    }

        //    if (string.IsNullOrWhiteSpace(textBox2_Fahrzeuge.Text))
        //    {
        //        MessageBox.Show("Bitte wählen Sie ein Fahrzeug aus.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        return;
        //    }

        //    if (dateTimePickerAnfang.Value <= DateTime.Today)
        //    {
        //        MessageBox.Show("Das Vertragsbeginn-Datum muss in der Zukunft liegen.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        return;
        //    }

        //    if (dateTimePickerEnde.Value <= dateTimePickerAnfang.Value)
        //    {
        //        MessageBox.Show("Das Vertragsende-Datum muss nach dem Beginn liegen.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        return;
        //    }

        //    int fahrzeugId = GetFahrzeugIdFromName(textBox2_Fahrzeuge.Text);

        //    if (fahrzeugId == -1)
        //    {
        //        MessageBox.Show("Fahrzeug nicht gefunden.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        return;
        //    }

        //    try
        //    {
        //                    string connectionString = "Server=localhost;Port=3306;Database=Autoleasing_MySQLABRA;Uid=root;Pwd=123Schule123;";

        //        using (MySqlConnection conn = new MySqlConnection(connectionString))
        //        {
        //            conn.Open();
        //            string query = @"INSERT INTO Leasingvertrag 
        //                     (V_ID, K_ID, F_ID, Vertragsbeginn, Vertragsende, monatliche_Rate)
        //                     VALUES (@V_ID, @K_ID, @F_ID, @Anfang, @Ende, @Rate)";

        //            MySqlCommand cmd = new MySqlCommand(query, conn);
        //            cmd.Parameters.AddWithValue("@V_ID", GeneriereNeueVertragsID());
        //            cmd.Parameters.AddWithValue("@K_ID", kundenId);
        //            cmd.Parameters.AddWithValue("@F_ID", fahrzeugId);
        //            cmd.Parameters.AddWithValue("@Anfang", dateTimePickerAnfang.Value.ToString("yyyy-MM-dd"));
        //            cmd.Parameters.AddWithValue("@Ende", dateTimePickerEnde.Value.ToString("yyyy-MM-dd"));
        //            cmd.Parameters.AddWithValue("@Rate", decimal.Parse(textBoxmrate.Text));

        //            cmd.ExecuteNonQuery();
        //        }

        //        MessageBox.Show("Vertrag erfolgreich erstellt!", "Erfolg", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Fehler beim Erstellen des Vertrags: " + ex.Message, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}
        private int GeneriereNeueVertragsID()
        {
            string connectionString = "Server=localhost;Port=3306;Database=Autoleasing_MySQLABRA;Uid=root;Pwd=123Schule123;";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                // Holt die höchste vorhandene V_ID und addiert 1
                MySqlCommand cmd = new MySqlCommand("SELECT MAX(V_ID) FROM Leasingvertrag", conn);
                object result = cmd.ExecuteScalar();

                return (result == DBNull.Value) ? 1 : Convert.ToInt32(result) + 1;
            }
        }

        private void dateTimePickerAnfang_ValueChanged(object sender, EventArgs e)
        {

            if (dateTimePickerAnfang.Value < DateTime.Today)
            {
                errorProvider1.SetError(dateTimePickerAnfang, "Datum darf nicht in der Vergangenheit liegen!");
            }
            else
            {
                errorProvider1.SetError(dateTimePickerAnfang, "");
            }

            // Enddatum automatisch anpassen (falls nötig)
            if (dateTimePickerEnde.Value <= dateTimePickerAnfang.Value)
            {
                dateTimePickerEnde.Value = dateTimePickerAnfang.Value.AddMonths(1);
            }
            AktualisiereRate();
        }

        private void dateTimePickerEnde_ValueChanged(object sender, EventArgs e)
        {
            if (dateTimePickerEnde.Value <= dateTimePickerAnfang.Value)
            {
                errorProvider1.SetError(dateTimePickerEnde, "Enddatum muss nach Anfangsdatum liegen!");
            }
            else
            {
                errorProvider1.SetError(dateTimePickerEnde, "");
            }
            AktualisiereRate();
        }
    
    private decimal BerechneMonatlicheRate(int fahrzeugId, DateTime beginn, DateTime ende)
        {
            string connectionString = "Server=localhost;Port=3306;Database=Autoleasing_MySQLABRA;Uid=root;Pwd=123Schule123;";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string preisQuery = "SELECT Listenpreis, Leasingkategorie FROM Fahrzeug WHERE F_ID = @F_ID";
                MySqlCommand preisCmd = new MySqlCommand(preisQuery, conn);
                preisCmd.Parameters.AddWithValue("@F_ID", fahrzeugId);

                using (MySqlDataReader reader = preisCmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        decimal listenpreis = Convert.ToDecimal(reader["Listenpreis"]);
                        string kategorie = reader["Leasingkategorie"].ToString();

                        int monate = (ende.Year - beginn.Year) * 12 + ende.Month - beginn.Month;
                        monate = Math.Max(monate, 1);

                        decimal kategoriefaktor;
                        switch (kategorie)
                        {
                            case "Premium":
                                kategoriefaktor = 0.02m;
                                break;
                            case "Standard":
                                kategoriefaktor = 0.015m;
                                break;
                            default:
                                kategoriefaktor = 0.01m;
                                break;
                        }

                        decimal rate = listenpreis * kategoriefaktor;
                        rate = Math.Max(rate, 100); // Mindestrate 100€
                        return rate;
                    }
                }
            }
            return 0;
        }
        
        


        private void AktualisiereRate()
        {
            if (currentFahrzeugId == -1) return;

            try
            {
                decimal rate = BerechneMonatlicheRate(currentFahrzeugId,
                                                      dateTimePickerAnfang.Value,
                                                      dateTimePickerEnde.Value);

                textBoxmrate.Text = rate.ToString("C");
            }
            catch (Exception ex)
            {
                textBoxmrate.Text = "Fehler";
                Console.WriteLine($"Fehler in AktualisiereRate: {ex.Message}");
            }
        }




        private void button2_Click(object sender, EventArgs e)
        {
            ResetAllForms();
        }

        private void ShowImage(int fahrzeugId)
        {
            string connectionString = "Server=localhost;Port=3306;Database=Autoleasing_MySQLABRA;Uid=root;Pwd=123Schule123;";
             using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT Bildpfad, Hersteller, Modell FROM Fahrzeug WHERE F_ID = @F_ID";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@F_ID", fahrzeugId);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string imageName = reader["Bildpfad"]?.ToString();

                            // Wenn kein Bildpfad vorhanden, erstelle einen Standardnamen
                            if (string.IsNullOrEmpty(imageName))
                            {
                                string hersteller = reader["Hersteller"].ToString().ToLower();
                                string modell = reader["Modell"].ToString().ToLower().Replace(" ", "_");
                                imageName = $"{hersteller}_{modell}.png";
                            }

                            // Bildpfad im Projektordner
                            string imagePath = Path.Combine(Application.StartupPath, "bilder_Fahrzeuge", imageName);

                            // Überprüfe, ob das Bild existiert
                            if (File.Exists(imagePath))
                            {
                                // Lade das Bild in die PictureBox
                                pictureBox10.Image = Image.FromFile(imagePath);
                                pictureBox10.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // Falls das Bild nicht gefunden wird, zeige ein Standardbild
                                pictureBox10.Image = Properties.Resources.Fahrzeuge;
                            }
                        }
                        else
                        {
                            // Kein Fahrzeug gefunden
                            MessageBox.Show("Fahrzeugbild nicht gefunden.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            pictureBox10.Image = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler beim Laden des Fahrzeugbildes: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        // Add this method to handle the SelectedIndexChanged event for comboboxFahrzeug

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {

            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                if (selectedRow.Cells["F_ID"].Value != null &&
                    int.TryParse(selectedRow.Cells["F_ID"].Value.ToString(), out int fahrzeugId))
                {
                    // Fahrzeugname in TextBox anzeigen
                    string hersteller = selectedRow.Cells["Hersteller"].Value?.ToString() ?? "";
                    string modell = selectedRow.Cells["Modell"].Value?.ToString() ?? "";
                    textBox2_Fahrzeuge.Text = $"{hersteller} {modell}";

                    // Monatliche Rate berechnen und in TextBox anzeigen
                    decimal rate = BerechneMonatlicheRate(fahrzeugId, dateTimePickerAnfang.Value, dateTimePickerEnde.Value);
                    textBoxmrate.Text = rate.ToString("C");

                    // Fahrzeugbild in PictureBox anzeigen
                    ShowImage(fahrzeugId);
                }
            }
            else
            {
                // Wenn keine Zeile ausgewählt ist, lösche den Inhalt von TextBox und PictureBox
                textBox2_Fahrzeuge.Clear();
                textBoxmrate.Clear();
                pictureBox10.Image = null;
            }
        }

       
        private bool IsValidPayment()
        {
            if (string.IsNullOrWhiteSpace(textBox2_Fahrzeuge.Text))
            {
                MessageBox.Show("Bitte geben Sie einen Fahrzeugnamen ein.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // 2. Optional: Existiert das Fahrzeug in der Datenbank?
            int? fahrzeugId = GetFahrzeugIdFromName(textBox2_Fahrzeuge.Text.Trim());
            if (!fahrzeugId.HasValue)
            {
                MessageBox.Show("Das eingegebene Fahrzeug existiert nicht in der Datenbank.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // 2. Prüfung: Vertragsbeginn in der Zukunft?
            if (dateTimePickerAnfang.Value < DateTime.Today)
            {
                MessageBox.Show("Das Vertragsbeginn-Datum muss in der Zukunft liegen.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // 3. Prüfung: Vertragsende nach Beginn?
            if (dateTimePickerEnde.Value <= dateTimePickerAnfang.Value)
            {
                MessageBox.Show("Das Vertragsende-Datum muss nach dem Beginn liegen.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Alles okay
            return true;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            // Zur Zahlungsseite wechseln
            tabControl1.SelectedTab = tabPage6_Zahlung;

            // Zahlungsdaten laden
            LoadZahlungenData();
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            radioButton1.Enabled = false;
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            radioButton2.Enabled = false;
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            radioButton3.Enabled = false;
        }

        private void textBoxmrate_TextChanged(object sender, EventArgs e)
        {

        }
        private void LoadZahlungenData()
        {
            string connectionString = "Server=localhost;Port=3306;Database=Autoleasing_MySQLABRA;Uid=root;Pwd=123Schule123;";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT z.Z_ID, z.Betrag, z.Zahlungsmethode, 
                   z.Status, z.Zahlungsfrist, l.V_ID AS Vertragsnummer,
                   CONCAT(f.Hersteller, ' ', f.Modell) AS Fahrzeug
                   FROM Zahlung z
                   JOIN Leasingvertrag l ON z.V_ID = l.V_ID
                   JOIN Fahrzeug f ON l.F_ID = f.F_ID
                   WHERE z.K_ID = @KundenID
                   ORDER BY z.Zahlungsfrist DESC";  // Sortierung nach Fälligkeit

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@KundenID", GetCurrentKundenID());

                    // Existierende Daten löschen
                    if (dataGridViewZahlung.DataSource != null)
                    {
                        ((DataTable)dataGridViewZahlung.DataSource).Clear();
                    }

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Datenquelle setzen
                    dataGridViewZahlung.DataSource = dt;

                    // Spalten konfigurieren
                    ConfigureZahlungGridViewColumns();

                    // Keine leere Zeile am Ende
                    dataGridViewZahlung.AllowUserToAddRows = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler beim Laden der Zahlungen: " + ex.Message);
                }
            }
        }

        private void ConfigureZahlungGridViewColumns()
        {
            // Nur wenn Spalten existieren
            if (dataGridViewZahlung.Columns.Count == 0) return;

            dataGridViewZahlung.Columns["Z_ID"].HeaderText = "Zahlungs-ID";
            dataGridViewZahlung.Columns["Betrag"].HeaderText = "Betrag (€)";
            dataGridViewZahlung.Columns["Betrag"].DefaultCellStyle.Format = "C2";
            dataGridViewZahlung.Columns["Zahlungsmethode"].HeaderText = "Zahlungsart";
            dataGridViewZahlung.Columns["Status"].HeaderText = "Status";

            // Datumsformat für die Frist
            dataGridViewZahlung.Columns["Zahlungsfrist"].HeaderText = "Fällig am";
            dataGridViewZahlung.Columns["Zahlungsfrist"].DefaultCellStyle.Format = "dd.MM.yyyy";

            dataGridViewZahlung.Columns["Fahrzeug"].HeaderText = "Fahrzeug";

            // Optional: Spaltenbreiten anpassen
            dataGridViewZahlung.Columns["Z_ID"].Width = 80;
            dataGridViewZahlung.Columns["Betrag"].Width = 100;
            dataGridViewZahlung.Columns["Status"].Width = 100;
            dataGridViewZahlung.Columns["Zahlungsfrist"].Width = 120;

            dataGridViewZahlung.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
        }
        private int GetCurrentKundenID()
        {
            // Extrahiert die Kunden-ID aus dem Label
            if (labelKundenID.Text.StartsWith("Kunden-ID: "))
            {
                return int.Parse(labelKundenID.Text.Replace("Kunden-ID: ", ""));
            }
            return -1; // Falls nicht gefunden
        }
        private void dataGridViewZahlung_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewZahlung.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridViewZahlung.SelectedRows[0];
                label23ZID.Text = selectedRow.Cells["Z_ID"].Value.ToString();
                textBoxbetrag.Text = selectedRow.Cells["Betrag"].Value.ToString();

                // Datum aus der DataGridView in den DateTimePicker laden
                if (DateTime.TryParse(selectedRow.Cells["Zahlungsfrist"].Value.ToString(), out DateTime fristDatum))
                {
                    dateTimePickerZahlungsfrist.Value = fristDatum;
                }
            }

        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (dataGridViewZahlung.SelectedRows.Count == 0)
            {
                MessageBox.Show("Bitte wählen Sie eine Zahlung aus");
                return;
            }

            int zahlungsID = Convert.ToInt32(dataGridViewZahlung.SelectedRows[0].Cells["Z_ID"].Value);
            DateTime neueFrist = dateTimePickerZahlungsfrist.Value;

            string connectionString = "Server=localhost;Port=3306;Database=Autoleasing_MySQLABRA;Uid=root;Pwd=123Schule123;";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"UPDATE Zahlung 
                            SET Status = 'BEZAHLT', 
                                Zahlungsfrist = @NeueFrist 
                            WHERE Z_ID = @ZahlungsID";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ZahlungsID", zahlungsID);
                    cmd.Parameters.AddWithValue("@NeueFrist", neueFrist.ToString("yyyy-MM-dd"));

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Zahlung erfolgreich verbucht!");
                        LoadZahlungenData(); // Daten aktualisieren
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler: " + ex.Message);
                }
            }
        }
    }

}

