using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JWTTest.JWT
{
    public class JWTModel
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int DurationInDays { get; set; }
    }
}
