using System;
using System.Collections.Generic;
using System.Text;

namespace Weather.App.Models
{
    public class BackgroundInfo
    {
        public int Total_Results { get; set; }
        public int Page { get; set; }
        public int Per_Page { get; set; }
        public Photo[] Photos { get; set; }
        public string Next_Page { get; set; }
    }

    public class Photo
    {
        public int Id { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Url { get; set; }
        public string Photographer { get; set; }
        public string Photographer_Url { get; set; }
        public int Photographer_Id { get; set; }
        public Src Src { get; set; }
        public bool Liked { get; set; }
    }

    public class Src
    {
        public string Original { get; set; }
        public string Large2x { get; set; }
        public string Large { get; set; }
        public string Medium { get; set; }
        public string Small { get; set; }
        public string Portrait { get; set; }
        public string Landscape { get; set; }
        public string Tiny { get; set; }
    }
}
