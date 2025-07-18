using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.Shell.Interop;
using Props2JsonExtension.ExtensionOptions;

namespace Props2JsonExtension;

[Command(PackageIds.MyCommand)]
internal sealed class Props2Json : BaseCommand<Props2Json>
{
    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

        DTE2 dte = (DTE2)ServiceProvider.GlobalProvider.GetService(typeof(DTE));
        Document activeDoc = dte?.ActiveDocument;

        if (activeDoc == null)
        {
            await VS.MessageBox.ShowAsync("No active document!");
            return;
        }

        var selection = (TextSelection)dte.ActiveDocument.Selection;

        string selectedText = selection.Text;

        if (string.IsNullOrWhiteSpace(selectedText))
        {
            await VS.MessageBox.ShowAsync("No text selected!");
            return;
        }

        // 2. Roslyn parse
        string codeToParse = $"class DummyClass {{ {selectedText} }}";
        var tree = CSharpSyntaxTree.ParseText(codeToParse);
        var root = tree.GetRoot();
        var properties = root.DescendantNodes().OfType<PropertyDeclarationSyntax>().ToList();

        // 3. Build JSON
        var jsonPairs = new List<string>();
        foreach (var prop in properties)
        {
            string type = prop.Type.ToString();
            string name = prop.Identifier.Text;
            string jsonName = char.ToLowerInvariant(name[0]) + name.Substring(1);

            string value;
            bool isListType =
                type.StartsWith("IEnumerable")
                || type.StartsWith("List")
                || type.StartsWith("ICollection")
                || type.StartsWith("IReadOnlyList");

            if (isListType)
            {
                value = "[]";
            }
            else
            {
                value = type switch
                {
                    "string" => "\"\"",
                    "int" or "long" or "short" or "float" or "double" or "decimal" => "0",
                    "bool" => "false",
                    _ => "null",
                };
            }

            jsonPairs.Add($"  \"{jsonName}\": {value}");
        }
        string jsonOutput = "{\n" + string.Join(",\n", jsonPairs) + "\n}";

        // 4. Copy to clipboard
        Clipboard.SetText(jsonOutput);

        var options = (Props2JsonOptions)Package.GetDialogPage(typeof(Props2JsonOptions));

        // Notify the user if the option is enabled
        if (options.ShowNotifications)
        {
            // Show the message box
            VsShellUtilities.ShowMessageBox(
                Package,
                "JSON copied to clipboard:\n\n" + jsonOutput,
                "CSharp Props to JSON",
                OLEMSGICON.OLEMSGICON_INFO,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST
            );
        }
    }
}
