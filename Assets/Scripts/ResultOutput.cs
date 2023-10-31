using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Scripting;

namespace Assets.Scripts
{
    [Preserve]
    internal class ResultOutput
    {
        private List<ResultRecord> _records;

        public ResultOutput()
        {
            _records = new List<ResultRecord>();
        }

        public ResultOutput(List<ResultRecord> records)
        {
            _records = records;
        }

        public List<ResultRecord> GetRecords()
        {
            return _records;
        }

        public void AddRecord(ResultRecord record)
        {
            _records.Add(record);
        }

        public string ToCSV()
        {
            StringBuilder sb = new StringBuilder();

            // Write the header
            sb.AppendLine(ResultRecord.GetHeader());
            foreach (ResultRecord record in _records)
            {
                sb.AppendLine(record.ToCSV());
            }
            return sb.ToString();
        }
    }
}
