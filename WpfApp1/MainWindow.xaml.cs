using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1
{
    // Fenêtre principale de l'application.
    public partial class MainWindow : Window
    {
        // Constructeur qui initialise les composants de la fenêtre.
        public MainWindow()
        {
            InitializeComponent();
        }

        // Ouvre une boîte de dialogue pour choisir l'emplacement du fichier de sortie.
        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Fichier texte (*.txt)|*.txt",
                FileName = "dictionnaire.txt"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                TxtOutputPath.Text = saveFileDialog.FileName;
            }
        }

        // Lance la génération du dictionnaire après avoir validé les entrées.
        private async void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            
            if (!int.TryParse(TxtMinLength.Text, out int min) || !int.TryParse(TxtMaxLength.Text, out int max) || min > max)
            {
                MessageBox.Show("Veuillez saisir des longueurs valides (Min <= Max).", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(TxtOutputPath.Text))
            {
                MessageBox.Show("Veuillez choisir un emplacement de sortie.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string charset = BuildCharset();
            if (string.IsNullOrEmpty(charset))
            {
                MessageBox.Show("Veuillez sélectionner au moins un jeu de caractères.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            long totalCombinations = CalculateTotal(charset.Length, min, max);

           
            string finalPath = TxtOutputPath.Text;
            BtnGenerate.IsEnabled = false;
            ProgressBarGen.Value = 0;
            LblPercentage.Visibility = Visibility.Visible;
            LblStatus.Text = "Génération en cours...";
            await Task.Run(() => StartGeneration(charset, min, max, finalPath, totalCombinations));
            LblStatus.Text = "Terminé !";
            BtnGenerate.IsEnabled = true;
            MessageBox.Show($"Le dictionnaire a été généré avec succès dans :\n{finalPath}", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        
        // Construit la chaîne de caractères à utiliser pour la génération.
        private string BuildCharset()
        {
            StringBuilder sb = new StringBuilder();
            if (ChkLower.IsChecked == true) sb.Append("abcdefghijklmnopqrstuvwxyz");
            if (ChkUpper.IsChecked == true) sb.Append("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
            if (ChkDigits.IsChecked == true) sb.Append("0123456789");
            if (ChkSpecial.IsChecked == true)
            {
                
                sb.Append(@"!#$%&'()*+,-./:;<=>?@[\]^_`{|}~");
                sb.Append("\""); 
            }
            sb.Append(TxtCharacters.Text); 

            
            return new string(sb.ToString().Distinct().ToArray());
        }

        
        // Calcule le nombre total de mots qui seront générés.
        private long CalculateTotal(int n, int min, int max)
        {
            long total = 0;
            for (int i = min; i <= max; i++)
            {
                total += (long)Math.Pow(n, i);
            }
            return total;
        }

       
        // Démarre le processus d'écriture des mots générés dans le fichier.
        private void StartGeneration(string charset, int min, int max, string path, long total)
        {
            long count = 0;
            using (StreamWriter writer = new StreamWriter(path))
            {
                for (int length = min; length <= max; length++)
                {
                    GenerateRecursive("", charset, length, writer, ref count, total);
                }
            }
        }

        // Génère récursivement toutes les combinaisons de mots et met à jour l'interface.
        private void GenerateRecursive(string current, string charset, int length, StreamWriter writer, ref long count, long total)
        {
            if (current.Length == length)
            {
                writer.WriteLine(current);
                count++;

                
                if (count % 1000 == 0 || count == total)
                {
                    double progress = (double)count / total * 100;
                    Dispatcher.Invoke(() => {
                        ProgressBarGen.Value = progress;
                        LblPercentage.Text = $"{(int)progress}%";
                    });
                }
                return;
            }

            foreach (char c in charset)
            {
                GenerateRecursive(current + c, charset, length, writer, ref count, total);
            }
        }

        // Ces gestionnaires d'événements sont actuellement vides et non utilisés.
        private void ChkSpecial_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void ChkLower_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}