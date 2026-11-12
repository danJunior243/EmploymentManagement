namespace EmploymentManagement.BLL.Outils
{
    public static class OutilsDates
    {
        /// <summary>
        /// Compte les jours du lundi au vendredi entre deux dates, bornes incluses.
        /// Les jours fériés ne sont pas pris en compte.
        /// </summary>
        public static int CompterJoursOuvrables(DateTime debut, DateTime fin)
        {
            int total = 0;
            for (var jour = debut.Date; jour <= fin.Date; jour = jour.AddDays(1))
            {
                if (jour.DayOfWeek != DayOfWeek.Saturday && jour.DayOfWeek != DayOfWeek.Sunday)
                    total++;
            }
            return total;
        }
    }
}