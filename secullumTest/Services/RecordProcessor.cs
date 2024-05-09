using secullumTest.Models;

namespace secullumTest.Services {

    public class RecordProcessor {

        public static List<EmployeeDayHours> CalculateTotalHoursPerEmployeePerDay(List<Record> records) {
            var groupedRecords = records.GroupBy(record => new { record.Employee, record.Date }).ToList();

            var result = new List<EmployeeDayHours>();

            foreach (var group in groupedRecords) {
                string employee = group.Key.Employee;

                TimeSpan totalHoursForDay = TimeSpan.Zero;

                for (int i = 0; i < group.Count(); i += 2) {
                    var checkInRecord = group.ElementAt(i);
                    var checkOutRecord = i + 1 < group.Count() ? group.ElementAt(i + 1) : null;

                    if (checkOutRecord != null) {
                        TimeSpan workHours = TimeSpan.Parse(checkOutRecord.Hour)
                            - TimeSpan.Parse(checkInRecord.Hour);
                        totalHoursForDay += workHours;
                    }
                }

                var employeeDayHours = new EmployeeDayHours {
                    Employee = employee,
                    Date = group.Key.Date,
                    TotalHours = totalHoursForDay
                };

                result.Add(employeeDayHours);
            }

            return result;
        }

        public static string CalculateTotalHoursPerEmployeePerMonth(List<Record> records) {
            var groupedRecords = records.GroupBy(r => new { r.Employee, r.Date });

            var totalHoursPerEmployee = new Dictionary<string, TimeSpan>();

            foreach (var group in groupedRecords) {
                string employee = group.Key.Employee;

                TimeSpan totalHoursForDay = TimeSpan.Zero;

                for (int i = 0; i < group.Count(); i += 2) {
                    var checkInRecord = group.ElementAt(i);
                    var checkOutRecord = i + 1 < group.Count() ? group.ElementAt(i + 1) : null;

                    if (checkOutRecord != null) {
                        TimeSpan workHours = TimeSpan.Parse(checkOutRecord.Hour)
                            - TimeSpan.Parse(checkInRecord.Hour);
                        totalHoursForDay += workHours;
                    }
                }

                if (!totalHoursPerEmployee.ContainsKey(employee)) {
                    totalHoursPerEmployee[employee] = TimeSpan.Zero;
                }
                totalHoursPerEmployee[employee] += totalHoursForDay;
            }

            TimeSpan totalHours = TimeSpan.Zero;
            foreach (var hours in totalHoursPerEmployee.Values) {
                totalHours += hours;
            }

            return $"{(int)totalHours.TotalHours}:{totalHours.Minutes}";
        }
    }
}