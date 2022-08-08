using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace EWS_Config_Tool
{
    public partial class UsbFolderBrowser : Form
    {
        private System.Windows.Forms.TreeView FVdirectoryTreeView;
        private System.Windows.Forms.TextBox FVdirectoryRoot;
        private System.Windows.Forms.Label FVrootDirectoryLabel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        public String FVrootdirectoryfull = "";
        public String FVrootdirectory { get; set; }
        public String FVrootdirectorylable { get; set; }
        /// <summary>
        ///  on ok clicked
        /// </summary>
        public string SelectedDrive { get; set; }

        public UsbFolderBrowser()
        {
            InitializeComponent();
            FVInitializeComponent();
        }

        public void FVRootDirectory(String s)
        {
            FVrootdirectory = s;
        }

        public void FVRootDirectoryLable(String s)
        {
            FVrootdirectorylable = s;
        }

        private void UsbFolderBrowser_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            CenterToScreen();
            FormBorderStyle = FormBorderStyle.FixedDialog;
            FVdirectoryTreeView.Nodes.Clear();

            // get all removable drives
            var driveList = DriveInfo.GetDrives().Where(d => d.DriveType == DriveType.Removable);
            foreach (var d in driveList)
            {
                Console.WriteLine(d.Name);
                FVrootdirectory = d.Name;
                FVdirectoryTreeView.Nodes.Add(FVrootdirectory);
                //SelectedDrive = FVrootdirectory;
                //FVdirectoryRoot.Text = "Selected: " + SelectedDrive;
            }

            // now add each of these as a node
            if ((FVrootdirectory == "") || (FVrootdirectory == null))
            {
                MessageBox.Show("Please insert a Removable Drive", "No USB Device Detected", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                /*
                // Populate files in each Node on removable drives
                foreach (TreeNode node in FVdirectoryTreeView.Nodes)
                {
                    FVPopulateTreeView(node.Text, FVdirectoryTreeView.Nodes[0]);
                }
                */
                Cursor.Current = Cursors.Default;
                //FVrootDirectoryLabel.Text = FVrootdirectorylable;
                Console.WriteLine("Here !");
            }

        }

        private void FVPopulateTreeView(String directoryValue, TreeNode parentNode)
        {
            String[] directoryArray = Directory.GetDirectories(directoryValue);
            String substringDirectory;
            if (directoryArray.Length != 0)
            {
                foreach (String directory in directoryArray)
                {
                    substringDirectory = directory.Substring(directory.LastIndexOf('\\') + 1, directory.Length - directory.LastIndexOf('\\') - 1);
                    TreeNode myNode = new TreeNode(substringDirectory);
                    try
                    {
                        parentNode.Nodes.Add(myNode);
                        FVPopulateTreeView(directory, myNode);
                    }
                    catch (UnauthorizedAccessException) { }
                }
            }
        }

        private void FVbtnCancel_Click(object sender, EventArgs e)
        {
            FVrootdirectoryfull = "";
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void FVbtnOk_Click(object sender, EventArgs e)
        {

            SelectedDrive = FVdirectoryTreeView.SelectedNode.FullPath;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void FVdirectoryTreeView_Click(object sender, EventArgs e)
        {
            //SelectedDrive = FVdirectoryTreeView.SelectedNode.FullPath;
            FVdirectoryRoot.Text = "Selected: " + SelectedDrive;

            btnOk.Enabled = true;

        }

        private void FVInitializeComponent()
        {
            FVdirectoryTreeView = new System.Windows.Forms.TreeView();
            FVdirectoryRoot = new System.Windows.Forms.TextBox();
            FVrootDirectoryLabel = new System.Windows.Forms.Label();
            btnOk = new System.Windows.Forms.Button();
            btnCancel = new System.Windows.Forms.Button();
            SuspendLayout();
            //
            // btnOK
            //
            btnOk.BackColor = Color.LightGray;
            btnOk.Text = "OK";
            btnOk.Location = new System.Drawing.Point(10, 310);
            btnOk.Size = new System.Drawing.Size(60, 30);
            btnOk.Click += new System.EventHandler(FVbtnOk_Click);
            btnOk.Enabled = false;
            //
            // btnCancel
            //
            btnCancel.BackColor = Color.LightGray;
            btnCancel.Text = "Cancel";
            btnCancel.Location = new System.Drawing.Point(235, 310);
            btnCancel.Size = new System.Drawing.Size(60, 30);
            btnCancel.Click += new System.EventHandler(FVbtnCancel_Click);
            //
            // FVrootDirectoryLabel
            //
            FVrootDirectoryLabel.Location = new System.Drawing.Point(10, 5);
            FVrootDirectoryLabel.Name = "FVrootDirectoryLabel";
            FVrootDirectoryLabel.Size = new System.Drawing.Size(284, 20);
            FVrootDirectoryLabel.Text = "Click on a Drive to Select, then Click OK";
            //
            // directoryTreeRoot
            //
            FVdirectoryRoot.Location = new System.Drawing.Point(10, 25);
            FVdirectoryRoot.Name = "FVdirectoryRoot";
            FVdirectoryRoot.Size = new System.Drawing.Size(284, 20);
            FVdirectoryRoot.ReadOnly = true;
            //
            // directoryTreeView
            //
            FVdirectoryTreeView.Location = new System.Drawing.Point(10, 55);
            FVdirectoryTreeView.Name = "FVdirectoryTreeView";
            FVdirectoryTreeView.Size = new System.Drawing.Size(285, 245);
            FVdirectoryTreeView.TabIndex = 0;
            FVdirectoryTreeView.Click += new System.EventHandler(FVdirectoryTreeView_Click);
            //
            // TreeViewDirectoryStructureForm
            //
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(305, 350);
            Controls.Add(FVdirectoryTreeView);
            Controls.Add(FVdirectoryRoot);
            Controls.Add(FVrootDirectoryLabel);
            Controls.Add(btnOk);
            Controls.Add(btnCancel);
            Name = "Removable_Drive_Browser";
            Text = "Removable Drive Browser";
            ResumeLayout(false);
            PerformLayout();

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
        }

    }
}
