using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoMicroSQL.Models
{
    public class Info : IComparable
    {
        public int PrimaryKey { get; set; }
        public int[] Int1 { get; set; }
        public List<int> ListaInt { get; set; }

        public char[] char1 { get; set; }
        public List<char> ListaChar { get; set; }

        public DateTime[] dt1 { get; set; }
        public List<DateTime> ListaDT { get; set; }

        public Info()
        {
            Int1 = new int[3];
            char1 = new char[3];
            dt1 = new DateTime[3];
            ListaChar = new List<char>();
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
            return PrimaryKey.CompareTo(comparable.PrimaryKey);
        }
    }
}