using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.PerformanceData;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UPS.Homework.DTO;
using UPS.Homework.Service;

namespace UPS.HomeWork.Presenetation
{
    
    public partial class Form1 : Form
    {
        
        
        private IUserService _userService;
        private int _currentPage = 1;
        private int _totalPages;
        public Form1()
        {
            InitializeComponent();
            _userService = new UserService(Application.StartupPath);
            UsersDataGridView.SetDoubleBuffered();
            UsersDataGridView.DataSource = bindingSource1;
            

        }

        private async Task SetData(int page)
        {
           
            var serviceResult = await _userService.LoadPage(page);
            if (serviceResult.Succeeded)
            {
                noTotalRecords.Text = serviceResult.Result.Item3.total.ToString();
                lblTotalPage.Text = serviceResult.Result.Item3.pages.ToString();
                txtCurrentPage.Text = serviceResult.Result.Item3.page.ToString();
                bindingSource1.DataMember = serviceResult.Result.Item1.TableName;
                bindingSource1.DataSource = serviceResult.Result.Item2;
                UsersDataGridView.RowHeadersVisible = true;
                UsersDataGridView.Columns["edit"].Width = 25;
                UsersDataGridView.Columns["edit"].HeaderText = "";
                UsersDataGridView.Columns["edit"].Resizable = DataGridViewTriState.False;
                UsersDataGridView.Columns["delete"].HeaderText = "";
                UsersDataGridView.Columns["delete"].Width = 25;
                UsersDataGridView.Columns["delete"].Resizable = DataGridViewTriState.False;
            }
            else
            {
                MessageBox.Show(serviceResult.Messages[0].Message.ToString());
            }
            
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
            bool restartsearch = true;
            int startColumn = 0;
            int startRow = 0;
            if (!e.FromBegin)
            {
                bool endcol = UsersDataGridView.CurrentCell.ColumnIndex + 1 >= UsersDataGridView.ColumnCount;
                bool endrow = UsersDataGridView.CurrentCell.RowIndex + 1 >= UsersDataGridView.RowCount;

                if (endcol && endrow)
                {
                    startColumn = UsersDataGridView.CurrentCell.ColumnIndex;
                    startRow = UsersDataGridView.CurrentCell.RowIndex;
                }
                else
                {
                    startColumn = endcol ? 0 : UsersDataGridView.CurrentCell.ColumnIndex + 1;
                    startRow = UsersDataGridView.CurrentCell.RowIndex + (endcol ? 1 : 0);
                }
            }
            DataGridViewCell c = UsersDataGridView.FindCell(
                e.ValueToSearch,
                e.ColumnToSearch != null ? e.ColumnToSearch.Name : null,
                startRow,
                startColumn,
                e.WholeWord,
                e.CaseSensitive);
            if (c == null && restartsearch)
                c = UsersDataGridView.FindCell(
                    e.ValueToSearch,
                    e.ColumnToSearch != null ? e.ColumnToSearch.Name : null,
                    0,
                    0,
                    e.WholeWord,
                    e.CaseSensitive);
            if (c != null)
                UsersDataGridView.CurrentCell = c;
        }

        private async void btnNextPage_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            if (_currentPage < _totalPages)
            {
                _currentPage++;
                await SetData(_currentPage);
            }
            
            Cursor = Cursors.Default;
        }

        private async void btnPreviousPage_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            if (_currentPage > 1)
            {
                _currentPage--;
                await SetData(_currentPage);
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

        private void txtCurrentPage_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private async void txtCurrentPage_Leave(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            
            if ( Int32.TryParse(txtCurrentPage.Text,out int page) &&  page > 0 )
            {
                _currentPage = page;
                if (page > _totalPages)
                {
                    _currentPage = _totalPages;
                    page = _totalPages;
                }
                    
                
                await SetData(page);
            }

            Cursor = Cursors.Default;
        }

        private async void advancedDataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == UsersDataGridView.Columns["delete"].Index)
            {
                var resultBtn = MessageBox.Show("Are you sure to delete this row", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (resultBtn == DialogResult.Yes)
                {
                    var idStr = UsersDataGridView.Rows[e.RowIndex].Cells[0].Value.ToString();
                    if (Int32.TryParse(idStr, out int id))
                    {
                        var serviceResult = await _userService.DeleteUser(id);
                        if (serviceResult.Succeeded)
                        {
                            await SetData(_currentPage);
                        }
                        
                    }
                }
                
                
            }
            if (e.ColumnIndex == UsersDataGridView.Columns["edit"].Index)
            {
                MessageBox.Show("edit");
            }

        }
    }
}
