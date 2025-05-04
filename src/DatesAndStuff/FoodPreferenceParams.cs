using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatesAndStuff
{
    /// <summary>
    /// DTO like field.
    /// </summary>
    public class FoodPreferenceParams
    {
        public bool CanEatGluten { get; set; }

        public bool CanEatLactose { get; set; }

        public bool CanEatEgg { get; set; }

        public bool CanEatChocolate { get; set; }
    }
}
