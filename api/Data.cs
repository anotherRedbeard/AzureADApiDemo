using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api
{
    public class Data
    {
        public static List<NameModel> NamesList = new List<NameModel>
        {
            new NameModel{FirstName="Adam",LastName="King"},
            new NameModel{FirstName="Robert",LastName="William"}
        };
    }
}