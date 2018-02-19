using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using FirebaseAppDemo.models;

namespace FirebaseAppDemo
{
    public class DetailModel : Java.Lang.Object, Java.IO.ISerializable
    {
        public DetailModel()
        {
        }

        public string cuse_name
        {
            get;
            set;
        }

        public string distance
        {
            get;
            set;
        }


        public string email
        {
            get;
            set;
        }

        public string mobile
        {
            get;
            set;
        }

        public string name
        {
            get;
            set;
        }

        public string cust_id
        {
            get;
            set;
        }

        public string date
        {
            get;
            set;
        }


        public string emp_id
        {
            get;
            set;
        }

        public string id
        {
            get;
            set;
        }

        public string time
        {
            get;
            set;
        }

        public string status
        {
            get;
            set;
        }

        public string location
        {
            get;
            set;
        }

        public string money
        {
            get;
            set;
        }

        public string net_price
        {
            get;
            set;
        }

        public string pic
        {
            get;
            set;
        }

        public string vat
        {
            get;
            set;
        }

        public string worktime
        {
            get;
            set;
        }

        public string showtime
        {
            get;
            set;
        }

        public string availabledate
        {
            get;
            set;
        }

        public string availabletime
        {
            get;
            set;
        }

      
        public string availableslotid
        {
            get;
            set;
        }

        public string bookstatus
        {
            get;
            set;
        }

        public List<AvailablityModelClass> availableslotList
        {
            get;
            set;
        }
    }
}
