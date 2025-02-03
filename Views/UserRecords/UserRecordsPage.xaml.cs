namespace AerobicWithMe.Views;

public partial class UserRecordsPage : ContentPage
{

    private static UserRecordsPage _instance;
    private static readonly object _lock = new();

    public static UserRecordsPage Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new UserRecordsPage();
                }
                return _instance;
            }
        }
    }


    public UserRecordsPage()
	{
		InitializeComponent();
	}

    private void Switch_Toggled(object sender, ToggledEventArgs e)
    {
        bool newValue = e.Value;
        Console.WriteLine($"IsShowAllTasks toggled to: {newValue}");

    }

}
