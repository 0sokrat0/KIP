using Avalonia.Controls;
using pr8.ViewModels;

namespace pr8;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    
    private void RemoveFromCartButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is int productId && DataContext is MainWindowViewModel vm)
        {
            vm.RemoveFromCartByProductId(productId);
        }
    }
}