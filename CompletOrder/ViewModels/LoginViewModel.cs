using CompletOrder.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Security.Cryptography;

namespace CompletOrder.ViewModels
{
     
    public class LoginViewModel : BaseViewModel
    {
        public ObservableCollection<User> Users { get; set; }
        public Command LoadItemsCommand { get; set; }

        private User selectUser;

        public User SelectUser
        {
            get { return selectUser; }

            set { SetProperty(ref selectUser, value); }
        }

        public LoginViewModel()
        {
            Users = new ObservableCollection<User>();

            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            
        }

        private async Task ExecuteLoadItemsCommand()
        {
            string Webquery = $@"cdn.PC_WykonajSelect N' 
                select 
	                 usr.User_Login as Login
	                ,usr.User_Name as Name 
	                ,usr.User_Password as Password
	                ,usr.User_Salt as Salt	 
                 
                from cdn.pc_UserToApp usrapp
	                join cdn.pc_users usr on usr.User_Id= usrapp.UsrApp_UsrId
	                join cdn.PC_Applications app on app.App_Id=usrapp.UsrApp_AppId
                where  
                usr.User_IsActive=1 
                and app.App_PackName=''com.completorder'' '";

            try
            {
                Users.Clear();
                var items = await App.TodoManager.PobierzDaneZWeb<User>(Webquery);
                foreach (var item in items)
                {
                    if (string.IsNullOrEmpty(item.Password))
                        item.VisiblePass = true;
                    else
                        item.VisiblePass = false;

                    Users.Add(item);

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
             
        
        }

        
    }
}
