using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Runtime.Serialization;

namespace Wavelength.TestClasses
{
    [DataContract]
    public class Person
    {
        public Person() { }
        public Person(string firstname, string lastname)
        {
            this.FirstName = firstname;
            this.LastName = lastname;
        }
        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }
    }
}
