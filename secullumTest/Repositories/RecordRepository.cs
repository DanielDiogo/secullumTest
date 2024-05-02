using Npgsql;
using secullumTest.infrastructure;
using secullumTest.Models;

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

            using (var command = new NpgsqlCommand(@"WITH ranked_records AS
              (SELECT date, hour, employee, LAG(hour)
              OVER (PARTITION BY employee, date ORDER BY hour)
              AS previous_hour FROM public.record
              WHERE employee = @Employee and TO_CHAR(date, 'MM/YYYY') = @MonthNumber),
              time_diffs AS
             (SELECT date, hour, employee, CASE WHEN previous_hour IS NULL THEN '00:00:00'::interval ELSE
             (hour - previous_hour) END AS time_diff
             FROM ranked_records)
             SELECT employee, TO_CHAR(date, 'MM/YYYY') AS month_year,
             (SUM(EXTRACT(HOUR FROM time_diff)) + SUM(EXTRACT(MINUTE FROM time_diff)) / 60)::int ||
             ':' || TO_CHAR(SUM(EXTRACT(MINUTE FROM time_diff)) % 60, 'FM00')
             AS total_time FROM time_diffs GROUP BY employee, TO_CHAR(date, 'MM/YYYY')
             ORDER BY employee, TO_CHAR(date, 'MM/YYYY');", _dbConnection.Connection)) {
                command.Parameters.AddWithValue("@Employee", record.Employee);
                command.Parameters.AddWithValue("@MonthNumber", record.MonthNumber);

                using (var reader = command.ExecuteReader()) {
                    while (reader.Read()) {
                        record = new Record {
                            Employee = reader.GetString(0),
                            MonthNumber = reader.GetString(1),
                            TotalTime = reader.GetString(2)
                        };
                    }
                }
            }

            return record;
        }

        public IEnumerable<Record> GetRecords() {
            List<Record> records = new List<Record>();

            using (var command = new NpgsqlCommand("SELECT * FROM Record", _dbConnection.Connection)) {
                using (var reader = command.ExecuteReader()) {
                    while (reader.Read()) {
                        records.Add(new Record {
                            Id = reader.GetInt32(0),
                            Date = reader.GetDateTime(1).ToString("yyyy-MM-dd"),
                            Hour = !reader.IsDBNull(2) ? reader.GetTimeSpan(2).ToString(@"hh\:mm\:ss") : null,
                            Employee = !reader.IsDBNull(3) ? reader.GetString(3) : null
                        });
                    }
                }
            }

            return records;
        }

        public IEnumerable<object> GetTotalHoursPerEmployeePerDay() {
            List<object> result = new List<object>();

            using (var command = new NpgsqlCommand(@"WITH ranked_records AS (
            SELECT
            id,
            date,
            hour,
            employee,
            LAG(hour) OVER (PARTITION BY employee, date ORDER BY hour) AS previous_hour
            FROM
            public.record
            ),
            time_diffs AS (
            SELECT
            id,
            date,
            hour,
            employee,
            CASE
            WHEN previous_hour IS NULL THEN '00:00:00'::interval
            ELSE (hour - previous_hour)
            END AS time_diff
            FROM
            ranked_records
            )
            SELECT
            employee,
            date,
            (SUM(EXTRACT(HOUR FROM time_diff)) + SUM(EXTRACT(MINUTE FROM time_diff)) / 60)::int || ':' ||
            TO_CHAR(SUM(EXTRACT(MINUTE FROM time_diff)) % 60, 'FM00') AS total_time
            FROM
            time_diffs
            GROUP BY
            employee, date
            ORDER BY
            employee, date;", _dbConnection.Connection)) {
                using (var reader = command.ExecuteReader()) {
                    while (reader.Read()) {
                        string totalTimeString = reader.GetString(2);
                        TimeSpan totalTime;
                        if (TimeSpan.TryParse(totalTimeString, out totalTime)) {
                            result.Add(new {
                                funcionario = reader.GetString(0),
                                data = reader.GetDateTime(1).ToString("yyyy-MM-dd"),
                                total = totalTime.ToString(@"hh\:mm")
                            });
                        }
                    }
                }
            }
            return result;
        }
    }
}