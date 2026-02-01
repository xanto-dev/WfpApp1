using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // 1. Bouton Parcourir
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

        // 2. Bouton Générer (Asynchrone)
        private async void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            // Validation des entrées
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

            // Calcul du total pour la barre de progression
            long totalCombinations = CalculateTotal(charset.Length, min, max);

            // CORRECTION IMPORTANTE : 
            // On stocke le chemin dans une variable string locale AVANT de lancer le Task.
            // On le fait ici car on est encore sur le thread UI.
            string finalPath = TxtOutputPath.Text;

            // Préparation UI
            BtnGenerate.IsEnabled = false;
            ProgressBarGen.Value = 0;
            LblPercentage.Visibility = Visibility.Visible;
            LblStatus.Text = "Génération en cours...";

            // Lancement de la génération sur un thread séparé
            // On passe 'finalPath' au lieu de 'TxtOutputPath.Text'
            await Task.Run(() => StartGeneration(charset, min, max, finalPath, totalCombinations));

            // Fin
            LblStatus.Text = "Terminé !";
            BtnGenerate.IsEnabled = true;
            MessageBox.Show($"Le dictionnaire a été généré avec succès dans :\n{finalPath}", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // 3. Construction de la chaîne de caractères
        private string BuildCharset()
        {
            StringBuilder sb = new StringBuilder();
            if (ChkLower.IsChecked == true) sb.Append("abcdefghijklmnopqrstuvwxyz");
            if (ChkUpper.IsChecked == true) sb.Append("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
            if (ChkDigits.IsChecked == true) sb.Append("0123456789");
            if (ChkSpecial.IsChecked == true)
            {
                // On utilise le préfixe @ pour faciliter l'écriture, 
                // mais attention : pour le guillemet (") il faut le doubler ("")
                sb.Append(@"!#$%&'()*+,-./:;<=>?@[\]^_`{|}~");
                sb.Append("\""); // Ajoute le guillemet double
            }
            sb.Append(TxtCharacters.Text); // Caractères personnalisés

            // On enlève les doublons au cas où
            return new string(sb.ToString().Distinct().ToArray());
        }

        // 4. Calcul mathématique du total : Somme de n^i
        private long CalculateTotal(int n, int min, int max)
        {
            long total = 0;
            for (int i = min; i <= max; i++)
            {
                total += (long)Math.Pow(n, i);
            }
            return total;
        }

        // 5. Logique d'écriture et Récursivité
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

        private void GenerateRecursive(string current, string charset, int length, StreamWriter writer, ref long count, long total)
        {
            if (current.Length == length)
            {
                writer.WriteLine(current);
                count++;

                // Mise à jour de la progression (tous les 1000 mots pour ne pas ralentir)
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

        private void ChkSpecial_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void ChkLower_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}