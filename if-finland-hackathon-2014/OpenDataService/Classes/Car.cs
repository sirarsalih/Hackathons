using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace OpenDataService.Classes {

    [DataContract]
    public class Car {

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int CylinderVolume { get; set; }

        [DataMember]
        public int NumberOfSeats { get; set; }

        [DataMember]
        public string Color { get; set; }

        [DataMember]
        public int Length { get; set; }

        [DataMember]
        public int Width { get; set; }

        [DataMember]
        public int DistanceBetweenAxes { get; set; }

        [DataMember]
        public int Kilowatts { get; set; }

        [DataMember]
        public FuelType FuelType { get; set; }

        [DataMember]
        public int Weight { get; set; }

        [DataMember]
        public DateTime FirstUseDate { get; set; }

        [DataMember]
        public int Brand { get; set; }
    }

    [DataContract]
    public enum FuelType {
        [EnumMember]
        Benzine,
        [EnumMember]
        Diesel,
        [EnumMember]
        Undefined
    }
}