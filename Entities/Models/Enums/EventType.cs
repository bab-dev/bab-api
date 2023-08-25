using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models.Enums
{
    public enum EventType
    {
        DELIVERY = 0,
        PICK_UP = 1,
        FOOD_SELECTION = 2,
        DISTRIBUTION_TO_FAMILIES = 3,
        CLEANING = 4,
        MEETING = 5,
        OTHER = 6
    }
}
