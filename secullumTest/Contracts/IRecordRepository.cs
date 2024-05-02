using secullumTest.Models;

public interface IRecordRepository {

    Record GetRecordByEmployee(Record record);

    IEnumerable<Record> GetRecords();

    IEnumerable<object> GetTotalHoursPerEmployeePerDay();
}