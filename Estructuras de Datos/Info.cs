using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Estructuras_de_Datos
{
    public class Info : IComparable
    {

        public Dictionary<string, List<string>> Variables { get; set; } 
        public int ContadorInt { get; set; }
        public int ContadorChar { get; set; }
        public int ContadorDT { get; set; }
        public int Maximo { get; set; }
        public Info()
        {
            Variables = new Dictionary<string, List<string>>();
            ContadorChar = 0;
            ContadorDT = 0;
            ContadorInt = 0;
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int CompareTo(object obj)
        {
            var info = (Info)obj;
            return 1;
        }

        //public static Comparison<List<int>> CompareByID = delegate (List<int> p1, List<int> p2)
        //{
        //    return p1[0].CompareTo(p2[2]);
        //};
    }
}