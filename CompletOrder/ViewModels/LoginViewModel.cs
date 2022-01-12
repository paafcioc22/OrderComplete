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
        public ObservableCollection<User> Users { get; set; } = new ObservableCollection<User>();
        public Command LoadItemsCommand { get; set; }

        private User selectUser;

        public User SelectUser
        {
            get { return selectUser; }

            set { SetProperty(ref selectUser, value); }
        }


        private string pass1;

        public string Pass1
        {
            get { return pass1; }
             
            set 
            { 
              SetProperty(ref pass1, value);
                OnPropertyChanged(nameof(IsUserIdsEqual));
            }
        }

        private string pass2;

        public string Pass2
        {
            get { return pass2; }
            set { SetProperty(ref pass2, value);
                OnPropertyChanged(nameof(IsUserIdsEqual));
            }
        }


        private bool isUserIdsEqual;
        public bool IsUserIdsEqual
        {
            get { return Pass1 == Pass2 && !string.IsNullOrEmpty(Pass1) && !string.IsNullOrEmpty(Pass2); }
            set { SetProperty(ref isUserIdsEqual, value); }
        }



        public LoginViewModel()
        {
            

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
                if(Users.Count>0)
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
