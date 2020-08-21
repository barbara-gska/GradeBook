using GradeBook.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace GradeBook
{
    public partial class Main : Form
    {
        private FileHelper<List<Student>> _fileHelper = 
            new GradeBook.FileHelper<List<Student>>(Program.FilePath);

        private List<string> _groups;

        public bool IsMaximize
        {
            get
            {
                return Settings.Default.IsMaximize;
            }
            set
            {
                Settings.Default.IsMaximize = value;
            }
        }

        public Main()
        {
            InitializeComponent();

            InitComboboxGroups();

            RefreshBook();

            SetColumnsHeader();

            if (IsMaximize)
                WindowState = FormWindowState.Maximized;


        }

        private void InitComboboxGroups()
        {
            _groups = new List<string>() { "Wszystkie", "1A", "1B", "2A", "2B", "3A", "3B" };

            cbGroups.DataSource = _groups;
        }

        private void RefreshBook()
        {
            var students = _fileHelper.DeserializeFromFile();
            if (cbGroups.SelectedIndex == 0)
                dgvBook.DataSource = students.OrderBy(x => x.Id).ToList();

            else
                dgvBook.DataSource = students.Where(x => x.GroupId == cbGroups.SelectedItem.ToString()).
                                    OrderBy(x => x.Id).ToList();

        }

        private void SetColumnsHeader()
        {
            dgvBook.Columns[0].HeaderText = "Numer";
            dgvBook.Columns[1].HeaderText = "Imię";
            dgvBook.Columns[2].HeaderText = "Nazwisko";
            dgvBook.Columns[3].HeaderText = "Uwagi";
            dgvBook.Columns[4].HeaderText = "Matematyka";
            dgvBook.Columns[5].HeaderText = "Technologia";
            dgvBook.Columns[6].HeaderText = "Fizyka";
            dgvBook.Columns[7].HeaderText = "Język polski";
            dgvBook.Columns[8].HeaderText = "Język obcy";
            dgvBook.Columns[9].HeaderText = "Zajęcia dodatkowe";

        }


        private void btnAdd_Click(object sender, EventArgs e)
        {
            var addEditStudent = new AddEditStudent();
            addEditStudent.FormClosing += AddEditStudent_FormClosing;
            addEditStudent.ShowDialog();
            
        }

        private void AddEditStudent_FormClosing(object sender, FormClosingEventArgs e)
        {
            RefreshBook();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvBook.SelectedRows.Count == 0)
            {
                MessageBox.Show("Proszę zaznacz ucznia, którego dane chcesz edytować.");
                return;
            }

            var addEditStudent = new AddEditStudent(
                Convert.ToInt32(dgvBook.SelectedRows[0].Cells[0].Value));
            addEditStudent.FormClosing += AddEditStudent_FormClosing;
            addEditStudent.ShowDialog();

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvBook.SelectedRows.Count == 0)
            {
                MessageBox.Show("Proszę zaznacz ucznia, którego chcesz usunąć.");
                return;
            }

            var selectedStudent = dgvBook.SelectedRows[0];

            var confirmDelete = MessageBox.Show($"Czy na pewno chcesz usunąć ucznia " +
                $"{(selectedStudent.Cells[1].Value.ToString() + " " + selectedStudent.Cells[2].Value.ToString()).Trim()}",
                "Usuwanie ucznia", 
                MessageBoxButtons.OKCancel);

            if (confirmDelete == DialogResult.OK)
            {
                DeleteStudent(Convert.ToInt32(selectedStudent.Cells[0].Value));
                RefreshBook();
            }
        }

        private void DeleteStudent(int id)
        {
                var students = _fileHelper.DeserializeFromFile();
                students.RemoveAll(x => x.Id == id);
                _fileHelper.SerializeToFile(students);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshBook();
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WindowState == FormWindowState.Maximized)
                IsMaximize = true;
            else
                IsMaximize = false;

            Settings.Default.Save();
            
        }

        private void cbGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshBook();
        }
    }
}
