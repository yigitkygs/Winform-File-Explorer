using System.IO;

namespace WinFormsAppFileExplorer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string currPath = "";
        string oldName = "";


        private void Form1_Load(object sender, EventArgs e)
        {
            getDirectories();

            listView1.Columns.Add("Adı", 350);
            listView1.Columns.Add("Değiştirme Tarihi", 100);
            listView1.Columns.Add("Boyutu", 100);

            ContextMenuStrip cm = new ContextMenuStrip();
            var yeniKlasör = cm.Items.Add("Yeni Klasör");
            yeniKlasör.Click += YeniKlasör_Click;
            var yeniDosya=cm.Items.Add("Yeni Dosya");
            yeniDosya.Click += YeniDosya_Click;
            var yenidenAdlandir = cm.Items.Add("Yeniden Adlandır");
            yenidenAdlandir.Click += YenidenAdlandir_Click;
            var sil = cm.Items.Add("Sil");
            sil.Click += Sil_Click;

            listView1.ContextMenuStrip = cm;
            listView1.ContextMenuStrip.Visible = false;
        }

        void getDirectories()
        {
            string[] drives = Environment.GetLogicalDrives();

            foreach (var pathDrive in drives)
            {
                DriveInfo di = new DriveInfo(pathDrive);
                TreeNode node = new TreeNode(pathDrive.Substring(0, 1));
                node.Text = pathDrive;
                var location = pathDrive;
                if (di.IsReady == true)
                {
                    foreach (var dir in Directory.GetDirectories(location))
                    {
                        if (dir.Contains("$"))
                        {
                            node.Nodes.Add(dir.Substring(dir.IndexOf("$") + 1));
                        }
                        else if (dir.Contains("\\"))
                        {
                            node.Nodes.Add(dir.Substring(dir.IndexOf("\\") + 1));
                        }
                        else
                        {
                            node.Nodes.Add(dir);
                        }
                    }
                    
                }
                    treeView1.Nodes.Add(node);
            }
        }

        void ViewDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                listView1.Items.Clear();
                currPath=path;
                try
                {
                    listView1.Items.Add(new ListViewItem(new string[1] {"..."}));
                    foreach (var dir in Directory.GetDirectories(path))
                    {
                        var tempInfo = new DirectoryInfo(dir);
                        string[] arr = new string[3];
                        arr[0] = tempInfo.Name;
                        arr[1] = tempInfo.LastWriteTime.ToShortDateString();
                        listView1.View = View.Details;
                        listView1.Items.Add(new ListViewItem(arr));
                    }
                    foreach (var file in Directory.GetFiles(path))
                    {
                        var tempInfo = new FileInfo(file);
                        string[] arr = new string[3];
                        arr[0] = tempInfo.Name;
                        arr[1] = tempInfo.LastWriteTime.ToShortDateString();
                        arr[2] = (tempInfo.Length/(1024*1024)).ToString()+" MB";
                        listView1.View = View.Details;
                        listView1.Items.Add(new ListViewItem(arr));
                    }
                }
                catch { }
            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            currPath= e.Node.FullPath;
            if (currPath.Contains(":\\"))
            {
                if(currPath.Contains(":\\:\\"))
                {
                    ViewDirectory(currPath.Replace(":\\:\\", ":\\"));
                }
                ViewDirectory(currPath);
            }
            else ViewDirectory(currPath+":\\");
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
           if(e.Button == MouseButtons.Right)
            {
                
                listView1.ContextMenuStrip.Show();
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            var selectedFolder = sender as ListView;
            var folderPath="";
            if (selectedFolder.SelectedItems[0].Text == "...")
            {
                 folderPath = currPath.Substring(0, currPath.LastIndexOf("\\"));
            }
            else
            {
                 folderPath = currPath + "\\" + selectedFolder.SelectedItems[0].Text;
            }
            ViewDirectory(folderPath);
        }

        private void YeniKlasör_Click(object? sender, EventArgs e)
        {
            int i = 1;
            var folderPath = currPath + "\\Yeni Klasör";
            while (i!=0)
            {
                if (Directory.Exists(folderPath))
                {
                    folderPath = currPath + "\\Yeni Klasör ("+i+")";
                    i++;
                    continue;
                }
                else
                {
                    var dir = Directory.CreateDirectory(folderPath);
                    i = 0;
                }
            }
            ViewDirectory(currPath);
        }
        private void YeniDosya_Click(object? sender, EventArgs e)
        {
            int i = 1;
            var filePath = currPath + "\\Yeni Dosya.txt";
            while (i != 0)
            {
                if (File.Exists(filePath))
                {
                    filePath = currPath + "\\Yeni Dosya (" + i + ").txt";
                    i++;
                    continue;
                }
                else
                {
                    var dir = File.Create(filePath);
                    i = 0;
                }
            }
            ViewDirectory(currPath);
        }

        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Enter && listView1.SelectedItems.Count > 0 && listView1.LabelEdit)
            {
                listView1.LabelEdit = false;
                if (File.Exists(currPath + "\\" + oldName))
                {
                    File.Move(currPath + "\\" + oldName, currPath + "\\" + listView1.SelectedItems[0].Text);
                }
                else if(Directory.Exists(currPath + "\\" + oldName))
                {
                    Directory.Move(currPath + "\\" + oldName, currPath + "\\" + listView1.SelectedItems[0].Text);
                }
                ViewDirectory(currPath);
            }
        }
        private void Sil_Click(object? sender, EventArgs e)
        {
            if(listView1.SelectedItems.Count > 0)
            {
                if (File.Exists(currPath + "\\" + listView1.SelectedItems[0].Text))
                {
                    File.Delete(currPath + "\\" + listView1.SelectedItems[0].Text);
                }
                else if (Directory.Exists(currPath + "\\" + listView1.SelectedItems[0].Text))
                {
                    Directory.Delete(currPath + "\\" + listView1.SelectedItems[0].Text);
                }
                ViewDirectory(currPath);
            }
        }

        private void YenidenAdlandir_Click(object? sender, EventArgs e)
        {
            if(listView1.SelectedItems.Count > 0)
            {
                listView1.LabelEdit = true;
                listView1.SelectedItems[0].BeginEdit();
                oldName = listView1.SelectedItems[0].Text;
            }
        }

        private void listView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                foreach (var item in listView1.ContextMenuStrip.Items)
                {
                    var col = item as ToolStripItem;
                    if (col.Text.Contains("Klasör") || col.Text.Contains("Dosya"))
                    {
                        col.Visible = false;
                    }
                    else
                    {
                        col.Visible = true;
                    }
                }
            }
            else if (listView1.SelectedItems.Count == 0)
            {
                foreach (var item in listView1.ContextMenuStrip.Items)
                {
                    var col = item as ToolStripItem;
                    if (col.Text.Contains("Klasör") || col.Text.Contains("Dosya"))
                    {
                        col.Visible = true;
                    }
                    else
                    {
                        col.Visible = false;
                    }
                }
            }
        }

        private void listView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            DoDragDrop(e.Item, DragDropEffects.Move);
        }

        private void listView1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.AllowedEffect;
        }

        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            if (listView1.SelectedItems.Count == 0) { return; }

            Point point = listView1.PointToClient(new Point(e.X, e.Y));
            ListViewItem draggedItem = listView1.GetItemAt(point.X, point.Y);
            if (draggedItem == null) { return; }
            var newFolderPath = "";
            if (draggedItem.Text == "...")
            {
                newFolderPath = currPath.Substring(0, currPath.LastIndexOf("\\"));
            }
            else
            {
                newFolderPath = currPath + "\\" + draggedItem.Text + "\\";
            }
            if (File.Exists(currPath + "\\" + listView1.SelectedItems[0].Text))
            {   
                File.Move(currPath + "\\" + oldName+ listView1.SelectedItems[0].Text, newFolderPath + "\\" + listView1.SelectedItems[0].Text);

            }
            else if (Directory.Exists(currPath + "\\" + listView1.SelectedItems[0].Text))
            {
                Directory.Move(currPath + "\\" + oldName+ listView1.SelectedItems[0].Text, newFolderPath + "\\" + listView1.SelectedItems[0].Text);
            }
            ViewDirectory(currPath);
        }
    }
}