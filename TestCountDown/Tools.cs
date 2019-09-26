using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TestCountDown {
    class Tools {
        public static void fillTimeDDLs(ComboBox ddlHour, ComboBox ddlMinutes, DateTime selectedTime) {
            // add hours to the form
            Tools.FillTimeDDL(ddlHour, 0, 24, selectedTime.Hour);

            // add minutes to the form
            Tools.FillTimeDDL(ddlMinutes, 0, 60, selectedTime.Minute, "00");
        }
        private static void FillTimeDDL(ComboBox ddl, int start, int end, int selected, string format = "0") {
            string formatter = "{0:" + format + "}";
            for (int h = start; h < end; h++)
                ddl.Items.Add(String.Format(formatter,h));
            ddl.SelectedIndex = selected;
        }
    }
}