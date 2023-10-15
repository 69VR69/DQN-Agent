using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public class Matrix<T> : List<List<T>>
    {
        public override string ToString()
        {
            StringBuilder sb = new(); // row separated by ; and col separated by ,
            foreach (var row in this)
            {
                sb.Append(string.Join(",", row));
                sb.Append(";");
            }
            return sb.ToString();
        }
    }
}
