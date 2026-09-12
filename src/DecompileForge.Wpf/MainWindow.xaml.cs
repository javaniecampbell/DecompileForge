using System.Windows;
using DecompileForge.Wpf.ViewModels;
namespace DecompileForge.Wpf;

public partial class MainWindow : Window { public MainWindow(MainViewModel vm) { InitializeComponent(); DataContext = vm; } }
