using MySql.Data.MySqlClient;
using System;
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

        private void LoadFahrzeugComboBox()
        {
          
            string connectionString = "Server=localhost;Database=Autoleasing_MySQLABRA;Uid=root;Pwd=123Schule123;";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT F_ID, CONCAT(Hersteller, ' ', Modell) AS FahrzeugInfo FROM Fahrzeug";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    comboboxFahrzeug.Items.Clear();
                    while (reader.Read())
                    {
                        // Füge ein Objekt mit ID und Text hinzu
                        comboboxFahrzeug.Items.Add(new
                        {
                            F_ID = Convert.ToInt32(reader["F_ID"]), // ID als Zahl
                            DisplayText = reader["FahrzeugInfo"].ToString() // Anzeigetext
                        });
                    }

                    // Textanzeige konfigurieren
                    comboboxFahrzeug.DisplayMember = "DisplayText"; // Zeigt "BMW i4" an
                    comboboxFahrzeug.ValueMember = "F_ID";          // Speichert die ID (z. B. 1)
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler beim Laden: " + ex.Message);
                }
            }
        
        }
        private void LoadFahrzeugeData()
        {
            string connectionString = "Server=localhost;Database=Autoleasing_MySQLABRA;Uid=root;Pwd=123Schule123;";

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

                    // Spaltenreihenfolge festlegen
                    dataGridView1.Columns["F_ID"].DisplayIndex = 0;
                    dataGridView1.Columns["Hersteller"].DisplayIndex = 1;
                    dataGridView1.Columns["Modell"].DisplayIndex = 2;
                    dataGridView1.Columns["Farbe"].DisplayIndex = 3;
                    dataGridView1.Columns["Listenpreis"].DisplayIndex = 4;
                    dataGridView1.Columns["Baujahr"].DisplayIndex = 5;
                    dataGridView1.Columns["Leasingkategorie"].DisplayIndex = 6;

                    // Header-Texte und Formatierungen
                    dataGridView1.Columns["F_ID"].HeaderText = "Fahrzeug-ID";
                    dataGridView1.Columns["Listenpreis"].DefaultCellStyle.Format = "C2";
                    dataGridView1.ReadOnly = true;
                    dataGridView1.AutoResizeColumns();

                    // Nach ganz links scrollen
                    dataGridView1.FirstDisplayedScrollingColumnIndex = 0;
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
            string connectionString = "Server=localhost;Database=Autoleasing_MySQLABRA;Uid=root;Pwd=123Schule123;";
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
                string connectionString = "Server=localhost;Database=Autoleasing_MySQLABRA;Uid=root;Pwd=123Schule123;";
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
            string connectionString = "Server=localhost;Database=Autoleasing_MySQLABRA;Uid=root;Pwd=123Schule123;";
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
            this.comboboxFahrzeug.SelectedIndex = -1;

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
            string connectionString = "Server=localhost;Database=Autoleasing_MySQLABRA;Uid=root;Pwd=123Schule123;";

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

            string connectionString = "Server=localhost;Database=Autoleasing_MySQLABRA;Uid=root;Pwd=123Schule123;";
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

                    string connectionString = "Server=localhost;Database=Autoleasing_MySQLABRA;Uid=root;Pwd=123Schule123;";
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

                    string connectionString = "Server=localhost;Database=Autoleasing_MySQLABRA;Uid=root;Pwd=123Schule123;";
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

        private void buttonvertrag_Click(object sender, EventArgs e)
        {

            // 1. Validierungen
            if (!int.TryParse(labelKundenID.Text.Replace("Kunden-ID: ", ""), out int kundenId))
            {
                MessageBox.Show("Ungültige Kunden-ID!", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (comboboxFahrzeug.SelectedItem == null)
            {
                MessageBox.Show("Bitte ein Fahrzeug auswählen!", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Rate berechnen (GLEICHE LOGIK wie in AktualisiereRate)
            dynamic selectedItem = comboboxFahrzeug.SelectedItem;
            int fahrzeugId = selectedItem.F_ID;
            decimal rate = BerechneMonatlicheRate(fahrzeugId, dateTimePickerAnfang.Value, dateTimePickerEnde.Value);

            // 3. Textbox und MessageBox synchronisieren
            textBoxmrate.Text = rate.ToString("C"); // Gleiche Formatierung

            // 4. Vertrag in Datenbank speichern
            string connectionString = "Server=localhost;Database=Autoleasing_MySQLABRA;Uid=root;Pwd=123Schule123;";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = @"INSERT INTO Leasingvertrag 
              (K_ID, F_ID, Vertragsbeginn, Vertragsende, monatliche_Rate)
              VALUES (@K_ID, @F_ID, @Anfang, @Ende, @Rate)";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@K_ID", kundenId);
                cmd.Parameters.AddWithValue("@F_ID", fahrzeugId);
                cmd.Parameters.AddWithValue("@Anfang", dateTimePickerAnfang.Value.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@Ende", dateTimePickerEnde.Value.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@Rate", rate);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show($"Vertrag erstellt! Monatliche Rate: {rate:C}", "Erfolg", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        

        
        private void ErstelleLeasingvertrag()
        {
            try
            {
                string connectionString = "Server=localhost;Database=Autoleasing_MySQLABRA;Uid=root;Pwd=123Schule123;";
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"INSERT INTO Leasingvertrag 
                            (V_ID, K_ID, F_ID, Vertragsbeginn, Vertragsende, monatliche_Rate)
                            VALUES (@V_ID, @K_ID, @F_ID, @Anfang, @Ende, @Rate)";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@V_ID", GeneriereNeueVertragsID());
                    cmd.Parameters.AddWithValue("@K_ID", labelKundenID.Text); // Angenommen, KundenID ist in einem Label
                    cmd.Parameters.AddWithValue("@F_ID", ((dynamic)comboboxFahrzeug.SelectedItem).F_ID);
                    cmd.Parameters.AddWithValue("@Anfang", dateTimePickerAnfang.Value.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@Ende", dateTimePickerEnde.Value.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@Rate", decimal.Parse(textBoxmrate.Text));

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Vertrag erfolgreich erstellt!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler: " + ex.Message);
            }
        }
        private int GeneriereNeueVertragsID()
        {
            string connectionString = "Server=localhost;Database=Autoleasing_MySQLABRA;Uid=root;Pwd=123Schule123;";
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


       
            string connectionString = "Server=localhost;Database=Autoleasing_MySQLABRA;Uid=root;Pwd=123Schule123;";
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

                        // Debug-Ausgabe zur Überprüfung
                        Console.WriteLine($"Fahrzeug-ID: {fahrzeugId}, Listenpreis: {listenpreis}, Kategorie: {kategorie}");

                        // Berechne Laufzeit in Monaten
                        int monate = (ende.Year - beginn.Year) * 12 + ende.Month - beginn.Month;
                        monate = Math.Max(monate, 1); // Mindestens 1 Monat

                        // Realistische Formel: (Listenpreis * Faktor) / Laufzeit
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

                        Console.WriteLine($"Berechnete Rate: {rate}"); // Debug
                        return rate;
                    }
                }
            }
            return 0; // Fallback
        }







        private void AktualisiereRate()
        {
            if (comboboxFahrzeug.SelectedItem == null) return;

            try
            {
                dynamic selectedItem = comboboxFahrzeug.SelectedItem;
                int fahrzeugId = selectedItem.F_ID;

                decimal rate = BerechneMonatlicheRate(
                    fahrzeugId,
                    dateTimePickerAnfang.Value,
                    dateTimePickerEnde.Value);

                // Formatierung als Währung (z. B. "6.000,00 €")
                textBoxmrate.Text = rate.ToString("C");
            }
            catch (Exception ex)
            {
                textBoxmrate.Text = "Fehler";
                Console.WriteLine($"Fehler in AktualisiereRate: {ex.Message}");
            }
        }



        private void comboboxFahrzeug_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            AktualisiereRate();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ResetAllForms();
        }
    }

    }

