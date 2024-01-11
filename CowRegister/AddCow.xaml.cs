using CowRegister.Model;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace CowRegister
{
    /// <summary>
    /// Interaction logic for AddCow.xaml
    /// </summary>
    public partial class AddCow : Window
    {
        private CattleContext _context = new CattleContext();
        public AddCow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbGender.ItemsSource = Enum.GetValues(typeof(Gender)).Cast<Gender>();
            cbGender.SelectedIndex = 0;
            cbBreed.ItemsSource = Enum.GetValues(typeof(Color)).Cast<Color>();
            cbBreed.SelectedIndex = 0;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtEnar.Text) &&
                !string.IsNullOrEmpty(txtMotherEnar.Text) &&
                !string.IsNullOrEmpty(dpBirth.Text) &&
                !string.IsNullOrEmpty(dpArrived.Text) &&
                !string.IsNullOrEmpty(cbBreed.Text) &&
                !string.IsNullOrEmpty(cbGender.Text) &&
                _context.Cattle.Count(x => x.Enar == (long)Convert.ToDouble(txtEnar.Text)) == 0)
            {
                Enum.TryParse(cbGender.Text, out Gender selectedGender);
                Enum.TryParse(cbBreed.Text, out Color selectedColor);

                Cattle newCow = new Cattle
                {
                    Enar = (long)Convert.ToDouble(txtEnar.Text),
                    EarLetter = txtEarLetter.Text != "" ? Convert.ToInt32(txtEarLetter.Text) : null,
                    MotherEnar = (long)Convert.ToDouble(txtMotherEnar.Text),
                    Birth = (DateTime)dpBirth.SelectedDate,
                    Gender = selectedGender,
                    Breed = selectedColor,
                    ArrivedStock = (DateTime)dpArrived.SelectedDate,
                    AbandonedStock = dpAbandoned.SelectedDate,
                    NoteOfAbandon = txtNote.Text != "" ? txtNote.Text : null
                };

                _context.Add(newCow);
                _context.SaveChanges();

                MessageBoxResult result = MessageBox.Show("Sikeres mentés!", "Egyed Elmentve", MessageBoxButton.OK, MessageBoxImage.Asterisk);

                this.Close();
            }
            else if(_context.Cattle.Count(x => x.Enar == (long)Convert.ToDouble(txtEnar.Text)) == 1)
            {
                MessageBoxResult result = MessageBox.Show("A megadott ENAR-számmal rendelkező egyed már létezik. Törölje, vagy módosítsa azt a példányt!", "Létező Egyed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Minden kötelező adatot (vastag fekete keretű szövegdobozok) ki kell tölteni!", "Hiányzó Adatok", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void cbSameAsBirth_Checked(object sender, RoutedEventArgs e)
        {
            dpArrived.SelectedDate = dpBirth.SelectedDate;
        }

        private void cbIsInStock_Checked(object sender, RoutedEventArgs e)
        {
            dpAbandoned.SelectedDate = null;
            dpAbandoned.IsEnabled = false;
        }

        private void cbIsInStock_Unchecked(object sender, RoutedEventArgs e)
        {
            dpAbandoned.IsEnabled = true;
        }

        private void txtOnlyDigits_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void txtNoSpace_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void textBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Copy ||
                e.Command == ApplicationCommands.Cut ||
                e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }
    }
}
