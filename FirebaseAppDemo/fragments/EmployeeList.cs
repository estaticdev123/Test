
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Firebase.Database;
using FirebaseAppDemo.adapters;
using FirebaseAppDemo.models;
using Newtonsoft.Json;
using static FirebaseAppDemo.BookingRecyclerViewAdapter;

namespace FirebaseAppDemo
{
    public class EmployeeList : Android.Support.V4.App.Fragment, IValueEventListener, IFileGrabber
    {

        private TextView txtTimeBooking;

        private RecyclerView recyclerView;
        private List<DetailModel> arrayListBooking;
        private BookingRecyclerViewAdapter bookingAdapter;
        private TextView txtSummary;

        public IFileGrabber onItemClick;
        DetailModel detailModel;

        private LinearLayout lnrSummary;
        Dialog dialogEmployee;

        public EmployeeList()
        {

        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            var view = inflater.Inflate(Resource.Layout.ListEmployee, container, false);
            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            //toolbar
            Android.Support.V7.Widget.Toolbar toolbar = (Android.Support.V7.Widget.Toolbar)((DashboardActivity)Activity).FindViewById(Resource.Id.toolbar);
            TextView mTitle = (TextView)toolbar.FindViewById(Resource.Id.toolbar_title);
            if (mTitle != null)
                mTitle.Text = "Booking";

            detailModel = (FirebaseAppDemo.DetailModel)Arguments.GetSerializable("time");
            lnrSummary = (Android.Widget.LinearLayout)view.FindViewById(Resource.Id.lnrSummary);
            txtSummary = (Android.Widget.TextView)view.FindViewById(Resource.Id.txt_summary_booking);

            onItemClick = this;

            onInit(view, detailModel);

            txtSummary.Click += delegate
            {
                OnItemClick(arrayListBooking[0].emp_id);
            };
        }


        private void onInit(View view, DetailModel detailModel)
        {

            recyclerView = (RecyclerView)view.FindViewById(Resource.Id.recycler_view);
            txtTimeBooking = view.FindViewById<TextView>(Resource.Id.txt_time_booking);

            var dateToGet = new DateTime(int.Parse(detailModel.availabledate.Substring(6)), int.Parse(detailModel.availabledate.Substring(3, 2)), int.Parse(detailModel.availabledate.Substring(0, 2)), int.Parse(detailModel.availabletime), 0, 0);

            var format = "MMMM dd, hh:mm";
            var dtString = dateToGet.ToString(format);

            txtTimeBooking.Text = dtString;

            arrayListBooking = new List<DetailModel>();

            bookingAdapter = new BookingRecyclerViewAdapter(Activity, arrayListBooking, recyclerView, (FirebaseAppDemo.DashboardActivity)Activity, onItemClick);

            RecyclerView.LayoutManager mLayoutManager = new LinearLayoutManager(Activity);
            recyclerView.SetLayoutManager(mLayoutManager);
            recyclerView.SetAdapter(bookingAdapter);

            getEmployeeData(detailModel);

        }


        private void getEmployeeData(DetailModel detailModel)
        {

            //var user = FirebaseAuth.Instance.CurrentUser;

            DatabaseReference database = FirebaseDatabase.Instance.Reference;

            Query query = database.Child("employee");
            query.AddListenerForSingleValueEvent(this);

        }

        public void OnCancelled(DatabaseError error)
        {

        }

        public void OnDataChange(DataSnapshot snapshot)
        {

            if (snapshot.Exists())
            {
                arrayListBooking.Clear();

                var obj = snapshot.Children;


                foreach (DataSnapshot snapshotChild in obj.ToEnumerable())
                {
                    if (snapshotChild.GetValue(true) == null) continue;

                    DetailModel model = new DetailModel();
                    model.cuse_name = snapshotChild.Child("cust_name")?.GetValue(true)?.ToString();
                    model.distance = snapshotChild.Child("distance")?.GetValue(true)?.ToString();
                    model.email = snapshotChild.Child("email")?.GetValue(true)?.ToString();
                    model.id = snapshotChild.Child("id")?.GetValue(true)?.ToString();
                    model.location = snapshotChild.Child("location")?.GetValue(true)?.ToString();
                    model.mobile = snapshotChild.Child("mobile")?.GetValue(true)?.ToString();
                    model.name = snapshotChild.Child("name")?.GetValue(true)?.ToString();
                    model.net_price = snapshotChild.Child("net_price")?.GetValue(true)?.ToString();
                    model.pic = snapshotChild.Child("pic")?.GetValue(true)?.ToString();
                    model.status = snapshotChild.Child("status")?.GetValue(true)?.ToString();
                    model.vat = snapshotChild.Child("vat")?.GetValue(true)?.ToString();
                    model.worktime = snapshotChild.Child("worktime")?.GetValue(true)?.ToString();
                    model.emp_id = snapshotChild.Child("emp_id")?.GetValue(true)?.ToString();

                    var data = snapshotChild.Child("avaibility").Value;

                    //model.availabletime = snapshotChild.Child("availabletime")?.GetValue(true)?.ToString();
                    //model.availabledate = snapshotChild.Child("availabledate")?.GetValue(true)?.ToString();

                    model.showtime = detailModel.availabletime;

                    string output = JsonConvert.SerializeObject(data);
                    var arraycombination = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AvailablityModelClass>>(output);

                    model.availableslotList = arraycombination;


                    for (int i = 0; i < model.availableslotList.Count; i++)
                    {

                        if (model.availableslotList[i].availabletime.Equals(detailModel.availabletime) && detailModel.availabledate.Equals(model.availableslotList[i].availabledate) && model.availableslotList[i].status.Equals("true"))
                        {
                            model.time = model.availableslotList[i].availabletime;
                            model.availabledate = model.availableslotList[i].availabledate;
                            model.availabletime = model.availableslotList[i].availabletime;
                            model.status = model.availableslotList[i].status;

                            model.availableslotid = model.availableslotList[i].id;

                            arrayListBooking.Add(model);
                            break;
                        }
                    }
                }

                if (arrayListBooking.Count == 1)
                {
                    lnrSummary.Visibility = ViewStates.Visible;
                }
                else
                {
                    lnrSummary.Visibility = ViewStates.Gone;
                }

                if (bookingAdapter != null)
                    bookingAdapter.NotifyDataSetChanged();
            }
        }

        public void OnItemClick(string fileUri)
        {
            if (fileUri.Equals(""))
            {
                if (arrayListBooking.Count != 1)
                    ShowEmployeeDialog();
            }
            else
            {
                if (dialogEmployee != null)
                    dialogEmployee.Dismiss();

                ((DashboardActivity)Activity).moveToSummary(arrayListBooking[0]);
            }
        }

        public void ShowEmployeeDialog()
        {
            dialogEmployee = new Dialog(Activity);
            dialogEmployee.RequestWindowFeature((int)WindowFeatures.NoTitle);
            View v = dialogEmployee.Window.DecorView;
            v.SetBackgroundResource(Resource.Color.colorwhite);
            dialogEmployee.SetContentView(Resource.Layout.DialogSelectEmployee);
            dialogEmployee.SetCanceledOnTouchOutside(false);
            dialogEmployee.SetCancelable(true);

            RecyclerView recyclerView = (RecyclerView)dialogEmployee.FindViewById(Resource.Id.rcv_dialog_select_emp);
            EmployeeDialogRecyclerViewAdapter bookingAdapter = new EmployeeDialogRecyclerViewAdapter(Activity, arrayListBooking, recyclerView, (FirebaseAppDemo.DashboardActivity)Activity, onItemClick, dialogEmployee);


            RecyclerView.LayoutManager mLayoutManager = new LinearLayoutManager(Activity);
            recyclerView.SetLayoutManager(mLayoutManager);
            recyclerView.SetAdapter(bookingAdapter);

            dialogEmployee.Show();
        }
    }
}







//var obj = snapshot.Children;

//foreach (DataSnapshot snapshotChild in obj.ToEnumerable())
//{
//    if (snapshotChild.GetValue(true) == null) continue;
//    DetailModel model = new DetailModel();
//    model.cuse_name = snapshotChild.Child("cust_name")?.GetValue(true)?.ToString();
//    model.distance = snapshotChild.Child("distance")?.GetValue(true)?.ToString();
//    model.email = snapshotChild.Child("email")?.GetValue(true)?.ToString();
//    model.id = snapshotChild.Child("id")?.GetValue(true)?.ToString();
//    model.location = snapshotChild.Child("location")?.GetValue(true)?.ToString();
//    model.mobile = snapshotChild.Child("mobile")?.GetValue(true)?.ToString();
//    model.name = snapshotChild.Child("name")?.GetValue(true)?.ToString();
//    model.net_price = snapshotChild.Child("net_price")?.GetValue(true)?.ToString();
//    model.pic = snapshotChild.Child("pic")?.GetValue(true)?.ToString();
//    model.status = snapshotChild.Child("status")?.GetValue(true)?.ToString();
//    model.vat = snapshotChild.Child("vat")?.GetValue(true)?.ToString();
//    model.worktime = snapshotChild.Child("worktime")?.GetValue(true)?.ToString();
//    model.emp_id = snapshotChild.Child("emp_id")?.GetValue(true)?.ToString();
//    model.availabletime = snapshotChild.Child("availabletime")?.GetValue(true)?.ToString();
//    model.availabledate = snapshotChild.Child("availabledate")?.GetValue(true)?.ToString();

//    model.showtime = time;

//    if (model.availabletime.Equals(time))
//        arrayListBooking.Add(model);

//    if (bookingAdapter != null)
//        bookingAdapter.NotifyDataSetChanged();

//    // Use type conversions as required. I have used string properties only
//}