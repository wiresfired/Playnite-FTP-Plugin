using System.Windows;
using System.Windows.Controls;

namespace ftp
{
    public partial class FtpSettingsView : UserControl
    {
        private bool showpass = true;
        private FtpSettings _settings;
        private PlatformRomsChecker _platformRomsChecker=null;

        public FtpSettingsView(FtpSettings settings, Playnite.SDK.IPlayniteAPI api)
        {
            _settings = settings;
            _platformRomsChecker=new PlatformRomsChecker(api);
            InitializeComponent();
            if (settings.FtpPassword != null)
            {
                pass.Password = settings.FtpPassword;
            }
            pass.PasswordChanged += Pass_PasswordChanged;
            txtPassword.TextChanged += TxtPassword_TextChanged;

            if (settings.CheckBoxes == null)
            {
                settings.CheckBoxes = new System.Collections.Generic.Dictionary<string, bool>();
            }
            if (settings.Extensions == null)
            {
                settings.Extensions = new System.Collections.Generic.Dictionary<string, bool>();
            }

            var platforms = _platformRomsChecker.CheckPlatformsForRoms();
            foreach (var platform in platforms)
            {
                if (!settings.CheckBoxes.ContainsKey(platform.Key)&&platform.Value==true)
                {
                    settings.CheckBoxes.Add(platform.Key,true);
                }
            }
            var extensions = _platformRomsChecker.Getextensions(settings.CheckBoxes);
            foreach (var extension in extensions)
            {
                if(!settings.Extensions.ContainsKey(extension))
                {
                    settings.Extensions.Add(extension,true);
                }
            }
        }

        private void TxtPassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtPassword.Visibility == Visibility.Visible)
            {
                pass.Password = txtPassword.Text;
            }
        }

        private void Pass_PasswordChanged(object sender, RoutedEventArgs e)
        {
            txtPassword.Text = pass.Password;
            _settings.FtpPassword = pass.Password;
        }

        private void show_Click(object sender, RoutedEventArgs e)
        {
            switch (showpass)
            {
                case true:
                    showpass = false;
                    pass.Visibility = Visibility.Collapsed;
                    txtPassword.Visibility = Visibility.Visible;
                    toggleShow.Content = "Hide";
                    break;

                case false:
                    showpass = true;
                    pass.Visibility = Visibility.Visible;
                    txtPassword.Visibility = Visibility.Collapsed;
                    toggleShow.Content = "Show";
                    break;
            }
        }
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            if (checkBox != null && _settings.CheckBoxes.ContainsKey(checkBox.Content.ToString()))
            {
                _settings.CheckBoxes[checkBox.Content.ToString()] = true;
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            if (checkBox != null && _settings.CheckBoxes.ContainsKey(checkBox.Content.ToString()))
            {
                _settings.CheckBoxes[checkBox.Content.ToString()] = false;
            }
        }
        private void EXT_Unchecked(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            if (checkBox != null && _settings.Extensions.ContainsKey(checkBox.Content.ToString()))
            {
                _settings.Extensions[checkBox.Content.ToString()] = false;
            }
        }
        private void EXT_Checked(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            if (checkBox != null && _settings.Extensions.ContainsKey(checkBox.Content.ToString()))
            {
                _settings.Extensions[checkBox.Content.ToString()] = true;
            }
        }
    }
   

    }
