
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using Firebase.Database;

namespace FirebaseAppDemo
{
    public class SummaryFragment : Android.Support.V4.App.Fragment
    {
        private TextView txtHeaderTime, txtEmpName, txtEmpLocation, txtWorkingTime, txtCustomerName, txtEmail, txtMobile, txtWorkingLocation,
         txtNetPrice, txtVat, txtTotal, txtBookNow;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            var view = inflater.Inflate(Resource.Layout.SummaryFragment, container, false);
            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            //toolbar
            Android.Support.V7.Widget.Toolbar toolbar = (Android.Support.V7.Widget.Toolbar)((DashboardActivity)Activity).FindViewById(Resource.Id.toolbar);
            TextView mTitle = (TextView)toolbar.FindViewById(Resource.Id.toolbar_title);
            if (mTitle != null)
                mTitle.Text = "Summary";


            DetailModel detailModel = (FirebaseAppDemo.DetailModel)Arguments.GetSerializable("SomeTag");

            if(detailModel!=null)
            {
                onInit(view, detailModel);
            }

        }

        private void onInit(View view, DetailModel model)
        {
            txtHeaderTime = (TextView)view.FindViewById(Resource.Id.txt_header_time);
            txtEmpName = (TextView)view.FindViewById(Resource.Id.txt_employee_name_summary);
            txtEmpLocation = (TextView)view.FindViewById(Resource.Id.txt_employee_location_summary);
            txtWorkingTime = (TextView)view.FindViewById(Resource.Id.txt_working_time);
            txtCustomerName = (TextView)view.FindViewById(Resource.Id.txt_customer_name);
            txtEmail = (TextView)view.FindViewById(Resource.Id.txt_email);
            txtMobile = (TextView)view.FindViewById(Resource.Id.txt_mobile);
            txtWorkingLocation = (TextView)view.FindViewById(Resource.Id.txt_working_location);

            txtNetPrice = (TextView)view.FindViewById(Resource.Id.txt_net_price);
            txtVat = (TextView)view.FindViewById(Resource.Id.txt_vat);
            txtTotal = (TextView)view.FindViewById(Resource.Id.txt_total);

            txtBookNow = (TextView)view.FindViewById(Resource.Id.txt_book_now);

            txtBookNow.Click += delegate {
                BookNow(model);
            }; 

            txtEmpName.Text = model.name;
            txtHeaderTime.Text = model.worktime;
            txtEmpLocation.Text = model.distance;
            txtWorkingTime.Text = model.availabletime;
            txtCustomerName.Text = model.cuse_name;
            txtEmail.Text = model.email;
            txtMobile.Text = model.mobile;
            txtWorkingLocation.Text = model.location;

            txtNetPrice.Text = model.net_price;
            txtVat.Text = model.vat;
            txtTotal.Text = ("250 EURO");

            var dateToGet = new DateTime(int.Parse(model.availabledate.Substring(6)), int.Parse(model.availabledate.Substring(3, 2)), int.Parse(model.availabledate.Substring(0, 2)), int.Parse(model.availabletime), 0, 0);

            var format = "MMMM dd, hh:mm";
            var dtString = dateToGet.ToString(format);

            txtHeaderTime.Text = dtString;

        }


        protected async void BookNow(DetailModel model)
        {
       

            DatabaseReference database = FirebaseDatabase.Instance.Reference;

            var stringEndPoint = database.Child("Booked").Push().Key;
            var stringpath = database.Child("Booked").Child(stringEndPoint);

            stringpath.Child("cust_id").SetValue(FirebaseAuth.Instance.CurrentUser.Uid);
            stringpath.Child("date").SetValue(model.availabledate);
            stringpath.Child("emp_id").SetValue(model.emp_id);
            stringpath.Child("id").SetValue(stringEndPoint);
            stringpath.Child("time").SetValue(model.availabletime);
            stringpath.Child("availableslotid").SetValue(model.availableslotid);
            stringpath.Child("cust_name").SetValue(model.cuse_name);
            stringpath.Child("book_status").SetValue("pending");

                   
            var pathForEmplBook = database.Child("employee").Child(model.emp_id).Child("avaibility").Child(model.availableslotid);
            pathForEmplBook.Child("status").SetValue("false");

            Toast.MakeText(Activity, "Booking Success!!", ToastLength.Short).Show();

            ((DashboardActivity)Activity).moveToCalander();

                 

        }

    }
}
