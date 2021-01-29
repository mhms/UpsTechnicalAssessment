using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.PerformanceData;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UPS.Homework.CrossCutting;
using UPS.Homework.DTO;
using UPS.Homework.Service;

namespace UPS.HomeWork.Presenetation
{

    public partial class UserDataGridForm : Form
    {
        

        private IUserService _userService;
        public int _currentPage = 1;
        private int _totalPages;
        private const string GenderColName = "gender1";
        private const string StatusColName = "status1";
        private const string EditColName = "edit";
        private const string DeleteColName = "delete";
        private bool flag = true;

        public UserDataGridForm()
        {
            InitializeComponent();
            _userService = new UserService(Application.StartupPath);
            UsersDataGridView.SetDoubleBuffered();
            UsersDataGridView.DataSource = bindingSource1;
            UsersDataGridView.FilterAndSortEnabled = false;
            btnAddUser.Image = Image.FromFile(Path.Combine(Application.StartupPath,"add24.png"));
            btnAddUser.Width = 25;
            btnAddUser.Height = 25;

        }

        public  async Task SetData(int page,string name="")
        {
           

            var serviceResult = await _userService.LoadPage(page,name);
            if (serviceResult.Succeeded)
            {
                noTotalRecords.Text = serviceResult.Result.Item3.total.ToString();
                _totalPages = serviceResult.Result.Item3.pages;
                lblTotalPage.Text = serviceResult.Result.Item3.pages.ToString();
                txtCurrentPage.Text = serviceResult.Result.Item3.page.ToString();
                bindingSource1.DataMember = serviceResult.Result.Item1.TableName;
                bindingSource1.DataSource = serviceResult.Result.Item2;


                UserDataGridSetting();

                foreach (DataGridViewRow row in UsersDataGridView.Rows)
                {
                    DataGridViewComboBoxCell genderCell = (DataGridViewComboBoxCell)(row.Cells[GenderColName]);
                    genderCell.DataSource = new string[] { "Male", "Female" };
                    genderCell.Value = row.Cells[nameof(UserDto.gender)].Value;
                    DataGridViewComboBoxCell statusCell = (DataGridViewComboBoxCell)(row.Cells[StatusColName]);
                    statusCell.DataSource = new string[] { "Active", "Inactive" };
                    statusCell.Value = row.Cells[nameof(UserDto.status)].Value;
                }
                
            }
            else
            {
                MessageBox.Show(serviceResult.Messages[0].Message.ToString());
            }

        }

        private void UserDataGridSetting()
        {
            UsersDataGridView.RowHeadersVisible = true;
            UsersDataGridView.Columns[EditColName].Width = 25;
            UsersDataGridView.Columns[EditColName].HeaderText = "";
            UsersDataGridView.Columns[EditColName].Resizable = DataGridViewTriState.False;
            UsersDataGridView.Columns[DeleteColName].HeaderText = "";
            UsersDataGridView.Columns[DeleteColName].Width = 25;
            UsersDataGridView.Columns[DeleteColName].Resizable = DataGridViewTriState.False;
            UsersDataGridView.Columns[nameof(UserDto.id)].Visible = false;
            
            UsersDataGridView.Columns[nameof(UserDto.gender)].Visible = false;
            UsersDataGridView.Columns[nameof(UserDto.status)].Visible = false;
            if (flag)
            {
                //advancedDataGridViewSearchToolBar1.SetColumns(UsersDataGridView.Columns);
                flag = false;
                var genderDataGridViewCmb = new DataGridViewComboBoxColumn();
                genderDataGridViewCmb.Name = GenderColName;
                UsersDataGridView.Columns.Add(genderDataGridViewCmb);
                var statusDataGridViewCmb = new DataGridViewComboBoxColumn();
                statusDataGridViewCmb.Name = StatusColName;
                UsersDataGridView.Columns.Add(statusDataGridViewCmb);
                
            }
            
            UsersDataGridView.Columns[GenderColName].DisplayIndex = UsersDataGridView.Columns[nameof(UserDto.gender)].DisplayIndex;
            UsersDataGridView.Columns[StatusColName].DisplayIndex = UsersDataGridView.Columns[nameof(UserDto.status)].DisplayIndex;
            UsersDataGridView.Columns[GenderColName].HeaderText = "Gender";
            UsersDataGridView.Columns[StatusColName].HeaderText = "Status";
            UsersDataGridView.Columns[nameof(UserDto.created_at)].ReadOnly = true;
            UsersDataGridView.Columns[nameof(UserDto.updated_at)].ReadOnly = true;


        }


        private async void Form1_Load(object sender, EventArgs e)
        {

            Cursor = Cursors.WaitCursor;
            await SetData(1);
            Int32.TryParse(lblTotalPage.Text, out _totalPages);
            Cursor = Cursors.Default;
        }

        private void advancedDataGridViewSearchToolBar1_Search(object sender, Zuby.ADGV.AdvancedDataGridViewSearchToolBarSearchEventArgs e)
        {
           
        }

        private async void btnNextPage_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            if (_currentPage < _totalPages)
            {
                _currentPage++;
                await SetData(_currentPage,txtSearch.Text);
            }

            Cursor = Cursors.Default;
        }

        private async void btnPreviousPage_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            if (_currentPage > 1)
            {
                _currentPage--;
                await SetData(_currentPage,txtSearch.Text);
            }

            Cursor = Cursors.Default;
        }

        private void txtCurrentPage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }


        }

        private async void txtCurrentPage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Cursor = Cursors.WaitCursor;

                await LoadPage();

                Cursor = Cursors.Default;
            }
        }

        private async Task LoadPage()
        {
            if (Int32.TryParse(txtCurrentPage.Text, out int page) && page > 0)
            {
                _currentPage = page;
                if (page > _totalPages)
                {
                    _currentPage = _totalPages;
                    page = _totalPages;
                }
                await SetData(page,txtSearch.Text);
            }
        }

        private async void txtCurrentPage_Leave(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            await LoadPage();
            Cursor = Cursors.Default;
        }

        private async void advancedDataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == UsersDataGridView.Columns[DeleteColName].Index)
            {
                var resultBtn = MessageBox.Show("Are you sure to delete this row", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (resultBtn == DialogResult.Yes)
                {
                    var idStr = UsersDataGridView.Rows[e.RowIndex].Cells[nameof(UserDto.id)].Value.ToString();
                    if (Int32.TryParse(idStr, out int id))
                    {
                        var serviceResult = await _userService.DeleteUser(id);
                        if (serviceResult.Succeeded)
                        {
                            await SetData(_currentPage,txtSearch.Text);
                        }

                    }
                }


            }
            if (e.ColumnIndex == UsersDataGridView.Columns[EditColName].Index)
            {
                var resultBtn = MessageBox.Show("Sure to edit?", "Edit", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (resultBtn == DialogResult.Yes)
                {
                    var idStr = UsersDataGridView.Rows[e.RowIndex].Cells[nameof(UserDto.id)].Value.ToString();
                    if (Int32.TryParse(idStr, out int id))
                    {
                        var userDto = new UserDto()
                        {
                            id = id,
                            updated_at = DateTime.Now,
                            name = UsersDataGridView.Rows[e.RowIndex].Cells[nameof(UserDto.name)].Value.ToString(),
                            email = UsersDataGridView.Rows[e.RowIndex].Cells[nameof(UserDto.email)].Value.ToString(),
                            gender = UsersDataGridView.Rows[e.RowIndex].Cells[GenderColName].Value.ToString(),
                            status = UsersDataGridView.Rows[e.RowIndex].Cells[StatusColName].Value.ToString(),


                        };
                        var serviceResult = await _userService.UpdateUser(userDto);
                        if (serviceResult.Succeeded)
                        {
                            await SetData(_currentPage,txtSearch.Text);
                        }

                    }
                }
            }

        }

        private async void btnLastPage_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            _currentPage = _totalPages;
            await SetData(_currentPage,txtSearch.Text);


            Cursor = Cursors.Default;
        }

        private async void btnFirstPage_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            _currentPage = 1;
            await SetData(_currentPage,txtSearch.Text);
            Cursor = Cursors.Default;
        }

        private void btnAddUser_Click(object sender, EventArgs e)
        {
            var AddForm = new AddUserForm(this);
            AddForm.Show();
        }

        private async void btnExport_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Xlsx files|*.xlsx";
            var dialogResult = saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "")
            {
                var serviceResult = await _userService.GetUsers(txtSearch.Text,_currentPage);
                if(serviceResult.Succeeded)
                    Export.ExcelExport(serviceResult.Result.data, saveFileDialog1.FileName);
            }
            
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            await SetData(1, txtSearch.Text);
            Cursor = Cursors.Default;
        }
    }
}
