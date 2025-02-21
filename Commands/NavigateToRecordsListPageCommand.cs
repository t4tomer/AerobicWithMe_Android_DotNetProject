using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AerobicWithMe.Interfaces;
using AerobicWithMe.ViewModels;
using AerobicWithMe.Views;
using Microsoft.Maui.Controls; // Required for navigation



namespace AerobicWithMe.Commands
{

    public class NavigateToRecordsListPageCommand : I_Command
    {
        private readonly string _mapTitle;
        public ICommand NavigateCommand { get; private set; }

        public NavigateToRecordsListPageCommand(string mapTitle)
        {
            _mapTitle = mapTitle;
        }

        public async Task Execute()
        {
            UserRecordsViewModel userRecordsVM_Page = new UserRecordsViewModel();

            userRecordsVM_Page.setTrackName(_mapTitle);

            // Create a new instance of the UserRecordsPage and bind it to the ViewModel
            var userRecordsPage = new UserRecordsPage
            {
                BindingContext = userRecordsVM_Page
            };

            await Shell.Current.Navigation.PushAsync(userRecordsPage);

        }
    }
}
