using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.App;
using Android.Support.V4.Widget;
using System.Collections.Generic;
using Android.Support.V4.App;
using Android;
using Android.Support.V7.Widget;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Content.Res;
using Firebase.Auth;
using Firebase.Database;
using System.Linq;
using Java.Util;
using Android.Graphics;
using Xamarin.Facebook.Login;

namespace FirebaseAppDemo
{
    [Activity(Label = "DashboardActivity", WindowSoftInputMode = SoftInput.AdjustPan, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Theme = "@style/MyTheme")]
    public class DashboardActivity : AppCompatActivity, IValueEventListener
    {

        DrawerLayout drawerLayout;
        NavigationView navigationView;
        IMenuItem previousItem;
        Android.Support.V7.App.ActionBarDrawerToggle toggle;
        public TextView txtName;
        public TextView txtEmail;
        public TextView mTitle;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.DashboardActivity);
            // Create your application here


            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            //For showing back button  
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayShowTitleEnabled(false);
            //setting Hamburger icon Here  
            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.cancel);
            //Getting Drawer Layout declared in UI and handling closing and open events  
            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawerLayout.DrawerOpened += DrawerLayout_DrawerOpened;
            drawerLayout.DrawerClosed += DrawerLayout_DrawerClosed;
            navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            toggle = new Android.Support.V7.App.ActionBarDrawerToggle
            (
                    this,
                    drawerLayout,
             Resource.String.str_drawer_open,
             Resource.String.str_drawer_close
            );
            drawerLayout.AddDrawerListener(toggle);
            //Synchronize the state of the drawer indicator/affordance with the linked DrawerLayout  
            toggle.SyncState();
            //Handling click events on Menu items  
            navigationView.NavigationItemSelected += (sender, e) =>
            {

                if (previousItem != null)
                    previousItem.SetChecked(false);

                navigationView.SetCheckedItem(e.MenuItem.ItemId);

                previousItem = e.MenuItem;

                switch (e.MenuItem.ItemId)
                {
                    case Resource.Id.nav_calander:
                        ListItemClicked(0);
                        break;
                    case Resource.Id.nav_reservation:
                        ListItemClicked(1);
                        break;
                    case Resource.Id.nav_logout:
                        ListItemClicked(2);
                        break;
                }


                drawerLayout.CloseDrawers();
            };

            var headerView = navigationView.GetHeaderView(0);
            txtName = headerView.FindViewById<TextView>(Resource.Id.navheader_name);
            txtEmail = headerView.FindViewById<TextView>(Resource.Id.navheader_email);


            getUserData();

            initCalanderView();

        }

        private void initCalanderView()
        {
            Android.Support.V4.App.Fragment fragment = new CalanderFragment();

            if (fragment != null)
            {
                SupportFragmentManager.BeginTransaction()
                               .Replace(Resource.Id.content_frame, fragment)
                               .Commit();
            }

            var toolbarTop = (Android.Support.V7.Widget.Toolbar)FindViewById(Resource.Id.toolbar);
            mTitle = (TextView)toolbarTop.FindViewById(Resource.Id.toolbar_title);

            mTitle.Text = "March 2018";
        }



        private void getUserData()
        {

            var user = FirebaseAuth.Instance.CurrentUser;

            DatabaseReference database = FirebaseDatabase.Instance.Reference;
            database.Child("User").Child(user.Uid).AddListenerForSingleValueEvent(this);
        }

        private void DrawerLayout_DrawerClosed(object sender, DrawerLayout.DrawerClosedEventArgs e)
        {
            //SupportActionBar.SetHomeAsUpIndicator(VAMOS.Droid.Resource.Drawable.ic_menu);  
        }

        private void DrawerLayout_DrawerOpened(object sender, DrawerLayout.DrawerOpenedEventArgs e)
        {
            // SupportActionBar.SetHomeAsUpIndicator(VAMOS.Droid.Resource.Drawable.ic_back);  
        }

        private void ListItemClicked(int position)
        {

            Android.Support.V4.App.Fragment fragment = null;
            switch (position)
            {
                case 0:
                    fragment = new CalanderFragment();
                    if (mTitle != null)
                        mTitle.Text = "March 2018";

                    break;
                case 1:
                    fragment = new ReservationFragment();
                    mTitle.Text = "Reservations";
                    break;
                case 2:
                    logoutuser();
                    break;
            }
            if (fragment != null)
            {
                SupportFragmentManager.BeginTransaction()
                               .Replace(Resource.Id.content_frame, fragment)
                               .Commit();
            }


        }


        private void logoutuser()
        {
            View view = LayoutInflater.Inflate(Resource.Layout.LogoutAlertDialog, null);


            Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);
            alert.SetView(view);
            alert.SetCancelable(false);
            Button btnNo = view.FindViewById<Button>(Resource.Id.btnNo);
            Button btnYes = view.FindViewById<Button>(Resource.Id.btnYes);
            Dialog dialog = alert.Create();

            btnNo.Click += delegate
            {
                dialog.Dismiss();
            };

            btnYes.Click += delegate
            {
                dialog.Dismiss();

                try
                {

                    FirebaseAuth.Instance.SignOut();
                    Toast.MakeText(this, "Logout succesfully", ToastLength.Short).Show();

                    LoginManager.Instance.LogOut();

                    Intent dashboard = new Intent(this, typeof(LoginActivity));
                    StartActivity(dashboard);
                    Finish();
                }
                catch (Exception e)
                {

                }

            };

            dialog.Show();


        }

        //Handling Back Key Press  
        public override void OnBackPressed()
        {
            if (drawerLayout.IsDrawerOpen((int)GravityFlags.Start))
            {
                drawerLayout.CloseDrawer((int)GravityFlags.Start);
            }
            else
            {
                base.OnBackPressed();
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:

                    drawerLayout.OpenDrawer(Android.Support.V4.View.GravityCompat.Start);
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        //Resposnible for mainting state,suppose if you suddenly rotated screen than drawer should not losse it context so you have save drawer states like below  
        protected override void OnPostCreate(Bundle savedInstanceState)
        {

            base.OnPostCreate(savedInstanceState);
            toggle.SyncState();

        }
        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            toggle.OnConfigurationChanged(newConfig);
        }

        public void OnCancelled(DatabaseError error)
        {
            throw new NotImplementedException();
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Exists())
            {
                UserProfileModel model = new UserProfileModel();

                model.name = snapshot.Child("name")?.GetValue(true)?.ToString();
                model.email = snapshot.Child("email")?.GetValue(true)?.ToString();

                SetData(model);
                // Use type conversions as required. I have used string properties only

            }
        }

        public void SetData(UserProfileModel model)
        {
            if (!String.IsNullOrEmpty(model.name) && !String.IsNullOrEmpty(model.email))
            {
                txtName.Text = model.name;
                txtEmail.Text = model.email;
            }

        }

        public void moveToCalander()
        {

            Android.Support.V4.App.Fragment fragment = new CalanderFragment();

            if (fragment != null)
            {
                SupportFragmentManager.BeginTransaction()
                               .Replace(Resource.Id.content_frame, fragment)
                               .Commit();
            }
        }



        public void moveToSummary(DetailModel model)
        {

            Android.Support.V4.App.Fragment fragment = new SummaryFragment();

            Bundle utilBundle = new Bundle();
            utilBundle.PutSerializable("SomeTag", (Java.IO.ISerializable)model);

            if (fragment != null)
            {
                SupportFragmentManager.BeginTransaction()
                               .Replace(Resource.Id.content_frame, fragment)
                                      .AddToBackStack(null)
                                .Commit();

                fragment.Arguments = utilBundle;
            }
        }

        public void moveToEmployeeDetail(DetailModel model)
        {


            Bundle utilBundle = new Bundle();
            utilBundle.PutSerializable("time", model);

            Android.Support.V4.App.Fragment fragment = new EmployeeList();

            if (fragment != null)
            {
                SupportFragmentManager.BeginTransaction()
                               .Replace(Resource.Id.content_frame, fragment)
                                      .AddToBackStack(null)
                               .Commit();

                fragment.Arguments = utilBundle;
            }
        }
    }
}
