using System.Runtime.InteropServices;

namespace Props2JsonExtension.ExtensionOptions;

[ProvideOptionPage(
    typeof(Props2JsonOptions),
    "Props2Json Extension", // Category name in Tools > Options
    "General", // Page name
    0,
    0,
    true
)]
[ComVisible(true)]
public class Props2JsonOptions : DialogPage
{
    private bool _showNotifications = true;

    public bool ShowNotifications
    {
        get => _showNotifications;
        set => _showNotifications = value;
    }
}
