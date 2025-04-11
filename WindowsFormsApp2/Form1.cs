using Siticone.UI.WinForms;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net; // For WebClient
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        #region DllImports and UI Setup

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect, int nTopRect,
            int nRightRect, int nBottomRect,
            int nWidthEllipse, int nHeightEllipse
        );

        public Form1()
        {
            InitializeComponent();

            // Make the form borderless and optionally rounded.
            this.FormBorderStyle = FormBorderStyle.None;
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));

            // Auto-elevate if not running as administrator.
            EnsureElevatedPrivileges();
        }

        #endregion

        #region Elevation Logic

        private void EnsureElevatedPrivileges()
        {
            if (!IsAdministrator())
            {
                try
                {
                    var startInfo = new ProcessStartInfo
                    {
                        FileName = Application.ExecutablePath,
                        UseShellExecute = true,
                        Verb = "runas" // Triggers the UAC prompt.
                    };
                    Process.Start(startInfo);
                    Application.Exit();
                }
                catch
                {
                    MessageBox.Show("Administrator privileges are required to run this application.",
                                    "Elevation Required", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
            }
        }

        private bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        #endregion

        #region Button Click and Spoofing Execution

        // This button click triggers the spoofing process.
        private void guna2Button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Starting spoofing process...");
            ExecuteSpoofing();
        }

        private void ExecuteSpoofing()
        {
            // Download and load drivers first.
            if (!LoadDrivers())
            {
                MessageBox.Show("Failed to load AMI drivers. Ensure the downloadable links are valid and the app is running as administrator.",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Execute the AMIDEWIN spoofing commands.
            PerformSpoofingCommands();
        }

        #endregion

        #region Downloading Drivers and Loading Them

        private bool LoadDrivers()
        {
            try
            {
                // Replace these URLs with your actual driver download links.
                string driverUrl1 = "https://files.catbox.moe/6nbaxw.sys";
                string driverUrl2 = "https://files.catbox.moe/ds7b9a.sys";

                // Create a temporary folder for the downloaded drivers.
                string tempFolder = Path.Combine(Path.GetTempPath(), "MyDriverDownloads");
                if (!Directory.Exists(tempFolder))
                {
                    Directory.CreateDirectory(tempFolder);
                }

                // Full local paths for the drivers.
                string driverPath1 = Path.Combine(tempFolder, "amifldrv64.sys");
                string driverPath2 = Path.Combine(tempFolder, "amigengdrv64.sys");

                // Download the driver files.
                using (var client = new WebClient())
                {
                    client.DownloadFile(driverUrl1, driverPath1);
                    client.DownloadFile(driverUrl2, driverPath2);
                }

                // Check if downloads succeeded.
                if (!File.Exists(driverPath1) || !File.Exists(driverPath2))
                {
                    MessageBox.Show("Driver files failed to download.",
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // Remove any previous driver installations.
                RemoveDriver("amifldrv64");
                RemoveDriver("amigengdrv64");

                // Install and start the downloaded drivers.
                if (!InstallAndStartDriver("amifldrv64", driverPath1))
                    return false;
                if (!InstallAndStartDriver("amigengdrv64", driverPath2))
                    return false;

                MessageBox.Show("Drivers loaded successfully!",
                                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading drivers: {ex.Message}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        #endregion

        #region Driver Management Methods

        private void RemoveDriver(string driverName)
        {
            try
            {
                ExecuteCommand($"sc stop \"{driverName}\"");
                ExecuteCommand($"sc delete \"{driverName}\"");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to remove driver {driverName}: {ex.Message}",
                                "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // This method attempts two quoting methods for creating and starting the driver.
        private bool InstallAndStartDriver(string driverName, string driverPath)
        {
            try
            {
                // Option 1: Using nested quotes.
                string createCmd1 = $"sc create \"{driverName}\" binPath= \"\\\"{driverPath}\\\"\" type= kernel start= demand";
                Debug.WriteLine("Trying command: " + createCmd1);
                ExecuteCommand(createCmd1);
                ExecuteCommand($"sc start \"{driverName}\"");

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Nested quote method failed for {driverName}: {ex.Message}");
                // Option 2: Without extra nested quotes.
                try
                {
                    string createCmd2 = $"sc create \"{driverName}\" binPath= \"{driverPath}\" type= kernel start= demand";
                    Debug.WriteLine("Trying fallback command: " + createCmd2);
                    ExecuteCommand(createCmd2);
                    ExecuteCommand($"sc start \"{driverName}\"");
                    return true;
                }
                catch (Exception ex2)
                {
                    MessageBox.Show($"Failed to install/start driver {driverName}:\n{ex2.Message}",
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
        }

        #endregion

        #region Spoofing Commands

        private void PerformSpoofingCommands()
        {
            // Update this path to your actual location of AMIDEWINx64.EXE.
            string exePath = Path.GetFullPath(@"C:\Users\Fordc\Desktop\New folder (3)\AMIDEWINx64.EXE");

            if (!File.Exists(exePath))
            {
                MessageBox.Show($"AMIDEWINx64.EXE is missing at: {exePath}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Execute the spoofing commands.
            RunAmiCommand(exePath, $"/SU /UUID {Guid.NewGuid()}", "UUID Spoof");
            RunAmiCommand(exePath, $"/SU /CPUID {GenerateRandomSerial(16)}", "CPU ID Spoof");
            RunAmiCommand(exePath, $"/SU /DISK {GenerateRandomSerial(8)}", "Disk Serial Spoof");
            RunAmiCommand(exePath, $"/SU /BS {GenerateRandomSerial(12)}", "Baseboard Serial Spoof");
            RunAmiCommand(exePath, $"/SM \"Random Manufacturer\"", "System Manufacturer Spoof");
            RunAmiCommand(exePath, $"/SP \"Random Product\"", "System Product Spoof");
            RunAmiCommand(exePath, $"/SV \"Random Version\"", "System Version Spoof");
            RunAmiCommand(exePath, $"/SS \"{GenerateRandomSerial(10)}\"", "System Serial Number Spoof");
            RunAmiCommand(exePath, $"/BM \"Random Baseboard Manufacturer\"", "Baseboard Manufacturer Spoof");
            RunAmiCommand(exePath, $"/BP \"Random Baseboard Product\"", "Baseboard Product Spoof");
            RunAmiCommand(exePath, $"/BV \"Random Baseboard Version\"", "Baseboard Version Spoof");
            RunAmiCommand(exePath, $"/BS \"{GenerateRandomSerial(8)}\"", "Baseboard Serial Spoof");
        }

        // Updated RunAmiCommand method with detailed logging.
        private void RunAmiCommand(string exePath, string arguments, string spoofType)
        {
            try
            {
                // Construct the full command string for logging.
                string fullCommand = $"{exePath} {arguments}";
                Debug.WriteLine($"Executing AMI command: {fullCommand}");

                // Verify the executable exists.
                if (!File.Exists(exePath))
                {
                    MessageBox.Show($"The executable was not found at: {exePath}",
                                    "Executable Missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Set up the process start info.
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = exePath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (Process process = new Process())
                {
                    process.StartInfo = psi;
                    process.Start();

                    // Read the standard output and error.
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    int exitCode = process.ExitCode;
                    Debug.WriteLine($"AMI command exit code: {exitCode}");
                    Debug.WriteLine($"AMI command output: {output}");
                    Debug.WriteLine($"AMI command error: {error}");

                    // Display the result.
                    if (exitCode == 0)
                    {
                        MessageBox.Show($"{spoofType} completed successfully!\nOutput:\n{output}",
                                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show($"{spoofType} failed!\nExit Code: {exitCode}\nError:\n{error}\nOutput:\n{output}",
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during {spoofType}:\n{ex.Message}",
                                "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Utility Methods

        private void ExecuteCommand(string command)
        {
            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/c {command}",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };

                    process.Start();
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    if (process.ExitCode != 0 && !string.IsNullOrEmpty(error))
                    {
                        throw new Exception(error);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Command execution failed: {command}\n{ex.Message}");
            }
        }

        private string GenerateRandomSerial(int length)
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)])
                .ToArray());
        }

        #endregion

        #region Exit Button

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        #endregion
    }
}