using EmploymentManagement.Data.Entities;

namespace EmploymentManagementSystem.Models
{
    public class PaieIndexViewModel
    {
        public int Annee { get; set; }
        public int Mois { get; set; }
        public List<Paie> Paies { get; set; } = new();

        public decimal TotalBrut => Paies.Sum(p => p.SalaireBrut);
        public decimal TotalDeductions => Paies.Sum(p => p.Deductions);
        public decimal TotalNet => Paies.Sum(p => p.NetAPayer);
    }
}
