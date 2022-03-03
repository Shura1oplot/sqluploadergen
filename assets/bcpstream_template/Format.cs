using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bcpstream
{

    [Table("{{table}}")]
    public class DataRow
    {
{{columns}}

        public DataRow(string line)
        {
            string[] values = line.Split('{{delimiter}}');

{{converters}}
        }
    }
}
