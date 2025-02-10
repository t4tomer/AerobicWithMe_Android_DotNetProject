using System.Threading.Tasks;

namespace AerobicWithMe.Services
{
    public interface ILogin
    {
        Task OnAppearing();
        Task Login();
        Task SignUp();
        Task<bool> VeryifyEmailAndPassword();
        Task GoToMainPage();
    }
}
