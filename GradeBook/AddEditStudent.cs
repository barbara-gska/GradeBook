using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GradeBook
{
    public partial class AddEditStudent : Form
    {

        private FileHelper<List<Student>> _fileHelper =
           new GradeBook.FileHelper<List<Student>>(Program.FilePath);

        private int _studentId;
        private Student _student;
        private List<string> _groups;

        public AddEditStudent(int id = 0)
        {
            InitializeComponent();
            _studentId = id;
            InitComboboxGroups();

            GetStudentData();
            tbFirstName.Select();
        }

        private void InitComboboxGroups()
        {
            _groups = new List<string>() { "brak", "1A", "1B", "2A", "2B", "3A", "3B" };

            cbGroupId.DataSource = _groups;
        }



        private void GetStudentData()
        {
            if (_studentId != 0)
            {
                Text = "Edytowanie danych ucznia";

                var students = _fileHelper.DeserializeFromFile();
                _student = students.FirstOrDefault(x => x.Id == _studentId);

                if (_student == null)
                    throw new Exception("Brak ucznia o podanym Id");

                FillTextBoxes();
            }
        }

        private void FillTextBoxes()
        { 
            tbId.Text = _student.Id.ToString();
            tbFirstName.Text = _student.FirstName;
            tbLastName.Text = _student.LastName;
            tbMath.Text = _student.Math;
            tbTechnology.Text = _student.Technology;
            tbPhysics.Text = _student.Physics;
            tbPolishLang.Text = _student.PolishLang;
            tbForeignLang.Text = _student.ForeignLang;
            rtbComments.Text = _student.Comments;
            cbExtra.Checked = _student.Extracurricullum;
            cbGroupId.Text = _student.GroupId;
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            var students = _fileHelper.DeserializeFromFile();

            if (_studentId != 0)
                students.RemoveAll(x => x.Id == _studentId);
            else
                AssignIdToNewStudent(students);

            if(cbGroupId.SelectedItem.ToString() != "brak")
            {
                AddNewStudentToList(students);

                _fileHelper.SerializeToFile(students);

                Close();
            }
            else
                MessageBox.Show("Proszę wybrać grupę ucznia.");
        }


        private void AddNewStudentToList(List<Student> students)
        {

            var student = new Student
            {
                Id = _studentId,
                FirstName = tbFirstName.Text,
                LastName = tbLastName.Text,
                Math = tbMath.Text,
                Technology = tbTechnology.Text,
                Physics = tbPhysics.Text,
                PolishLang = tbPolishLang.Text,
                ForeignLang = tbForeignLang.Text,
                Comments = rtbComments.Text,
                Extracurricullum = cbExtra.Checked,
                GroupId = cbGroupId.SelectedItem.ToString()
            };


            students.Add(student);

        }

        private void AssignIdToNewStudent(List<Student> students)
        {
            var studentWithHighestId = students.OrderByDescending(x => x.Id).FirstOrDefault();

            _studentId = studentWithHighestId == null ? 1 : studentWithHighestId.Id + 1;

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}
