using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FilePathOnFooter;

public sealed class FilePathBottomMargin : IWpfTextViewMargin
{
    public const string MarginName = "FilePathBottomMargin";

    private readonly TextBox _textBox;
    private readonly ITextDocument _textDocument;
    private bool _isDisposed;

    public FilePathBottomMargin(ITextDocument textDocument)
    {
        _textDocument = textDocument;

        _textBox = new TextBox
        {
            Text = _textDocument.FilePath,
            IsReadOnly = true,
            BorderThickness = new Thickness(0),
            Padding = new Thickness(4, 2, 4, 2),
            FontSize = 12,
            VerticalAlignment = VerticalAlignment.Center
        };

        _textBox.SetResourceReference(Control.BackgroundProperty, EnvironmentColors.ToolWindowBackgroundBrushKey);
        _textBox.SetResourceReference(Control.ForegroundProperty, EnvironmentColors.ToolWindowTextBrushKey);

        _textBox.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
        _textDocument.FileActionOccurred += OnFileActionOccurred;
    }

    private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 3)
        {
            _textBox.SelectAll();
            e.Handled = true;
        }
    }

    private void OnFileActionOccurred(object sender, TextDocumentFileActionEventArgs e)
    {
        if (e.FileActionType == FileActionTypes.DocumentRenamed)
        {
            _textBox.Text = _textDocument.FilePath;
        }
    }

    public FrameworkElement VisualElement => _textBox;

    public double MarginSize => _textBox.ActualHeight;

    public bool Enabled => true;

    public ITextViewMargin GetTextViewMargin(string marginName)
    {
        return string.Equals(marginName, MarginName, StringComparison.OrdinalIgnoreCase) ? this : null;
    }

    public void Dispose()
    {
        if (!_isDisposed)
        {
            _textBox.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
            _textDocument.FileActionOccurred -= OnFileActionOccurred;
            _isDisposed = true;
        }
    }
}
