using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marvin.Model
{
    [AttributeUsage(AttributeTargets.Property)]
    public class JsonColumnAttribute : ColumnAttribute
    {
        public JsonColumnAttribute()
        {
            TypeName = "jsonb";
        }
    }
}
