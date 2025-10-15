using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using Illustra.Events;
using MahApps.Metro.Controls;

namespace Illustra.Views
{
    // MainWindowクラスのキーボード操作に関するpartialクラス
    public partial class MainWindow : MetroWindow
    {
        // キーボード操作のイベントを発行するメソッド
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            // テキストボックスにフォーカスがある場合はキーボードショートカットを無効化
            if (e.OriginalSource is TextBox)
            {
                System.Diagnostics.Debug.WriteLine($"[MainWindow.KeyboardHandler] キー '{e.Key}' をテキストボックス用に無効化");
                e.Handled = false;
                return;
            }

            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Z)
            {
                _eventAggregator?.GetEvent<ShortcutKeyEvent>().Publish(new ShortcutKeyEventArgs { Key = Key.Z, Modifiers = ModifierKeys.Control, SourceId = CONTROL_ID });
                e.Handled = true;
            }
        }

        // サムネイルリストから次の画像ファイルパスを取得するメソッド
        internal string? GetNextImage(string currentFilePath)
        {
            return ThumbnailList.GetNextImage(currentFilePath);
        }

        internal string? GetPreviousImage(string currentFilePath)
        {
            return ThumbnailList.GetPreviousImage(currentFilePath);
        }

    }
}
