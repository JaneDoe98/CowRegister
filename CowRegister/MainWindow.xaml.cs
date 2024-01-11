using CowRegister.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CowRegister
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CattleContext _context = new CattleContext();

        public MainWindow()
        {
            InitializeComponent();
            _context.Database.EnsureCreated();

            ReloadList();
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            AddCow window = new AddCow();
            window.ShowDialog();

            ReloadList();

        }

        private void ReloadList()
        {
            List<Cattle> cattleList = _context.Cattle.ToList();
            var items = cattleList.OrderBy(x => x.AbandonedStock).ThenBy(x => x.Enar);
            cattleView.ItemsSource = items;

            lblCount.Content = cattleList.Count(x => x.AbandonedStock == null);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (cattleView.SelectedItems.Count == 1)
            {
                MessageBoxResult result = MessageBox.Show("A kiválasztott egyed végleg törlődik az adatbázisból. Biztosan folytatja?", "Törlés az Adatbázisból", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    Cattle selectedRow = (Cattle)cattleView.SelectedItem;
                    _context.Remove(selectedRow);
                    _context.SaveChanges();

                    ReloadList();
                }
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Először jelölje ki a törölni kívánt egyedet!", "Nincs Kijelölt Egyed", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private string savedColumnNames = "ENAR-szám | Marhalevél száma | Anyja ENAR-száma | Születési ideje | Neme | Fajtája/színe | A tenyésztésbe érkezési dátuma | A tenyésztés elhagyásának dátuma | Megjegyzés";

        private void btnBackup_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Szeretne készíteni egy biztonsági mentést az aktuális adatokból?", "Biztonsági Mentés Készítése", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.FileName = $"{DateTime.Now.ToString("yyyy-MM-dd")}_marha-biztonsági-mentés";
                dialog.DefaultExt = ".txt";
                dialog.Filter = "Szöveg fájl (.txt)|*.txt";

                if (dialog.ShowDialog() == true)
                {
                    List<Cattle> cattleList = _context.Cattle.ToList();
                    List<string> stringList = new List<string>();
                    stringList.Add(savedColumnNames);
                    foreach (Cattle cattle in cattleList)
                    {
                        string abandonDate = cattle.AbandonedStock != null ? cattle.AbandonedStock?.ToString("yyyy-MM-dd") : "állományban";
                        string note = cattle.NoteOfAbandon != null ? cattle.NoteOfAbandon : "-";

                        stringList.Add($"{cattle.Enar} | {cattle.EarLetter} | {cattle.MotherEnar} | {cattle.Birth.ToString("yyyy-MM-dd")}  | {cattle.Gender} | {cattle.Breed} | {cattle.ArrivedStock.ToString("yyyy-MM-dd")} | {abandonDate} | {note}");
                    }
                    File.WriteAllLines(dialog.FileName, stringList);
                }
            }
        }

        private void btnLoadBackup_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Szeretné betölteni az adatokat egy már létező biztonsági mentésből? Ez a művelet a jelenlegi adatok törlésével járul!", "Adatok Betöltése", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if(result == MessageBoxResult.Yes)
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.DefaultExt = ".txt";
                dialog.Filter = "Szöveg fájl (.txt)|*.txt";

                if (dialog.ShowDialog() == true)
                {
                    if (!File.ReadLines(dialog.FileName).Any())
                    {
                        MessageBoxResult emptyFile = MessageBox.Show("A kiválasztott fájl üres. Válasszon ki egy másik fájlt! A jelenlegi adatok nem kerültek törlésre.", "Üres Fájl", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }

                    string columnLine = File.ReadLines(dialog.FileName).First();

                    if (columnLine != savedColumnNames)
                    {
                        MessageBoxResult notSaveFile = MessageBox.Show("A kiválasztott fájl nem egy mentési fájl. Válasszon ki egy másik fájlt! A jelenlegi adatok nem kerültek törlésre.", "Nem Mentési Fájl", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }

                    _context = new CattleContext();
                    _context.Database.ExecuteSqlRaw("DELETE FROM Cattle");

                    _context.SaveChanges();

                    string[] rows = File.ReadAllLines(dialog.FileName);

                    rows = rows.Skip(1).ToArray();

                    foreach(string row in rows)
                    {
                        string[] columns = row.Split(" | ");

                        Enum.TryParse(columns[4], out Gender selectedGender);
                        Enum.TryParse(columns[5], out Color selectedColor);
                        try
                        {
                            Cattle newCow = new Cattle
                            {
                                Enar = (long)Convert.ToDouble(columns[0]),
                                EarLetter = columns[1] != "" ? Convert.ToInt32(columns[1]) : null,
                                MotherEnar = (long)Convert.ToDouble(columns[2]),
                                Birth = Convert.ToDateTime(columns[3]).Date,
                                Gender = selectedGender,
                                Breed = selectedColor,
                                ArrivedStock = Convert.ToDateTime(columns[6]).Date,
                                AbandonedStock = columns[7] != "állományban" ? Convert.ToDateTime(columns[7]).Date : null,
                                NoteOfAbandon = columns[8] != "" && columns[8] != "-" ? columns[8] : null
                            };

                            _context.Add(newCow);
                        }
                        catch(System.FormatException)
                        {
                            MessageBoxResult wrongFormat = MessageBox.Show("A kiválasztott fájl mentési fájlnak tűnik, de a formátuma hibás. Válasszon ki egy másik fájlt!", "Hibás Formátum", MessageBoxButton.OK, MessageBoxImage.Error);
                            ReloadList();

                            return;
                        }
                    }

                    _context.SaveChanges();
                    ReloadList();
                }
            }
        }



        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                cattleView.Items.Filter = new Predicate<object>(SearchFilter);
            }
            else
            {
                cattleView.Items.Filter = null;
            }

        }

        private bool SearchFilter(object obj)
        {
            Cattle? x = obj as Cattle;
            return x.Enar.ToString().Contains(txtSearch.Text);
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (cattleView.SelectedItems.Count == 1)
            {
                Cattle selectedRow = (Cattle)cattleView.SelectedItem;
                ModifyCow window = new ModifyCow(selectedRow);
                window.ShowDialog();

                lblCount.Content = _context.Cattle.ToList().Count(x => x.AbandonedStock == null);
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Először jelölje ki a módosítani kívánt egyedet!", "Nincs Kijelölt Egyed", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
