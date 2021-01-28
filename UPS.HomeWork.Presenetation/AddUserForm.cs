using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using UPS.Homework.CrossCutting;
using UPS.Homework.DTO;
using UPS.Homework.Service;

namespace UPS.HomeWork.Presenetation
{
    public partial class AddUserForm : Form
    {
        private readonly IUserService _userService;
        private readonly UserDataGridForm _userDataGridForm;
        public AddUserForm(UserDataGridForm userDataGridForm)
        {
            _userService = new UserService(Application.StartupPath);
            _userDataGridForm = userDataGridForm;
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            
            if (!Validation.IsEmailValid(txtEmail.Text))
            {
                MessageBox.Show("the Email is not valid");
            }
            var gender = "Male";
            if (rdbFemale.Checked)
                gender = "Female";
            var status = "Active";
            if (rdbInActive.Checked)
                status = "Inactive";
            var AddServiceResult = await _userService.AddUser(new UserDto()
            {
                name = txtName.Text,
                email = txtEmail.Text,
                gender = gender,
                status = status,
                created_at = DateTime.Now,
                updated_at = DateTime.Now,
                
            });
            var message = AddServiceResult.Messages.FirstOrDefault();
            if (AddServiceResult.Succeeded)
            {
                await _userDataGridForm.SetData(_userDataGridForm._currentPage);
                MessageBox.Show(message.Message.GetEnumDescription(), message.Type.GetEnumDescription());
                Close();
            }
            else
            {
                MessageBox.Show(message.Message.GetEnumDescription(), message.Type.GetEnumDescription());
            }
        }

       

        
    }
}
