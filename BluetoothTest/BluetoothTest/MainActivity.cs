using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace BluetoothTest
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.content_main);

            Button greenButton = FindViewById<Button>(Resource.Id.GreenButton);
            greenButton.Click += GreenButton_Click;

            Button blueButton = FindViewById<Button>(Resource.Id.BlueButton);
            blueButton.Click += BlueButton_Click;

            Button redButton = FindViewById<Button>(Resource.Id.RedButton);
            redButton.Click += RedButton_Click;

        }

        private void RedButton_Click(object sender, EventArgs e)
        {
            Bluetooth bt = new Bluetooth();
            bt.Start("HC-05", "red");
        }

        private void BlueButton_Click(object sender, EventArgs e)
        {
            Bluetooth bt = new Bluetooth();
            bt.Start("HC-05", "blue");
        }

        private void GreenButton_Click(object sender, EventArgs e)
        {
            Bluetooth bt = new Bluetooth();
            bt.Start("HC-05", "green");
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }
	}
}

