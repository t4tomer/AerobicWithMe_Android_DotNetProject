namespace AerobicWithMe.Views;


public partial class MapsPage : ContentPage
{
	public MapsPage()
	{

        InitializeComponent();
     



    }

    private void Switch_Toggled(object sender, ToggledEventArgs e)
    {
        bool newValue = e.Value;
        Console.WriteLine($"IsShowAllTasks toggled to: {newValue}");

    }

}
