using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoMicroSQL.Models
{
    public class Info : IComparable
    {
        public int id { get; set; }

        //public int[] Ints { get; set; }
        public List<int> ListaInt { get; set; }

        public Dictionary<string, List<object>> Lista { get; set; }
        

        //public string[] strings { get; set; }
        public List<string> ListaChar { get; set; }

        //public DateTime[] dts { get; set; }
        public List<DateTime> ListaDT { get; set; }

        public Info()
        {
            //Ints = new int[3];
            //chars = new string[3];
            //dts = new DateTime[3];
            ListaChar = new List<string>();
            ListaDT = new List<DateTime>();
            ListaInt = new List<int>();
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int CompareTo(object obj)
        {
            var comparable = (Info)obj;
            return id.CompareTo(comparable.id);
        }
    }
}