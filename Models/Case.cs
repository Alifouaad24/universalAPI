using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Universal_server.Models;

public class Case
{
    [Key]
    public int CaseId { get; set; }

    [ForeignKey(nameof(Customer))]
    public int CustomerId { get; set; }
    public GlobalCustomer? Customer { get; set; }

    [ForeignKey(nameof(CarInfoTbl))]
    public int CarInfoTblId { get; set; }

    public CarInfoTbl? CarInfoTbl { get; set; }

    public List<caseImage>? caseImages { get; set; }
    public List<Case_Service>? Case_Services { get; set; }

    public DateOnly? Date { get; set; }
}