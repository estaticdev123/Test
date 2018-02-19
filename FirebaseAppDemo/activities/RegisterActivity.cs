
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using Firebase.Database;

namespace FirebaseAppDemo
{
    [Activity(Label = "RegisterActivity", WindowSoftInputMode = SoftInput.AdjustPan, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class RegisterActivity : Activity
    {

        string emailText = "";
        string passwordText = "";
        string confirmpasswordText = "";
        string nameText = "";
        string mobileText = "";
        string locationText = "";
        string uidText = "";


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.RegisterActivity);

            TextView txtRegister = FindViewById<TextView>(Resource.Id.txtRegister);
            txtRegister.Click += async delegate (object sender, EventArgs e)
            {
                await registerAsync(sender, e);
            };

            ImageView imgCancel = FindViewById<ImageView>(Resource.Id.imgCancel);
            imgCancel.Click += cancel;


        }

        protected void AuthStateChanged(object sender, FirebaseAuth.AuthStateEventArgs e)
        {
            var user = e.Auth.CurrentUser;

            if (user != null)
            {
                uidText = user.Uid;
                CreateUser(uidText);
            }
            else
            {

            }
        }

        protected async void CreateUser(string uid)
        {
            UserProfileModel user = new UserProfileModel();
            user.uid = uid;
            user.name = nameText;
            user.email = emailText;
            user.mobileno = mobileText;
            user.location = locationText;
            user.password = passwordText;

            DatabaseReference database = FirebaseDatabase.Instance.Reference;

            var stringEndPoint = database.Child("User").Child(uid);
            stringEndPoint.Child("name").SetValue(nameText);
            stringEndPoint.Child("uid").SetValue(uid);
            stringEndPoint.Child("email").SetValue(emailText);
            stringEndPoint.Child("mobileno").SetValue(mobileText);
            stringEndPoint.Child("location").SetValue(locationText);
            stringEndPoint.Child("password").SetValue(passwordText);


            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutString("location", locationText);
            editor.Apply();

            Toast.MakeText(this, "Register Success!!", ToastLength.Short).Show();

            MoveToDashboard();

        }

        protected void MoveToDashboard()
        {
            Intent dashboard = new Intent(this, typeof(DashboardActivity));
            StartActivity(dashboard);
            Finish();
        }

        protected override void OnStart()
        {
            base.OnStart();

            FirebaseAuth.Instance.AuthState += AuthStateChanged;
        }

        protected override void OnStop()
        {
            base.OnStop();

            FirebaseAuth.Instance.AuthState -= AuthStateChanged;
        }

        public void cancel(object sender, EventArgs e)
        {
            Finish();
        }

        public async System.Threading.Tasks.Task registerAsync(object sender, EventArgs e)
        {
            if (isValid())
            {

                try
                {
                    await FirebaseAuth.Instance.CreateUserWithEmailAndPasswordAsync(emailText, passwordText);
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                }


            }
        }

        private bool isValid()
        {
            EditText email = FindViewById<EditText>(Resource.Id.edtEmail);
            EditText password = FindViewById<EditText>(Resource.Id.edtPassword);
            EditText confirmpass = FindViewById<EditText>(Resource.Id.edtConfirmPassword);
            EditText name = FindViewById<EditText>(Resource.Id.edtName);
            EditText mobile = FindViewById<EditText>(Resource.Id.edtMobile);
            EditText location = FindViewById<EditText>(Resource.Id.edtLocation);


            emailText = email.Text.ToString();
            passwordText = password.Text.ToString();
            confirmpasswordText = confirmpass.Text.ToString();
            nameText = name.Text.ToString();
            mobileText = mobile.Text.ToString();
            locationText = location.Text.ToString();


            if (emailText != null && emailText.Length > 0 && isValidEmail(emailText))
            {
                if (passwordText != null && passwordText.Length > 6)
                {
                    if (confirmpasswordText != null && confirmpasswordText.Equals(passwordText))
                    {
                        if (nameText != null && !nameText.Equals(""))
                        {
                            if (mobileText != null && mobileText.Length > 6)
                            {
                                if (locationText != null && locationText.Length > 0)
                                {
                                    return true;
                                }
                                else
                                    Toast.MakeText(this, "Enter Proper Location", ToastLength.Short).Show();
                            }
                            else
                                Toast.MakeText(this, "Enter Proper Mobile Number", ToastLength.Short).Show();
                        }
                        else
                            Toast.MakeText(this, "Enter Proper Name", ToastLength.Short).Show();
                    }
                    else
                        Toast.MakeText(this, "Enter Proper confirm password", ToastLength.Short).Show();
                }
                else
                    Toast.MakeText(this, "Enter Password more than 6 character", ToastLength.Short).Show();
            }

            else Toast.MakeText(this, "Enter Valid Email Address", ToastLength.Short).Show();

            return false;
        }



        public bool isValidEmail(string email)
        {
            return Android.Util.Patterns.EmailAddress.Matcher(email).Matches();
        }
    }
}
