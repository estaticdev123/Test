using System;
using System.Collections.Generic;
using Android.Content;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace FirebaseAppDemo
{
    public class BookingRecyclerViewAdapter : RecyclerView.Adapter
    {
        private Context context;
        private List<DetailModel> arrayList;
        private RecyclerView recyclerView;
        private DashboardActivity activity;
        private IFileGrabber onItemClick;

        public BookingRecyclerViewAdapter()
        {
        }

        public BookingRecyclerViewAdapter(Context context, List<DetailModel> arrayList, RecyclerView recyclerView, DashboardActivity activity, IFileGrabber onItemClick)
        {
            this.context = context;
            this.arrayList = arrayList;
            this.recyclerView = recyclerView;
            this.activity = activity;
            this.onItemClick = onItemClick;
        }

        public override int ItemCount
        {
            get
            {
                if (arrayList != null && arrayList.Count > 0)
                    return 1;
                else
                    return 0;
            }
        }


        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            MyView myholder = holder as MyView;

            myholder.txtEmpName.Text = arrayList[position].name;
            myholder.txtEmpLocation.Text = arrayList[position].location;

            //if (arrayList[position].pic != null)
            //Glide
            //.With(context)
            //.Load(arrayList[position].pic)
            //.Apply(RequestOptions.NoAnimation())
            //.Into(myholder.imgEmp);

            //if (arrayList[position].pic != null)
                //Picasso.With(context)
                       //.Load(arrayList[position].pic)
                       //.Into(myholder.imgEmp);

            myholder.mainview.Click += Mainview_Click;

        }

        private void Mainview_Click(object sender, EventArgs e)
        {
            //int position = recyclerView.GetChildAdapterPosition((View)sender);
            //activity.moveToSummary(arrayList[position]);

            onItemClick.OnItemClick("");
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(context)
                                          .Inflate(Resource.Layout.RawLayoutBooking, parent, false);


            TextView txtEmpName = itemView.FindViewById<TextView>(Resource.Id.txt_employee_booking);
            TextView txtEmpLocation = itemView.FindViewById<TextView>(Resource.Id.txt_location_booking);
            ImageView imgEmp = itemView.FindViewById<ImageView>(Resource.Id.img_booking);

            MyView view = new MyView(itemView)
            {
                txtEmpName = txtEmpName,
                txtEmpLocation = txtEmpLocation,
                imgEmp = imgEmp
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


        public interface IFileGrabber
        {
            void OnItemClick(string fileUri);
        }

    }
}
