using System;
using System.Windows;
using System.Windows.Input;

namespace RootApp.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
        }

        private void SetRandomPath()
        {
            Array values = Enum.GetValues(typeof(Environment.SpecialFolder));
            Random random = new Random();
            Environment.SpecialFolder specialFolder = (Environment.SpecialFolder)values.GetValue(random.Next(values.Length));
            explorer1.Path = Environment.GetFolderPath(specialFolder);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SetRandomPath();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            explorer1.Path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
        }
    }
}
