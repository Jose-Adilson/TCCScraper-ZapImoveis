
namespace ConsoleApp1
{
    public class Imovel
    {
        public string id { get; set; }
        public string title { get; set; }
        public string address { get; set; }
        public string price { get; set; }
        public string rooms { get; set; }
        public string desc { get; set; }
        public List<string> images { get; set; }
        public string map { get; set; }

        public Imovel(string id, string title, string address, string price, string rooms, string desc, List<string> images, string map)
        {
            this.id = id;
            this.title = title;
            this.address = address;
            this.price = price;
            this.rooms = rooms;
            this.desc = desc;
            this.images = images;
            this.map = map;
        }
    }
}
