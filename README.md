## FTP Plugin Readme
***This Will Download The Entire Contents of the selected game folder***
***It Only keeps the currently downloaded rom file on disk, it removes previous cache*****
### Overview

This is an extension for Playnite that allows users to Download Remote ROM files from an FTP server that is alo a SAMBA share server. Roms on the Samba share must be scanned into Playnite (ie: mapped network drive). The ServerBasePath should be the actual location of the files on the FTP server).
(highly reccomend excluding files from checksum scan if on network)

FTP path must be set correctly, the path to the roms folder on the FTP servers filesystem
FTP address must be set correctly
FTP port must be set correctly
as well as FTP user and password 
(Password is encrypted by windows user account before plugins settings are saved) **Password Security**
### Installation

1. **Download the Plugin**: Download the latest version of `FTP_Plugin.dll` from the release page or repository.
2:
   - install the plugin using the .pext file
3. **Restart Playnite**: Restart Playnite to load the new extension.

### Configuration

1. **Access Plugin Settings**:
   - Open Playnite.
   - Go to `Main Menu`>`Addons`>`Extension Settings`>`Generic`>`FTP_Plugin`.
2. **Set FTP Credentials**:
   - Enter the FTP server address, server port, username, and password.
   - set the base path where your ROM files(mapped network drive) lives on the FTP server.
   - set the temporary directory (c:/temp)
   - adjust the minimum file size
3. **Select Platforms**:
   - Check the boxes next to the platforms you want to support with this plugin.
### Usage
Anything you launch is checked to see if it exists on a network drive -> then it is checked against platform ->then it is checked against filename extension. If these 3 conditions are set to true (ie: checked in the settings) then connect to ftp server and with the right settings and credentials, the folder will download and launch when the progressbar is completed. 


### Troubleshooting
  **Check the extensions log file -> %appdata%/playnite
- **Connection Issues**: Ensure that your FTP server details are correct and that the server is accessible.
- **Permissions**: Make sure that the FTP user has the necessary permissions to read/write files on the server.
- **Network Issues**: Sometimes network problems can cause downloads to fail. Try again later or contact your network administrator.

### Contributing

We welcome contributions to improve and expand the functionality of the FTP Plugin. If you find any issues or have suggestions, feel free to open an issue on our GitHub repository.

### Contact

WiresFired

---

**Note:** Always ensure that your FTP credentials are kept secure and not shared with others..
