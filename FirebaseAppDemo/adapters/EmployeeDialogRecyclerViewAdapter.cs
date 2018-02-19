using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using static FirebaseAppDemo.BookingRecyclerViewAdapter;

namespace FirebaseAppDemo.adapters
{
    public class EmployeeDialogRecyclerViewAdapter : RecyclerView.Adapter
    {

        private Context context;
        private List<DetailModel> arrayList;
        private RecyclerView recyclerView;
        private DashboardActivity activity;
        private IFileGrabber onItemClick;
        private Dialog dialogEmployee;

        public EmployeeDialogRecyclerViewAdapter()
        {
        }

        public EmployeeDialogRecyclerViewAdapter(Context context, List<DetailModel> arrayList, RecyclerView recyclerView, DashboardActivity activity, IFileGrabber onItemClick, Dialog dialogEmployee)
        {
            this.context = context;
            this.arrayList = arrayList;
            this.recyclerView = recyclerView;
            this.activity = activity;
            this.onItemClick = onItemClick;
            this.dialogEmployee = dialogEmployee;
        }

        public override int ItemCount
        {
            get
            {
                if (arrayList != null && arrayList.Count > 0)
                    return arrayList.Count;
                else
                    return 0;
            }
        }


        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            MyView myholder = holder as MyView;

            myholder.txtEmpName.Text = arrayList[position].name;
            myholder.txtEmpLocation.Text = arrayList[position].location;

            myholder.mainview.Click += Mainview_Click;

        }

        private void Mainview_Click(object sender, EventArgs e)
        {
            if (dialogEmployee != null)
                dialogEmployee.Dismiss();

            int position = recyclerView.GetChildAdapterPosition((View)sender);
            activity.moveToSummary(arrayList[position]);

        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(context)
                                          .Inflate(Resource.Layout.RawLayoutBooking, parent, false);


            TextView txtEmpName = itemView.FindViewById<TextView>(Resource.Id.txt_employee_booking);
            TextView txtEmpLocation = itemView.FindViewById<TextView>(Resource.Id.txt_location_booking);

            MyView view = new MyView(itemView)
            {
                txtEmpName = txtEmpName,
                txtEmpLocation = txtEmpLocation
            };


            return view;
        }


        public class MyView : RecyclerView.ViewHolder
        {
            public View mainview
            {
                get;
                set;
            }


            public TextView txtEmpName
            {
                get;
                set;
            }
            public TextView txtEmpLocation
            {
                get;
                set;
            }

            public ImageView imgEmp
            {
                get;
                set;
            }


            public MyView(View view) : base(view)
            {
                mainview = view;
            }
        }



    }
}
