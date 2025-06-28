using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalonKosmetycznyApp.Model
{
    public class TreatmentRoomType
    {
        public static ObservableCollection<string> GetRoomTypeList()
        {
            return new ObservableCollection<string>
            {
                "Do twarzy",
                "Do ciała",
                "Do włosów",
                "Do dłoni",
                "Do stóp",
                "Do depilacji",
                "Do masażu"
            };
        }

        public static ObservableCollection<string> GetAvailabilityList()
        {
            return new ObservableCollection<string>
            {
                "Tak",
                "Nie"
            };
        }
    }
}
