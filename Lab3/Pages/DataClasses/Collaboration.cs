namespace Lab3.Pages.DataClasses
{
    public class CollabClass
    {
        public int? CollabID { get; set; }
        public string Name { get; set; }
        public List<Chat>? Chats { get; set; }
        public int? UserID { get; set; }
        public int? KnowledgeID { get; set; }
        public int? PlanID { get; set; }
    }
}
