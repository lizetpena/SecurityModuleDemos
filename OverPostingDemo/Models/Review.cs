namespace OverPostingDemo
{
    public class Review
    {
        public int ReviewID { get; set; } // PK
        public int PlaceID { get; set; } // FK
        public int NumberOfLikes { get; set; }
        public string UserName { get; set; }
        public string UserComment { get; set; }
    }
}
