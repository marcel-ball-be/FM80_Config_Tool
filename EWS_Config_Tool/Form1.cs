using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Security.Cryptography;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using Microsoft.WindowsAPICodePack.Shell;
using System.Media;
using Microsoft.Win32;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NAudio.Wave.Compression;
using NAudio.Wave.Asio;




/*-----------------------------------------------------------------------------------------
Author: Neel Sahdeo
For: EWS : Ivan Carter


Libraries Used: 
1. Log4Net : install via Nuget
2. NewtonSoft JSON: install via Nuget


Version Control
2016_02_05: V0.002: Basic GUI, for Button 1, Instructions, installed Log4Net
2016_02_06: V0.003: Button 1-6 tabs done: Left dataview
2016_02_07: V0.004: Button 1-6 tabs GUI shell done with arrows and dgv's
2016_02_08: V0.005: staions Tab GUI shell done : TODO make freq a collection class
2016_02_10: V0.006: Fixed Layout issue on tab 3 to 6 buttons, 
                    Added Newtonsoft, Json.net via PM Nuget
                    Added JSON Language class creator and load
2016_02_12: V0.007: Create basic Output, channel class, 
                    TODO: FIX File Open / Save Dialog with USB only filter
2016_02_15: V0.008: formatted JSON language file.
2016_02_17: V0.009: populate Instructions combo box languages from loaded JSON
                    Create template output.json file from Output class
                    Create template channels.json file from Channels class
2016_02_17: V0.010: Create a Main_Config to hold everything we want to do, this keeps everything separated from the GUI.
2016_02_18: V0.011: Create an Instruction Record and add it to the collection, then bind to dgv
2016_02_18: V0.012: Clean up Add instruction code to binding list, for auto updates in dgv, 
                    This is Databound to the Instructions List
2016_02_18: V0.013: Instructions Add also add Name / Language of Instruction to the 1-6 button Tabs
                    Updated from the same collection on Add and Remove
                    Manual Column headings are removed, as these are now databound
2016_02_20: V0.014: Save / load Instructions created
2016_02_24: V0.015: Buttons 1-6 tab move from list of instructions to each button instruction
                    Create pool of instructions when loading excluding the assigned instructions to each button
                    Save load controls for buttons
2016_02_26: V0.016: DGV full row select
                    Language must display the full Language name
                    Setup Default Language, 
                    save / load automatic field - load into checkbox
2016_02_27: v0.017: Frequency in Mhz display, but store in Khz
2016_02_27: v0.018: Bind Main freq to dgv, and exclude, save only exclude
2016_02_28: v0.019: Fixes from IC review
                    - On Add click, all entry boxes are cleared.
                    - Max 8 characters for Authority
                    - Language in Json is full word
                    - Stop user from using delete key on buttons tab (only use arrows, as initially spec'd)
2016_02_28: v0.020  - Language = Undetermined
2016_03_01: v0.021  - Station include combo box
2016_03_01: v0.022  - Clear Audio file label when instr added
                    - Pre-lim work on Freq include stations tab
2016_03_01: v0.023  - Needs testing can load / save stations file
2016_03_03: v0.024  - When loading frequencies, also remove included from main pool
2016_03_03: v0.025  - Save all to drive
                    - Temp path for all files (config, output and audio) 
2016_03_03: v0.026  - Language File Loaded Application Startup path
                    - Updated Auto Level Field
                    - Added Usb Browser
2016_03_11: v0.027  - Saving Language in Id, but displaying in Full
2016_03_12: v0.028  - Saving Language in Id, but displaying in Full fixed some exceptions, add clone interface
                    - Load Id lang but display full
                    - Load All from USB Drive
2016_03_12: v0.029  - Save All to USB Drive
2016_03_12: v0.029  - Save output only
                    - Save config only
2016_03_12: v0.030  - Testing
                    - Test Case 1: Start clean
                                    Create instructions
                                    Add to buttons
                                    Add stations
                                    Save all

                    - Test Case 2: Load config.ews created above
                                   Verify All tabs populated correctly
                                   Add new instruction and save all

                    - Test Case 3: Load config.ews created in 1
                                   Verify All tabs populated correctly
                                   Add new instruction and save stations only

                    - Test Case 4: Load config.ews created in 1
                                   Verify All tabs populated correctly
                                   Add new instruction and save channels only

2016_03_30: v0.031 - Bugs fixes
                    - Save all with selcted empty row in instructions : Fixed
                    - Max 8 characters for Authority : Fixed
                    - No spaces in name : Fixed
                    - Stations Tab : Freq in Mhz : Fixed
                    - Sorted Datagridview on Stations tab - dataGridView_stations_all_freq upon add/ remove : Fixed

2016_03_31: v0.032 - Bugs fixes
                    - Authority had language values in jason file : Fixed
                    - Addition: Save all / Load / status on statusbat

2016_04_6: v0.033 - sort languages
2016_04_7: v0.034 - channels heading added
2016_05_10: v0.035 - delete tempfiles on exit
2016_05_11: v0.037 - truncate to 8 chars only for authority, was trancating to 7 (bug)
                   - delete on insructions in pool removes from button tabs as well

2016_05_11: v0.038 - remove special chars from Authority

2016_05_19: v0.039 - NOT TEsted or completed
                    save and config from usb\config\config.ews
                    basically added a folder called "config"

2016_05_16: v0.040   - Added MD5 hash of all files being saved. 
                     - Added serial number tab that opens all all files in SN folder and populates a data grid view with serial number and FPGA unlock code
                     - fpga code is appended to the end of the MD5 generated previously and that string is then MD5 hashed.  A new json file is created with an array of serial numbers and related MD5.
                     - Icon changed to EWS icon.

Notes:

    1. Load All
       - Open UsbBrowser
       - Get selected Path (ok click, else do nothing (cancel)
       - Check for config.ews (zip file), if found unzip to TempConfig folder
       - Load all from tempconfig
                 
    2. Save All
       - Open UsbBrowser
       - Get selected Path (ok click, else do nothing (cancel)
       - Zip all files in TempConfig Folder to config.ews
       - Copy config.ews (zip file) to UsbPath

    3. Save Config Only
       - Open UsbBrowser
       - Get selected Path (ok click, else do nothing (cancel)
       - Only output.json + audio files in zip

    4. Save Button Only
       - Open UsbBrowser
       - Get selected Path (ok click, else do nothing (cancel)
       - only channel.json in zip

    2016_06_15: v0.041  - Instructions are now removed from button instruction lists when the instruction is deleted from the instruction tab. FMM-404
                        - When a config is loaded and it includes included channels, those included channels are now also added to the binding list: Frequencies_Included.  This fixes the bug with an unhandled exception when a config is loaded and included channels deleted.  FMM-403
                        - Removed all files from file filter when loading sound file.  Only option now is .wav files
                        - Save Stations Only and Save Buttons Instructions Only menus have been hidden from the user
                        - Added text box for fast selection of frequency
                        - Auto station is now implemented in the output to the channels.json file.
                        - Menu option "Prepare USB" has been added.  When selected it creates a directory named "log" on the selected USB stick
                        - Menu Option "Backup" implemented.  When selected it will backup the current configuration to any user selected folder.  Folder is selected by using the standard windows save dialog box
                        - When the wav file is selected by the user it is now checked that it is compatible.  The sample rate and number of channels are evaluated.  The file is valid if it is mono with a sample rate of 88200 or it is stereo with a sample rate of 44100.  If the file is invalid the user is notified via a message box.
                        - When the instruction list row header is clicked the audio file associated with that row wil be played.
                        - When stations selected for Included or Excluded stations the highlighted row of the all station dgv is now the frequency next to the selected frequency, ie the top of the table is no longer selected.  When a frequncy is  returned to the frequency pool that frequency row is now highlighted.
                        - Can now restore backup from any file location
                        - Now checking that all used languages have an associated instruction.  Saving to USB is not possible if any languages used don't have an associated intruction.  The user is prompted to when backing up and can chose to save or not.
                        - Added play button and extra column in instruction list to preview selected audio file.
    
    2016_6_22:  v0.041  - Added icons for all menu actions
                        - Changed the arrow picture used for adding button instructions and station include/exclude buttons.  Buttons are only visible when their actions are possible

    2016_6_23:  v0.041  - Added Titles and icons to message boxes to make them a little more professional looking.
                        - Can no longer save a configuration if no instructions have been assigned to buttons.  Can backup with a warning that it is an invalid configuration.
                        - All buttons with instructions now check they have an intruction for all included frequency languages...unless the language is undetermined.
                        - The included frequency list now highlights any frequencies with languages that don't have an isntruction in that language for all used buttons.
                        - Added tool tips to included channel rows that don't have an instruction for the included language.  Tool tip indicates the button and language problem.
                        - Added advanced tab menu and toolstrip
                        - Advanced settings enabled in included frequncies dgv.  Auto can be changed just by clicking it, level can be entered from 1 to 100, frequency can no longer be edited.
                        - Advanced settings are password protected.  Password stored in Password.txt
                        - Can now open key files from any location
                        
    2016_7_11:  v0.041  - Last folder opened to import key files is now saved in FBD.txt and used next time the folder browser is used.
                        - Added Manifest file
                        - Modified code so the main form is visible before the load key file dialogue is visible

    2016_7_18:  v0.042  - Changed the file sorting when SHA1 files to ordinal so the file order matches the sorting wth the alphasort in the FM80 app.  FMM-426
                        - Added a check when adding a wav file to an instruction to make sure a file of the same name has not already been selected.
                        - Main form title from EWS Configuration Tool to Radiolert Configuration Tool.  FMM-424
                        - Added an exit command to the drop down menu and check when closing to ensure changes are not inadvertently lost.  FMM-425

    2016_7_19:  v0.042  - Unhandled exception when saving config file to USB is now handled.  FMM-427
                        - Fixed unhandled exception when the cell of the instruction table that has been clicked had a column or row index < 0.  FMM-428
                        - Added check to make sure security keys have been loaded before a config file can be saved to USB.  If no keys are loaded the config file can be saved using the backup command.  FMM431
                        - Added try/catch to deal with corrup audio files.  FMM-430

    2016-7-22:  v0.043  - password and last folder used info is now stored in properties.settings instead of in text files

    2016-9-08:  v1.0    - Added chck to ensure only one insruction for each language is used in each button.
                        - Removed always in always included.

    2016-9-20:  v1.0    - Changed Languages.json from content to a resource

    2017-1-20:  v1.01   - Changed automatic stations to auto level by default
                        - Added menu item and icon to load key files after start up.
                        - Added play buttons to button tables

    2017-1-30: v1.01    - Wav files are now deleted from the audio file list when an instruction is deleted
                        - Included channels are now always Auto level by default, even when the Auto Station Selection is not ticked.  Can only change to fixed power through advanced settings

    2017-3-8:  v1.01    - MP3 files and incompatible wav files an now be concerted to compatible wav files
                        - Silence added to the start and end of the audio files as they are selected.

    2017-6-21:  v1.02   - Fixed bug where auto station level got set to no if checkbox deselected and never returned to yes if checkbox reselected.

    2017-6-26:  v1.03   - If an isntruction does not have a name when the audio file is selected the Instruction Name will automatically be populated with the Audio file name
                        - Moved default language drop down box so it is always visible

    2017-7-27:  v1.04   - When loading key files we were filtering files by making sure they started with "FM80".  If a folder was selected that had files that met this criteria but was not 
                          actually a key file a blank row was added to the Serial Numbers DGV.  This meant that we could save configurations even if there were no valid key files loaded.
                          We now also make sure the Json objects after deserialisation are not null.  This confirms the file is actually a key file.

    2017-10-05:  v1.04  - Added enhanced station configuration items
                        - Added location drop down box that automatically sets frequency range and spacing, and the default language.
                          The Languages.Json has also been modified to include location info.
                        - Changed labelling as requested from marketing
                        - Added the auto stations check box to the advanced settings - hidden unless advanced setting enabled

    2017-10-24  v1.05   - Fixed vug when opening saved configurations.  Bug introduced with the change in the Languages file to include locations
    
    2017-11-23  v1.06   - Paste alpha characters into the level cells of the included stations now handled
                        - Check added to ensure each used button has an instruction that uses the default language.  
              
    2017-11-27  v1.06   - Tidied input validation for instruction name and audiofilename text boxes     
    
    2018-02-02  v1.07   - Arguments now passed to main at start-up.  If application started by double clicking on a config file then that config will now be loaded when the application starts.
                        - Had to change the way the current directory was determined because if started by double clicking a file, the current directory was now the directory of the clicked file rather than the application.
                        - This meant that the languages file was not found and an execption thrown.
                        
    2018-02-26  v1.07   - Added parameters to the output cfg to enable periodic stop of message transmission.     
    
    2018-03-01  v1.07   - Added tool tip to the periodic pause check button.  Tool tip could be used for any other control as well - just need to add text in the main form load event.

    2018-03-16  v1.07   - Periodic stop of transmission renamed to C-Mode (Congestion Mode)
                        - Minimum speed now able to swap between MPH and KPH by clicking on the label
                        - New parameter added for C-Mode so we can set the hysteresis for the minimum speed.  ie Need to be below the minimum speed for the set amount of time before C-Mode activated.

    2018-03-27  v1.07   - C-Mode disabled for button 6

    2019-06-25  v1.08   - Added a column to the intcluded channels for TA enable/disable
                        - Added a config version parameter to the channel and output json files (FMM-594)
                        - Fixed error with initial frequency selection as reported by Ralph

    2019-06-27  v1.09   - Accidentally left save stations only and save button instructions only in the drop down menu

    ????-??-??  v1.10   - Proof of concept version.  No details.

    2022-08-22  v1.11   - Added location "Extended" that enables extended stations down to 82.0MHz
-----------------------------------------------------------------------------------------*/



namespace EWS_Config_Tool
{
    public partial class EWS_MainForm : Form
    {
        public const Int32 WAVSTEREOSAMPLERATE = 44100;
        public const Int32 WAVMONOAMPLERATE = 88200;
        public const Int16 MONOCHANNELS = 1;
        public const Int16 STEREOCHANNELS = 2;
        public const Int16 AUDIOFILECOLUMNINDEX = 2;
        public const Int16 PLAYCOLUMNWIDTH = 20;
        public const Int16 NUMBEROFINSTRUCTIONTABLECOLUMNS = 5;
        public const Int16 NUMBEROFBUTTONINSTRUCTIONTABLECOLUMNS = 3;
        public const Int16 PLAYCOLUMNINDEX = 1;
        public const Int16 BUTTONPLAYCOLUMNINDEX = 2;
        public const Int16 AUTOLEVELCOLUMNINDEX = 3;
        public const Int16 LEVELCOLUMNINDEX = 4;
        public const Int16 TACOLUMNINDEX = 5;
        public const Int16 FREQCOLUMNINDEX = 0;
        public const Int16 LANGUAGECOLUMNINDEX = 1;
        public const Int16 PREFERREDCOLUMNINDEX = 2;
        public const Int16 AUDIO_SILENCE_LENGTH_START = 500;
        public const Int16 AUDIO_SILENCE_LENGTH_END = 250;
        public const Int16 REQUIREDBITSPERSAMPLE = 16;

        public const Int16 SPACE_KEY_VALUE = 32;
        public const Int16 FORWARD_SLASH_KEY_VALUE = 191;
        public const Int16 DASH_KEY_VALUE = 189;
        public const Int16 ZERO_KEY_VALUE = 48;
        public const Int16 NINE_KEY_VALUE = 57;
        public const Int16 A_KEY_VALUE = 65;
        public const Int16 Z_KEY_VALUE = 90;

        public const Int16 DEFAULT_MESSAGE_BREAK_DURATION = 60;
        public const Int16 DEFAULT_MINIMUM_SPEED_FOR_MESSAGE_BREAK = 10;
        public const Int16 DEFAULT_C_MODE_HYSTERESIS_VALUE = 30;

        public const Int16 MAXIMUM_C_MODE_MINIMUM_SPEED_KPH = 100;
        public const Int16 MAXIMUM_C_MODE_MINIMUM_SPEED_MPH = 62;
        public const Int16 MINIMUM_C_MODE_MINIMUM_SPEED = 1;
        public const Int16 TEMPORARY_MAXIMUM_C_MODE_MINIMUM = 200;
        public const Int16 TEMPORARY_MINIMUM_C_MODE_MINIMUM = 0;

        public const double KPH_TO_MPH_CONVERSION_FACTOR = 0.621371;



        public const string CONFIG_FILENAME = "config.ews";
        public const string CONFIG_FILENAME_EXTENSION = ".ews";


        public string InitDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);


        Main_Config my_config; // = new Main_Config();
        public Language_JSON_Record LanguagesJsonGlobal;
        public BindingList<Language_Record> LanguagesGlobal;
        public BindingList<Location_Record> LocationGlogal;
        public Location_Record SelectedLocation;

        bool _Updating = false;
        bool _AdvancedSettings = false;
        bool _OKToCloseandExit = true;
        int frequencyColumnWidth = 70;
        int languageColumnWidth = 10;
        int preferredColumnWidth = 60;
        int autoColumnWidth = 20;
        int levelColumnWidth = 25;
        int TAColumnWidth = 1;

        public DataGridViewTextBoxColumn dgvtxt = new DataGridViewTextBoxColumn();
        public DataGridViewComboBoxColumn cmb = new DataGridViewComboBoxColumn();
        TabPage AdvancedSettings = new TabPage();
        /// <summary>
        /// This is where all temp files to be zipped and unzipped are
        /// </summary>
        string temppath = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + @"\TempConfig";
        //       string currentPath = Directory.GetCurrentDirectory() + "\\";
        string currentPath = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\";
        string ConfigMD5;
        string[] SNMD5 = new string[1000];
        string[] FPGA_Unlock_Code = new string[1000];
        string[] Serial_Numbers = new string[1000];
        string AdvancedSettingsPassword = "ews";
        string OpenStartFile = "";

        public EWS_MainForm(string StartFile)
        {
            if (StartFile != "")
            {
                OpenStartFile = StartFile;
            }

            InitializeComponent();
        }

        #region initial loading

        private void EWS_MainForm_Load(object sender, EventArgs e)
        {
            //Create tooltip
            ToolTip ttFM80_Config_Tool = new ToolTip();

            //Setup the tooltip
            ttFM80_Config_Tool.AutoPopDelay = 3000;
            ttFM80_Config_Tool.InitialDelay = 1000;
            ttFM80_Config_Tool.ReshowDelay = 500;
            ttFM80_Config_Tool.ShowAlways = true;

            //Tooltip text
            ttFM80_Config_Tool.SetToolTip(this.chb_Button1_Message_Break, "Select to periodically pause messages when low speed is detected");
            ttFM80_Config_Tool.SetToolTip(this.chb_Button2_Message_Break, "Select to periodically pause messages when low speed is detected");
            ttFM80_Config_Tool.SetToolTip(this.chb_Button3_Message_Break, "Select to periodically pause messages when low speed is detected");
            ttFM80_Config_Tool.SetToolTip(this.chb_Button4_Message_Break, "Select to periodically pause messages when low speed is detected");
            ttFM80_Config_Tool.SetToolTip(this.chb_Button5_Message_Break, "Select to periodically pause messages when low speed is detected");
            ttFM80_Config_Tool.SetToolTip(this.chb_Button6_Message_Break, "Select to periodically pause messages when low speed is detected");

            ttFM80_Config_Tool.SetToolTip(this.lblButton1MinimumSpeed, "Click here to switch between MPH and KPH");
            ttFM80_Config_Tool.SetToolTip(this.lblButton2MinimumSpeed, "Click here to switch between MPH and KPH");
            ttFM80_Config_Tool.SetToolTip(this.lblButton3MinimumSpeed, "Click here to switch between MPH and KPH");
            ttFM80_Config_Tool.SetToolTip(this.lblButton4MinimumSpeed, "Click here to switch between MPH and KPH");
            ttFM80_Config_Tool.SetToolTip(this.lblButton5MinimumSpeed, "Click here to switch between MPH and KPH");
            ttFM80_Config_Tool.SetToolTip(this.lblButton6MinimumSpeed, "Click here to switch between MPH and KPH");

            ttFM80_Config_Tool.SetToolTip(this.lblButton1SpeedHysteresisSeconds, "C-Mode will not be active until the vehicle speed has dropped below the minimum for this amount of time.");
            ttFM80_Config_Tool.SetToolTip(this.lblButton2SpeedHysteresisSeconds, "C-Mode will not be active until the vehicle speed has dropped below the minimum for this amount of time.");
            ttFM80_Config_Tool.SetToolTip(this.lblButton3SpeedHysteresisSeconds, "C-Mode will not be active until the vehicle speed has dropped below the minimum for this amount of time.");
            ttFM80_Config_Tool.SetToolTip(this.lblButton4SpeedHysteresisSeconds, "C-Mode will not be active until the vehicle speed has dropped below the minimum for this amount of time.");
            ttFM80_Config_Tool.SetToolTip(this.lblButton5SpeedHysteresisSeconds, "C-Mode will not be active until the vehicle speed has dropped below the minimum for this amount of time.");
            ttFM80_Config_Tool.SetToolTip(this.lblButton6SpeedHysteresisSeconds, "C-Mode will not be active until the vehicle speed has dropped below the minimum for this amount of time.");

            ttFM80_Config_Tool.SetToolTip(this.btnCModeApplyToAllButtons, "Apply Button 1 C-Mode settings to all other buttons");
            //this.TopMost = true;
            my_config = new Main_Config(temppath);

            
            // load all images for buttons
            if (!Directory.Exists(Path.Combine(currentPath, temppath)))
            {
                Directory.CreateDirectory(Path.Combine(currentPath, temppath));
            }

            // load language from file, once created this just needs updating
            try
            {
                LanguagesJsonGlobal = Languages.Read_Languages_Json(@currentPath + "Resources\\");
            }
            catch (Exception)
            {
                MessageBox.Show("Language_Codes.json file not found !", "json File Load Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Close();
            }

            my_config.LanguagesLoaded = LanguagesJsonGlobal.Language;

            // ------------------------------Instructions Tab --------------------------------------------------
            // populate the comboBox_Instr_Lang
            // linq : simple lamba fx to extract all full names
            comboBox_Instr_Lang.Items.AddRange(my_config.LanguagesLoaded.Select(x => x.Ref_Name).ToArray());
            comboBox_Instr_Lang.SelectedIndex = 0;
            comboBox_Instr_Default_Lang.Items.AddRange(my_config.LanguagesLoaded.Select(x => x.Ref_Name).ToArray());
            comboBox_Instr_Default_Lang.Text = "Undetermined";

            my_config.LocationsLoaded = LanguagesJsonGlobal.Location;

            //Check we have a matching language for the location in the loaded languages
            bool fLanguageMatchFound = false;
            foreach (Location_Record LoR in my_config.LocationsLoaded)
            {
                fLanguageMatchFound = false;

                foreach (Language_Record LaR in my_config.LanguagesLoaded)
                {
                    if (LoR.Default_Language == LaR.Ref_Name)
                    {
                        fLanguageMatchFound = true;
                        break;
                    }
                }

                if (fLanguageMatchFound == false)
                {
                    //No match found so set default language to undetermined
                    LoR.Default_Language = "Undetermined";
                }

            }

            //Populate the locations combo box
            cbLocation.Items.AddRange(my_config.LocationsLoaded.Select(x => x.Loc).ToArray());
            cbLocation.Text = "Undetermined";

            // bind the list to the dgv
            var instructions_full_source = new BindingSource(my_config.EWS_Config_output.INSTRUCTIONS, null);
            dataGridView_Instructions.DataSource = instructions_full_source;

            Load_Button_sources(); // in Buttons tab region

            //Stations Tab
            Load_Frequencies_sources(); // Stations tab region

            Create_Dgv_error_handler();



            // Load_Serial_Numbers(); //in Serial Number Tab Region

            try
            {
                AdvancedSettingsPassword = Properties.Settings.Default.Password;
                //string AdvancedSettingsPassword = Properties.Resources.Password;
                //AdvancedSettingsPassword = File.ReadAllText(@currentPath + "Resources\\" + "Password.txt");
            }
            catch (Exception)
            {
                MessageBox.Show("Password file not found !", "Password Read Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            // Load += new EventHandler(Load_Serial_Numbers);
            Shown += Load_Serial_Numbers;

            //If the application was started by double clicking a file and it is a valid configuration file then open the configuration
            if (OpenStartFile != "")
            {
                if (OpenStartFile.Contains(CONFIG_FILENAME_EXTENSION))
                {
                    toolStripMenuItemRestoreConfiguration_Click(null, null);
                }

                else
                {
                    MessageBox.Show("The selected file is not a valid EWS Configuration File.", "Invalid File", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            //Select 87.5 as the first selected frequency.  This fixes a bug where sometimes the first selected frequency in the frequency pool returned an incorrect frequency
            textBox_StationFrequency.Text = "87.5";
        }

        /// <summary>
        /// Freq Include list 
        /// non Data bound manual col creation and updates
        /// </summary>
        private void create_col_freq_inlcude_dgv()
        {
            dataGridView_stations_Always_Included.Columns.Clear();
            dataGridView_stations_Always_Included.Rows.Clear();

            dgvtxt.HeaderText = "Frequency";
            dgvtxt.Name = "dgvtxtFreq";
            dgvtxt.ReadOnly = true;

            cmb.HeaderText = "Language";
            cmb.Name = "cmbLanguage";

            dataGridView_stations_Always_Included.Columns.Add(dgvtxt);
            dataGridView_stations_Always_Included.Columns.Add(cmb);

            if (cmb.Items.Count == 0)
            {
                foreach (Language_Record l in my_config.LanguagesLoaded)
                {
                    cmb.Items.Add(l.Ref_Name);
                }
            }

            // we want to capture the change on combobox for language
            dataGridView_stations_Always_Included.CellValueChanged += DataGridView_stations_Always_Included_CellValueChanged;
            dataGridView_stations_Always_Included.CurrentCellDirtyStateChanged += DataGridView_stations_Always_Included_CurrentCellDirtyStateChanged; ;
        }

        private void DataGridView_stations_Always_Included_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // My combobox column is the second one 1 is the index
            DataGridViewComboBoxCell cb = (DataGridViewComboBoxCell)dataGridView_stations_Always_Included.Rows[e.RowIndex].Cells[LANGUAGECOLUMNINDEX];
            DataGridViewTextBoxCell tb = (DataGridViewTextBoxCell)dataGridView_stations_Always_Included.Rows[e.RowIndex].Cells[FREQCOLUMNINDEX];
            DataGridViewTextBoxCell au = (DataGridViewTextBoxCell)dataGridView_stations_Always_Included.Rows[e.RowIndex].Cells[AUTOLEVELCOLUMNINDEX];
            DataGridViewTextBoxCell le = (DataGridViewTextBoxCell)dataGridView_stations_Always_Included.Rows[e.RowIndex].Cells[LEVELCOLUMNINDEX];
            DataGridViewTextBoxCell pr = (DataGridViewTextBoxCell)dataGridView_stations_Always_Included.Rows[e.RowIndex].Cells[PREFERREDCOLUMNINDEX];
            DataGridViewCheckBoxCell chb = (DataGridViewCheckBoxCell)dataGridView_stations_Always_Included.Rows[e.RowIndex].Cells[TACOLUMNINDEX];

            //           DataGridViewTextBoxCell pi = (DataGridViewTextBoxCell)dataGridView_stations_Always_Included.Rows[e.RowIndex].Cells[PICODECOLUMNINDEX];

            if ((cb.Value != null) && (tb.Value != null) && (au.Value != null) && (le.Value != null))
            {
                try
                {
                    if (int.Parse(le.Value.ToString()) > 100)
                    {
                        le.Value = 100;
                    }

                    if (int.Parse(le.Value.ToString()) < 1)
                    {
                        le.Value = 1;
                    }
                }

                catch
                {
                    le.Value = 100;
                }


                // update the binding list and channel json list with the new lanaguage
                // TODO !!!! Update_Inc_freq_list(tb.Value.ToString(), cb.Value.ToString());
                my_config.Update_Include_Frequency(tb.Value.ToString(), cb.Value.ToString(), au.Value.ToString(), le.Value.ToString(), pr.Value.ToString(), (bool)chb.Value);
                dataGridView_stations_Always_Included.Invalidate();
                CheckLanguageHasAUsedInstruction();
            }
        }

        private void DataGridView_stations_Always_Included_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView_stations_Always_Included.IsCurrentCellDirty)
            {
                // This fires the cell value changed handler below
                dataGridView_stations_Always_Included.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void SetFrequencyInformationColumnWidths()
        {
            if (dataGridView_stations_Always_Included.ColumnCount == 6)
            {
                if (_AdvancedSettings == true)
                {
                    autoColumnWidth = 60;
                    levelColumnWidth = 50;
                    TAColumnWidth = 50;
                }

                else
                {
                    autoColumnWidth = 1;
                    levelColumnWidth = 1;
                    TAColumnWidth = 1;
                }

                languageColumnWidth = dataGridView_stations_Always_Included.Width - autoColumnWidth - levelColumnWidth - preferredColumnWidth - frequencyColumnWidth - TAColumnWidth;

                dataGridView_stations_Always_Included.Columns[FREQCOLUMNINDEX].Width = frequencyColumnWidth;
                dataGridView_stations_Always_Included.Columns[LANGUAGECOLUMNINDEX].Width = languageColumnWidth;
                dataGridView_stations_Always_Included.Columns[AUTOLEVELCOLUMNINDEX].Width = autoColumnWidth;
                dataGridView_stations_Always_Included.Columns[LEVELCOLUMNINDEX].Width = levelColumnWidth;
                dataGridView_stations_Always_Included.Columns[PREFERREDCOLUMNINDEX].Width = preferredColumnWidth;
                dataGridView_stations_Always_Included.Columns[TACOLUMNINDEX].Width = TAColumnWidth;
            }
        }

        private void Load_Frequencies_sources()
        {
            // Bind the Stations pool
            List<Frequency_Record> sortedList = my_config.Frequencies_Pool.OrderBy(x => x.FREQUENCY).ToList();
            BindingList<Frequency_Record> sortedListB = new BindingList<Frequency_Record>(sortedList);
            my_config.Frequencies_Pool = sortedListB;

            var frequencies_full_source = new BindingSource(my_config.Frequencies_Pool, null);
            dataGridView_stations_all_freq.DataSource = frequencies_full_source;
            if ((dataGridView_stations_all_freq.Columns.Contains("LANGUAGE") && (dataGridView_stations_all_freq.Columns.Contains("LEVEL"))
                && (dataGridView_stations_all_freq.Columns.Contains("FREQUENCY")) && (dataGridView_stations_all_freq.Columns.Contains("AUTO_LEVEL")) && dataGridView_stations_all_freq.Columns.Contains("TA")))
            {
                dataGridView_stations_all_freq.Columns.Remove("FREQUENCY");
                dataGridView_stations_all_freq.Columns.Remove("LANGUAGE");
                dataGridView_stations_all_freq.Columns.Remove("LEVEL");
                dataGridView_stations_all_freq.Columns.Remove("AUTO_LEVEL");
                dataGridView_stations_all_freq.Columns.Remove("POPULAR");
                dataGridView_stations_all_freq.Columns.Remove("TA");
            }
            dataGridView_stations_all_freq.AutoGenerateColumns = false;


            // bind the excluded list
            var frequencies_excluded_source = new BindingSource(my_config.Frequencies_Excluded, null);
            dataGridView_stations_excluded.DataSource = frequencies_excluded_source;
            if ((dataGridView_stations_excluded.Columns.Contains("LANGUAGE") && (dataGridView_stations_excluded.Columns.Contains("LEVEL"))
                && (dataGridView_stations_excluded.Columns.Contains("FREQUENCY")) && (dataGridView_stations_excluded.Columns.Contains("AUTO_LEVEL")) && dataGridView_stations_excluded.Columns.Contains("TA")))
            {
                dataGridView_stations_excluded.Columns.Remove("FREQUENCY");
                dataGridView_stations_excluded.Columns.Remove("LANGUAGE");
                dataGridView_stations_excluded.Columns.Remove("LEVEL");
                dataGridView_stations_excluded.Columns.Remove("AUTO_LEVEL");
                dataGridView_stations_excluded.Columns.Remove("POPULAR");
                dataGridView_stations_excluded.Columns.Remove("TA");
            }
            dataGridView_stations_excluded.AutoGenerateColumns = false;

            // include freq 
            create_col_freq_inlcude_dgv();

            //Add Preferref Column
            DataGridViewTextBoxColumn dgvPreferred = new DataGridViewTextBoxColumn();
            dgvPreferred.HeaderText = "Preferred";
            dgvPreferred.Name = "dgvtxtPreferred";
            dgvPreferred.ReadOnly = true;

            dataGridView_stations_Always_Included.Columns.Add(dgvPreferred);

            //add advanced settings columns to Included frequency dgv and then hide them
            DataGridViewTextBoxColumn dgvauto = new DataGridViewTextBoxColumn();
            dgvauto.HeaderText = "Control";
            dgvauto.Name = "dgvtxtAuto";
            dgvauto.ReadOnly = true;

            DataGridViewTextBoxColumn dgvlevel = new DataGridViewTextBoxColumn();
            dgvlevel.HeaderText = "Level";
            dgvlevel.Name = "dgvtxtLevel";

            DataGridViewCheckBoxColumn dgvTA = new DataGridViewCheckBoxColumn();
            dgvTA.HeaderText = "TA";
            dgvTA.Name = "dgvtxtTA";

            dataGridView_stations_Always_Included.Columns.Add(dgvauto);
            dataGridView_stations_Always_Included.Columns.Add(dgvlevel);
            dataGridView_stations_Always_Included.Columns.Add(dgvTA);

            dataGridView_stations_Always_Included.Columns[AUTOLEVELCOLUMNINDEX].Visible = _AdvancedSettings;
            dataGridView_stations_Always_Included.Columns[LEVELCOLUMNINDEX].Visible = _AdvancedSettings;
            dataGridView_stations_Always_Included.Columns[TACOLUMNINDEX].Visible = _AdvancedSettings;

            //set widths
            SetFrequencyInformationColumnWidths();


            Add_data_to_include();
        }

        private void Create_Dgv_error_handler()
        {
            dataGridView_btn_1_list_instr.DataError += DataGridView_btn_X_list_instr_DataError;
            dataGridView_btn_2_list_instr.DataError += DataGridView_btn_X_list_instr_DataError;
            dataGridView_btn_3_list_instr.DataError += DataGridView_btn_X_list_instr_DataError;
            dataGridView_btn_4_list_instr.DataError += DataGridView_btn_X_list_instr_DataError;
            dataGridView_btn_5_list_instr.DataError += DataGridView_btn_X_list_instr_DataError;
            dataGridView_btn_6_list_instr.DataError += DataGridView_btn_X_list_instr_DataError;
        }

        private void DataGridView_btn_X_list_instr_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            Console.WriteLine("hmmm collection has changed..no worries the underlying structure is good");
        }

        private void DataGridView_Instructions_DataSourceChanged(object sender, EventArgs e)
        {
            Console.WriteLine("Datasource has changed !");
        }


        #endregion

        #region Menu Load / Save
        private void loadAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tslEWS_ConfigStatus.Text = "Load All Starting";


            UsbFolderBrowser usbBrowser = new UsbFolderBrowser();
            // Show the Usb Browser
            usbBrowser.ShowDialog();
            //Open brower dialog and get path
            if (usbBrowser.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                // clear all sources
                INSTRUCTIONS_POOL_Begin_Update();
                Frequencies_POOL_Begin_Update();

                Console.WriteLine("Selected: " + usbBrowser.SelectedDrive);
                // if valid path set the my_config path to save
                my_config = new Main_Config(temppath);
                if (!Directory.Exists(temppath))
                {
                    try
                    {
                        Directory.CreateDirectory(temppath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error Creating Directory", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }

                my_config.UsbPath = usbBrowser.SelectedDrive;
                my_config.LanguagesLoaded = LanguagesJsonGlobal.Language;
                // look for config.ews and unzip to temp directory
                my_config.zippath = my_config.UsbPath + @"\config\config.ews";

                if (File.Exists(my_config.zippath))
                {
                    Console.WriteLine("Upzipping");
                    if (System.IO.Directory.Exists(my_config.TempPath))
                    {
                        System.IO.Directory.Delete(my_config.TempPath, true);
                    }
                    ZipFile.ExtractToDirectory(my_config.zippath, my_config.TempPath);

                    //--------------------------------------
                    my_config = new Main_Config(temppath);
                    //my_config.UsbPath = usbBrowser.SelectedDrive;
                    my_config.LanguagesLoaded = LanguagesJsonGlobal.Language;

                    my_config.Load_output_config();
                    my_config.Load_channels_config();
                    // instructions tab
                    // bind the list to the dgv
                    dataGridView_Instructions.DataSource = my_config.EWS_Config_output.INSTRUCTIONS;

                    Load_Button_sources();

                    if (my_config.EWS_Channels.CHANNELS.AUTOMATIC.ACTIVE == "YES")
                    {
                        checkBox_stations_auto_stations.Checked = true;
                    }
                    else
                    {
                        checkBox_stations_auto_stations.Checked = false;
                    }

                    //Update enhanced stations info
                    nudMessageRepeat.Value = my_config.EWS_Channels.CHANNELS.ENHANCED.REPEAT;


                    if (my_config.EWS_Channels.CHANNELS.ENHANCED.ACTIVE == "YES")
                    {
                        cbEnhancedStations.Checked = true;
                        nudMessageRepeat.Visible = true;
                        lblRepeat.Visible = true;
                    }

                    else
                    {
                        cbEnhancedStations.Checked = false;
                        nudMessageRepeat.Visible = false;
                        lblRepeat.Visible = false;
                    }

                    //Update message break properties

                    //First Load with defaults
                    //Button 1
                    chb_Button1_Message_Break.Checked = false;
                    nudButton1BreakDuration.Value = DEFAULT_MESSAGE_BREAK_DURATION;
                    nudButton1MinimumSpeed.Value = DEFAULT_MINIMUM_SPEED_FOR_MESSAGE_BREAK;
                    nudButton1CModeHysteresis.Value = DEFAULT_C_MODE_HYSTERESIS_VALUE;

                    //Button 2
                    chb_Button2_Message_Break.Checked = false;
                    nudButton2BreakDuration.Value = DEFAULT_MESSAGE_BREAK_DURATION;
                    nudButton2MinimumSpeed.Value = DEFAULT_MINIMUM_SPEED_FOR_MESSAGE_BREAK;
                    nudButton2CModeHysteresis.Value = DEFAULT_C_MODE_HYSTERESIS_VALUE;

                    //Button 3
                    chb_Button3_Message_Break.Checked = false;
                    nudButton3BreakDuration.Value = DEFAULT_MESSAGE_BREAK_DURATION;
                    nudButton3MinimumSpeed.Value = DEFAULT_MINIMUM_SPEED_FOR_MESSAGE_BREAK;
                    nudButton3CModeHysteresis.Value = DEFAULT_C_MODE_HYSTERESIS_VALUE;

                    //Button 4
                    chb_Button4_Message_Break.Checked = false;
                    nudButton4BreakDuration.Value = DEFAULT_MESSAGE_BREAK_DURATION;
                    nudButton4MinimumSpeed.Value = DEFAULT_MINIMUM_SPEED_FOR_MESSAGE_BREAK;
                    nudButton4CModeHysteresis.Value = DEFAULT_C_MODE_HYSTERESIS_VALUE;

                    //Button 5
                    chb_Button5_Message_Break.Checked = false;
                    nudButton5BreakDuration.Value = DEFAULT_MESSAGE_BREAK_DURATION;
                    nudButton5MinimumSpeed.Value = DEFAULT_MINIMUM_SPEED_FOR_MESSAGE_BREAK;
                    nudButton5CModeHysteresis.Value = DEFAULT_C_MODE_HYSTERESIS_VALUE;

                    //Button 6
                    chb_Button6_Message_Break.Checked = false;
                    nudButton6BreakDuration.Value = DEFAULT_MESSAGE_BREAK_DURATION;
                    nudButton6MinimumSpeed.Value = DEFAULT_MINIMUM_SPEED_FOR_MESSAGE_BREAK;
                    nudButton6CModeHysteresis.Value = DEFAULT_C_MODE_HYSTERESIS_VALUE;



                    //Then update each used control
                    foreach (Control c in my_config.EWS_Config_output.CONTROL)
                    {
                        if (c.TRIGGER == "Button1")
                        {
                            if (c.MESSAGE_BREAK_DURATION != 0 && c.MESSAGE_BREAK_MINIMUM_SPEED != 0)
                            {
                                chb_Button1_Message_Break.Checked = true;

                                nudButton1BreakDuration.Value = c.MESSAGE_BREAK_DURATION;
                                nudButton1CModeHysteresis.Value = c.C_MODE_HYSTERESIS;

                                if (lblButton1MinimumSpeed.Text.Contains("KPH"))
                                {
                                    nudButton1MinimumSpeed.Value = c.MESSAGE_BREAK_MINIMUM_SPEED;
                                }

                                else
                                {
                                    double flTempSpeed = Math.Round(c.MESSAGE_BREAK_MINIMUM_SPEED * KPH_TO_MPH_CONVERSION_FACTOR);

                                    if (flTempSpeed > MAXIMUM_C_MODE_MINIMUM_SPEED_MPH)
                                    {
                                        nudButton1MinimumSpeed.Value = MAXIMUM_C_MODE_MINIMUM_SPEED_MPH;
                                    }

                                    else if (flTempSpeed < MINIMUM_C_MODE_MINIMUM_SPEED)
                                    {
                                        nudButton1MinimumSpeed.Value = MINIMUM_C_MODE_MINIMUM_SPEED;
                                    }

                                    else
                                    {
                                        nudButton1MinimumSpeed.Value = (decimal)flTempSpeed;
                                    }
                                }
                            }
                        }

                        else if (c.TRIGGER == "Button2")
                        {
                            if (c.MESSAGE_BREAK_DURATION != 0 && c.MESSAGE_BREAK_MINIMUM_SPEED != 0)
                            {
                                chb_Button2_Message_Break.Checked = true;

                                nudButton2BreakDuration.Value = c.MESSAGE_BREAK_DURATION;
                                nudButton2CModeHysteresis.Value = c.C_MODE_HYSTERESIS;

                                if (lblButton2MinimumSpeed.Text.Contains("KPH"))
                                {
                                    nudButton2MinimumSpeed.Value = c.MESSAGE_BREAK_MINIMUM_SPEED;
                                }

                                else
                                {
                                    double flTempSpeed = Math.Round(c.MESSAGE_BREAK_MINIMUM_SPEED * KPH_TO_MPH_CONVERSION_FACTOR);

                                    if (flTempSpeed > MAXIMUM_C_MODE_MINIMUM_SPEED_MPH)
                                    {
                                        nudButton2MinimumSpeed.Value = MAXIMUM_C_MODE_MINIMUM_SPEED_MPH;
                                    }

                                    else if (flTempSpeed < MINIMUM_C_MODE_MINIMUM_SPEED)
                                    {
                                        nudButton2MinimumSpeed.Value = MINIMUM_C_MODE_MINIMUM_SPEED;
                                    }

                                    else
                                    {
                                        nudButton2MinimumSpeed.Value = (decimal)flTempSpeed;
                                    }
                                }
                            }
                        }

                        else if (c.TRIGGER == "Button3")
                        {
                            if (c.MESSAGE_BREAK_DURATION != 0 && c.MESSAGE_BREAK_MINIMUM_SPEED != 0)
                            {
                                chb_Button3_Message_Break.Checked = true;

                                nudButton3BreakDuration.Value = c.MESSAGE_BREAK_DURATION;
                                nudButton3CModeHysteresis.Value = c.C_MODE_HYSTERESIS;

                                if (lblButton3MinimumSpeed.Text.Contains("KPH"))
                                {
                                    nudButton3MinimumSpeed.Value = c.MESSAGE_BREAK_MINIMUM_SPEED;
                                }

                                else
                                {
                                    double flTempSpeed = Math.Round(c.MESSAGE_BREAK_MINIMUM_SPEED * KPH_TO_MPH_CONVERSION_FACTOR);

                                    if (flTempSpeed > MAXIMUM_C_MODE_MINIMUM_SPEED_MPH)
                                    {
                                        nudButton3MinimumSpeed.Value = MAXIMUM_C_MODE_MINIMUM_SPEED_MPH;
                                    }

                                    else if (flTempSpeed < MINIMUM_C_MODE_MINIMUM_SPEED)
                                    {
                                        nudButton3MinimumSpeed.Value = MINIMUM_C_MODE_MINIMUM_SPEED;
                                    }

                                    else
                                    {
                                        nudButton3MinimumSpeed.Value = (decimal)flTempSpeed;
                                    }
                                }
                            }
                        }

                        else if (c.TRIGGER == "Button4")
                        {
                            if (c.MESSAGE_BREAK_DURATION != 0 && c.MESSAGE_BREAK_MINIMUM_SPEED != 0)
                            {
                                chb_Button4_Message_Break.Checked = true;

                                nudButton4BreakDuration.Value = c.MESSAGE_BREAK_DURATION;
                                nudButton4CModeHysteresis.Value = c.C_MODE_HYSTERESIS;

                                if (lblButton4MinimumSpeed.Text.Contains("KPH"))
                                {
                                    nudButton4MinimumSpeed.Value = c.MESSAGE_BREAK_MINIMUM_SPEED;
                                }

                                else
                                {
                                    double flTempSpeed = Math.Round(c.MESSAGE_BREAK_MINIMUM_SPEED * KPH_TO_MPH_CONVERSION_FACTOR);

                                    if (flTempSpeed > MAXIMUM_C_MODE_MINIMUM_SPEED_MPH)
                                    {
                                        nudButton4MinimumSpeed.Value = MAXIMUM_C_MODE_MINIMUM_SPEED_MPH;
                                    }

                                    else if (flTempSpeed < MINIMUM_C_MODE_MINIMUM_SPEED)
                                    {
                                        nudButton4MinimumSpeed.Value = MINIMUM_C_MODE_MINIMUM_SPEED;
                                    }

                                    else
                                    {
                                        nudButton4MinimumSpeed.Value = (decimal)flTempSpeed;
                                    }
                                }
                            }
                        }

                        else if (c.TRIGGER == "Button5")
                        {
                            if (c.MESSAGE_BREAK_DURATION != 0 && c.MESSAGE_BREAK_MINIMUM_SPEED != 0)
                            {
                                chb_Button5_Message_Break.Checked = true;

                                nudButton5BreakDuration.Value = c.MESSAGE_BREAK_DURATION;
                                nudButton5CModeHysteresis.Value = c.C_MODE_HYSTERESIS;

                                if (lblButton5MinimumSpeed.Text.Contains("KPH"))
                                {
                                    nudButton5MinimumSpeed.Value = c.MESSAGE_BREAK_MINIMUM_SPEED;
                                }

                                else
                                {
                                    double flTempSpeed = Math.Round(c.MESSAGE_BREAK_MINIMUM_SPEED * KPH_TO_MPH_CONVERSION_FACTOR);

                                    if (flTempSpeed > MAXIMUM_C_MODE_MINIMUM_SPEED_MPH)
                                    {
                                        nudButton5MinimumSpeed.Value = MAXIMUM_C_MODE_MINIMUM_SPEED_MPH;
                                    }

                                    else if (flTempSpeed < MINIMUM_C_MODE_MINIMUM_SPEED)
                                    {
                                        nudButton5MinimumSpeed.Value = MINIMUM_C_MODE_MINIMUM_SPEED;
                                    }

                                    else
                                    {
                                        nudButton5MinimumSpeed.Value = (decimal)flTempSpeed;
                                    }
                                }
                            }
                        }

                        else if (c.TRIGGER == "Button6")
                        {
                            if (c.MESSAGE_BREAK_DURATION != 0 && c.MESSAGE_BREAK_MINIMUM_SPEED != 0)
                            {
                                chb_Button6_Message_Break.Checked = true;

                                nudButton6BreakDuration.Value = c.MESSAGE_BREAK_DURATION;
                                nudButton6CModeHysteresis.Value = c.C_MODE_HYSTERESIS;

                                if (lblButton6MinimumSpeed.Text.Contains("KPH"))
                                {
                                    nudButton6MinimumSpeed.Value = c.MESSAGE_BREAK_MINIMUM_SPEED;
                                }

                                else
                                {
                                    double flTempSpeed = Math.Round(c.MESSAGE_BREAK_MINIMUM_SPEED * KPH_TO_MPH_CONVERSION_FACTOR);

                                    if (flTempSpeed > MAXIMUM_C_MODE_MINIMUM_SPEED_MPH)
                                    {
                                        nudButton6MinimumSpeed.Value = MAXIMUM_C_MODE_MINIMUM_SPEED_MPH;
                                    }

                                    else if (flTempSpeed < MINIMUM_C_MODE_MINIMUM_SPEED)
                                    {
                                        nudButton6MinimumSpeed.Value = MINIMUM_C_MODE_MINIMUM_SPEED;
                                    }

                                    else
                                    {
                                        nudButton6MinimumSpeed.Value = (decimal)flTempSpeed;
                                    }
                                }
                            }
                        }
                    }

                    Load_Frequencies_sources(); // Stations tab region

                    //set the combobox default language
                    try
                    {
                        comboBox_Instr_Default_Lang.Text = my_config.Default_language.Ref_Name;
                    }
                    catch
                    {
                        comboBox_Instr_Default_Lang.Text = "Undetermined";
                    }



                    AddPlayColumnToInstructionList();

                    ActionButtonButtonVisibilty();
                    ActionStationButtonVisibilty();
                    CheckLanguageHasAUsedInstruction();

                    tslEWS_ConfigStatus.Text = "Load All Complete";
                }
                else
                {
                    Console.WriteLine("No Configs ??");
                    //EWS_MainForm_Load(this, null);
                    tslEWS_ConfigStatus.Text = "Load All No Configs Found !";
                }

            }
            else
            {
                Console.WriteLine("None selected");
                tslEWS_ConfigStatus.Text = "Load All None Selected";
            }

        }

        private void prUpdateMessageBreakParameters()
        {
            foreach (Control con in my_config.EWS_Config_output.CONTROL)
            {
                if (con.TRIGGER == "Button1" && chb_Button1_Message_Break.Checked == true)
                {
                    con.MESSAGE_BREAK_DURATION = (int)nudButton1BreakDuration.Value;
                    con.C_MODE_HYSTERESIS = (int)nudButton1CModeHysteresis.Value;

                    if (lblButton1MinimumSpeed.Text.Contains("MPH"))
                    {
                        con.MESSAGE_BREAK_MINIMUM_SPEED = (int)((double)nudButton1MinimumSpeed.Value / KPH_TO_MPH_CONVERSION_FACTOR);
                    }

                    else
                    {
                        con.MESSAGE_BREAK_MINIMUM_SPEED = (int)nudButton1MinimumSpeed.Value;
                    }
                }

                else if (con.TRIGGER == "Button2" && chb_Button2_Message_Break.Checked == true)
                {
                    con.MESSAGE_BREAK_DURATION = (int)nudButton2BreakDuration.Value;
                    con.C_MODE_HYSTERESIS = (int)nudButton2CModeHysteresis.Value;

                    if (lblButton2MinimumSpeed.Text.Contains("MPH"))
                    {
                        con.MESSAGE_BREAK_MINIMUM_SPEED = (int)((double)nudButton2MinimumSpeed.Value / KPH_TO_MPH_CONVERSION_FACTOR);
                    }

                    else
                    {
                        con.MESSAGE_BREAK_MINIMUM_SPEED = (int)nudButton2MinimumSpeed.Value;
                    }
                }

                else if (con.TRIGGER == "Button3" && chb_Button3_Message_Break.Checked == true)
                {
                    con.MESSAGE_BREAK_DURATION = (int)nudButton3BreakDuration.Value;
                    con.C_MODE_HYSTERESIS = (int)nudButton3CModeHysteresis.Value;

                    if (lblButton3MinimumSpeed.Text.Contains("MPH"))
                    {
                        con.MESSAGE_BREAK_MINIMUM_SPEED = (int)((double)nudButton3MinimumSpeed.Value / KPH_TO_MPH_CONVERSION_FACTOR);
                    }

                    else
                    {
                        con.MESSAGE_BREAK_MINIMUM_SPEED = (int)nudButton3MinimumSpeed.Value;
                    }
                }

                else if (con.TRIGGER == "Button4" && chb_Button4_Message_Break.Checked == true)
                {
                    con.MESSAGE_BREAK_DURATION = (int)nudButton4BreakDuration.Value;
                    con.C_MODE_HYSTERESIS = (int)nudButton4CModeHysteresis.Value;

                    if (lblButton4MinimumSpeed.Text.Contains("MPH"))
                    {
                        con.MESSAGE_BREAK_MINIMUM_SPEED = (int)((double)nudButton4MinimumSpeed.Value / KPH_TO_MPH_CONVERSION_FACTOR);
                    }

                    else
                    {
                        con.MESSAGE_BREAK_MINIMUM_SPEED = (int)nudButton4MinimumSpeed.Value;
                    }
                }

                else if (con.TRIGGER == "Button5" && chb_Button5_Message_Break.Checked == true)
                {
                    con.MESSAGE_BREAK_DURATION = (int)nudButton5BreakDuration.Value;
                    con.C_MODE_HYSTERESIS = (int)nudButton5CModeHysteresis.Value;

                    if (lblButton5MinimumSpeed.Text.Contains("MPH"))
                    {
                        con.MESSAGE_BREAK_MINIMUM_SPEED = (int)((double)nudButton5MinimumSpeed.Value / KPH_TO_MPH_CONVERSION_FACTOR);
                    }

                    else
                    {
                        con.MESSAGE_BREAK_MINIMUM_SPEED = (int)nudButton5MinimumSpeed.Value;
                    }
                }


                //C-Mode for button 6 has been disabled.  This has been left here in case we want to enable it later
                //else if (con.TRIGGER == "Button6" && chb_Button6_Message_Break.Checked == true)
                //{
                //    con.MESSAGE_BREAK_DURATION = (int)nudButton6BreakDuration.Value;
                //    con.C_MODE_HYSTERESIS = (int)nudButton6CModeHysteresis.Value;

                //    if (lblButton6MinimumSpeed.Text.Contains("MPH"))
                //    {
                //        con.MESSAGE_BREAK_MINIMUM_SPEED = (int)((double)nudButton6MinimumSpeed.Value / KPH_TO_MPH_CONVERSION_FACTOR);
                //    }

                //    else
                //    {
                //        con.MESSAGE_BREAK_MINIMUM_SPEED = (int)nudButton6MinimumSpeed.Value;
                //    }
                //}
            }
        }

        /// <summary>
        /// Save output.json and thr channels.json files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tslEWS_ConfigStatus.Text = "Save All Started";
            //            pbFileProgress.Value = 0;
            //            pbFileProgress.Visible = true;
            bool continueSave = true;

            if (dataGridView_btn_1_instr.RowCount < 1 && dataGridView_btn_2_instr.RowCount < 1 && dataGridView_btn_3_instr.RowCount < 1 && dataGridView_btn_4_instr.RowCount < 1 && dataGridView_btn_5_instr.RowCount < 1 && dataGridView_btn_6_instr.RowCount < 1)
            {

                MessageBox.Show("You are trying to save an invalid configuration.\n\nNo buttons have an instruction assigned.\n\nIf you would like to save the configuration for future editing, use the Backup menu", "Invalid Configuration", MessageBoxButtons.OK, MessageBoxIcon.Error);
                continueSave = false;
            }

            else if (dataGridView_SN_FPGA_Code.RowCount < 1)
            {
                MessageBox.Show("You are trying to save an invalid configuration.\n\nNo security keys have been loaded.\n\nIf you would like to save the configuration for future editing, use the Backup menu", "Invalid Configuration", MessageBoxButtons.OK, MessageBoxIcon.Error);
                continueSave = false;
            }

            else
            {
                continueSave = my_config.CheckAllLanguagesUsedHaveInstructions("saveAllToolStripMenuItem_Click");
            }


            if (continueSave)
            {
                //Update message break properties
                prUpdateMessageBreakParameters();

                //the output.json file
                my_config.EWS_Config_output.CFG_VERSION = ActiveForm.Text;
                my_config.Save_output_config();
                //               pbFileProgress.Value = 15;
                my_config.EWS_Channels.CFG_VERSION = ActiveForm.Text;
                my_config.Save_channel_config();
                //                pbFileProgress.Value = 30;

                UsbFolderBrowser usbBrowser = new UsbFolderBrowser();
                // Show the Usb Browser
                usbBrowser.ShowDialog();
                //Open brower dialog and get path
                if (usbBrowser.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    Console.WriteLine("Selected: " + usbBrowser.SelectedDrive);
                    // if valid path set the my_config path to save
                    my_config.UsbPath = usbBrowser.SelectedDrive;
                    my_config.LanguagesLoaded = LanguagesJsonGlobal.Language;
                    // look for config.ews and unzip to temp directory
                    my_config.zippath = my_config.UsbPath + @"\config\config.ews";

                    if (File.Exists(currentPath + @"\config.ews"))
                    {
                        File.Delete(currentPath + @"\config.ews");
                    }

                    //Get MD5 Checksum
                    ConfigMD5 = Encrypt_Folder(my_config.TempPath);
                    //                    pbFileProgress.Value = 40;
                    CreateEncriptionJson(ConfigMD5, my_config.TempPath);
                    //                    pbFileProgress.Value = 50;

                    Application.UseWaitCursor = true;
                    //update status bar

                    Console.WriteLine("zipping");

                    ZipFile.CreateFromDirectory(my_config.TempPath, currentPath + @"\config.ews");
                    //                    pbFileProgress.Value = 80;

                    try
                    {
                        //check if config folder is present on USB drive, and delete any old files
                        if (Directory.Exists(@my_config.UsbPath + @"\config"))
                        {
                            Directory.Delete(@my_config.UsbPath + @"\config", true);
                        }
                        // now create a new clean one
                        Directory.CreateDirectory(@my_config.UsbPath + @"\config");

                        try
                        {
                            File.Copy(currentPath + @"\config.ews", @my_config.UsbPath + @"\config\config.ews", true);
                            if (File.Exists(currentPath + @"\config.ews"))
                            {
                                File.Delete(currentPath + @"\config.ews");
                            }
                            //                            pbFileProgress.Value = 90;

                            Application.UseWaitCursor = false;
                            tslEWS_ConfigStatus.Text = "Save All Complete";

                            //                            pbFileProgress.Value = 100;
                            Thread.Sleep(250);
                            //                           pbFileProgress.Visible = false;

                            _OKToCloseandExit = true;  //now saved so OK to close
                        }

                        catch (Exception oops)
                        {
                            MessageBox.Show(oops.Message, "Error Saving File", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            Application.UseWaitCursor = false;
                            tslEWS_ConfigStatus.Text = "Save All Cancelled";
                            //                           pbFileProgress.Value = 100;
                            //                           Thread.Sleep(100);
                            //                           pbFileProgress.Visible = false;
                        }
                    }


                    catch (Exception ex)
                    {
                        MessageBox.Show(@my_config.UsbPath + @"\config" + ": " + ex.Message, "Error Creating Directory", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        Application.UseWaitCursor = false;
                        tslEWS_ConfigStatus.Text = "Save All Cancelled";
                        //                       pbFileProgress.Value = 100;
                        //                        Thread.Sleep(100);
                        //                        pbFileProgress.Visible = false;
                    }




                }

                else
                {
                    tslEWS_ConfigStatus.Text = "Save All Cancelled";
                    //                   pbFileProgress.Value = 100;
                    //                    Thread.Sleep(100);
                    //                   pbFileProgress.Visible = false;
                }

                clean();
            }

            else
            {
                tslEWS_ConfigStatus.Text = "Save All Cancelled";
                //                pbFileProgress.Value = 100;
                //                Thread.Sleep(100);
                //                pbFileProgress.Visible = false;
            }

        }
        /// <summary>
        /// Buttons config -> output.json
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveButtonsInstructionsOnlyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tslEWS_ConfigStatus.Text = "ButtonsInstructionsOnly Started";
            //zip the output.json file to the usb drive
            //first copy to a new folder
            string temp_output_file_only = currentPath + @"\tempO";
            if (Directory.Exists(temp_output_file_only))
                Directory.Delete(temp_output_file_only, true);

            Directory.CreateDirectory(temp_output_file_only);
            Thread.Sleep(150);

            my_config.Save_output_config();

            UsbFolderBrowser usbBrowser = new UsbFolderBrowser();
            // Show the Usb Browser
            usbBrowser.ShowDialog();
            //Open brower dialog and get path
            if (usbBrowser.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                my_config.UsbPath = usbBrowser.SelectedDrive;

                File.Copy(my_config.TempPath + @"\output.json", temp_output_file_only + @"\output.json", true);
                //copy all wavs
                string[] filePaths = Directory.GetFiles(@my_config.TempPath, "*.wav", SearchOption.AllDirectories);

                string sourcePath = @my_config.TempPath;
                string targetPath = @temp_output_file_only;

                var extensions = new[] { ".wav" };

                var files = (from file in Directory.EnumerateFiles(sourcePath)
                             where extensions.Contains(Path.GetExtension(file), StringComparer.InvariantCultureIgnoreCase) // comment this out if you don't want to filter extensions
                             select new
                             {
                                 Source = file,
                                 Destination = Path.Combine(targetPath, Path.GetFileName(file))
                             });

                foreach (var file in files)
                {
                    File.Copy(file.Source, file.Destination);
                }

                // create the zip and copy
                Application.UseWaitCursor = true;
                //update status bar

                //Get MD5 Checksum
                ConfigMD5 = Encrypt_Folder(temp_output_file_only);
                CreateEncriptionJson(ConfigMD5, temp_output_file_only);

                Console.WriteLine("zipping channels only");

                ZipFile.CreateFromDirectory(temp_output_file_only, currentPath + @"\config.ews");

                try
                {
                    //check if config folder is present on USB drive, and delete any old files
                    if (Directory.Exists(@my_config.UsbPath + @"\config"))
                    {
                        Directory.Delete(@my_config.UsbPath + @"\config", true);
                    }
                    // now create a new clean one
                    Directory.CreateDirectory(@my_config.UsbPath + @"\config");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(@my_config.UsbPath + @"\config" + ": " + ex.Message, "Error Creating Directory", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

                File.Copy(currentPath + @"\config.ews", @my_config.UsbPath + @"\config\config.ews", true);
                if (File.Exists(currentPath + @"\config.ews"))
                {
                    File.Delete(currentPath + @"\config.ews");
                }

                Application.UseWaitCursor = false;
                tslEWS_ConfigStatus.Text = "ButtonsInstructionsOnly Complete";

            }

            clean();


        }

        private void clean()
        {
            if (Directory.Exists(currentPath + @"\tempO"))
            {
                Directory.Delete(currentPath + @"\tempO", true);
            }

            if (Directory.Exists(currentPath + @"\tempC"))
            {
                Directory.Delete(currentPath + @"\tempC", true);
            }

        }

        /// <summary>
        /// Stations config ->channels.json
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveStationsOnlyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tslEWS_ConfigStatus.Text = "SaveStationsOnly Started";
            //delete everything else
            my_config.Save_channel_config();

            UsbFolderBrowser usbBrowser = new UsbFolderBrowser();
            // Show the Usb Browser
            usbBrowser.ShowDialog();
            //Open brower dialog and get path
            if (usbBrowser.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                my_config.UsbPath = usbBrowser.SelectedDrive;
                //zip the channel.json file to the usb drive
                //first copy to a new folder
                string temp_channel_file_only = currentPath + @"\tempC";

                Directory.CreateDirectory(temp_channel_file_only);
                File.Copy(my_config.TempPath + @"\channels.json", temp_channel_file_only + @"\channels.json", true);

                //Get MD5 Checksum
                ConfigMD5 = Encrypt_Folder(temp_channel_file_only);
                CreateEncriptionJson(ConfigMD5, temp_channel_file_only);

                Application.UseWaitCursor = true;
                //update status bar

                Console.WriteLine("zipping channels only");
                if (File.Exists(currentPath + @"\config.ews"))
                {
                    File.Delete(currentPath + @"\config.ews");
                }
                ZipFile.CreateFromDirectory(temp_channel_file_only, currentPath + @"\config.ews");

                try
                {
                    //check if config folder is present on USB drive, and delete any old files
                    if (Directory.Exists(@my_config.UsbPath + @"\config"))
                    {
                        Directory.Delete(@my_config.UsbPath + @"\config", true);
                    }
                    // now create a new clean one
                    Directory.CreateDirectory(@my_config.UsbPath + @"\config");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(@my_config.UsbPath + @"\config" + ": " + ex.Message, "Error Creating Directory", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

                File.Copy(currentPath + @"\config.ews", @my_config.UsbPath + @"\config\config.ews", true);

                if (File.Exists(currentPath + @"\config.ews"))
                {
                    File.Delete(currentPath + @"\config.ews");
                }

                Application.UseWaitCursor = false;
                tslEWS_ConfigStatus.Text = "SaveStationsOnly Complete";
            }

            clean();
        }

        #region Backup

        private void bacKupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //bool continueBackup = my_config.CheckAllLanguagesUsedHaveInstructions("bacKupToolStripMenuItem_Click");

            bool continueBackup = true;

            if (dataGridView_btn_1_instr.RowCount < 1 && dataGridView_btn_2_instr.RowCount < 1 && dataGridView_btn_3_instr.RowCount < 1 && dataGridView_btn_4_instr.RowCount < 1 && dataGridView_btn_5_instr.RowCount < 1 && dataGridView_btn_6_instr.RowCount < 1)
            {
                DialogResult dialogResult = MessageBox.Show("Your configuration is invalid because no instructions have been assigned to buttons.  Would you like to continue?", "Configuration Error", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (dialogResult == DialogResult.Yes)
                {
                    continueBackup = true;
                }
                else if (dialogResult == DialogResult.No)
                {
                    continueBackup = false;
                }
            }

            else
            {
                continueBackup = my_config.CheckAllLanguagesUsedHaveInstructions("bacKupToolStripMenuItem_Click");
            }

            if (continueBackup)
            {
                tslEWS_ConfigStatus.Text = "Backup Started";

                //Update message break properties
                prUpdateMessageBreakParameters();

                //the output.json file
                my_config.EWS_Config_output.CFG_VERSION = ActiveForm.Text;
                my_config.Save_output_config();

                my_config.EWS_Channels.CFG_VERSION = ActiveForm.Text;
                my_config.Save_channel_config();

                if (File.Exists(currentPath + @"\config.ews"))
                {
                    File.Delete(currentPath + @"\config.ews");
                }

                //Get MD5 Checksum
                ConfigMD5 = Encrypt_Folder(my_config.TempPath);
                CreateEncriptionJson(ConfigMD5, my_config.TempPath);

                Application.UseWaitCursor = true;
                //update status bar

                Console.WriteLine("zipping");

                ZipFile.CreateFromDirectory(my_config.TempPath, currentPath + @"\config.ews");

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "EWS_Config|*.ews";
                sfd.Title = "Backup EWS Configuration File";

                sfd.InitialDirectory = InitDirectory;
                sfd.RestoreDirectory = true;
                sfd.FileName = "config.ews";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        System.IO.FileStream fs = (System.IO.FileStream)sfd.OpenFile();

                        //save saved file directory
                        string bupCopyPath = fs.Name;

                        //Close Filestream

                        fs.Close();

                        File.Delete(bupCopyPath);

                        File.Copy(currentPath + @"\config.ews", bupCopyPath, true);
                        if (File.Exists(currentPath + @"\config.ews"))
                        {
                            File.Delete(currentPath + @"\config.ews");
                        }

                        Application.UseWaitCursor = false;
                        tslEWS_ConfigStatus.Text = "Backup Complete";

                        _OKToCloseandExit = true;  //config saved so OK to close
                    }

                    catch (Exception exc)
                    {
                        MessageBox.Show(exc.Message);
                        Application.UseWaitCursor = false;
                        tslEWS_ConfigStatus.Text = "Backup Cancelled";
                    }

                }

                else
                {
                    Application.UseWaitCursor = false;
                    tslEWS_ConfigStatus.Text = "Backup Cancelled";
                }
                clean();
            }

            else
            {
                tslEWS_ConfigStatus.Text = "Backup Cancelled";
            }
        }


        private void toolStripMenuItemRestoreConfiguration_Click(object sender, EventArgs e)
        {
            string bupCopyPath = "";

            tslEWS_ConfigStatus.Text = "Restore Configuration Starting";

            if (sender == null && e == null)
            {
                //save saved file directory
                bupCopyPath = OpenStartFile;
            }

            else
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "EWS_Config|*.ews";
                ofd.Title = "Restore EWS Configuration";
                //check if c:ews exists...if not create it

                ofd.InitialDirectory = InitDirectory;
                ofd.RestoreDirectory = true;
                ofd.FileName = CONFIG_FILENAME;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    INSTRUCTIONS_POOL_Begin_Update();
                    Frequencies_POOL_Begin_Update();

                    System.IO.FileStream fs = (System.IO.FileStream)ofd.OpenFile();

                    //save saved file directory
                    bupCopyPath = fs.Name;

                    //Close Filestream
                    fs.Close();
                }
            }
            // if valid path set the my_config path to save
            my_config = new Main_Config(temppath);
            if (!Directory.Exists(temppath))
            {
                try
                {
                    Directory.CreateDirectory(temppath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error Creating Directory", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }

            my_config.LanguagesLoaded = LanguagesJsonGlobal.Language;

            if (File.Exists(bupCopyPath))
            {
                Console.WriteLine("Upzipping");
                if (System.IO.Directory.Exists(my_config.TempPath))
                {
                    System.IO.Directory.Delete(my_config.TempPath, true);
                }
                ZipFile.ExtractToDirectory(bupCopyPath, my_config.TempPath);

                //--------------------------------------
                my_config = new Main_Config(temppath);

                my_config.LanguagesLoaded = LanguagesJsonGlobal.Language;

                my_config.Load_output_config();
                my_config.Load_channels_config();
                // instructions tab
                // bind the list to the dgv
                dataGridView_Instructions.DataSource = my_config.EWS_Config_output.INSTRUCTIONS;

                Load_Button_sources();

                if (my_config.EWS_Channels.CHANNELS.AUTOMATIC.ACTIVE == "YES")
                {
                    checkBox_stations_auto_stations.Checked = true;
                }
                else
                {
                    checkBox_stations_auto_stations.Checked = false;
                }

                //Update enhanced stations info
                nudMessageRepeat.Value = my_config.EWS_Channels.CHANNELS.ENHANCED.REPEAT;

                if (my_config.EWS_Channels.CHANNELS.ENHANCED.ACTIVE == "YES")
                {
                    cbEnhancedStations.Checked = true;
                    nudMessageRepeat.Visible = true;
                    lblRepeat.Visible = true;
                }

                else
                {
                    cbEnhancedStations.Checked = false;
                    nudMessageRepeat.Visible = false;
                    lblRepeat.Visible = false;
                }

                //Update message break properties

                //First Load with defaults
                //Button 1
                chb_Button1_Message_Break.Checked = false;
                nudButton1BreakDuration.Value = DEFAULT_MESSAGE_BREAK_DURATION;
                nudButton1MinimumSpeed.Value = DEFAULT_MINIMUM_SPEED_FOR_MESSAGE_BREAK;
                nudButton1CModeHysteresis.Value = DEFAULT_C_MODE_HYSTERESIS_VALUE;

                //Button 2
                chb_Button2_Message_Break.Checked = false;
                nudButton2BreakDuration.Value = DEFAULT_MESSAGE_BREAK_DURATION;
                nudButton2MinimumSpeed.Value = DEFAULT_MINIMUM_SPEED_FOR_MESSAGE_BREAK;
                nudButton2CModeHysteresis.Value = DEFAULT_C_MODE_HYSTERESIS_VALUE;

                //Button 3
                chb_Button3_Message_Break.Checked = false;
                nudButton3BreakDuration.Value = DEFAULT_MESSAGE_BREAK_DURATION;
                nudButton3MinimumSpeed.Value = DEFAULT_MINIMUM_SPEED_FOR_MESSAGE_BREAK;
                nudButton3CModeHysteresis.Value = DEFAULT_C_MODE_HYSTERESIS_VALUE;

                //Button 4
                chb_Button4_Message_Break.Checked = false;
                nudButton4BreakDuration.Value = DEFAULT_MESSAGE_BREAK_DURATION;
                nudButton4MinimumSpeed.Value = DEFAULT_MINIMUM_SPEED_FOR_MESSAGE_BREAK;
                nudButton4CModeHysteresis.Value = DEFAULT_C_MODE_HYSTERESIS_VALUE;

                //Button 5
                chb_Button5_Message_Break.Checked = false;
                nudButton5BreakDuration.Value = DEFAULT_MESSAGE_BREAK_DURATION;
                nudButton5MinimumSpeed.Value = DEFAULT_MINIMUM_SPEED_FOR_MESSAGE_BREAK;
                nudButton5CModeHysteresis.Value = DEFAULT_C_MODE_HYSTERESIS_VALUE;

                //Button 6
                chb_Button6_Message_Break.Checked = false;
                nudButton6BreakDuration.Value = DEFAULT_MESSAGE_BREAK_DURATION;
                nudButton6MinimumSpeed.Value = DEFAULT_MINIMUM_SPEED_FOR_MESSAGE_BREAK;
                nudButton6CModeHysteresis.Value = DEFAULT_C_MODE_HYSTERESIS_VALUE;


                //Then update each used control
                foreach (Control c in my_config.EWS_Config_output.CONTROL)
                {
                    if (c.TRIGGER == "Button1")
                    {
                        if (c.MESSAGE_BREAK_DURATION != 0 && c.MESSAGE_BREAK_MINIMUM_SPEED != 0 && c.C_MODE_HYSTERESIS != 0)
                        {
                            chb_Button1_Message_Break.Checked = true;

                            nudButton1BreakDuration.Value = c.MESSAGE_BREAK_DURATION;
                            nudButton1CModeHysteresis.Value = c.C_MODE_HYSTERESIS;

                            if (lblButton1MinimumSpeed.Text.Contains("KPH"))
                            {
                                nudButton1MinimumSpeed.Value = c.MESSAGE_BREAK_MINIMUM_SPEED;
                            }

                            else
                            {
                                double flTempSpeed = Math.Round(c.MESSAGE_BREAK_MINIMUM_SPEED * KPH_TO_MPH_CONVERSION_FACTOR);

                                if (flTempSpeed > MAXIMUM_C_MODE_MINIMUM_SPEED_MPH)
                                {
                                    nudButton1MinimumSpeed.Value = MAXIMUM_C_MODE_MINIMUM_SPEED_MPH;
                                }

                                else if (flTempSpeed < MINIMUM_C_MODE_MINIMUM_SPEED)
                                {
                                    nudButton1MinimumSpeed.Value = MINIMUM_C_MODE_MINIMUM_SPEED;
                                }

                                else
                                {
                                    nudButton1MinimumSpeed.Value = (decimal)flTempSpeed;
                                }
                            }
                        }
                    }

                    else if (c.TRIGGER == "Button2")
                    {
                        if (c.MESSAGE_BREAK_DURATION != 0 && c.MESSAGE_BREAK_MINIMUM_SPEED != 0 && c.C_MODE_HYSTERESIS != 0)
                        {
                            chb_Button2_Message_Break.Checked = true;

                            nudButton2BreakDuration.Value = c.MESSAGE_BREAK_DURATION;
                            nudButton2CModeHysteresis.Value = c.C_MODE_HYSTERESIS;

                            if (lblButton2MinimumSpeed.Text.Contains("KPH"))
                            {
                                nudButton2MinimumSpeed.Value = c.MESSAGE_BREAK_MINIMUM_SPEED;
                            }

                            else
                            {
                                double flTempSpeed = Math.Round(c.MESSAGE_BREAK_MINIMUM_SPEED * KPH_TO_MPH_CONVERSION_FACTOR);

                                if (flTempSpeed > MAXIMUM_C_MODE_MINIMUM_SPEED_MPH)
                                {
                                    nudButton2MinimumSpeed.Value = MAXIMUM_C_MODE_MINIMUM_SPEED_MPH;
                                }

                                else if (flTempSpeed < MINIMUM_C_MODE_MINIMUM_SPEED)
                                {
                                    nudButton2MinimumSpeed.Value = MINIMUM_C_MODE_MINIMUM_SPEED;
                                }

                                else
                                {
                                    nudButton2MinimumSpeed.Value = (decimal)flTempSpeed;
                                }
                            }
                        }
                    }

                    else if (c.TRIGGER == "Button3")
                    {
                        if (c.MESSAGE_BREAK_DURATION != 0 && c.MESSAGE_BREAK_MINIMUM_SPEED != 0 && c.C_MODE_HYSTERESIS != 0)
                        {
                            chb_Button3_Message_Break.Checked = true;

                            nudButton3BreakDuration.Value = c.MESSAGE_BREAK_DURATION;
                            nudButton3CModeHysteresis.Value = c.C_MODE_HYSTERESIS;

                            if (lblButton3MinimumSpeed.Text.Contains("KPH"))
                            {
                                nudButton3MinimumSpeed.Value = c.MESSAGE_BREAK_MINIMUM_SPEED;
                            }

                            else
                            {
                                double flTempSpeed = Math.Round(c.MESSAGE_BREAK_MINIMUM_SPEED * KPH_TO_MPH_CONVERSION_FACTOR);

                                if (flTempSpeed > MAXIMUM_C_MODE_MINIMUM_SPEED_MPH)
                                {
                                    nudButton3MinimumSpeed.Value = MAXIMUM_C_MODE_MINIMUM_SPEED_MPH;
                                }

                                else if (flTempSpeed < MINIMUM_C_MODE_MINIMUM_SPEED)
                                {
                                    nudButton3MinimumSpeed.Value = MINIMUM_C_MODE_MINIMUM_SPEED;
                                }

                                else
                                {
                                    nudButton3MinimumSpeed.Value = (decimal)flTempSpeed;
                                }
                            }
                        }
                    }

                    else if (c.TRIGGER == "Button4")
                    {
                        if (c.MESSAGE_BREAK_DURATION != 0 && c.MESSAGE_BREAK_MINIMUM_SPEED != 0 && c.C_MODE_HYSTERESIS != 0)
                        {
                            chb_Button4_Message_Break.Checked = true;

                            nudButton4BreakDuration.Value = c.MESSAGE_BREAK_DURATION;
                            nudButton4CModeHysteresis.Value = c.C_MODE_HYSTERESIS;

                            if (lblButton4MinimumSpeed.Text.Contains("KPH"))
                            {
                                nudButton4MinimumSpeed.Value = c.MESSAGE_BREAK_MINIMUM_SPEED;
                            }

                            else
                            {
                                double flTempSpeed = Math.Round(c.MESSAGE_BREAK_MINIMUM_SPEED * KPH_TO_MPH_CONVERSION_FACTOR);

                                if (flTempSpeed > MAXIMUM_C_MODE_MINIMUM_SPEED_MPH)
                                {
                                    nudButton4MinimumSpeed.Value = MAXIMUM_C_MODE_MINIMUM_SPEED_MPH;
                                }

                                else if (flTempSpeed < MINIMUM_C_MODE_MINIMUM_SPEED)
                                {
                                    nudButton4MinimumSpeed.Value = MINIMUM_C_MODE_MINIMUM_SPEED;
                                }

                                else
                                {
                                    nudButton4MinimumSpeed.Value = (decimal)flTempSpeed;
                                }
                            }
                        }
                    }

                    else if (c.TRIGGER == "Button5")
                    {
                        if (c.MESSAGE_BREAK_DURATION != 0 && c.MESSAGE_BREAK_MINIMUM_SPEED != 0 && c.C_MODE_HYSTERESIS != 0)
                        {
                            chb_Button5_Message_Break.Checked = true;

                            nudButton5BreakDuration.Value = c.MESSAGE_BREAK_DURATION;
                            nudButton5CModeHysteresis.Value = c.C_MODE_HYSTERESIS;

                            if (lblButton5MinimumSpeed.Text.Contains("KPH"))
                            {
                                nudButton5MinimumSpeed.Value = c.MESSAGE_BREAK_MINIMUM_SPEED;
                            }

                            else
                            {
                                double flTempSpeed = Math.Round(c.MESSAGE_BREAK_MINIMUM_SPEED * KPH_TO_MPH_CONVERSION_FACTOR);

                                if (flTempSpeed > MAXIMUM_C_MODE_MINIMUM_SPEED_MPH)
                                {
                                    nudButton5MinimumSpeed.Value = MAXIMUM_C_MODE_MINIMUM_SPEED_MPH;
                                }

                                else if (flTempSpeed < MINIMUM_C_MODE_MINIMUM_SPEED)
                                {
                                    nudButton5MinimumSpeed.Value = MINIMUM_C_MODE_MINIMUM_SPEED;
                                }

                                else
                                {
                                    nudButton5MinimumSpeed.Value = (decimal)flTempSpeed;
                                }
                            }
                        }
                    }

                    else if (c.TRIGGER == "Button6")
                    {
                        if (c.MESSAGE_BREAK_DURATION != 0 && c.MESSAGE_BREAK_MINIMUM_SPEED != 0 && c.C_MODE_HYSTERESIS != 0)
                        {
                            chb_Button6_Message_Break.Checked = true;

                            nudButton6BreakDuration.Value = c.MESSAGE_BREAK_DURATION;
                            nudButton6CModeHysteresis.Value = c.C_MODE_HYSTERESIS;

                            if (lblButton6MinimumSpeed.Text.Contains("KPH"))
                            {
                                nudButton6MinimumSpeed.Value = c.MESSAGE_BREAK_MINIMUM_SPEED;
                            }

                            else
                            {
                                double flTempSpeed = Math.Round(c.MESSAGE_BREAK_MINIMUM_SPEED * KPH_TO_MPH_CONVERSION_FACTOR);

                                if (flTempSpeed > MAXIMUM_C_MODE_MINIMUM_SPEED_MPH)
                                {
                                    nudButton6MinimumSpeed.Value = MAXIMUM_C_MODE_MINIMUM_SPEED_MPH;
                                }

                                else if (flTempSpeed < MINIMUM_C_MODE_MINIMUM_SPEED)
                                {
                                    nudButton6MinimumSpeed.Value = MINIMUM_C_MODE_MINIMUM_SPEED;
                                }

                                else
                                {
                                    nudButton6MinimumSpeed.Value = (decimal)flTempSpeed;
                                }
                            }
                        }
                    }
                }


                Load_Frequencies_sources(); // Stations tab region

                //set the combobox default language
                try
                {
                    comboBox_Instr_Default_Lang.Text = my_config.Default_language.Ref_Name;
                }
                catch
                {
                    comboBox_Instr_Default_Lang.Text = "Undetermined";
                }

                AddPlayColumnToInstructionList();

                for (int i = 1; i <= 6; i++)
                {
                    AddPlayColumnToButtonList(i);
                }

                ActionButtonButtonVisibilty();
                ActionStationButtonVisibilty();
                CheckLanguageHasAUsedInstruction();

                tslEWS_ConfigStatus.Text = "Restore Configuration Complete";
            }

            else
            {
                Console.WriteLine("No Configs ??");
                //EWS_MainForm_Load(this, null);
                tslEWS_ConfigStatus.Text = "Restore No Configs Found !";
            }

            //}

        }

        #endregion Backup

        private void prepareUSBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tslEWS_ConfigStatus.Text = "Prepare USB for Log File Started";


            UsbFolderBrowser usbBrowser = new UsbFolderBrowser();
            // Show the Usb Browser
            usbBrowser.ShowDialog();
            //Open brower dialog and get path
            if (usbBrowser.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                Console.WriteLine("Selected: " + usbBrowser.SelectedDrive);

                try
                {
                    //create log directory
                    Directory.CreateDirectory(usbBrowser.SelectedDrive + @"\log");
                    Console.WriteLine(usbBrowser.SelectedDrive + @"\log directory created");
                }

                catch (Exception ex)
                {
                    Console.WriteLine("Error createing 'log' directory on USB stick:  " + ex);
                }

                clean();
                tslEWS_ConfigStatus.Text = "Prepare USB for Log File Complete";
            }

            else
            {
                tslEWS_ConfigStatus.Text = "Prepare USB for Log File Cancelled";
            }

        }

        private void aboutEWSConfigToolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TODO display credits
            /* credit 
                JSON.NET
                Log4Net

                <div>Icon made by <a href="http://www.freepik.com" title="Freepik">Freepik</a> from <a href="http://www.flaticon.com" title="Flaticon">www.flaticon.com</a> is licensed under <a href="http://creativecommons.org/licenses/by/3.0/" title="Creative Commons BY 3.0">CC BY 3.0</a></div>




            */

        }

        #endregion

        #region Instruction Tab events
        /// <summary>
        ///  set the default language
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox_Instr_Default_Lang_SelectedIndexChanged(object sender, EventArgs e)
        {

            // get the langauge record with the name in the combobox
            Language_Record lr = LanguagesJsonGlobal.Language.First(x => x.Ref_Name == comboBox_Instr_Default_Lang.Text);
            if (lr != null)
            {
                my_config.Default_language = lr;
                comboBox_Instr_Lang.Text = my_config.Default_language.Ref_Name;
                my_config.UpdateDefaultLanguage();
            }
        }
        private void button_Instr_Add_Click_1(object sender, EventArgs e)
        {
            // Make sure Name is not EmptydataGridView_Instructions
            if ((textBox_Instr_Name.Text.Count() < 1) || (my_config.Wav_File_Name == ""))
            {
                MessageBox.Show("Please Enter values in all fields !", "Instruction Input Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            else if (textBox_Instr_Authority_8.Text.Replace(" ", String.Empty).Length > 8)
            {
                MessageBox.Show("The authority can be a maximum of 8 digits long.", "Invalid Authority", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // so if we have 4 items create a Instruction record and add it to the list, 
                Instruction i = new Instruction { NAME = textBox_Instr_Name.Text, AUDIO_FILE = my_config.Wav_File_Name, AUTHORITY = textBox_Instr_Authority_8.Text, LANGUAGE = comboBox_Instr_Lang.Text };

                //make sure only one Name in list
                bool containsInstr = my_config.EWS_Config_output.INSTRUCTIONS.Any(item => item.NAME == i.NAME);
                if (containsInstr)
                {
                    MessageBox.Show("Message already defined !", "Add Instruction Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    _OKToCloseandExit = false;  //there has been a change so it is not ok to close until it has been saved

                    my_config.EWS_Config_output.Add_Instruction(new Instruction { NAME = textBox_Instr_Name.Text, AUDIO_FILE = my_config.Wav_File_Name, AUTHORITY = textBox_Instr_Authority_8.Text, LANGUAGE = comboBox_Instr_Lang.Text });
                    my_config.INSTRUCTIONS_POOL.Add(new Instruction { NAME = textBox_Instr_Name.Text, AUDIO_FILE = my_config.Wav_File_Name, AUTHORITY = textBox_Instr_Authority_8.Text, LANGUAGE = comboBox_Instr_Lang.Text });
                    my_config.Audio_files.Add(my_config.Wav_File_Name_path);
                    // the wav file to the temp directory

                    if (File.Exists(my_config.Wav_File_Name_path))
                    {
                        Console.WriteLine("File Source: " + @my_config.Wav_File_Name_path);
                        Console.WriteLine("File Dest: " + my_config.TempPath + my_config.Wav_File_Name);
                        File.Copy(my_config.Wav_File_Name_path, my_config.TempPath + "\\" + my_config.Wav_File_Name, true);
                    }



                    AddPlayColumnToInstructionList();


                    // all entry boxes cleared
                    textBox_Instr_Name.Clear();
                    my_config.Wav_File_Name = "";
                    textBox_Instr_Authority_8.Clear();
                    label_Instr_AudioFile.Text = "";

                    //Make all button add instruction buttons visible
                    button_btn_1_move_left.Visible = true;
                    button_btn_2_move_left.Visible = true;
                    button_btn_3_move_left.Visible = true;
                    button_btn_4_move_left.Visible = true;
                    button_btn_5_move_left.Visible = true;
                    button_btn_6_move_left.Visible = true;
                }

                // auto update dgv
                dataGridView_stations_all_freq.AutoGenerateColumns = false;

                tslEWS_ConfigStatus.Text = i.NAME + " intruction added";
            }

        }


        private void AddPlayColumnToInstructionList()
        {


            if (dataGridView_Instructions.ColumnCount < NUMBEROFINSTRUCTIONTABLECOLUMNS)
            {
                //Add play image column
                DataGridViewImageColumn img = new DataGridViewImageColumn();
                //Image image = Image.FromFile(@"C:\\EWS\\download.jpg");
                Image image = Properties.Resources.Play;
                img.Image = image;
                dataGridView_Instructions.Columns.Insert(PLAYCOLUMNINDEX, img);
                img.Width = image.Width;
                //img.HeaderText = "PLAY";
                img.HeaderText = "";
                img.Name = "img";
            }

        }

        private static string GetValue(IShellProperty value)
        {
            if (value == null || value.ValueAsObject == null)
            {
                return String.Empty;
            }
            return value.ValueAsObject.ToString();
        }

        private void AddSilenceToAudioFiles(ISampleProvider isp, string sfn)
        {
            //add silence at start
            var offset = new OffsetSampleProvider(isp);

            offset.DelayBy = TimeSpan.FromMilliseconds(AUDIO_SILENCE_LENGTH_START);

            IWaveProvider iwp = offset.ToWaveProvider16();

            string newAudioFile = sfn;
            string newAudioFilePathAndName = "";

            //remove .wav and rename file
            newAudioFile = newAudioFile.Remove(newAudioFile.Length - 4);
            newAudioFile = newAudioFile + "_silence.wav";
            newAudioFilePathAndName = currentPath + "temp\\" + newAudioFile;
            try
            {
                //create new audio file          
                WaveFileWriter.CreateWaveFile(newAudioFilePathAndName, iwp);
            }

            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }


            //Add silence at end of the file
            using (var afr = new AudioFileReader(newAudioFilePathAndName))
            {
                var osp = new OffsetSampleProvider(afr);

                osp.LeadOut = TimeSpan.FromMilliseconds(AUDIO_SILENCE_LENGTH_END);

                iwp = osp.ToWaveProvider16();

                //remove .wav and rename file
                newAudioFile = newAudioFile.Remove(newAudioFile.Length - 4);
                newAudioFile = newAudioFile + "_silence.wav";
                newAudioFilePathAndName = currentPath + "temp\\" + newAudioFile;

                try
                {
                    //create new audio file
                    WaveFileWriter.CreateWaveFile(newAudioFilePathAndName, iwp);
                }

                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }

            }

            //copy and replace old file with the new file
            try
            {
                File.Copy(newAudioFilePathAndName, sfn, true);
                my_config.Wav_File_Name = sfn;
                my_config.Wav_File_Name_path = currentPath + "\\" + sfn;
                Console.WriteLine(my_config.Wav_File_Name);
            }

            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }






            label_Instr_AudioFile.Text = my_config.Wav_File_Name;

            //if the Instruction name is blank automatically add the audio file name to the Instruction name text box
            if (textBox_Instr_Name.Text == "")
            {
                string InstructionName = "";
                //remove .wav
                InstructionName = sfn.Split('.')[0];

                //Remove converted if necessary
                if (InstructionName.StartsWith("Converted_"))
                {
                    InstructionName = InstructionName.Remove(0, 10);
                }

                textBox_Instr_Name.Text = InstructionName;
            }
        }

        private void ConvertAudioFile(string audioFileName, string audioFileNameAndPath)
        {
            DialogResult dr = MessageBox.Show("The audio file selected is incompatible with the FM80.  Would you like this file to be converted to a compatible type?\nNote:  This will not replace the original file", "Audio File Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
            if (dr == DialogResult.No)
            {
                MessageBox.Show("The audio file selected is incompatible with the FM80.  Please select a Mono file with a sample rate of 88200 Samples/S or a Stereo file with a sample rate of 44100 Samples/S.", "Audio File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            else
            {
                Cursor.Current = Cursors.WaitCursor;
                if (audioFileName.Contains("mp3"))
                {
                    Console.WriteLine("MP3 Conversion");
                    tslEWS_ConfigStatus.Text = "MP3 Conversion Started";

                    try
                    {
                        using (var reader = new Mp3FileReader(audioFileNameAndPath))
                        {
                            string converted_file_name;
                            var newFormat = new WaveFormat(WAVSTEREOSAMPLERATE, 16, STEREOCHANNELS);

                            using (var conversionStream = new WaveFormatConversionStream(newFormat, reader))
                            {
                                //remove mp3 extension
                                audioFileName = audioFileName.Remove(audioFileName.Length - 3);
                                audioFileName = "Converted_" + audioFileName + "wav";
                                converted_file_name = currentPath + "temp\\" + audioFileName;
                                WaveFileWriter.CreateWaveFile(converted_file_name, conversionStream);

                                //Add silence
                                using (var aud = new WaveFileReader(converted_file_name))
                                {
                                    var v = aud.ToSampleProvider();

                                    AddSilenceToAudioFiles(v, audioFileName);
                                }
                            }

                            //                       newFormat = null;
                        }
                        tslEWS_ConfigStatus.Text = "MP3 Conversion Complete";
                    }

                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                        tslEWS_ConfigStatus.Text = "MP3 Conversion Failed";
                    }


                }

                else
                {
                    tslEWS_ConfigStatus.Text = "WAV Conversion Started";
                    try
                    {
                        string converted_file_name = "";

                        using (var reader = new WaveFileReader(audioFileNameAndPath))
                        {
                            var bps = reader.WaveFormat.BitsPerSample;

                            if (bps == 32)
                            {
                                //Convert to 16 bits per sample
                                using (var wav32to16 = new Wave32To16Stream(reader))
                                {
                                    var newFormat = new WaveFormat(WAVSTEREOSAMPLERATE, 16, STEREOCHANNELS);
                                    using (var conversionStream = new WaveFormatConversionStream(newFormat, wav32to16))
                                    {
                                        converted_file_name = "Converted_" + audioFileName;
                                        string converted_file_name_and_path = currentPath + "temp\\" + converted_file_name; ;
                                        WaveFileWriter.CreateWaveFile(converted_file_name_and_path, conversionStream);

                                        //Add silence
                                        using (var aud = new WaveFileReader(converted_file_name_and_path))
                                        {
                                            var v = aud.ToSampleProvider();

                                            AddSilenceToAudioFiles(v, converted_file_name);
                                        }
                                    }
                                }
                            }

                            else if (bps == REQUIREDBITSPERSAMPLE)
                            {
                                var newFormat = new WaveFormat(WAVSTEREOSAMPLERATE, 16, STEREOCHANNELS);
                                using (var conversionStream = new WaveFormatConversionStream(newFormat, reader))
                                {
                                    converted_file_name = "Converted_" + audioFileName;
                                    string converted_file_name_and_path = currentPath + "temp\\" + converted_file_name; ;
                                    WaveFileWriter.CreateWaveFile(converted_file_name_and_path, conversionStream);

                                    //Add silence
                                    using (var aud = new WaveFileReader(converted_file_name_and_path))
                                    {
                                        var v = aud.ToSampleProvider();
                                        AddSilenceToAudioFiles(v, converted_file_name);
                                    }
                                }
                            }

                            else
                            {
                                MessageBox.Show("Unable to convert this audio file format");
                                tslEWS_ConfigStatus.Text = "WAV Conversion Failed";
                            }
                        }

                        tslEWS_ConfigStatus.Text = "WAV Conversion Complete";
                    }

                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                        tslEWS_ConfigStatus.Text = "WAV Conversion Failed";
                    }


                }

                Cursor.Current = Cursors.Default;
            }
        }

        private void button_Instr_Select_AudioFile_Click(object sender, EventArgs e)
        {
            //create temperary folder if it does not exist
            Directory.CreateDirectory(currentPath + "temp");

            bool _addWavFile = true;
            // Create an instance of the open file dialog box.
            OpenFileDialog fd = new OpenFileDialog();

            // Set filter options and filter index.
            fd.Filter = "Audio Files (.wav, .mp3)|*.wav;*.mp3";
            fd.FilterIndex = 1;

            fd.Multiselect = true;

            // Call the ShowDialog method to show the dialog box.
            DialogResult result = fd.ShowDialog();

            // Process input if the user clicked OK.
            if (result == DialogResult.OK)
            {
                foreach (string s in my_config.Audio_files)
                {
                    if (s.Contains(fd.SafeFileName))
                    {
                        DialogResult dresult = MessageBox.Show("A file with this name:  " + fd.SafeFileName + " has already been chosen for use in a previuos instruction.\n\nIf you continue, the prevoius audio file will be overwritten with this one.\n\nWould you like to continue?", "Identical Audio File Names Detected", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);

                        if (dresult == DialogResult.No)
                        {
                            _addWavFile = false;
                        }
                        break;  //Break here so we don't have multiple occurences of the dialogue box if there is more than one instruction with the same audio file.
                    }
                }


                if (_addWavFile)
                {
                    if (fd.FileName.Contains("mp3"))
                    {
                        ConvertAudioFile(fd.SafeFileName, fd.FileName);
                    }

                    else
                    {
                        try
                        {
                            using (var reader = new WaveFileReader(fd.FileName))
                            {
                                var br = reader.WaveFormat.AverageBytesPerSecond * 8;
                                var bps = reader.WaveFormat.BitsPerSample;
                                var sRate = reader.WaveFormat.SampleRate;
                                var en = reader.WaveFormat.Encoding;
                                var cc = reader.WaveFormat.Channels;
                                Console.WriteLine("Bit Rate: " + br);
                                Console.WriteLine("Bits Per Sample: " + bps);
                                Console.WriteLine("Sample Rate: " + sRate);
                                Console.WriteLine("Encoding: " + en);
                                Console.WriteLine("Number of Channels: " + cc);

                                try
                                {
                                    if (((cc == MONOCHANNELS && sRate == WAVMONOAMPLERATE) || (cc == STEREOCHANNELS && sRate == WAVSTEREOSAMPLERATE)) && bps == REQUIREDBITSPERSAMPLE)
                                    {
                                        //Add silence
                                        using (var aud = new AudioFileReader(fd.FileName))
                                        {
                                            AddSilenceToAudioFiles(aud, fd.SafeFileName);
                                        }
                                    }

                                    else
                                    {
                                        ConvertAudioFile(fd.SafeFileName, fd.FileName);
                                    }
                                }

                                catch
                                {
                                    MessageBox.Show("The audio file is corrupt.\n\nPlease select a different file.", "Audio File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }

                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            tslEWS_ConfigStatus.Text = "The file selected is not an audio file!";
                        }
                    }
                }
            }

            try
            {
                //delete temperary folder if it does not exist
                Directory.Delete(currentPath + "temp", true);
            }

            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        private void textBox_Instr_Authority_8_KeyDown(object sender, KeyEventArgs e)
        {
            // Check for a naughty character in the KeyDown event.  Allowed characters are a-z, 0-9 and space, shift, control, forward slash and underscore and backspace and delete
            if (!((e.KeyValue >= ZERO_KEY_VALUE && e.KeyValue <= NINE_KEY_VALUE) || (e.KeyValue >= A_KEY_VALUE && e.KeyValue <= Z_KEY_VALUE) || e.Shift || e.Control || e.KeyValue == SPACE_KEY_VALUE || e.KeyValue == FORWARD_SLASH_KEY_VALUE || e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete || e.KeyValue == DASH_KEY_VALUE || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right || e.KeyCode == Keys.Tab))
            {
                e.SuppressKeyPress = true;
            }

            //If the shift key is pressed it is only a valid key press if it is a letter or an underscore
            if (e.Modifiers == Keys.Shift)
            {
                if (!((e.KeyValue >= A_KEY_VALUE && e.KeyValue <= Z_KEY_VALUE) || e.KeyValue == DASH_KEY_VALUE))
                {
                    e.SuppressKeyPress = true;
                }
            }
        }

        private void textBox_Instr_Authority_8_TextChanged(object sender, EventArgs e)
        {
            foreach (char c in textBox_Instr_Authority_8.Text)
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(c.ToString(), @"[a-zA-Z0-9 _/-]"))
                {
                    MessageBox.Show("The authority entered contains invalid characters!", "Authority Input Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    textBox_Instr_Authority_8.Text = "";
                    break;
                }
            }

            if (textBox_Instr_Authority_8.TextLength > 8)
            {
                MessageBox.Show("Maximum 8 characters!", "Authority Input Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox_Instr_Authority_8.Text = textBox_Instr_Authority_8.Text.Remove(8);

                textBox_Instr_Authority_8.SelectionStart = textBox_Instr_Authority_8.Text.Length;
                textBox_Instr_Authority_8.SelectionLength = 0;
            }
        }

        private void dataGridView_Instructions_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                _OKToCloseandExit = false;  //there has been a change so it is not ok to close until it has been saved

                Console.WriteLine("Okay now delete !");
                //dataGridView_Instructions.CurrentCell.Selected
                //get index of selected cell
                foreach (DataGridViewRow row in dataGridView_Instructions.SelectedRows)
                {
                    Instruction currentObject = row.DataBoundItem as Instruction;
                    Console.WriteLine(row);
                    Console.WriteLine(currentObject);
                    Console.WriteLine("Intruction count = " + dataGridView_Instructions.RowCount);

                    //Remove audio file from audio file list.
                    foreach (string s in my_config.Audio_files)
                    {
                        if (s.Contains(currentObject.AUDIO_FILE))
                        {
                            my_config.Audio_files.Remove(s);
                            break;  //break here so only one instance of each file is removed.  There may be multiple instructions using the same file name.
                        }
                    }


                    //Don't delete the instruction if it is being used

                    //Button 1?
                    for (int i = 0; i < my_config.Button1_Instructions.Count; i++)
                    {
                        if (my_config.Button1_Instructions[i].NAME == currentObject.NAME)
                        {
                            Console.WriteLine("Remove instruction from Button 1 list first");
                            my_config.Remove_Button_Instruction("Button1", currentObject);
                        }
                    }
                    //Button 2?
                    for (int i = 0; i < my_config.Button2_Instructions.Count; i++)
                    {
                        if (my_config.Button2_Instructions[i].NAME == currentObject.NAME)
                        {
                            Console.WriteLine("Remove instruction from Button 2 list first");
                            my_config.Remove_Button_Instruction("Button2", currentObject);
                        }
                    }

                    //Button 3?
                    for (int i = 0; i < my_config.Button3_Instructions.Count; i++)
                    {
                        if (my_config.Button3_Instructions[i].NAME == currentObject.NAME)
                        {
                            Console.WriteLine("Remove instruction from Button 3 list first");
                            my_config.Remove_Button_Instruction("Button3", currentObject);
                        }
                    }

                    //Button 4?
                    for (int i = 0; i < my_config.Button4_Instructions.Count; i++)
                    {
                        if (my_config.Button4_Instructions[i].NAME == currentObject.NAME)
                        {
                            Console.WriteLine("Remove instruction from Button 4 list first");
                            my_config.Remove_Button_Instruction("Button4", currentObject);
                        }
                    }

                    //Button 5?
                    for (int i = 0; i < my_config.Button5_Instructions.Count; i++)
                    {
                        if (my_config.Button5_Instructions[i].NAME == currentObject.NAME)
                        {
                            Console.WriteLine("Remove instruction from Button 5 list first");
                            my_config.Remove_Button_Instruction("Button5", currentObject);
                        }
                    }

                    //Button 6?
                    for (int i = 0; i < my_config.Button6_Instructions.Count; i++)
                    {
                        if (my_config.Button6_Instructions[i].NAME == currentObject.NAME)
                        {
                            Console.WriteLine("Remove instruction from Button 6 list first");
                            my_config.Remove_Button_Instruction("Button6", currentObject);
                        }
                    }

                    var itemToRemove = my_config.INSTRUCTIONS_POOL.SingleOrDefault(x => x.NAME == currentObject.NAME);
                    if (itemToRemove != null)
                    {
                        INSTRUCTIONS_POOL_Begin_Update();
                        my_config.INSTRUCTIONS_POOL.Remove(itemToRemove);


                        INSTRUCTIONS_POOL_End_Update();

                    }
                }
                ActionButtonButtonVisibilty();
            }
        }

        private void dataGridView_Instructions_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                if (dataGridView_Instructions.Columns[e.ColumnIndex].HeaderText == "")
                {
                    string audiofilename = (dataGridView_Instructions.Rows[e.RowIndex].Cells[AUDIOFILECOLUMNINDEX].Value).ToString();

                    //            MessageBox.Show("Play Audio from row:  " + e.RowIndex + "\n\rAudio file is:  " + audiofilename);
                    SoundPlayer audiofile = new SoundPlayer(my_config.TempPath + "\\" + audiofilename);
                    audiofile.Play();
                }
            }
        }

        private void textBox_Instr_Name_KeyDown(object sender, KeyEventArgs e)
        {
            // Check for a naughty character in the KeyDown event.  Allowed characters are a-z, 0-9 and space, shift, control, forward slash and underscore and backspace and delete, left or right or tab
            if (!((e.KeyValue >= ZERO_KEY_VALUE && e.KeyValue <= NINE_KEY_VALUE) || (e.KeyValue >= A_KEY_VALUE && e.KeyValue <= Z_KEY_VALUE) || e.Shift || e.Control || e.KeyValue == SPACE_KEY_VALUE || e.KeyValue == FORWARD_SLASH_KEY_VALUE || e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right || e.KeyCode == Keys.Tab))
            {
                e.SuppressKeyPress = true;
            }

            //If the shift key is pressed it is only a valid key press if it is a letter or an underscore
            if (e.Modifiers == Keys.Shift)
            {
                if (!((e.KeyValue >= A_KEY_VALUE && e.KeyValue <= Z_KEY_VALUE) || e.KeyValue == DASH_KEY_VALUE))
                {
                    e.SuppressKeyPress = true;
                }
            }
        }

        private void textBox_Instr_Name_TextChanged(object sender, EventArgs e)
        {
            foreach (char c in textBox_Instr_Name.Text)
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(c.ToString(), @"[a-zA-Z0-9 _/-]"))
                {
                    MessageBox.Show("The Instruction Name entered contains invalid characters!", "Instruction Name Input Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    textBox_Instr_Name.Text = "";
                    break;
                }
            }
        }

        #endregion

        #region Stations tab
        /// <summary>
        /// set automatic
        /// level auto
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControl_Main_Click(object sender, EventArgs e)
        {
            CheckLanguageHasAUsedInstruction();
        }

        private void checkBox_stations_auto_stations_CheckedChanged(object sender, EventArgs e)
        {
            _OKToCloseandExit = false;  //there has been a change so it is not ok to close until it has been saved

            if (checkBox_stations_auto_stations.Checked)
            {
                // my_config.EWS_Channels.CHANNELS.AUTOMATIC = new Automatic_Record { ACTIVE = "YES", LEVEL = 100 };
                my_config.EWS_Channels.CHANNELS.AUTOMATIC.ACTIVE = "YES";
                my_config.EWS_Channels.CHANNELS.AUTOMATIC.AUTO_LEVEL = "YES";
                my_config.EWS_Channels.CHANNELS.AUTOMATIC.LEVEL = 100;
            }
            else
            {
                //my_config.EWS_Channels.CHANNELS.AUTOMATIC = new Automatic_Record { ACTIVE = "NO", LEVEL = 100 };
                my_config.EWS_Channels.CHANNELS.AUTOMATIC.ACTIVE = "NO";
                my_config.EWS_Channels.CHANNELS.AUTOMATIC.AUTO_LEVEL = "NO";
                my_config.EWS_Channels.CHANNELS.AUTOMATIC.LEVEL = 100;

            }
        }

        private void ActionStationButtonVisibilty()
        {

            //Include and exclude buttons
            if (dataGridView_stations_all_freq.RowCount < 1)
            {
                button_stations_move_left_include.Visible = false;
                button_stations_move_right_exclude.Visible = false;
            }

            else
            {
                button_stations_move_left_include.Visible = true;
                button_stations_move_right_exclude.Visible = true;
            }

            //return to frequncy pool buttons
            //Include button
            if (dataGridView_stations_Always_Included.RowCount < 1)
            {
                button_stations_move_right_include.Visible = false;
            }

            else
            {
                button_stations_move_right_include.Visible = true;
            }

            //Exclude button
            if (dataGridView_stations_excluded.RowCount < 1)
            {
                button_stations_move_left_exclude.Visible = false;
            }

            else
            {
                button_stations_move_left_exclude.Visible = true;
            }
        }

        private void cbLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool fUpdateLocation = true;

            if (SelectedLocation == null || SelectedLocation.Loc != cbLocation.Text)
            {
                // update the frequency list
                if (dataGridView_stations_Always_Included.RowCount > 0 || dataGridView_stations_excluded.RowCount > 0)
                {
                    DialogResult dialogResult = MessageBox.Show("You have station information assigned to your configuration.  This will be lost if you continue with the location update.", "Station Information Exists", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (dialogResult == DialogResult.Yes)
                    {
                        //Remove station information stations
                        while (dataGridView_stations_Always_Included.RowCount > 0)
                        {

                            // now remove this freq from the main list and add it to freq included
                            string freq = dataGridView_stations_Always_Included.SelectedRows[0].Cells[0].Value.ToString();
                            string lang = dataGridView_stations_Always_Included.SelectedRows[0].Cells[1].Value.ToString();

                            Console.WriteLine(freq);
                            Console.WriteLine(lang);

                            Frequencies_POOL_Begin_Update();

                            my_config.Remove_Frequency_Freq_Include_List_to_Frequency_Pool(freq);

                            Frequencies_POOL_End_Update();
                            ActionStationButtonVisibilty();
                        }

                        //Remove excluded stations
                        while (dataGridView_stations_excluded.RowCount > 0)
                        {
                            Frequency_Record currentObject = null;

                            foreach (DataGridViewRow dgvr in dataGridView_stations_excluded.Rows)
                            {
                                currentObject = dgvr.DataBoundItem as Frequency_Record;

                                Frequencies_POOL_Begin_Update();

                                my_config.Remove_Frequency_Freq_Exclude_List_to_Frequency_Pool(currentObject);

                                Frequencies_POOL_End_Update();
                                ActionStationButtonVisibilty();
                            }
                        }
                    }
                    else
                    {
                        cbLocation.Text = SelectedLocation.Loc;
                        fUpdateLocation = false;
                        tslEWS_ConfigStatus.Text = "Location update cancelled";
                    }

                }

                if (fUpdateLocation == true)
                {
                    SelectedLocation = my_config.LocationsLoaded.First(x => x.Loc == cbLocation.Text);
                    comboBox_Instr_Default_Lang.Text = SelectedLocation.Default_Language;

                    my_config.Frequencies_Pool.Clear();
                    for (int f = SelectedLocation.Start_Freq; f <= SelectedLocation.End_Frequency; f = f + SelectedLocation.Channel_Spacing)
                    {
                        my_config.Frequencies_Pool.Add(new Frequency_Record { FREQUENCY = f, LANGUAGE = "", LEVEL = 100, AUTO_LEVEL = "AUTO", POPULAR = "NO", TA = 1 });
                    }

                    tslEWS_ConfigStatus.Text = "Location and Default Language has have been updated";
                }
            }
        }

        #region include freq
        private void button_stations_move_left_include_Click(object sender, EventArgs e)
        {
            _OKToCloseandExit = false;  //there has been a change so it is not ok to close until it has been saved

            int IncludedStaionsTableRowIndex = 0;
            try
            {

                //Save highlighted row position
                if (dataGridView_stations_Always_Included.RowCount > 0)
                {
                    IncludedStaionsTableRowIndex = dataGridView_stations_Always_Included.SelectedRows[0].Index;
                }
                foreach (DataGridViewRow row in this.dataGridView_stations_all_freq.SelectedRows)
                {
                    Frequency_Record currentObject = row.DataBoundItem as Frequency_Record;

                    //save the position of the frequency list
                    if (dataGridView_stations_all_freq.RowCount > 1)
                    {
                        if (row.Index > 0)
                        {
                            textBox_StationFrequency.Text = (dataGridView_stations_all_freq.Rows[row.Index - 1].Cells[0].Value).ToString();
                        }

                        else
                        {
                            textBox_StationFrequency.Text = (dataGridView_stations_all_freq.Rows[row.Index + 1].Cells[0].Value).ToString();
                        }
                    }

                    else //last frequency left in table
                    {
                        textBox_StationFrequency.Text = "";
                    }

                    if (currentObject != null)
                    {
                        Frequencies_POOL_Begin_Update();

                        my_config.Add_Frequency_from_Frequency_Pool_to_Freq_Include_List(currentObject);

                        Frequencies_POOL_End_Update();
                        
                        ActionStationButtonVisibilty();
                    }
                }
            }
            catch (Exception)
            {

            }

            StationFrequencyTerxtBoxChanged(textBox_StationFrequency.Text, e);

            //Make sure TA checkbox starts checked
            dataGridView_stations_Always_Included.Rows[dataGridView_stations_Always_Included.RowCount - 1].Cells[TACOLUMNINDEX].Value = true;

            dataGridView_stations_Always_Included.Rows[IncludedStaionsTableRowIndex].Selected = true;

            CheckLanguageHasAUsedInstruction();


        }

        /// <summary>
        /// Equ of data bind
        /// </summary>
        private void Add_data_to_include()
        {
            dataGridView_stations_Always_Included.Rows.Clear();
            // now iterate the freq include list
            foreach (Frequency_Record iff in my_config.EWS_Channels.CHANNELS.INCLUDE.ToList())
            //foreach (Frequency_Record iff in my_config.)
            {
                DataGridViewRow row = (DataGridViewRow)dataGridView_stations_Always_Included.RowTemplate.Clone();
                row.CreateCells(dataGridView_stations_Always_Included, iff.FREQUENCY_MHz);
                try
                {
                    // get full name of language from id
                    //Language_Record lr = my_config.Get_From_ID(iff.LANGUAGE);
                    row.Cells[LANGUAGECOLUMNINDEX].Value = iff.LANGUAGE;
                    row.Cells[AUTOLEVELCOLUMNINDEX].Value = iff.AUTO_LEVEL;
                    row.Cells[LEVELCOLUMNINDEX].Value = iff.LEVEL;
                    row.Cells[PREFERREDCOLUMNINDEX].Value = iff.POPULAR;
                    row.Cells[TACOLUMNINDEX].Value = iff.TA==1?true:false;
                }
                catch (Exception)
                {
                    row.Cells[LANGUAGECOLUMNINDEX].Value = "Undetermined";
                    row.Cells[AUTOLEVELCOLUMNINDEX].Value = iff.AUTO_LEVEL;
                    row.Cells[LEVELCOLUMNINDEX].Value = iff.LEVEL;
                    row.Cells[PREFERREDCOLUMNINDEX].Value = iff.POPULAR;
                    row.Cells[TACOLUMNINDEX].Value = iff.TA == 1 ? true : false;
                }

                if ((string)row.Cells[AUTOLEVELCOLUMNINDEX].Value == "AUTO")
                {
                    row.Cells[LEVELCOLUMNINDEX].Style.BackColor = Color.Silver;
                    row.Cells[LEVELCOLUMNINDEX].Style.ForeColor = Color.Silver;
                }

                else

                {
                    row.Cells[LEVELCOLUMNINDEX].Style.BackColor = Color.White;
                    row.Cells[LEVELCOLUMNINDEX].Style.ForeColor = Color.Black;
                }


                dataGridView_stations_Always_Included.Rows.Add(row);
            }



        }
        private void Frequencies_POOL_Begin_Update()
        {
            //if the DataSource is going to be empty, adding the first item will
            //always trigger an ArgumentOutOfRangeException on the selected index.
            //to avoid this, we must stop the binding during the modification of the list.
            _Updating = true;
            dataGridView_stations_Always_Included.DataSource = null;
            dataGridView_stations_all_freq.DataSource = null;
            dataGridView_stations_excluded.DataSource = null;

        }
        private void Frequencies_POOL_End_Update()
        {
            // reload sources
            Load_Frequencies_sources();
            // TODO check if Included pool is updated ?
            _Updating = false;
        }


        private void button_stations_move_right_include_Click(object sender, EventArgs e)
        {
            _OKToCloseandExit = false;  //there has been a change so it is not ok to close until it has been saved

            // get selcted row index
            int ItemIndex = -1;
            // get the selected row
            if (dataGridView_stations_Always_Included.SelectedRows.Count > 0)
            {

                ItemIndex = dataGridView_stations_Always_Included.SelectedRows[0].Index;



                Console.WriteLine("selected freq is " + ItemIndex.ToString());
                // now remove this freq from the main list and add it to freq included
                string freq = dataGridView_stations_Always_Included.SelectedRows[0].Cells[0].Value.ToString();
                string lang = dataGridView_stations_Always_Included.SelectedRows[0].Cells[1].Value.ToString();

                Console.WriteLine(freq);
                Console.WriteLine(lang);

                //position highlighted row of frequencies available to the frequency that is returned
                textBox_StationFrequency.Text = freq;

                Frequencies_POOL_Begin_Update();

                my_config.Remove_Frequency_Freq_Include_List_to_Frequency_Pool(freq);

                Frequencies_POOL_End_Update();
                ActionStationButtonVisibilty();
            }

            //reposition highlighted rows
            StationFrequencyTerxtBoxChanged(textBox_StationFrequency.Text, e);

            if (dataGridView_stations_Always_Included.RowCount > 1)
            {

                if (ItemIndex > 0)
                {
                    ItemIndex--;
                }

                dataGridView_stations_Always_Included.Rows[ItemIndex].Selected = true;
                CheckLanguageHasAUsedInstruction();
            }
        }

        private void dataGridView_stations_Always_Included_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            Console.WriteLine("the value is unknown" + e.Exception.Message);
        }

        private void cbEnhancedStations_CheckedChanged(object sender, EventArgs e)
        {
            if (cbEnhancedStations.Checked)
            {
                nudMessageRepeat.Visible = true;
                lblRepeat.Visible = true;

                my_config.EWS_Channels.CHANNELS.ENHANCED.ACTIVE = "YES";
                my_config.EWS_Channels.CHANNELS.ENHANCED.REPEAT = (int)nudMessageRepeat.Value;
            }

            else
            {
                nudMessageRepeat.Visible = false;
                lblRepeat.Visible = false;

                my_config.EWS_Channels.CHANNELS.ENHANCED.ACTIVE = "NO";
                my_config.EWS_Channels.CHANNELS.ENHANCED.REPEAT = (int)nudMessageRepeat.Value;
            }
        }

        private void nudMessageRepeat_ValueChanged(object sender, EventArgs e)
        {
            my_config.EWS_Channels.CHANNELS.ENHANCED.REPEAT = (int)nudMessageRepeat.Value;
        }
        #endregion

        #region exclude
        /// <summary>
        /// Adds any excluded frequencies back into the Frequencies pool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_stations_move_left_exclude_Click(object sender, EventArgs e)
        {
            _OKToCloseandExit = false;  //there has been a change so it is not ok to close until it has been saved

            // get selcted row index
            int ItemIndex = 0;
            // get the selected row
            if (dataGridView_stations_excluded.SelectedRows.Count > 0)
            {
                Frequency_Record currentObject = null;

                try
                {
                    foreach (DataGridViewRow row in this.dataGridView_stations_excluded.SelectedRows)
                    {

                        //save the position of the frequency list
                        if (row.Index > 0)
                        {
                            ItemIndex = row.Index - 1;
                        }



                        currentObject = row.DataBoundItem as Frequency_Record;
                        if (currentObject != null)
                        {
                            Console.WriteLine(currentObject);
                        }

                        //position highlighted row of frequencies available to the frequency that is returned
                        textBox_StationFrequency.Text = currentObject.FREQUENCY_MHz.ToString();
                    }

                }
                catch (Exception)
                {

                }

                Frequencies_POOL_Begin_Update();

                my_config.Remove_Frequency_Freq_Exclude_List_to_Frequency_Pool(currentObject);

                Frequencies_POOL_End_Update();

                ActionStationButtonVisibilty();
            }

            //reposition highlighted rows
            StationFrequencyTerxtBoxChanged(textBox_StationFrequency.Text, e);
            if (dataGridView_stations_excluded.RowCount > 1)
            {
                dataGridView_stations_excluded.Rows[ItemIndex].Selected = true;
            }

        }

        /// <summary>
        /// Moves frequencies from the main pool to the excluded list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_stations_move_right_exclude_Click(object sender, EventArgs e)
        {
            _OKToCloseandExit = false;  //there has been a change so it is not ok to close until it has been saved

            try
            {
                foreach (DataGridViewRow row in this.dataGridView_stations_all_freq.SelectedRows)
                {
                    if (dataGridView_stations_all_freq.RowCount > 1)
                    {
                        //save the position of the frequency list
                        if (row.Index > 0)
                        {
                            textBox_StationFrequency.Text = (dataGridView_stations_all_freq.Rows[row.Index - 1].Cells[0].Value).ToString();
                        }

                        else
                        {
                            textBox_StationFrequency.Text = (dataGridView_stations_all_freq.Rows[row.Index + 1].Cells[0].Value).ToString();
                        }
                    }

                    else
                    {
                        textBox_StationFrequency.Text = "";
                    }

                    Frequency_Record currentObject = row.DataBoundItem as Frequency_Record;
                    if (currentObject != null)
                    {
                        my_config.Remove_Frequency_from_Frequency_Pool_to_Freq_Exclude_List(currentObject);
                        ActionStationButtonVisibilty();
                    }
                }
            }
            catch (Exception)
            {

            }

            StationFrequencyTerxtBoxChanged(textBox_StationFrequency.Text, e);
        }
        #endregion// exclude

        private void StationFrequencyTextBoxUpdate(object sender, DataGridViewCellEventArgs e)
        {

            //textBox_StationFrequency.Text = dataGridView_stations_all_freq.Rows[e.Index]
            foreach (DataGridViewRow row in this.dataGridView_stations_all_freq.SelectedRows)
            {
                Frequency_Record currentObject = row.DataBoundItem as Frequency_Record;

                textBox_StationFrequency.Text = currentObject.FREQUENCY_MHz.ToString();
            }
        }

        private void StationFrequencyTerxtBoxChanged(object sender, EventArgs e)
        {
            int rowIndex;
            foreach (DataGridViewRow row in dataGridView_stations_all_freq.Rows)
            {
                if (row.Cells[0].Value.ToString().StartsWith(textBox_StationFrequency.Text))
                {
                    rowIndex = row.Index;
                    dataGridView_stations_all_freq.Rows[rowIndex].Selected = true;
                    dataGridView_stations_all_freq.FirstDisplayedScrollingRowIndex = rowIndex;
                    break;
                }
            }
        }


        private void CheckLanguageHasAUsedInstruction()
        {
            int IncludedTableRowIndex = 0;
            bool btn1LanguageMatch = false;
            bool btn2LanguageMatch = false;
            bool btn3LanguageMatch = false;
            bool btn4LanguageMatch = false;
            bool btn5LanguageMatch = false;
            bool btn6LanguageMatch = false;

            foreach (Frequency_Record fr in my_config.EWS_Channels.CHANNELS.INCLUDE)
            {
                if (1 == 1)//Used to exclude Undetermined language.  (fr.LANGUAGE != "Undetermined").  Now it is included
                {
                    Console.WriteLine("Included channel language:  " + fr.LANGUAGE);

                    btn1LanguageMatch = false;
                    btn2LanguageMatch = false;
                    btn3LanguageMatch = false;
                    btn4LanguageMatch = false;
                    btn5LanguageMatch = false;
                    btn6LanguageMatch = false;


                    //Button1
                    if (my_config.Button1_Instructions.Count > 0)
                    {

                        foreach (Instruction btn1ins in my_config.Button1_Instructions)
                        {
                            Console.WriteLine("Button 1 Language:  " + btn1ins.LANGUAGE);
                            if (btn1ins.LANGUAGE == fr.LANGUAGE)
                            {
                                btn1LanguageMatch = true;
                                dataGridView_stations_Always_Included.Rows[IncludedTableRowIndex].DefaultCellStyle.BackColor = Color.White;
                                break;
                            }

                        }

                        if (!btn1LanguageMatch)
                        {
                            dataGridView_stations_Always_Included.Rows[IncludedTableRowIndex].DefaultCellStyle.BackColor = Color.MistyRose;
                            for (int i = 0; i < 4; i++)
                            {
                                dataGridView_stations_Always_Included.Rows[IncludedTableRowIndex].Cells[i].ToolTipText = "There is no instruction for button 1 that uses the " + fr.LANGUAGE + " language ";
                            }
                        }
                    }

                    else
                    {
                        btn1LanguageMatch = true;
                        dataGridView_stations_Always_Included.Rows[IncludedTableRowIndex].DefaultCellStyle.BackColor = Color.White;
                    }

                    //Button2 - Only check if previous button was ok
                    if (btn1LanguageMatch)
                    {
                        if (my_config.Button2_Instructions.Count > 0)
                        {
                            foreach (Instruction btn2ins in my_config.Button2_Instructions)
                            {
                                Console.WriteLine("Button 2 Language:  " + btn2ins.LANGUAGE);
                                if (btn2ins.LANGUAGE == fr.LANGUAGE)
                                {
                                    btn2LanguageMatch = true;
                                    dataGridView_stations_Always_Included.Rows[IncludedTableRowIndex].DefaultCellStyle.BackColor = Color.White;
                                    break;
                                }

                            }

                            if (!btn2LanguageMatch)
                            {
                                dataGridView_stations_Always_Included.Rows[IncludedTableRowIndex].DefaultCellStyle.BackColor = Color.MistyRose;
                                for (int i = 0; i < 4; i++)
                                {
                                    dataGridView_stations_Always_Included.Rows[IncludedTableRowIndex].Cells[i].ToolTipText = "There is no instruction for button 2 that uses the " + fr.LANGUAGE + " language ";
                                }
                            }
                        }

                        else
                        {
                            btn2LanguageMatch = true;
                            dataGridView_stations_Always_Included.Rows[IncludedTableRowIndex].DefaultCellStyle.BackColor = Color.White;
                        }
                    }

                    //Button3 - Only check if previous button was ok
                    if (btn2LanguageMatch)
                    {
                        if (my_config.Button3_Instructions.Count > 0)
                        {
                            foreach (Instruction btn3ins in my_config.Button3_Instructions)
                            {
                                Console.WriteLine("Button 3 Language:  " + btn3ins.LANGUAGE);
                                if (btn3ins.LANGUAGE == fr.LANGUAGE)
                                {
                                    btn3LanguageMatch = true;
                                    dataGridView_stations_Always_Included.Rows[IncludedTableRowIndex].DefaultCellStyle.BackColor = Color.White;
                                    break;
                                }

                            }

                            if (!btn3LanguageMatch)
                            {
                                dataGridView_stations_Always_Included.Rows[IncludedTableRowIndex].DefaultCellStyle.BackColor = Color.MistyRose;
                                for (int i = 0; i < 4; i++)
                                {
                                    dataGridView_stations_Always_Included.Rows[IncludedTableRowIndex].Cells[i].ToolTipText = "There is no instruction for button 3 that uses the " + fr.LANGUAGE + " language ";
                                }
                            }
                        }

                        else
                        {
                            btn3LanguageMatch = true;
                            dataGridView_stations_Always_Included.Rows[IncludedTableRowIndex].DefaultCellStyle.BackColor = Color.White;
                        }
                    }

                    //Button4 - Only check if previous button was ok
                    if (btn3LanguageMatch)
                    {
                        if (my_config.Button4_Instructions.Count > 0)
                        {
                            foreach (Instruction btn4ins in my_config.Button4_Instructions)
                            {
                                Console.WriteLine("Button 4 Language:  " + btn4ins.LANGUAGE);
                                if (btn4ins.LANGUAGE == fr.LANGUAGE)
                                {
                                    btn4LanguageMatch = true;
                                    dataGridView_stations_Always_Included.Rows[IncludedTableRowIndex].DefaultCellStyle.BackColor = Color.White;
                                    break;
                                }

                            }

                            if (!btn4LanguageMatch)
                            {
                                dataGridView_stations_Always_Included.Rows[IncludedTableRowIndex].DefaultCellStyle.BackColor = Color.MistyRose;
                                for (int i = 0; i < 4; i++)
                                {
                                    dataGridView_stations_Always_Included.Rows[IncludedTableRowIndex].Cells[i].ToolTipText = "There is no instruction for button 4 that uses the " + fr.LANGUAGE + " language ";
                                }
                            }
                        }

                        else
                        {
                            btn4LanguageMatch = true;
                            dataGridView_stations_Always_Included.Rows[IncludedTableRowIndex].DefaultCellStyle.BackColor = Color.White;
                        }
                    }

                    //Button5 - Only check if previous button was ok
                    if (btn4LanguageMatch)
                    {
                        if (my_config.Button5_Instructions.Count > 0)
                        {
                            foreach (Instruction btn5ins in my_config.Button5_Instructions)
                            {
                                Console.WriteLine("Button 5 Language:  " + btn5ins.LANGUAGE);
                                if (btn5ins.LANGUAGE == fr.LANGUAGE)
                                {
                                    btn5LanguageMatch = true;
                                    dataGridView_stations_Always_Included.Rows[IncludedTableRowIndex].DefaultCellStyle.BackColor = Color.White;
                                    break;
                                }

                            }

                            if (!btn5LanguageMatch)
                            {
                                dataGridView_stations_Always_Included.Rows[IncludedTableRowIndex].DefaultCellStyle.BackColor = Color.MistyRose;
                                for (int i = 0; i < 4; i++)
                                {
                                    dataGridView_stations_Always_Included.Rows[IncludedTableRowIndex].Cells[i].ToolTipText = "There is no instruction for button 5 that uses the " + fr.LANGUAGE + " language ";
                                }
                            }
                        }

                        else
                        {
                            btn5LanguageMatch = true;
                            dataGridView_stations_Always_Included.Rows[IncludedTableRowIndex].DefaultCellStyle.BackColor = Color.White;
                        }
                    }

                    //Button6 - Only check if previous button was ok
                    if (btn5LanguageMatch)
                    {
                        if (my_config.Button6_Instructions.Count > 0)
                        {
                            foreach (Instruction btn6ins in my_config.Button6_Instructions)
                            {
                                Console.WriteLine("Button 6 Language:  " + btn6ins.LANGUAGE);
                                if (btn6ins.LANGUAGE == fr.LANGUAGE)
                                {
                                    btn6LanguageMatch = true;
                                    dataGridView_stations_Always_Included.Rows[IncludedTableRowIndex].DefaultCellStyle.BackColor = Color.White;
                                    break;
                                }

                            }

                            if (!btn6LanguageMatch)
                            {
                                dataGridView_stations_Always_Included.Rows[IncludedTableRowIndex].DefaultCellStyle.BackColor = Color.MistyRose;
                                for (int i = 0; i < 4; i++)
                                {
                                    dataGridView_stations_Always_Included.Rows[IncludedTableRowIndex].Cells[i].ToolTipText = "There is no instruction for button 6 that uses the " + fr.LANGUAGE + " language ";
                                }
                            }
                        }
                    }
                }

                IncludedTableRowIndex++;
            }
        }
        #endregion // stations

        #region Buttons Tab
        private void tabControl_Btns_Click(object sender, EventArgs e)
        {
            Load_Button_sources();
            for (int i = 1; i <= 6; i++)
            {
                AddPlayColumnToButtonList(i);
            }
            ActionButtonButtonVisibilty();
        }

        private void btnCModeApplyToAllButtons_Click(object sender, EventArgs e)
        {
            Int16 wButton1MinimumSpeedKPH;
            Int16 wButton1MinimumSpeedMPH;

            //Copy C_Mode enable
            chb_Button2_Message_Break.Checked = chb_Button1_Message_Break.Checked;
            chb_Button3_Message_Break.Checked = chb_Button1_Message_Break.Checked;
            chb_Button4_Message_Break.Checked = chb_Button1_Message_Break.Checked;
            chb_Button5_Message_Break.Checked = chb_Button1_Message_Break.Checked;
            chb_Button6_Message_Break.Checked = chb_Button1_Message_Break.Checked;

            //Copy break duration
            nudButton2BreakDuration.Value = nudButton1BreakDuration.Value;
            nudButton3BreakDuration.Value = nudButton1BreakDuration.Value;
            nudButton4BreakDuration.Value = nudButton1BreakDuration.Value;
            nudButton5BreakDuration.Value = nudButton1BreakDuration.Value;
            nudButton6BreakDuration.Value = nudButton1BreakDuration.Value;

            //Copy Hysteresis
            nudButton2CModeHysteresis.Value = nudButton1CModeHysteresis.Value;
            nudButton3CModeHysteresis.Value = nudButton1CModeHysteresis.Value;
            nudButton4CModeHysteresis.Value = nudButton1CModeHysteresis.Value;
            nudButton5CModeHysteresis.Value = nudButton1CModeHysteresis.Value;
            nudButton6CModeHysteresis.Value = nudButton1CModeHysteresis.Value;

            //Copy Minimum Speed
            //Get minimum speed from button 1 nud
            if (lblButton1MinimumSpeed.Text.Contains("KPH"))
            {
                wButton1MinimumSpeedKPH = (short)nudButton1MinimumSpeed.Value;

                //Convert to MPH
                wButton1MinimumSpeedMPH = (short)(wButton1MinimumSpeedKPH * KPH_TO_MPH_CONVERSION_FACTOR);

                //Make sure we are not out of bounds to to rounding
                if (wButton1MinimumSpeedMPH < MINIMUM_C_MODE_MINIMUM_SPEED)
                {
                    wButton1MinimumSpeedMPH = MINIMUM_C_MODE_MINIMUM_SPEED;
                }

                else if (wButton1MinimumSpeedMPH > MAXIMUM_C_MODE_MINIMUM_SPEED_MPH)
                {
                    wButton1MinimumSpeedMPH = MAXIMUM_C_MODE_MINIMUM_SPEED_MPH;
                }
            }

            //Button 1 speed is in MPH
            else
            {
                wButton1MinimumSpeedMPH = (short)nudButton1MinimumSpeed.Value;

                //Convert to KPH
                wButton1MinimumSpeedKPH = (short)(wButton1MinimumSpeedMPH / KPH_TO_MPH_CONVERSION_FACTOR);

                //Make sure we are not out of bounds to to rounding
                if (wButton1MinimumSpeedKPH < MINIMUM_C_MODE_MINIMUM_SPEED)
                {
                    wButton1MinimumSpeedKPH = MINIMUM_C_MODE_MINIMUM_SPEED;
                }

                else if (wButton1MinimumSpeedKPH > MAXIMUM_C_MODE_MINIMUM_SPEED_KPH)
                {
                    wButton1MinimumSpeedKPH = MAXIMUM_C_MODE_MINIMUM_SPEED_KPH;
                }
            }

            //Now copy value to the other buttons
            if (lblButton2MinimumSpeed.Text.Contains("KPH"))
            {
                nudButton2MinimumSpeed.Value = wButton1MinimumSpeedKPH;
            }

            else
            {
                nudButton2MinimumSpeed.Value = wButton1MinimumSpeedMPH;
            }

            if (lblButton3MinimumSpeed.Text.Contains("KPH"))
            {
                nudButton3MinimumSpeed.Value = wButton1MinimumSpeedKPH;
            }

            else
            {
                nudButton3MinimumSpeed.Value = wButton1MinimumSpeedMPH;
            }

            if (lblButton4MinimumSpeed.Text.Contains("KPH"))
            {
                nudButton4MinimumSpeed.Value = wButton1MinimumSpeedKPH;
            }

            else
            {
                nudButton4MinimumSpeed.Value = wButton1MinimumSpeedMPH;
            }

            if (lblButton5MinimumSpeed.Text.Contains("KPH"))
            {
                nudButton5MinimumSpeed.Value = wButton1MinimumSpeedKPH;
            }

            else
            {
                nudButton5MinimumSpeed.Value = wButton1MinimumSpeedMPH;
            }

            if (lblButton6MinimumSpeed.Text.Contains("KPH"))
            {
                nudButton6MinimumSpeed.Value = wButton1MinimumSpeedKPH;
            }

            else
            {
                nudButton6MinimumSpeed.Value = wButton1MinimumSpeedMPH;
            }
        }

        private void ActionButtonButtonVisibilty()
        {
            //remove instruction buttons
            //Button 1
            if (dataGridView_btn_1_instr.RowCount < 1)
            {
                button_btn_1_move_right.Visible = false;
            }

            else
            {
                button_btn_1_move_right.Visible = true;
            }

            //Button2
            if (dataGridView_btn_2_instr.RowCount < 1)
            {
                button_btn_2_move_right.Visible = false;
            }

            else
            {
                button_btn_2_move_right.Visible = true;
            }

            //Button 3
            if (dataGridView_btn_3_instr.RowCount < 1)
            {
                Button_btn_3_move_right.Visible = false;
            }

            else
            {
                Button_btn_3_move_right.Visible = true;
            }

            //Button4
            if (dataGridView_btn_4_instr.RowCount < 1)
            {
                button_btn_4_move_right.Visible = false;
            }

            else
            {
                button_btn_4_move_right.Visible = true;
            }

            //Button 5
            if (dataGridView_btn_5_instr.RowCount < 1)
            {
                button_btn_5_move_right.Visible = false;
            }

            else
            {
                button_btn_5_move_right.Visible = true;
            }

            //Button6
            if (dataGridView_btn_6_instr.RowCount < 1)
            {
                button_btn_6_move_right.Visible = false;
            }

            else
            {
                button_btn_6_move_right.Visible = true;
            }





            //add instruction buttons

            //Button 1
            if (dataGridView_btn_1_list_instr.RowCount < 1)
            {
                button_btn_1_move_left.Visible = false;
            }

            else
            {
                button_btn_1_move_left.Visible = true;
            }

            //Button 2
            if (dataGridView_btn_2_list_instr.RowCount < 1)
            {
                button_btn_2_move_left.Visible = false;
            }

            else
            {
                button_btn_2_move_left.Visible = true;
            }

            //Button 3
            if (dataGridView_btn_3_list_instr.RowCount < 1)
            {
                button_btn_3_move_left.Visible = false;
            }

            else
            {
                button_btn_3_move_left.Visible = true;
            }

            //Button 4
            if (dataGridView_btn_4_list_instr.RowCount < 1)
            {
                button_btn_4_move_left.Visible = false;
            }

            else
            {
                button_btn_4_move_left.Visible = true;
            }

            //Button 5
            if (dataGridView_btn_5_list_instr.RowCount < 1)
            {
                button_btn_5_move_left.Visible = false;
            }

            else
            {
                button_btn_5_move_left.Visible = true;
            }

            //Button 6
            if (dataGridView_btn_6_list_instr.RowCount < 1)
            {
                button_btn_6_move_left.Visible = false;
            }

            else
            {
                button_btn_6_move_left.Visible = true;
            }


        }
        /// <summary>
        /// bind as we go dgv sources
        /// </summary>
        private void Load_Button_sources()
        {
            if (tabControl_Btns.SelectedTab == tabControl_Btns.TabPages["tabPage_1_btn"])
            {

                var instructions_btn1_source = new BindingSource(my_config.INSTRUCTIONS_POOL, null);
                dataGridView_btn_1_list_instr.DataSource = instructions_btn1_source;

                if ((dataGridView_btn_1_list_instr.Columns.Contains("AUDIO_FILE") && (dataGridView_btn_1_list_instr.Columns.Contains("AUTHORITY"))))
                {
                    dataGridView_btn_1_list_instr.Columns.Remove("AUDIO_FILE");
                    dataGridView_btn_1_list_instr.Columns.Remove("AUTHORITY");
                }
                dataGridView_btn_1_list_instr.AutoGenerateColumns = false;

                // load all instructions from Control with Trigger == Button1
                var control_instr_btn1_source = new BindingSource(my_config.Button1_Instructions, null);
                dataGridView_btn_1_instr.DataSource = control_instr_btn1_source;
                if ((dataGridView_btn_1_instr.Columns.Contains("AUDIO_FILE") && (dataGridView_btn_1_instr.Columns.Contains("AUTHORITY"))))
                {
                    dataGridView_btn_1_instr.Columns.Remove("AUDIO_FILE");
                    dataGridView_btn_1_instr.Columns.Remove("AUTHORITY");
                }
                dataGridView_btn_1_instr.AutoGenerateColumns = false;


            }

            if (tabControl_Btns.SelectedTab == tabControl_Btns.TabPages["tabPage_2_btn"])
            {

                var instructions_btn2_source = new BindingSource(my_config.INSTRUCTIONS_POOL, null);
                dataGridView_btn_2_list_instr.DataSource = instructions_btn2_source;

                if ((dataGridView_btn_2_list_instr.Columns.Contains("AUDIO_FILE") && (dataGridView_btn_2_list_instr.Columns.Contains("AUTHORITY"))))
                {
                    dataGridView_btn_2_list_instr.Columns.Remove("AUDIO_FILE");
                    dataGridView_btn_2_list_instr.Columns.Remove("AUTHORITY");
                }
                dataGridView_btn_2_list_instr.AutoGenerateColumns = false;

                // load all instructions from Control with Trigger == Button2
                var control_instr_btn2_source = new BindingSource(my_config.Button2_Instructions, null);
                dataGridView_btn_2_instr.DataSource = control_instr_btn2_source;
                if ((dataGridView_btn_2_instr.Columns.Contains("AUDIO_FILE") && (dataGridView_btn_2_instr.Columns.Contains("AUTHORITY"))))
                {
                    dataGridView_btn_2_instr.Columns.Remove("AUDIO_FILE");
                    dataGridView_btn_2_instr.Columns.Remove("AUTHORITY");
                }
                dataGridView_btn_2_instr.AutoGenerateColumns = false;

            }

            if (tabControl_Btns.SelectedTab == tabControl_Btns.TabPages["tabPage_3_btn"])
            {

                var instructions_btn3_source = new BindingSource(my_config.INSTRUCTIONS_POOL, null);
                dataGridView_btn_3_list_instr.DataSource = instructions_btn3_source;

                if ((dataGridView_btn_3_list_instr.Columns.Contains("AUDIO_FILE") && (dataGridView_btn_3_list_instr.Columns.Contains("AUTHORITY"))))
                {
                    dataGridView_btn_3_list_instr.Columns.Remove("AUDIO_FILE");
                    dataGridView_btn_3_list_instr.Columns.Remove("AUTHORITY");
                }
                dataGridView_btn_3_list_instr.AutoGenerateColumns = false;


                // load all instructions from Control with Trigger == Button3
                var control_instr_btn3_source = new BindingSource(my_config.Button3_Instructions, null);
                dataGridView_btn_3_instr.DataSource = control_instr_btn3_source;
                if ((dataGridView_btn_3_instr.Columns.Contains("AUDIO_FILE") && (dataGridView_btn_3_instr.Columns.Contains("AUTHORITY"))))
                {
                    dataGridView_btn_3_instr.Columns.Remove("AUDIO_FILE");
                    dataGridView_btn_3_instr.Columns.Remove("AUTHORITY");
                }
                dataGridView_btn_3_instr.AutoGenerateColumns = false;

            }

            if (tabControl_Btns.SelectedTab == tabControl_Btns.TabPages["tabPage_4_btn"])
            {

                var instructions_btn4_source = new BindingSource(my_config.INSTRUCTIONS_POOL, null);
                dataGridView_btn_4_list_instr.DataSource = instructions_btn4_source;

                if ((dataGridView_btn_4_list_instr.Columns.Contains("AUDIO_FILE") && (dataGridView_btn_4_list_instr.Columns.Contains("AUTHORITY"))))
                {
                    dataGridView_btn_4_list_instr.Columns.Remove("AUDIO_FILE");
                    dataGridView_btn_4_list_instr.Columns.Remove("AUTHORITY");
                }
                dataGridView_btn_4_list_instr.AutoGenerateColumns = false;

                // load all instructions from Control with Trigger == Button4
                var control_instr_btn4_source = new BindingSource(my_config.Button4_Instructions, null);
                dataGridView_btn_4_instr.DataSource = control_instr_btn4_source;
                if ((dataGridView_btn_4_instr.Columns.Contains("AUDIO_FILE") && (dataGridView_btn_4_instr.Columns.Contains("AUTHORITY"))))
                {
                    dataGridView_btn_4_instr.Columns.Remove("AUDIO_FILE");
                    dataGridView_btn_4_instr.Columns.Remove("AUTHORITY");
                }
                dataGridView_btn_4_instr.AutoGenerateColumns = false;

            }

            if (tabControl_Btns.SelectedTab == tabControl_Btns.TabPages["tabPage_5_btn"])
            {

                var instructions_btn5_source = new BindingSource(my_config.INSTRUCTIONS_POOL, null);
                dataGridView_btn_5_list_instr.DataSource = instructions_btn5_source;

                if ((dataGridView_btn_5_list_instr.Columns.Contains("AUDIO_FILE") && (dataGridView_btn_5_list_instr.Columns.Contains("AUTHORITY"))))
                {
                    dataGridView_btn_5_list_instr.Columns.Remove("AUDIO_FILE");
                    dataGridView_btn_5_list_instr.Columns.Remove("AUTHORITY");
                }
                dataGridView_btn_5_list_instr.AutoGenerateColumns = false;

                // load all instructions from Control with Trigger == Button5
                var control_instr_btn5_source = new BindingSource(my_config.Button5_Instructions, null);
                dataGridView_btn_5_instr.DataSource = control_instr_btn5_source;
                if ((dataGridView_btn_5_instr.Columns.Contains("AUDIO_FILE") && (dataGridView_btn_5_instr.Columns.Contains("AUTHORITY"))))
                {
                    dataGridView_btn_5_instr.Columns.Remove("AUDIO_FILE");
                    dataGridView_btn_5_instr.Columns.Remove("AUTHORITY");
                }
                dataGridView_btn_5_instr.AutoGenerateColumns = false;

            }

            if (tabControl_Btns.SelectedTab == tabControl_Btns.TabPages["tabPage_6_btn"])
            {

                var instructions_btn6_source = new BindingSource(my_config.INSTRUCTIONS_POOL, null);
                dataGridView_btn_6_list_instr.DataSource = instructions_btn6_source;

                if ((dataGridView_btn_6_list_instr.Columns.Contains("AUDIO_FILE") && (dataGridView_btn_6_list_instr.Columns.Contains("AUTHORITY"))))
                {
                    dataGridView_btn_6_list_instr.Columns.Remove("AUDIO_FILE");
                    dataGridView_btn_6_list_instr.Columns.Remove("AUTHORITY");
                }
                dataGridView_btn_6_list_instr.AutoGenerateColumns = false;

                // load all instructions from Control with Trigger == Button6
                var control_instr_btn6_source = new BindingSource(my_config.Button6_Instructions, null);
                dataGridView_btn_6_instr.DataSource = control_instr_btn6_source;
                if ((dataGridView_btn_6_instr.Columns.Contains("AUDIO_FILE") && (dataGridView_btn_6_instr.Columns.Contains("AUTHORITY"))))
                {
                    dataGridView_btn_6_instr.Columns.Remove("AUDIO_FILE");
                    dataGridView_btn_6_instr.Columns.Remove("AUTHORITY");
                }
                dataGridView_btn_6_instr.AutoGenerateColumns = false;

            }
        }


        #region Button 1
        /// <summary>
        /// Add a list of instructions to Button 1 Trigger
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_btn_1_move_left_Click(object sender, EventArgs e)
        {
            _OKToCloseandExit = false;  //there has been a change so it is not ok to close until it has been saved
            bool btn1LanguageAlreadyUsed = false;
            try
            {
                foreach (DataGridViewRow row in this.dataGridView_btn_1_list_instr.SelectedRows)
                {
                    Instruction currentObject = row.DataBoundItem as Instruction;
                    if (currentObject != null)
                    {
                        //Check if we already have an instructin with the selected language
                        foreach (Instruction btn1ins in my_config.Button1_Instructions)
                        {
                            Console.WriteLine("Button 1 Language:  " + btn1ins.LANGUAGE);
                            if (btn1ins.LANGUAGE == currentObject.LANGUAGE)
                            {
                                MessageBox.Show("Button 1 already has a message with the " + currentObject.LANGUAGE + " language", "Instruction Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                btn1LanguageAlreadyUsed = true;
                                break;
                            }

                        }

                        if (!btn1LanguageAlreadyUsed)
                        {
                            INSTRUCTIONS_POOL_Begin_Update();
                            my_config.Add_Button_Instruction("Button1", currentObject);
                            INSTRUCTIONS_POOL_End_Update();
                            ActionButtonButtonVisibilty();
                        }
                    }
                }

                AddPlayColumnToButtonList(1);
            }
            catch (Exception)
            {

            }



        }


        private void INSTRUCTIONS_POOL_Begin_Update()
        {
            //if the DataSource is going to be empty, adding the first item will
            //always trigger an ArgumentOutOfRangeException on the selected index.
            //to avoid this, we must stop the binding during the modification of the list.
            _Updating = true;
            dataGridView_btn_1_list_instr.DataSource = null;
            dataGridView_btn_1_instr.DataSource = null;

            dataGridView_btn_2_list_instr.DataSource = null;
            dataGridView_btn_2_instr.DataSource = null;

            dataGridView_btn_3_list_instr.DataSource = null;
            dataGridView_btn_3_instr.DataSource = null;

            dataGridView_btn_4_list_instr.DataSource = null;
            dataGridView_btn_4_instr.DataSource = null;

            dataGridView_btn_5_list_instr.DataSource = null;
            dataGridView_btn_5_instr.DataSource = null;

            dataGridView_btn_6_list_instr.DataSource = null;
            dataGridView_btn_6_instr.DataSource = null;

        }
        private void INSTRUCTIONS_POOL_End_Update()
        {
            Load_Button_sources();
            _Updating = false;
        }

        private void button_btn_1_move_right_Click(object sender, EventArgs e)
        {
            _OKToCloseandExit = false;  //there has been a change so it is not ok to close until it has been saved

            try
            {
                foreach (DataGridViewRow row in this.dataGridView_btn_1_instr.SelectedRows)
                {
                    Instruction currentObject = row.DataBoundItem as Instruction;
                    if (currentObject != null)
                    {
                        my_config.Remove_Button_Instruction("Button1", currentObject);
                        ActionButtonButtonVisibilty();
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void lblButton1MinimumSpeed_Click(object sender, EventArgs e)
        {
            //Set temporary nud limits so there is room for calculations
            nudButton1MinimumSpeed.Minimum = TEMPORARY_MINIMUM_C_MODE_MINIMUM;
            nudButton1MinimumSpeed.Maximum = TEMPORARY_MAXIMUM_C_MODE_MINIMUM;

            if (lblButton1MinimumSpeed.Text.Contains("KPH"))
            {
                //Change label to MPH
                lblButton1MinimumSpeed.Text = "Minimum Speed (MPH)";

                //Update value of minimum speed
                Math.Round(nudButton1MinimumSpeed.Value *= (Decimal)KPH_TO_MPH_CONVERSION_FACTOR);

                if (nudButton1MinimumSpeed.Value > MAXIMUM_C_MODE_MINIMUM_SPEED_MPH)
                {
                    nudButton1MinimumSpeed.Value = MAXIMUM_C_MODE_MINIMUM_SPEED_MPH;
                }

                else if (nudButton1MinimumSpeed.Value < MINIMUM_C_MODE_MINIMUM_SPEED)
                {
                    nudButton1MinimumSpeed.Value = MINIMUM_C_MODE_MINIMUM_SPEED;
                }

                //Change speed upper and lower limits
                nudButton1MinimumSpeed.Maximum = MAXIMUM_C_MODE_MINIMUM_SPEED_MPH;
                nudButton1MinimumSpeed.Minimum = MINIMUM_C_MODE_MINIMUM_SPEED;
            }

            else
            {
                //Change label to KPH
                lblButton1MinimumSpeed.Text = "Minimum Speed (KPH)";

                //Update value of minimum speed
                Math.Round(nudButton1MinimumSpeed.Value /= (Decimal)KPH_TO_MPH_CONVERSION_FACTOR);

                if (nudButton1MinimumSpeed.Value > MAXIMUM_C_MODE_MINIMUM_SPEED_KPH)
                {
                    nudButton1MinimumSpeed.Value = MAXIMUM_C_MODE_MINIMUM_SPEED_KPH;
                }

                else if (nudButton1MinimumSpeed.Value < MINIMUM_C_MODE_MINIMUM_SPEED)
                {
                    nudButton1MinimumSpeed.Value = MINIMUM_C_MODE_MINIMUM_SPEED;
                }


                //Change speed upper and lower limits
                nudButton1MinimumSpeed.Maximum = MAXIMUM_C_MODE_MINIMUM_SPEED_KPH;
                nudButton1MinimumSpeed.Minimum = MINIMUM_C_MODE_MINIMUM_SPEED;
            }

        }

        #endregion //button 1

        #region Button2
        private void button_btn_2_move_left_Click(object sender, EventArgs e)
        {
            _OKToCloseandExit = false;  //there has been a change so it is not ok to close until it has been saved
            bool btn2LanguageAlreadyUsed = false;
            try
            {
                foreach (DataGridViewRow row in this.dataGridView_btn_2_list_instr.SelectedRows)
                {
                    Instruction currentObject = row.DataBoundItem as Instruction;
                    if (currentObject != null)
                    {
                        //Check if we already have an instructin with the selected language
                        foreach (Instruction btn2ins in my_config.Button2_Instructions)
                        {
                            Console.WriteLine("Button 2 Language:  " + btn2ins.LANGUAGE);
                            if (btn2ins.LANGUAGE == currentObject.LANGUAGE)
                            {
                                MessageBox.Show("Button 2 already has a message with the " + currentObject.LANGUAGE + " language", "Instruction Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                btn2LanguageAlreadyUsed = true;
                                break;
                            }

                        }

                        if (!btn2LanguageAlreadyUsed)
                        {
                            INSTRUCTIONS_POOL_Begin_Update();
                            my_config.Add_Button_Instruction("Button2", currentObject);
                            INSTRUCTIONS_POOL_End_Update();
                            ActionButtonButtonVisibilty();
                        }
                    }
                }

                AddPlayColumnToButtonList(2);
            }
            catch (Exception)
            {

            }
        }
        private void button_btn_2_move_right_Click(object sender, EventArgs e)
        {
            _OKToCloseandExit = false;  //there has been a change so it is not ok to close until it has been saved

            try
            {
                foreach (DataGridViewRow row in this.dataGridView_btn_2_instr.SelectedRows)
                {
                    Instruction currentObject = row.DataBoundItem as Instruction;
                    if (currentObject != null)
                    {
                        my_config.Remove_Button_Instruction("Button2", currentObject);
                        ActionButtonButtonVisibilty();
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void lblButton2MinimumSpeed_Click(object sender, EventArgs e)
        {
            //Set temporary nud limits so there is room for calculations
            nudButton2MinimumSpeed.Minimum = TEMPORARY_MINIMUM_C_MODE_MINIMUM;
            nudButton2MinimumSpeed.Maximum = TEMPORARY_MAXIMUM_C_MODE_MINIMUM;

            if (lblButton2MinimumSpeed.Text.Contains("KPH"))
            {
                //Change label to MPH
                lblButton2MinimumSpeed.Text = "Minimum Speed (MPH)";

                //Update value of minimum speed
                Math.Round(nudButton2MinimumSpeed.Value *= (Decimal)KPH_TO_MPH_CONVERSION_FACTOR);

                if (nudButton2MinimumSpeed.Value > MAXIMUM_C_MODE_MINIMUM_SPEED_MPH)
                {
                    nudButton2MinimumSpeed.Value = MAXIMUM_C_MODE_MINIMUM_SPEED_MPH;
                }

                else if (nudButton2MinimumSpeed.Value < MINIMUM_C_MODE_MINIMUM_SPEED)
                {
                    nudButton2MinimumSpeed.Value = MINIMUM_C_MODE_MINIMUM_SPEED;
                }

                //Change speed upper and lower limits
                nudButton2MinimumSpeed.Maximum = MAXIMUM_C_MODE_MINIMUM_SPEED_MPH;
                nudButton2MinimumSpeed.Minimum = MINIMUM_C_MODE_MINIMUM_SPEED;
            }

            else
            {
                //Change label to KPH
                lblButton2MinimumSpeed.Text = "Minimum Speed (KPH)";

                //Update value of minimum speed
                Math.Round(nudButton2MinimumSpeed.Value /= (Decimal)KPH_TO_MPH_CONVERSION_FACTOR);

                if (nudButton2MinimumSpeed.Value > MAXIMUM_C_MODE_MINIMUM_SPEED_KPH)
                {
                    nudButton2MinimumSpeed.Value = MAXIMUM_C_MODE_MINIMUM_SPEED_KPH;
                }

                else if (nudButton2MinimumSpeed.Value < MINIMUM_C_MODE_MINIMUM_SPEED)
                {
                    nudButton2MinimumSpeed.Value = MINIMUM_C_MODE_MINIMUM_SPEED;
                }


                //Change speed upper and lower limits
                nudButton2MinimumSpeed.Maximum = MAXIMUM_C_MODE_MINIMUM_SPEED_KPH;
                nudButton2MinimumSpeed.Minimum = MINIMUM_C_MODE_MINIMUM_SPEED;
            }
        }
        #endregion //button2

        #region Button 3
        private void button_btn_3_move_left_Click(object sender, EventArgs e)
        {
            _OKToCloseandExit = false;  //there has been a change so it is not ok to close until it has been saved
            bool btn3LanguageAlreadyUsed = false;
            try
            {
                foreach (DataGridViewRow row in this.dataGridView_btn_3_list_instr.SelectedRows)
                {
                    Instruction currentObject = row.DataBoundItem as Instruction;
                    if (currentObject != null)
                    {
                        //Check if we already have an instructin with the selected language
                        foreach (Instruction btn3ins in my_config.Button3_Instructions)
                        {
                            Console.WriteLine("Button 3 Language:  " + btn3ins.LANGUAGE);
                            if (btn3ins.LANGUAGE == currentObject.LANGUAGE)
                            {
                                MessageBox.Show("Button 3 already has a message with the " + currentObject.LANGUAGE + " language", "Instruction Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                btn3LanguageAlreadyUsed = true;
                                break;
                            }

                        }

                        if (!btn3LanguageAlreadyUsed)
                        {
                            INSTRUCTIONS_POOL_Begin_Update();
                            my_config.Add_Button_Instruction("Button3", currentObject);
                            INSTRUCTIONS_POOL_End_Update();
                            ActionButtonButtonVisibilty();
                        }
                    }
                }

                AddPlayColumnToButtonList(3);
            }
            catch (Exception)
            {

            }
        }
        private void Button_btn_3_move_right_Click(object sender, EventArgs e)
        {
            _OKToCloseandExit = false;  //there has been a change so it is not ok to close until it has been saved

            try
            {
                foreach (DataGridViewRow row in this.dataGridView_btn_3_instr.SelectedRows)
                {
                    Instruction currentObject = row.DataBoundItem as Instruction;
                    if (currentObject != null)
                    {
                        my_config.Remove_Button_Instruction("Button3", currentObject);
                        ActionButtonButtonVisibilty();
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void lblButton3MinimumSpeed_Click(object sender, EventArgs e)
        {
            //Set temporary nud limits so there is room for calculations
            nudButton3MinimumSpeed.Minimum = TEMPORARY_MINIMUM_C_MODE_MINIMUM;
            nudButton3MinimumSpeed.Maximum = TEMPORARY_MAXIMUM_C_MODE_MINIMUM;

            if (lblButton3MinimumSpeed.Text.Contains("KPH"))
            {
                //Change label to MPH
                lblButton3MinimumSpeed.Text = "Minimum Speed (MPH)";

                //Update value of minimum speed
                Math.Round(nudButton3MinimumSpeed.Value *= (Decimal)KPH_TO_MPH_CONVERSION_FACTOR);

                if (nudButton3MinimumSpeed.Value > MAXIMUM_C_MODE_MINIMUM_SPEED_MPH)
                {
                    nudButton3MinimumSpeed.Value = MAXIMUM_C_MODE_MINIMUM_SPEED_MPH;
                }

                else if (nudButton3MinimumSpeed.Value < MINIMUM_C_MODE_MINIMUM_SPEED)
                {
                    nudButton3MinimumSpeed.Value = MINIMUM_C_MODE_MINIMUM_SPEED;
                }

                //Change speed upper and lower limits
                nudButton3MinimumSpeed.Maximum = MAXIMUM_C_MODE_MINIMUM_SPEED_MPH;
                nudButton3MinimumSpeed.Minimum = MINIMUM_C_MODE_MINIMUM_SPEED;
            }

            else
            {
                //Change label to KPH
                lblButton3MinimumSpeed.Text = "Minimum Speed (KPH)";

                //Update value of minimum speed
                Math.Round(nudButton3MinimumSpeed.Value /= (Decimal)KPH_TO_MPH_CONVERSION_FACTOR);

                if (nudButton3MinimumSpeed.Value > MAXIMUM_C_MODE_MINIMUM_SPEED_KPH)
                {
                    nudButton3MinimumSpeed.Value = MAXIMUM_C_MODE_MINIMUM_SPEED_KPH;
                }

                else if (nudButton3MinimumSpeed.Value < MINIMUM_C_MODE_MINIMUM_SPEED)
                {
                    nudButton3MinimumSpeed.Value = MINIMUM_C_MODE_MINIMUM_SPEED;
                }


                //Change speed upper and lower limits
                nudButton3MinimumSpeed.Maximum = MAXIMUM_C_MODE_MINIMUM_SPEED_KPH;
                nudButton3MinimumSpeed.Minimum = MINIMUM_C_MODE_MINIMUM_SPEED;
            }
        }
        #endregion // button 3


        #region Button 4
        private void button_btn_4_move_left_Click(object sender, EventArgs e)
        {
            _OKToCloseandExit = false;  //there has been a change so it is not ok to close until it has been saved
            bool btn4LanguageAlreadyUsed = false;
            try
            {
                foreach (DataGridViewRow row in this.dataGridView_btn_4_list_instr.SelectedRows)
                {
                    Instruction currentObject = row.DataBoundItem as Instruction;
                    if (currentObject != null)
                    {
                        //Check if we already have an instructin with the selected language
                        foreach (Instruction btn4ins in my_config.Button4_Instructions)
                        {
                            Console.WriteLine("Button 4 Language:  " + btn4ins.LANGUAGE);
                            if (btn4ins.LANGUAGE == currentObject.LANGUAGE)
                            {
                                MessageBox.Show("Button 4 already has a message with the " + currentObject.LANGUAGE + " language", "Instruction Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                btn4LanguageAlreadyUsed = true;
                                break;
                            }

                        }

                        if (!btn4LanguageAlreadyUsed)
                        {
                            INSTRUCTIONS_POOL_Begin_Update();
                            my_config.Add_Button_Instruction("Button4", currentObject);
                            INSTRUCTIONS_POOL_End_Update();
                            ActionButtonButtonVisibilty();
                        }
                    }
                }

                AddPlayColumnToButtonList(4);
            }
            catch (Exception)
            {

            }
        }
        private void button_btn_4_move_right_Click(object sender, EventArgs e)
        {
            _OKToCloseandExit = false;  //there has been a change so it is not ok to close until it has been saved

            try
            {
                foreach (DataGridViewRow row in this.dataGridView_btn_4_instr.SelectedRows)
                {
                    Instruction currentObject = row.DataBoundItem as Instruction;
                    if (currentObject != null)
                    {
                        my_config.Remove_Button_Instruction("Button4", currentObject);
                        ActionButtonButtonVisibilty();
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void lblButton4MinimumSpeed_Click(object sender, EventArgs e)
        {
            //Set temporary nud limits so there is room for calculations
            nudButton4MinimumSpeed.Minimum = TEMPORARY_MINIMUM_C_MODE_MINIMUM;
            nudButton4MinimumSpeed.Maximum = TEMPORARY_MAXIMUM_C_MODE_MINIMUM;

            if (lblButton4MinimumSpeed.Text.Contains("KPH"))
            {
                //Change label to MPH
                lblButton4MinimumSpeed.Text = "Minimum Speed (MPH)";

                //Update value of minimum speed
                Math.Round(nudButton4MinimumSpeed.Value *= (Decimal)KPH_TO_MPH_CONVERSION_FACTOR);

                if (nudButton4MinimumSpeed.Value > MAXIMUM_C_MODE_MINIMUM_SPEED_MPH)
                {
                    nudButton4MinimumSpeed.Value = MAXIMUM_C_MODE_MINIMUM_SPEED_MPH;
                }

                else if (nudButton4MinimumSpeed.Value < MINIMUM_C_MODE_MINIMUM_SPEED)
                {
                    nudButton4MinimumSpeed.Value = MINIMUM_C_MODE_MINIMUM_SPEED;
                }

                //Change speed upper and lower limits
                nudButton4MinimumSpeed.Maximum = MAXIMUM_C_MODE_MINIMUM_SPEED_MPH;
                nudButton4MinimumSpeed.Minimum = MINIMUM_C_MODE_MINIMUM_SPEED;
            }

            else
            {
                //Change label to KPH
                lblButton4MinimumSpeed.Text = "Minimum Speed (KPH)";

                //Update value of minimum speed
                Math.Round(nudButton4MinimumSpeed.Value /= (Decimal)KPH_TO_MPH_CONVERSION_FACTOR);

                if (nudButton4MinimumSpeed.Value > MAXIMUM_C_MODE_MINIMUM_SPEED_KPH)
                {
                    nudButton4MinimumSpeed.Value = MAXIMUM_C_MODE_MINIMUM_SPEED_KPH;
                }

                else if (nudButton4MinimumSpeed.Value < MINIMUM_C_MODE_MINIMUM_SPEED)
                {
                    nudButton4MinimumSpeed.Value = MINIMUM_C_MODE_MINIMUM_SPEED;
                }


                //Change speed upper and lower limits
                nudButton4MinimumSpeed.Maximum = MAXIMUM_C_MODE_MINIMUM_SPEED_KPH;
                nudButton4MinimumSpeed.Minimum = MINIMUM_C_MODE_MINIMUM_SPEED;
            }
        }
        #endregion // button 4

        #region Button 5
        private void button_btn_5_move_left_Click(object sender, EventArgs e)
        {
            _OKToCloseandExit = false;  //there has been a change so it is not ok to close until it has been saved
            bool btn5LanguageAlreadyUsed = false;
            try
            {
                foreach (DataGridViewRow row in this.dataGridView_btn_5_list_instr.SelectedRows)
                {
                    Instruction currentObject = row.DataBoundItem as Instruction;
                    if (currentObject != null)
                    {
                        //Check if we already have an instructin with the selected language
                        foreach (Instruction btn5ins in my_config.Button5_Instructions)
                        {
                            Console.WriteLine("Button 5 Language:  " + btn5ins.LANGUAGE);
                            if (btn5ins.LANGUAGE == currentObject.LANGUAGE)
                            {
                                MessageBox.Show("Button 5 already has a message with the " + currentObject.LANGUAGE + " language", "Instruction Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                btn5LanguageAlreadyUsed = true;
                                break;
                            }

                        }

                        if (!btn5LanguageAlreadyUsed)
                        {
                            INSTRUCTIONS_POOL_Begin_Update();
                            my_config.Add_Button_Instruction("Button5", currentObject);
                            INSTRUCTIONS_POOL_End_Update();
                            ActionButtonButtonVisibilty();
                        }
                    }
                }

                AddPlayColumnToButtonList(5);
            }
            catch (Exception)
            {

            }

        }

        private void button_btn_5_move_right_Click(object sender, EventArgs e)
        {
            _OKToCloseandExit = false;  //there has been a change so it is not ok to close until it has been saved

            try
            {
                foreach (DataGridViewRow row in this.dataGridView_btn_5_instr.SelectedRows)
                {
                    Instruction currentObject = row.DataBoundItem as Instruction;
                    if (currentObject != null)
                    {
                        my_config.Remove_Button_Instruction("Button5", currentObject);
                        ActionButtonButtonVisibilty();
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void lblButton5MinimumSpeed_Click(object sender, EventArgs e)
        {
            //Set temporary nud limits so there is room for calculations
            nudButton5MinimumSpeed.Minimum = TEMPORARY_MINIMUM_C_MODE_MINIMUM;
            nudButton5MinimumSpeed.Maximum = TEMPORARY_MAXIMUM_C_MODE_MINIMUM;

            if (lblButton5MinimumSpeed.Text.Contains("KPH"))
            {
                //Change label to MPH
                lblButton5MinimumSpeed.Text = "Minimum Speed (MPH)";

                //Update value of minimum speed
                Math.Round(nudButton5MinimumSpeed.Value *= (Decimal)KPH_TO_MPH_CONVERSION_FACTOR);

                if (nudButton5MinimumSpeed.Value > MAXIMUM_C_MODE_MINIMUM_SPEED_MPH)
                {
                    nudButton5MinimumSpeed.Value = MAXIMUM_C_MODE_MINIMUM_SPEED_MPH;
                }

                else if (nudButton5MinimumSpeed.Value < MINIMUM_C_MODE_MINIMUM_SPEED)
                {
                    nudButton5MinimumSpeed.Value = MINIMUM_C_MODE_MINIMUM_SPEED;
                }

                //Change speed upper and lower limits
                nudButton5MinimumSpeed.Maximum = MAXIMUM_C_MODE_MINIMUM_SPEED_MPH;
                nudButton5MinimumSpeed.Minimum = MINIMUM_C_MODE_MINIMUM_SPEED;
            }

            else
            {
                //Change label to KPH
                lblButton5MinimumSpeed.Text = "Minimum Speed (KPH)";

                //Update value of minimum speed
                Math.Round(nudButton5MinimumSpeed.Value /= (Decimal)KPH_TO_MPH_CONVERSION_FACTOR);

                if (nudButton5MinimumSpeed.Value > MAXIMUM_C_MODE_MINIMUM_SPEED_KPH)
                {
                    nudButton5MinimumSpeed.Value = MAXIMUM_C_MODE_MINIMUM_SPEED_KPH;
                }

                else if (nudButton5MinimumSpeed.Value < MINIMUM_C_MODE_MINIMUM_SPEED)
                {
                    nudButton5MinimumSpeed.Value = MINIMUM_C_MODE_MINIMUM_SPEED;
                }


                //Change speed upper and lower limits
                nudButton5MinimumSpeed.Maximum = MAXIMUM_C_MODE_MINIMUM_SPEED_KPH;
                nudButton5MinimumSpeed.Minimum = MINIMUM_C_MODE_MINIMUM_SPEED;
            }
        }

        #endregion button 5

        #region Button 6
        private void button_btn_6_move_left_Click(object sender, EventArgs e)
        {
            _OKToCloseandExit = false;  //there has been a change so it is not ok to close until it has been saved
            bool btn6LanguageAlreadyUsed = false;
            try
            {
                foreach (DataGridViewRow row in this.dataGridView_btn_6_list_instr.SelectedRows)
                {
                    Instruction currentObject = row.DataBoundItem as Instruction;
                    if (currentObject != null)
                    {
                        //Check if we already have an instructin with the selected language
                        foreach (Instruction btn6ins in my_config.Button6_Instructions)
                        {
                            Console.WriteLine("Button 6 Language:  " + btn6ins.LANGUAGE);
                            if (btn6ins.LANGUAGE == currentObject.LANGUAGE)
                            {
                                MessageBox.Show("Button 6 already has a message with the " + currentObject.LANGUAGE + " language", "Instruction Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                btn6LanguageAlreadyUsed = true;
                                break;
                            }

                        }

                        if (!btn6LanguageAlreadyUsed)
                        {
                            INSTRUCTIONS_POOL_Begin_Update();
                            my_config.Add_Button_Instruction("Button6", currentObject);
                            INSTRUCTIONS_POOL_End_Update();
                            ActionButtonButtonVisibilty();
                        }
                    }
                }

                AddPlayColumnToButtonList(6);
            }
            catch (Exception)
            {

            }
        }

        private void button_btn_6_move_right_Click(object sender, EventArgs e)
        {
            _OKToCloseandExit = false;  //there has been a change so it is not ok to close until it has been saved

            try
            {
                foreach (DataGridViewRow row in this.dataGridView_btn_6_instr.SelectedRows)
                {
                    Instruction currentObject = row.DataBoundItem as Instruction;
                    if (currentObject != null)
                    {
                        my_config.Remove_Button_Instruction("Button6", currentObject);
                        ActionButtonButtonVisibilty();
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void lblButton6MinimumSpeed_Click(object sender, EventArgs e)
        {
            //Set temporary nud limits so there is room for calculations
            nudButton6MinimumSpeed.Minimum = TEMPORARY_MINIMUM_C_MODE_MINIMUM;
            nudButton6MinimumSpeed.Maximum = TEMPORARY_MAXIMUM_C_MODE_MINIMUM;

            if (lblButton6MinimumSpeed.Text.Contains("KPH"))
            {
                //Change label to MPH
                lblButton6MinimumSpeed.Text = "Minimum Speed (MPH)";

                //Update value of minimum speed
                Math.Round(nudButton6MinimumSpeed.Value *= (Decimal)KPH_TO_MPH_CONVERSION_FACTOR);

                if (nudButton6MinimumSpeed.Value > MAXIMUM_C_MODE_MINIMUM_SPEED_MPH)
                {
                    nudButton6MinimumSpeed.Value = MAXIMUM_C_MODE_MINIMUM_SPEED_MPH;
                }

                else if (nudButton6MinimumSpeed.Value < MINIMUM_C_MODE_MINIMUM_SPEED)
                {
                    nudButton6MinimumSpeed.Value = MINIMUM_C_MODE_MINIMUM_SPEED;
                }

                //Change speed upper and lower limits
                nudButton6MinimumSpeed.Maximum = MAXIMUM_C_MODE_MINIMUM_SPEED_MPH;
                nudButton6MinimumSpeed.Minimum = MINIMUM_C_MODE_MINIMUM_SPEED;
            }

            else
            {
                //Change label to KPH
                lblButton6MinimumSpeed.Text = "Minimum Speed (KPH)";

                //Update value of minimum speed
                Math.Round(nudButton6MinimumSpeed.Value /= (Decimal)KPH_TO_MPH_CONVERSION_FACTOR);

                if (nudButton6MinimumSpeed.Value > MAXIMUM_C_MODE_MINIMUM_SPEED_KPH)
                {
                    nudButton6MinimumSpeed.Value = MAXIMUM_C_MODE_MINIMUM_SPEED_KPH;
                }

                else if (nudButton6MinimumSpeed.Value < MINIMUM_C_MODE_MINIMUM_SPEED)
                {
                    nudButton6MinimumSpeed.Value = MINIMUM_C_MODE_MINIMUM_SPEED;
                }


                //Change speed upper and lower limits
                nudButton6MinimumSpeed.Maximum = MAXIMUM_C_MODE_MINIMUM_SPEED_KPH;
                nudButton6MinimumSpeed.Minimum = MINIMUM_C_MODE_MINIMUM_SPEED;
            }
        }



        #endregion button 6

        private void AddPlayColumnToButtonList(int b)
        {

            switch (b)
            {
                case 1:
                    {
                        if (dataGridView_btn_1_instr.ColumnCount < NUMBEROFBUTTONINSTRUCTIONTABLECOLUMNS && dataGridView_btn_1_instr.RowCount > 0)
                        {
                            //Add play image column
                            DataGridViewImageColumn but1img = new DataGridViewImageColumn();
                            //Image image = Image.FromFile(@"C:\\EWS\\download.jpg");
                            Image image = Properties.Resources.Play;
                            but1img.Image = image;
                            dataGridView_btn_1_instr.Columns.Insert(BUTTONPLAYCOLUMNINDEX, but1img);
                            but1img.Width = image.Width;
                            //img.HeaderText = "PLAY";
                            but1img.HeaderText = "";
                            but1img.Name = "img";
                        }
                        break;
                    }

                case 2:
                    {
                        if (dataGridView_btn_2_instr.ColumnCount < NUMBEROFBUTTONINSTRUCTIONTABLECOLUMNS && dataGridView_btn_2_instr.RowCount > 0)
                        {
                            //Add play image column
                            DataGridViewImageColumn but2img = new DataGridViewImageColumn();
                            //Image image = Image.FromFile(@"C:\\EWS\\download.jpg");
                            Image image = Properties.Resources.Play;
                            but2img.Image = image;
                            dataGridView_btn_2_instr.Columns.Insert(BUTTONPLAYCOLUMNINDEX, but2img);
                            but2img.Width = image.Width;
                            //img.HeaderText = "PLAY";
                            but2img.HeaderText = "";
                            but2img.Name = "img";
                        }
                        break;
                    }

                case 3:
                    {
                        if (dataGridView_btn_3_instr.ColumnCount < NUMBEROFBUTTONINSTRUCTIONTABLECOLUMNS && dataGridView_btn_3_instr.RowCount > 0)
                        {
                            //Add play image column
                            DataGridViewImageColumn but3img = new DataGridViewImageColumn();
                            //Image image = Image.FromFile(@"C:\\EWS\\download.jpg");
                            Image image = Properties.Resources.Play;
                            but3img.Image = image;
                            dataGridView_btn_3_instr.Columns.Insert(BUTTONPLAYCOLUMNINDEX, but3img);
                            but3img.Width = image.Width;
                            //img.HeaderText = "PLAY";
                            but3img.HeaderText = "";
                            but3img.Name = "img";
                        }
                        break;
                    }

                case 4:
                    {
                        if (dataGridView_btn_4_instr.ColumnCount < NUMBEROFBUTTONINSTRUCTIONTABLECOLUMNS && dataGridView_btn_4_instr.RowCount > 0)
                        {
                            //Add play image column
                            DataGridViewImageColumn but4img = new DataGridViewImageColumn();
                            //Image image = Image.FromFile(@"C:\\EWS\\download.jpg");
                            Image image = Properties.Resources.Play;
                            but4img.Image = image;
                            dataGridView_btn_4_instr.Columns.Insert(BUTTONPLAYCOLUMNINDEX, but4img);
                            but4img.Width = image.Width;
                            //img.HeaderText = "PLAY";
                            but4img.HeaderText = "";
                            but4img.Name = "img";
                        }
                        break;
                    }

                case 5:
                    {
                        if (dataGridView_btn_5_instr.ColumnCount < NUMBEROFBUTTONINSTRUCTIONTABLECOLUMNS && dataGridView_btn_5_instr.RowCount > 0)
                        {
                            //Add play image column
                            DataGridViewImageColumn but5img = new DataGridViewImageColumn();
                            //Image image = Image.FromFile(@"C:\\EWS\\download.jpg");
                            Image image = Properties.Resources.Play;
                            but5img.Image = image;
                            dataGridView_btn_5_instr.Columns.Insert(BUTTONPLAYCOLUMNINDEX, but5img);
                            but5img.Width = image.Width;
                            //img.HeaderText = "PLAY";
                            but5img.HeaderText = "";
                            but5img.Name = "img";
                        }
                        break;
                    }

                case 6:
                    {
                        if (dataGridView_btn_6_instr.ColumnCount < NUMBEROFBUTTONINSTRUCTIONTABLECOLUMNS && dataGridView_btn_6_instr.RowCount > 0)
                        {
                            //Add play image column
                            DataGridViewImageColumn but6img = new DataGridViewImageColumn();
                            //Image image = Image.FromFile(@"C:\\EWS\\download.jpg");
                            Image image = Properties.Resources.Play;
                            but6img.Image = image;
                            dataGridView_btn_6_instr.Columns.Insert(BUTTONPLAYCOLUMNINDEX, but6img);
                            but6img.Width = image.Width;
                            //img.HeaderText = "PLAY";
                            but6img.HeaderText = "";
                            but6img.Name = "img";
                        }
                        break;
                    }

                default:
                    {
                        break;
                    }
            }
        }

        private void Play_Button_Instruction_Audio(string InstructionName)
        {
            foreach (DataGridViewRow row in dataGridView_Instructions.Rows)
            {
                for (int i = 0; i < dataGridView_Instructions.ColumnCount; i++)
                {
                    if (dataGridView_Instructions.Columns[i].HeaderText == "NAME")
                    {
                        var cellValue = row.Cells[i].Value;
                        if (cellValue != null && cellValue.ToString() == InstructionName)
                        {
                            for (int j = 0; j < dataGridView_Instructions.ColumnCount; j++)
                            {
                                if (dataGridView_Instructions.Columns[j].HeaderText == "AUDIO_FILE")
                                {
                                    var audioCellValue = row.Cells[j].Value;
                                    string audiofilename = audioCellValue.ToString();
                                    SoundPlayer audiofile = new SoundPlayer(my_config.TempPath + "\\" + audiofilename);
                                    audiofile.Play();
                                    return;
                                }
                            }

                        }
                    }
                }
            }
        }

        private void dataGridView_btn_1_instr_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                if (e.ColumnIndex == AUDIOFILECOLUMNINDEX)
                {
                    string instructionName = dataGridView_btn_1_instr.Rows[e.RowIndex].Cells[e.ColumnIndex - 2].Value.ToString();

                    Play_Button_Instruction_Audio(instructionName);
                }
            }
        }

        private void dataGridView_btn_2_instr_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                if (e.ColumnIndex == AUDIOFILECOLUMNINDEX)
                {
                    string instructionName = dataGridView_btn_2_instr.Rows[e.RowIndex].Cells[e.ColumnIndex - 2].Value.ToString();

                    Play_Button_Instruction_Audio(instructionName);
                }
            }
        }

        private void dataGridView_btn_3_instr_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                if (e.ColumnIndex == AUDIOFILECOLUMNINDEX)
                {
                    string instructionName = dataGridView_btn_3_instr.Rows[e.RowIndex].Cells[e.ColumnIndex - 2].Value.ToString();

                    Play_Button_Instruction_Audio(instructionName);
                }
            }
        }

        private void dataGridView_btn_4_instr_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                if (e.ColumnIndex == AUDIOFILECOLUMNINDEX)
                {
                    string instructionName = dataGridView_btn_4_instr.Rows[e.RowIndex].Cells[e.ColumnIndex - 2].Value.ToString();

                    Play_Button_Instruction_Audio(instructionName);
                }
            }
        }

        private void dataGridView_btn_5_instr_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                if (e.ColumnIndex == AUDIOFILECOLUMNINDEX)
                {
                    string instructionName = dataGridView_btn_5_instr.Rows[e.RowIndex].Cells[e.ColumnIndex - 2].Value.ToString();

                    Play_Button_Instruction_Audio(instructionName);
                }
            }
        }

        private void dataGridView_btn_6_instr_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                if (e.ColumnIndex == AUDIOFILECOLUMNINDEX)
                {
                    string instructionName = dataGridView_btn_6_instr.Rows[e.RowIndex].Cells[e.ColumnIndex - 2].Value.ToString();

                    Play_Button_Instruction_Audio(instructionName);
                }
            }
        }

        #region remove del key from buttons tab
        private void dataGridView_btn_1_instr_KeyPress(object sender, KeyPressEventArgs e)
        {
        }
        private void dataGridView_btn_2_instr_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void tabControl_Btns_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void dataGridView_btn_3_instr_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void dataGridView_btn_4_instr_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void dataGridView_btn_5_instr_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void dataGridView_btn_6_instr_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void dataGridView_btn_1_instr_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                Console.WriteLine("ignored");
            }
        }

        private void dataGridView_btn_1_list_instr_KeyDown(object sender, KeyEventArgs e)
        {

        }



        #endregion

        #endregion // Buttons Tab


        #region Serial Number Tab

        #region Serial Numbers and FPGA Codes

        private void Load_Serial_Numbers(object sender, EventArgs ea)
        {
            string json_string;

            FolderBrowserDialog fbd = new FolderBrowserDialog();
            try
            {
                fbd.SelectedPath = Properties.Settings.Default.LastFolder;
                //fbd.SelectedPath = Properties.Resources.FBD;
                //fbd.SelectedPath = File.ReadAllText(@currentPath + "Resources\\" + "FBD.txt");
            }

            catch (Exception fbde)
            {
                MessageBox.Show(fbde.Message);
            }
            fbd.Description = "Choose Key File Folder Location";
            fbd.ShowNewFolderButton = false;
            //            fbd.RootFolder = Environment.SpecialFolder.MyDocuments;
            fbd.SelectedPath = Properties.Settings.Default.LastFolder;

            //This ensures the selected folder is always shown in the folder dialog control
            SendKeys.Send("{TAB}{TAB}{RIGHT}");

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                //if the dgv already has data then it needs to be cleared before loading new key files.

                if (dataGridView_SN_FPGA_Code.RowCount > 0)
                {
                    dataGridView_SN_FPGA_Code.Rows.Clear();
                    dataGridView_SN_FPGA_Code.Refresh();

                    Array.Clear(Serial_Numbers, 0, Serial_Numbers.Length);
                    Array.Clear(FPGA_Unlock_Code, 0, FPGA_Unlock_Code.Length);
                }

                //Save last folder
                Properties.Settings.Default.LastFolder = fbd.SelectedPath;
                //File.WriteAllText(@currentPath + "Resources\\" + "FBD.txt", fbd.SelectedPath);

                string keyFilePath = fbd.SelectedPath;


                try
                {
                    //my_config.SNpath = keyFilePath;

                    var files = Directory.GetFiles(keyFilePath, "FM80*", SearchOption.TopDirectoryOnly).ToList();

                    files.Sort(StringComparer.Ordinal);

                    if (files.Count > 0)
                    {
                        for (int j = 0; j < files.Count; j++)
                        {
                            Console.WriteLine("File found: " + files[j]);


                            json_string = File.ReadAllText(files[j]);
                            SerialNumberAndFPGAUnlockCode SN = new SerialNumberAndFPGAUnlockCode { };


                            SN = JsonConvert.DeserializeObject<SerialNumberAndFPGAUnlockCode>(json_string);
                            if (SN.SECURITY_CODE != null)
                            {
                                Serial_Numbers[j] = SN.SERIAL_NUMBER;
                                FPGA_Unlock_Code[j] = SN.SECURITY_CODE;
                                dataGridView_SN_FPGA_Code.RowCount++;

                                dataGridView_SN_FPGA_Code[0, j].Value = Serial_Numbers[j].ToString();
                                dataGridView_SN_FPGA_Code[1, j].Value = FPGA_Unlock_Code[j];
                            }
                        }
                    }
                }

                catch (Exception e)
                {
                    Console.WriteLine("Path to serial numbers not found:  " + e);
                }

            }
        }

        #endregion  // Serial Numbers and FPGA Codes

        #region MD5

        private string Encrypt_Folder(string path)
        {
            string HashString;

            var files = Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly).ToList();

            files.Sort(StringComparer.Ordinal);



            // Remove the configKeys.json if it is in the list
            files.RemoveAll(u => u.Contains("configKeys.json"));   //.RemoveAll(u => u.Contains(active_user));

            SHA1 Hash = SHA1.Create();
            Hash.Initialize();

            for (int i = 0; i < files.Count; i++)
            {
                string file = files[i];

                Console.WriteLine(file);

                //string relativePath = file.Substring(path.Length + 1);
                string fileName = Path.GetFileName(file);
                byte[] pathBytes = Encoding.UTF8.GetBytes(fileName);
                Hash.TransformBlock(pathBytes, 0, pathBytes.Length, pathBytes, 0);

                byte[] contentBytes = File.ReadAllBytes(file);
                if (i == files.Count - 1)
                    Hash.TransformFinalBlock(contentBytes, 0, contentBytes.Length);
                else
                    Hash.TransformBlock(contentBytes, 0, contentBytes.Length, contentBytes, 0);
            }

            HashString = BitConverter.ToString(Hash.Hash).Replace("-", "").ToUpper();

            return HashString;
        }
        #endregion  // MD5

        #region Encription

        private void CreateEncriptionJson(string MD51, string savepath)
        {
            string keystring;

            List<EncriptionJson> _data = new List<EncriptionJson>();


            for (int i = 0; i < Serial_Numbers.Length; i++)
            {
                if (Serial_Numbers[i] != null)
                {
                    SHA1 Hash = SHA1.Create();
                    Hash.Initialize();

                    string Merged = MD51 + FPGA_Unlock_Code[i];
                    byte[] keybyte = Encoding.UTF8.GetBytes(MD51 + FPGA_Unlock_Code[i]);
                    byte[] kb2 = Encoding.UTF8.GetBytes(Merged);
                    Hash.TransformFinalBlock(keybyte, 0, keybyte.Length);
                    keystring = BitConverter.ToString(Hash.Hash).Replace("-", "").ToUpper();

                    _data.Add(new EncriptionJson()
                    {
                        SN = Serial_Numbers[i].ToString(),
                        Key = keystring
                    });
                }

                else
                {
                    string json = JsonConvert.SerializeObject(_data.ToArray(), Formatting.Indented);
                    System.IO.File.WriteAllText(savepath + "\\configKeys.json", json);

                    break;
                }
            }

        }

        #endregion //Encription

        #endregion  // Serial Number Tab


        private void EWS_MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();

            if (!_OKToCloseandExit)
            {
                // Display a MsgBox asking the user to save changes or abort.
                DialogResult dr = MessageBox.Show("Your configuration has not been saved.\n\nWould you like to save Configuration before exit?.", "Configuration Not Saved", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                switch (dr)
                {
                    case DialogResult.Yes:
                        {
                            Console.WriteLine("Save before close");
                            bacKupToolStripMenuItem_Click(sender, e);
                            break;
                        }

                    case DialogResult.No:
                        {
                            break;
                        }

                    case DialogResult.Cancel:
                        {
                            // Cancel the Closing event from closing the form.
                            e.Cancel = true;
                            break;
                        }

                    default:
                        {
                            break;
                        }
                }

            }


            if (Directory.Exists(temppath))
            {
                try
                {
                    Directory.Delete(temppath, true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }


        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void hideAdvancedSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tslEWS_ConfigStatus.Text = "Close Advanced Settings Tab Started";
            DialogResult dialogResult = MessageBox.Show("You are about to hide the Advanced Settings\n\nWould you like to continue?", "Close Advanced Settings Tab", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                //MOdify visible menu items to reflect where we are in the program

                hideAdvancedSettingsTabToolStripMenuItem.Visible = false;
                showAdvancedSettingsTabToolStripMenuItem.Visible = true;
                toolStripButtonHideAdvancedSettings.Visible = false;
                toolStripButtonShowAdvancedSettings.Visible = true;

                //Hide advanced settings columns
                _AdvancedSettings = false;
                dataGridView_stations_Always_Included.Columns[AUTOLEVELCOLUMNINDEX].Visible = _AdvancedSettings;
                dataGridView_stations_Always_Included.Columns[LEVELCOLUMNINDEX].Visible = _AdvancedSettings;
                dataGridView_stations_Always_Included.Columns[TACOLUMNINDEX].Visible = _AdvancedSettings;

                //Resize columns
                SetFrequencyInformationColumnWidths();

                //Hide Message Break controls
                btnCModeApplyToAllButtons.Visible = false;

                lblButton1BreakDuration.Visible = false;
                lblButton1MinimumSpeed.Visible = false;
                nudButton1BreakDuration.Visible = false;
                nudButton1MinimumSpeed.Visible = false;
                lblButton1SpeedHysteresisSeconds.Visible = false;
                nudButton1CModeHysteresis.Visible = false;

                lblButton2BreakDuration.Visible = false;
                lblButton2MinimumSpeed.Visible = false;
                nudButton2BreakDuration.Visible = false;
                nudButton2MinimumSpeed.Visible = false;
                lblButton2SpeedHysteresisSeconds.Visible = false;
                nudButton2CModeHysteresis.Visible = false;

                lblButton3BreakDuration.Visible = false;
                lblButton3MinimumSpeed.Visible = false;
                nudButton3BreakDuration.Visible = false;
                nudButton3MinimumSpeed.Visible = false;
                lblButton3SpeedHysteresisSeconds.Visible = false;
                nudButton3CModeHysteresis.Visible = false;

                lblButton4BreakDuration.Visible = false;
                lblButton4MinimumSpeed.Visible = false;
                nudButton4BreakDuration.Visible = false;
                nudButton4MinimumSpeed.Visible = false;
                lblButton4SpeedHysteresisSeconds.Visible = false;
                nudButton4CModeHysteresis.Visible = false;

                lblButton5BreakDuration.Visible = false;
                lblButton5MinimumSpeed.Visible = false;
                nudButton5BreakDuration.Visible = false;
                nudButton5MinimumSpeed.Visible = false;
                lblButton5SpeedHysteresisSeconds.Visible = false;
                nudButton5CModeHysteresis.Visible = false;

                lblButton6BreakDuration.Visible = false;
                lblButton6MinimumSpeed.Visible = false;
                nudButton6BreakDuration.Visible = false;
                nudButton6MinimumSpeed.Visible = false;
                lblButton6SpeedHysteresisSeconds.Visible = false;
                nudButton6CModeHysteresis.Visible = false;


                checkBox_stations_auto_stations.Visible = false;


                tslEWS_ConfigStatus.Text = "Advanced Settings Tab Closed";
            }
            else if (dialogResult == DialogResult.Cancel)
            {
                tslEWS_ConfigStatus.Text = "Close Advanced Settings Tab Cancelled";
            }
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AdvancedSettings.Name = "Advanced_Settings";
            AdvancedSettings.Text = "Advanced Settings";
            tslEWS_ConfigStatus.Text = "Show Advanced Settings Started";

            tslEWS_ConfigStatus.Text = "Waiting for Password";

            string pword = EnterPassword.ShowDialog("Enter Advanced Settings Password", "Enter Password");

            if (pword == "")
            {
                tslEWS_ConfigStatus.Text = "Show Advanced Settings Cancelled";
            }

            else
            {
                Console.WriteLine(pword);
                if (pword == AdvancedSettingsPassword)
                {
                    //Modify visible menu items to reflect where we are in the program
                    showAdvancedSettingsTabToolStripMenuItem.Visible = false;
                    hideAdvancedSettingsTabToolStripMenuItem.Visible = true;
                    toolStripButtonHideAdvancedSettings.Visible = true;
                    toolStripButtonShowAdvancedSettings.Visible = false;

                    //Show advanced settings columns
                    _AdvancedSettings = true;
                    dataGridView_stations_Always_Included.Columns[AUTOLEVELCOLUMNINDEX].Visible = _AdvancedSettings;
                    dataGridView_stations_Always_Included.Columns[LEVELCOLUMNINDEX].Visible = _AdvancedSettings;
                    dataGridView_stations_Always_Included.Columns[TACOLUMNINDEX].Visible = _AdvancedSettings;

                    //Resize columns
                    SetFrequencyInformationColumnWidths();

                    //Show Message Break controls
                    btnCModeApplyToAllButtons.Visible = true;

                    lblButton1BreakDuration.Visible = true;
                    lblButton1MinimumSpeed.Visible = true;
                    nudButton1BreakDuration.Visible = true;
                    nudButton1MinimumSpeed.Visible = true;
                    lblButton1SpeedHysteresisSeconds.Visible = true;
                    nudButton1CModeHysteresis.Visible = true;

                    lblButton2BreakDuration.Visible = true;
                    lblButton2MinimumSpeed.Visible = true;
                    nudButton2BreakDuration.Visible = true;
                    nudButton2MinimumSpeed.Visible = true;
                    lblButton2SpeedHysteresisSeconds.Visible = true;
                    nudButton2CModeHysteresis.Visible = true;

                    lblButton3BreakDuration.Visible = true;
                    lblButton3MinimumSpeed.Visible = true;
                    nudButton3BreakDuration.Visible = true;
                    nudButton3MinimumSpeed.Visible = true;
                    lblButton3SpeedHysteresisSeconds.Visible = true;
                    nudButton3CModeHysteresis.Visible = true;

                    lblButton4BreakDuration.Visible = true;
                    lblButton4MinimumSpeed.Visible = true;
                    nudButton4BreakDuration.Visible = true;
                    nudButton4MinimumSpeed.Visible = true;
                    lblButton4SpeedHysteresisSeconds.Visible = true;
                    nudButton4CModeHysteresis.Visible = true;

                    lblButton5BreakDuration.Visible = true;
                    lblButton5MinimumSpeed.Visible = true;
                    nudButton5BreakDuration.Visible = true;
                    nudButton5MinimumSpeed.Visible = true;
                    lblButton5SpeedHysteresisSeconds.Visible = true;
                    nudButton5CModeHysteresis.Visible = true;

                    //                    lblButton6BreakDuration.Visible = true;
                    //                    lblButton6MinimumSpeed.Visible = true;
                    //                    nudButton6BreakDuration.Visible = true;
                    //                    nudButton6MinimumSpeed.Visible = true;
                    //                    lblButton6SpeedHysteresisSeconds.Visible = true;
                    //                    nudButton6CModeHysteresis.Visible = true;

                    tslEWS_ConfigStatus.Text = "Advanced Settings Enabled";

                    checkBox_stations_auto_stations.Visible = true;
                }

                else
                {
                    MessageBox.Show("Incorrect Password Entered", "Incorrect Password", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tslEWS_ConfigStatus.Text = "Show Advanced Settings Tab Cancelled";
                }
            }

        }

        private void changePasswordToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            tslEWS_ConfigStatus.Text = "Changing Advanced Settings Password";

            string oldpword = EnterPassword.ShowDialog("Change Advanced Settings Password", "Enter Old Advanced Settings Password");

            if (oldpword == "")
            {
                tslEWS_ConfigStatus.Text = "Change Password Cancelled";
            }

            else
            {
                Console.WriteLine(oldpword);
                if (oldpword == AdvancedSettingsPassword)
                {
                    //If old password is correct, prompt for new password

                    string newpword = EnterPassword.ShowDialog("Change Advanced Settings Password", "Enter New Advanced Settings Password");

                    if (newpword == "")
                    {
                        tslEWS_ConfigStatus.Text = "Change Password Cancelled";
                    }

                    else
                    {
                        Properties.Settings.Default.Password = newpword;
                        //File.WriteAllText(@currentPath + "Resources\\" + "Password.txt", newpword);
                        AdvancedSettingsPassword = newpword;
                        tslEWS_ConfigStatus.Text = "Password Successfully Changed";
                    }


                }

                else
                {
                    MessageBox.Show("Incorrect Password Entered", "Incorrect Password", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tslEWS_ConfigStatus.Text = "Change Password Cancelled";
                }
            }
        }


        private void dataGridView_stations_Always_Included_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridViewComboBoxCell cb = (DataGridViewComboBoxCell)dataGridView_stations_Always_Included.Rows[e.RowIndex].Cells[LANGUAGECOLUMNINDEX];
                DataGridViewTextBoxCell tb = (DataGridViewTextBoxCell)dataGridView_stations_Always_Included.Rows[e.RowIndex].Cells[FREQCOLUMNINDEX];
                DataGridViewTextBoxCell au = (DataGridViewTextBoxCell)dataGridView_stations_Always_Included.Rows[e.RowIndex].Cells[AUTOLEVELCOLUMNINDEX];
                DataGridViewTextBoxCell le = (DataGridViewTextBoxCell)dataGridView_stations_Always_Included.Rows[e.RowIndex].Cells[LEVELCOLUMNINDEX];
                DataGridViewTextBoxCell pr = (DataGridViewTextBoxCell)dataGridView_stations_Always_Included.Rows[e.RowIndex].Cells[PREFERREDCOLUMNINDEX];
                DataGridViewCheckBoxCell chb = (DataGridViewCheckBoxCell)dataGridView_stations_Always_Included.Rows[e.RowIndex].Cells[TACOLUMNINDEX];

                if (e.ColumnIndex == AUTOLEVELCOLUMNINDEX)
                {
                    if ((dataGridView_stations_Always_Included.Rows[e.RowIndex].Cells[e.ColumnIndex].Value).ToString() == "AUTO")
                    {
                        dataGridView_stations_Always_Included.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "LEVEL";
                        dataGridView_stations_Always_Included.Rows[e.RowIndex].Cells[PREFERREDCOLUMNINDEX].Value = "NO";
                        dataGridView_stations_Always_Included.Rows[e.RowIndex].Cells[LEVELCOLUMNINDEX].Style.BackColor = Color.White;
                        dataGridView_stations_Always_Included.Rows[e.RowIndex].Cells[LEVELCOLUMNINDEX].Style.ForeColor = Color.Black;
                    }

                    else
                    {
                        dataGridView_stations_Always_Included.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "AUTO";
                        dataGridView_stations_Always_Included.Rows[e.RowIndex].Cells[LEVELCOLUMNINDEX].Style.BackColor = Color.Silver;
                        dataGridView_stations_Always_Included.Rows[e.RowIndex].Cells[LEVELCOLUMNINDEX].Style.ForeColor = Color.Silver;
                    }


                    if ((cb.Value != null) && (tb.Value != null) && (au.Value != null) && (le.Value != null) && (pr.Value != null))
                    {
                        // update the binding list and channel json
                                                my_config.Update_Include_Frequency(tb.Value.ToString(), cb.Value.ToString(), au.Value.ToString(), le.Value.ToString(), pr.Value.ToString(), (bool)chb.Value);
                        dataGridView_stations_Always_Included.Invalidate();
                    }
                }

                if (e.ColumnIndex == PREFERREDCOLUMNINDEX)
                {
                    if ((dataGridView_stations_Always_Included.Rows[e.RowIndex].Cells[e.ColumnIndex].Value).ToString() == "YES")
                    {
                        dataGridView_stations_Always_Included.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "NO";
                    }

                    else
                    {
                        dataGridView_stations_Always_Included.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "YES";
                        dataGridView_stations_Always_Included.Rows[e.RowIndex].Cells[AUTOLEVELCOLUMNINDEX].Value = "AUTO";
                        dataGridView_stations_Always_Included.Rows[e.RowIndex].Cells[LEVELCOLUMNINDEX].Style.BackColor = Color.Silver;
                        dataGridView_stations_Always_Included.Rows[e.RowIndex].Cells[LEVELCOLUMNINDEX].Style.ForeColor = Color.Silver;
                    }

                    if ((cb.Value != null) && (tb.Value != null) && (au.Value != null) && (le.Value != null) && (pr.Value != null))
                    {
                        // update the binding list and channel json
                        my_config.Update_Include_Frequency(tb.Value.ToString(), cb.Value.ToString(), au.Value.ToString(), le.Value.ToString(), pr.Value.ToString(), (bool)chb.Value);
                        dataGridView_stations_Always_Included.Invalidate();
                    }
                }
            }

            catch { }
        }

        private void dataGridView_stations_Always_Included_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(Column_Level_KeyPress);
            if (dataGridView_stations_Always_Included.CurrentCell.ColumnIndex == LEVELCOLUMNINDEX)
            {
                TextBox tb = e.Control as TextBox;
                if (tb != null)
                {
                    tb.KeyPress += new KeyPressEventHandler(Column_Level_KeyPress);
                }
            }
        }


        private void Column_Level_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void EWS_MainForm_FormClosing(object sender, EventArgs e)
        {
            this.Close();
        }

        private void menuToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void toolStripFileActions_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void dataGridView_Instructions_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView_stations_all_freq_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView_stations_Always_Included_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void EWS_MainForm_Resize(object sender, EventArgs e)
        {
            //Resize columns
            SetFrequencyInformationColumnWidths();
        }
    }


    public class SerialNumberAndFPGAUnlockCode
    {
        public string SERIAL_NUMBER { get; set; }
        public string SECURITY_CODE { get; set; }
    }

    public class EncriptionJson
    {
        public string SN { get; set; }
        public string Key { get; set; }
    }

    public static class EnterPassword
    {
        public static string ShowDialog(string Header, string text)
        {
            Form pword = new Form()
            {
                Width = 500,
                Height = 250,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = Header,
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 50, Top = 20, Text = text, Width = 400 };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400 };
            Button btnOK = new Button() { Text = "OK", Left = 350, Width = 100, Top = 125, DialogResult = DialogResult.OK };
            Button btncancel = new Button() { Text = "Cancel", Left = 50, Width = 100, Top = 125, DialogResult = DialogResult.Cancel };
            btnOK.Click += (sender, e) => { pword.Close(); };
            pword.Controls.Add(textBox);
            pword.Controls.Add(btnOK);
            pword.Controls.Add(btncancel);
            pword.Controls.Add(textLabel);
            pword.AcceptButton = btnOK;

            return pword.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
    }
}
