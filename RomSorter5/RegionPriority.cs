using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RomSorter5WinForms
{
    public partial class RegionPriority : Form
    {
        public List<string> entries = new List<string>();
        public RegionPriority()
        {
            InitializeComponent();
        }

        private void RegionPriority_Load(object sender, EventArgs e)
        {
            foreach (var entry in entries)
                lstRegions.Items.Add(entry);
        }

        private void btnDone_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            if (lstRegions.SelectedIndex >= 0)
            {
                var entry = entries[lstRegions.SelectedIndex];
                entries.Remove(entry);
                entries.Insert(lstRegions.SelectedIndex - 1, entry);
            }
            lstRegions.Items.Clear();
            foreach(var entry in entries)
                lstRegions.Items.Add(entry);
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            if (lstRegions.SelectedIndex >= 0)
            {
                var entry = entries[lstRegions.SelectedIndex];
                entries.Remove(entry);
                entries.Insert(lstRegions.SelectedIndex + 1, entry);
            }
            lstRegions.Items.Clear();
            foreach (var entry in entries)
                lstRegions.Items.Add(entry);
        }
    }
}
