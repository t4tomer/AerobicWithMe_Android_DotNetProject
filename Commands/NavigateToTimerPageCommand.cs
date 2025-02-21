

using AerobicWithMe.Views; 
using AerobicWithMe.Interfaces;


namespace AerobicWithMe.Commands
{
    public class NavigateToTimerPageCommand : I_Command
    {
        private readonly string _mapTitle;

        public NavigateToTimerPageCommand(string mapTitle)
        {
            _mapTitle = mapTitle;
        }

        public async Task Execute()
        {
            var timerPage = TimerPage.GetInstance;
            timerPage.setTitle(_mapTitle);
            await Shell.Current.Navigation.PushAsync(timerPage);
        }
    }

}
