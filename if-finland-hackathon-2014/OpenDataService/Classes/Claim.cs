using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace OpenDataService.Classes {

    [DataContract]
    public class Claim {

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public Product Product { get; set; }

        [DataMember]
        public string ClaimType { get; set; }

        [DataMember]
        public DateTime Date { get; set; }

        [DataMember]
        public Gender Gender { get; set; }

        [DataMember]
        public int? Age { get; set; }

        [DataMember]
        public string Venue { get; set; }

        [DataMember]
        public string Situation { get; set; }

        [DataMember]
        public string RoadType { get; set; }

        [DataMember]
        public string MunicipalityClaim { get; set; }

        [DataMember]
        public string MunicipalityPolicyholder{ get; set; }

        [DataMember]
        public Coordinates Location { get; set; }

        [DataMember]
        public Car Car { get; set; }
    }

    [DataContract]
    public enum Product
    {
        [EnumMember]
        MTPL,
        [EnumMember]
        Casco
    }

    [DataContract]
    public enum Gender
    {
        [EnumMember]
        Male,
        [EnumMember]
        Female,
        [EnumMember]
        Undefined
    }


}