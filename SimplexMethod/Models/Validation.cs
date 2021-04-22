using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SimplexMethod.Models
{
    public class Validation:ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value != null && CheckValues(value))
                return true;

            return false;
        }
        public bool CheckValues(object value)
        {
           if( value is List<List<string>>)
            {
                List<List<string>> tmp = (List<List<string>>)value;
                foreach(var item in tmp)
                {
                    for(int i = 0; i < item.Count; i++)
                    {
                        if (!int.TryParse(item[i], out int res))
                        {
                            ErrorMessage = " Вводите только числа!";
                            return false;
                        }
                        if (item[i] != "0")
                            return true;
                    }
                }
            }
           if(value is List<string>)
            {

                List<string> tmp = (List<string>)value;
                foreach (var item in tmp)
                {
                    if (!int.TryParse(item, out int res))
                    {
                        ErrorMessage = " Вводите только числа!";
                        return false;
                    }
                        
                    if (item != "0")
                            return true;
                }
            }
            return false;
        }
    }
}
