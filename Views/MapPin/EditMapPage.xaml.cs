




namespace AerobicWithMe.Views;

public partial class EditMapPage : ContentPage, IQueryAttributable
{
    public EditMapPage()
    {
        InitializeComponent();
        //TextBox.IsVisible = false;

    }

    public EditMapPage(bool ans)
    {
        InitializeComponent();
        TextBox.IsVisible = ans;
    }

    //remove the OkButton and Text box from user's 
    public void EditDisplay(bool ans)
    {
        //InitializeComponent();
        TextBox.IsVisible = ans;
        OkButton.IsVisible = ans;

    }

    // Implement the IQueryAttributable interface to handle query parameters
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {



        if (query.ContainsKey("isEditVisible") &&
            bool.TryParse(query["isEditVisible"].ToString(), out bool isVisible))
        {
            EditDisplay(isVisible);
        }
    }



}
