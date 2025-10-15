using Illustra.Helpers;
using Illustra.Models;

namespace Illustra.ViewModels.Settings
{
    public class ThumbnailSettingsViewModel : SettingsViewModelBase
    {
        private readonly AppSettingsModel _settings;

        private double _mouseWheelMultiplier;
        public double MouseWheelMultiplier
        {
            get => _mouseWheelMultiplier;
            set
            {
                if (_mouseWheelMultiplier != value)
                {
                    _mouseWheelMultiplier = value;
                    OnPropertyChanged(nameof(MouseWheelMultiplier));
                }
            }
        }

        private int _thumbnailCacheSize;
        public int ThumbnailCacheSize
        {
            get => _thumbnailCacheSize;
            set
            {
                if (_thumbnailCacheSize != value)
                {
                    _thumbnailCacheSize = value;
                    OnPropertyChanged(nameof(ThumbnailCacheSize));
                }
            }
        }

        public ThumbnailSettingsViewModel(AppSettingsModel settings)
        {
            _settings = settings;
        }

        public override void LoadSettings()
        {
            MouseWheelMultiplier = _settings.MouseWheelMultiplier;
            ThumbnailCacheSize = _settings.ThumbnailCacheSize;
        }

        public override void SaveSettings()
        {
            _settings.MouseWheelMultiplier = MouseWheelMultiplier;
            _settings.ThumbnailCacheSize = ThumbnailCacheSize;
        }

        public override bool ValidateSettings()
        {
            return MouseWheelMultiplier > 0 && ThumbnailCacheSize > 0;
        }
    }
}
