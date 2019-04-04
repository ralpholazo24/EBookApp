using SQLite; 

namespace EbookApp.Model
{
    public class QuestionItem
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public string Genre { get; set; }
        public string Title { get; set; }
        public string QOne { get; set; }
        public string QTwo { get; set; }
        public string QThree { get; set; }
    }
}
