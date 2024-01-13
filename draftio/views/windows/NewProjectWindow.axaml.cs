using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.VisualTree;
using draftio.models.dtos;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Threading.Tasks;

namespace draftio;

public partial class NewProjectWindow : Window
{
    Window? parent;

    Grid grid;
    StackPanel stackPanelInput;
    StackPanel stackPanelLabel;

    TextBlock projectNameLabel;
    TextBlock widthLabel;
    TextBlock heightLabel;
    TextBlock dimensionLabel;

    TextBox projectNameTextBox;
    TextBox widthTextBox;
    TextBox heightTextBox;
    ComboBox dimensionComboBox;
    Button createButton;
    Button closeButton;

    int width_window = 400;
    int height_window = 330;

    double height = 32;

    public NewProjectWindow()
    {
        InitializeComponent();
        SetWindow();
    }

    public void SetWindow()
    {
        Title = "Create New Project";
        this.Width = width_window;
        this.Height = height_window;
        this.MinWidth = width_window;
        this.MinHeight = height_window;
        this.MaxWidth = width_window;
        this.MaxHeight = height_window;
        this.CanResize = false;
        this.ExtendClientAreaChromeHints = Avalonia.Platform.ExtendClientAreaChromeHints.NoChrome;
        this.ExtendClientAreaToDecorationsHint = false;

        SetPanel();
        SetInputElements();
        SetLabelElements();
    }

    private void SetPanel()
    {
        this.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;

        grid = new Grid();
        grid.ColumnDefinitions = new ColumnDefinitions("*,20,*");
        grid.Margin = new Thickness(30);
        
        stackPanelLabel = new StackPanel();
        stackPanelLabel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
        stackPanelLabel.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;
        Grid.SetColumn(stackPanelLabel, 0);

        stackPanelInput = new StackPanel();
        stackPanelInput.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
        stackPanelInput.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;
        Grid.SetColumn(stackPanelInput, 2);

        grid.Children.Add(stackPanelInput);
        grid.Children.Add(stackPanelLabel);

        Content = grid;
    }

    private void SetInputElements()
    {
        projectNameTextBox = new TextBox();
        projectNameTextBox.BorderBrush = Brushes.Black;
        projectNameTextBox.Height = height - 2;
        projectNameTextBox.Watermark = "Project Name..";
        projectNameTextBox.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
        projectNameTextBox.BorderThickness = new Thickness(1);

        widthTextBox = new TextBox();
        widthTextBox.BorderBrush = Brushes.Black;
        widthTextBox.Height = height - 2;
        widthTextBox.Watermark = "1920px";
        widthTextBox.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
        widthTextBox.BorderThickness = new Thickness(1);

        heightTextBox = new TextBox();
        heightTextBox.Height = height - 2;
        heightTextBox.BorderBrush = Brushes.Black;
        heightTextBox.Watermark = "1080px";
        heightTextBox.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
        heightTextBox.BorderThickness = new Thickness(1);

        dimensionComboBox = new ComboBox();
        dimensionComboBox.Height = height - 2;
        dimensionComboBox.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;

        createButton = new Button();
        createButton.Padding = new Thickness(10);
        createButton.Height = height + 10;
        createButton.Background = Brushes.DarkBlue;
        createButton.Foreground = Brushes.White;
        createButton.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
        createButton.Content = "Create";

        stackPanelInput.Children.Add(projectNameTextBox);
        stackPanelInput.Children.Add(NPSeparator());
        stackPanelInput.Children.Add(widthTextBox);
        stackPanelInput.Children.Add(NPSeparator());
        stackPanelInput.Children.Add(heightTextBox);
        stackPanelInput.Children.Add(NPSeparator());
        stackPanelInput.Children.Add(dimensionComboBox);
        stackPanelInput.Children.Add(NPSeparator());
        stackPanelInput.Children.Add(createButton);

    }

    private void CloseButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.CloseWindow();
    }

    private void SetLabelElements()
    {
        projectNameLabel = new TextBlock();
        projectNameLabel.Text = "Project Name";
        projectNameLabel.Height = height;
        projectNameLabel.Padding = new Thickness(5);
        projectNameLabel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
        projectNameLabel.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;

        widthLabel = new TextBlock();
        widthLabel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
        widthLabel.Padding = new Thickness(5);
        widthLabel.Text = "Width (px)";
        widthLabel.Height = height;
        widthLabel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;

        heightLabel = new TextBlock();
        heightLabel.Text = "Height (px)";
        heightLabel.Padding = new Thickness(5);
        heightLabel.Height = height;
        heightLabel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;

        dimensionLabel = new TextBlock();
        dimensionLabel.Text = "Dimensions";
        dimensionLabel.Padding = new Thickness(5);
        dimensionLabel.Height = height;
        dimensionLabel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;

        closeButton = new Button();
        closeButton.Height = height + 10;
        closeButton.Padding = new Thickness(10);
        closeButton.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
        closeButton.Content = "Close";
        closeButton.Click += CloseButton_Click;

        stackPanelLabel.Children.Add(projectNameLabel);
        stackPanelLabel.Children.Add(NPSeparator());
        stackPanelLabel.Children.Add(widthLabel);
        stackPanelLabel.Children.Add(NPSeparator());
        stackPanelLabel.Children.Add(heightLabel);
        stackPanelLabel.Children.Add(NPSeparator());
        stackPanelLabel.Children.Add(dimensionLabel);
        stackPanelLabel.Children.Add(NPSeparator());
        stackPanelLabel.Children.Add(closeButton);
    }

    private Separator NPSeparator()
    {
        Separator separator = new Separator();

        separator.Height = 20;
        separator.BorderThickness = new Thickness(0);
        separator.Background = Brushes.Transparent;
        separator.Margin = new Avalonia.Thickness(0);

        return separator;

    }

    public override void Show()
    {

        base.Show();
    }

    public void CloseWindow()
    {
        this.Close();
    }

}