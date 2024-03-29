﻿using CompletOrder.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Security.Cryptography;
using System.Windows.Input;
using Microsoft.AspNetCore.Identity;
using CompletOrder.Views;
using System.Linq;
using Xamarin.Essentials;

namespace CompletOrder.ViewModels
{
     
    public class LoginViewModel : BaseViewModel
    {
        public ObservableCollection<User> Users { get; set; } = new ObservableCollection<User>();
        public Command LoadItemsCommand { get; set; }
        public ICommand SaveUserPass { private set; get; }
        public ICommand PassCheckCommand { private set; get; }
        //private readonly IPasswordHasher<User> passwordHasher;
        public IPasswordHasher<User> passwordHasher => DependencyService.Get<IPasswordHasher<User>>();

        private User selectUser;

        public User SelectUser
        {
            get { return selectUser; }

            set 
            { 
                SetProperty(ref selectUser, value);
                OnPropertyChanged(nameof(SelectUser));
                //Task.Run(async ()=>await ExecuteLoadItemsCommand()); 
            }
        }



        private int selectedIndex;

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { selectedIndex = value; }
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
            

            //this.passwordHasher = new PasswordHasher<User>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            SaveUserPass = new Command(async (User) => await UpdateUserPass(SelectUser));
            PassCheckCommand = new Command<string>(async (pass) => await TryToLogin(pass));
            
        }

        private async Task TryToLogin(string pass)
        {
            if (!selectUser.VisiblePass)
            {
                IsBusy = true;
                var isCorrect= passwordHasher.VerifyHashedPassword(selectUser, selectUser.Password, pass);

                if(isCorrect == PasswordVerificationResult.Success)
                {
                    Preferences.Set("user", selectUser.Login);
                    Pass1 = "";
                
                    await Application.Current.MainPage.Navigation.PushAsync(new OrderView());
                    IsBusy = false;
                }
                else
                {
                    IsBusy = false;
                    await Application.Current.MainPage.DisplayAlert("Uwaga","Podano błędne hasło","OK");
                }
            }
        }

        private async Task ExecuteLoadItemsCommand()
        {

            var tmp = SelectedIndex;
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


        internal async Task<bool> UpdateUserPass(User user)
        {
            try
            {
                bool IsAddRow = true;

                {

                    var Hashpass = passwordHasher.HashPassword(user, user.Password);
                     

                    var data = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    var sqlInsert = $@"cdn.PC_WykonajSelect N' update [CDNXL_JOART].[CDN].[PC_Users]
                                     set User_Password=''{Hashpass}''  
                                        where User_login=''{user.Login}''
                                    if @@ROWCOUNT>0
                                                select ''OK'' as User_Login
                    '";

                    var response = await App.TodoManager.PobierzDaneZWeb<User>(sqlInsert);
                    if (response != null)
                    {
                        if (response.Count > 0)
                        {
                            LoadItemsCommand.Execute(null);

                            
                            await Application.Current.MainPage.DisplayAlert("Info", "Hasło zapisane\nMozesz się teraz zalogowac", "OK");
                            Pass1 = "";
                            Pass2 = "";

                            
                            return IsAddRow;

                        }
                    }

                }

                return IsAddRow = false;
            }
            catch (Exception)
            {

                throw;
            }
        }


    }
}
