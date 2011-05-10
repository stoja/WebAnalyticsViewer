using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Data.Sql;

namespace WebAnalyticsViewer
{
    public class Helper_Data
    {
        private SqlConnection connection;
        private SqlDataAdapter _sqlDataAdapter;

        public Helper_Data()
        {

        }

        public void EstablishConnection()
        {
            connection = new SqlConnection("Data Source=changeme;Initial Catalog=WebAnalytics;User ID=changeme;Password=changeme;");
            connection.Open();
        }

        public DataSet ReturnDataset()
        {
            _sqlDataAdapter = new SqlDataAdapter();
            DataSet data = new DataSet();
            SqlCommand command = connection.CreateCommand();
            command.CommandText =
                @"SELECT  res.time_stamp,def.host ,def.page_name, def.referrer, def.var_name, res.actual_result, bas.expected_value, def.id, res.id, bas.id, res.time_stamp, bas.time_stamp
                    FROM dbo.Result res
                    JOIN dbo.Definition def ON def.id = res.definition_id 
                    LEFT OUTER JOIN dbo.Baseline bas ON bas.result_id = res.id
                    ORDER BY def.host, def.page_name, def.referrer";
            _sqlDataAdapter.SelectCommand = command;
            _sqlDataAdapter.Fill(data);

            connection.Close();
            return data;
        }

        public void UpdateRowExpValue(int? baseline_id, int? result_id, int? definition_id, string value)
        {
            connection.Open();
            string updateSQL;
            if (!baseline_id.Equals(null))
                updateSQL = "UPDATE baseline SET expected_value = '" + value + "', time_stamp = '" + DateTime.Now + "' WHERE id = '" + baseline_id + "'";
            else
                updateSQL = "INSERT INTO baseline ( result_id, definition_id, expected_value, time_stamp ) VALUES ('" + result_id + "', '" + definition_id + "', '" + value + "', '" + DateTime.Now + "')";

            SqlCommand command = connection.CreateCommand();
            command.CommandText = updateSQL;
            command.ExecuteReader();
            //command.ExecuteNonQuery();
            connection.Close();
        }

    }
}
