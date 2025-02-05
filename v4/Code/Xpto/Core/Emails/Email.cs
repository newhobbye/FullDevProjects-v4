﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xpto.Core.Emails
{
    public class Email
    {
        public Guid Id { get; set; }
        public int CustomerCode { get; set; } 
        public string Type { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }

        public Email()
        {
            Id = Guid.NewGuid();
        }

        public override string ToString()
        {
            return Address;
        }
    }
}
