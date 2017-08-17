using KursTest;

namespace Kursavaya 
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            PageMain pageMain = new PageMain();
            Content = pageMain;
        
        }
    }
}
