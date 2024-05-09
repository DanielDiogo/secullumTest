using Npgsql;
using secullumTest.infrastructure;
using secullumTest.Models;
using secullumTest.Services;

namespace secullumTest.Repositories {

    public class RecordRepository : IRecordRepository {
        private readonly DbConnection _dbConnection;

        public RecordRepository(DbConnection dbConnection) {
            _dbConnection = dbConnection;
        }

        public Record GetRecordByEmployee(Record record) {
            if (record == null) {
                return null;
            }
            var records = new List<Record>();

            using (var command = new NpgsqlCommand(@"SELECT * FROM Record WHERE
             Employee = @Employee AND TO_CHAR(date, 'MM/YYYY') = @MonthNumber", _dbConnection.Connection)) {
                command.Parameters.AddWithValue("@Employee", record.Employee);
                command.Parameters.AddWithValue("@MonthNumber", record.MonthNumber);

                using (var reader = command.ExecuteReader()) {
                    while (reader.Read()) {
                        records.Add(new Record(reader.GetString(3),
                        reader.GetString(1),
                        !reader.IsDBNull(2) ? reader.GetTimeSpan(2).ToString(@"hh\:mm\:ss") : null
                   ));
                        records.Add(record);
                    }
                }
            }

            record.Hour = RecordProcessor.CalculateTotalHoursPerEmployeePerMonth(records).ToString();
            return record;
        }

        public IEnumerable<Record> GetRecords() {
            List<Record> records = new List<Record>();

            using (var command = new NpgsqlCommand("SELECT * FROM Record", _dbConnection.Connection)) {
                using (var reader = command.ExecuteReader()) {
                    while (reader.Read()) {
                        records.Add(new Record(reader.GetString(3),
                             reader.GetString(1),
                             !reader.IsDBNull(2) ? reader.GetTimeSpan(2).ToString(@"hh\:mm\:ss") : null
                        ));
                    }
                }
            }

            return records;
        }

        public IEnumerable<object> GetTotalHoursPerEmployeePerDay() {
            List<Record> records = new List<Record>(); 

            using (var command = new NpgsqlCommand("SELECT * FROM Record", _dbConnection.Connection)) {
                using (var reader = command.ExecuteReader()) {
                    while (reader.Read()) {
                        records.Add(new Record(reader.GetString(3),
                             reader.GetDateTime(1).ToString(),
                             !reader.IsDBNull(2) ? reader.GetTimeSpan(2).ToString(@"hh\:mm\:ss") : null
                        ));
                    }
                }
            }

            return RecordProcessor.CalculateTotalHoursPerEmployeePerDay(records);
        }
    }
}