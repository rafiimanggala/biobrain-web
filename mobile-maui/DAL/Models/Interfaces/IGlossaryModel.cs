namespace DAL.Models.Interfaces
{
    public interface IGlossaryModel
    {
         int TermID { get; set; }

        string Ref { get; set; }

        string Term { get; set; }

        string Definition { get; set; }

        string Header { get; set; }
    }
}