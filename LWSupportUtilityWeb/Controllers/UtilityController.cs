using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using ADODB;
using System.Data.OleDb;
using System.Data;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

namespace LWSupportUtilityWeb.Controllers
{
    public class UtilityController : Controller
    {
        //
        // GET: /Utility/

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [ActionName("TagListEditor")]
        public ActionResult TagListEditor()
        {
            return View();
        }

        [HttpPost]
        [ActionName("TagListEditor")]
        public ActionResult TagListEditor_Post(HttpPostedFileBase fileupload)
        {
            OleDbDataAdapter _odbAdapter = new OleDbDataAdapter();
            DataTable _dtable = new DataTable();

            if (fileupload != null && fileupload.ContentLength > 0)
            {
                string _filename = Path.GetFileName(fileupload.FileName);
                string _fileExtension = Path.GetExtension(fileupload.FileName);
                string _path = Path.Combine(Server.MapPath("~/App_Data"), _filename);
                string _newFile = Path.Combine(Server.MapPath("~/App_Data"), _filename.Replace(_fileExtension,".txt"));

                if (_fileExtension == ".rs")
                {
                    fileupload.SaveAs(_path); fileupload.SaveAs(_newFile); 

                    Recordset _recordset = new Recordset();

                    _recordset.Open(_path);

                    _odbAdapter.Fill(_dtable, _recordset);

                    _dtable.Columns.Add("Actions",typeof(string));

                    //foreach (DataRow dt in _dtable.Rows)
                    //{
                    //    dt["Actions"] = "";
                    //}
                    //_dtable.AcceptChanges();

                    StreamWriter _sWriter = new StreamWriter(_newFile, false);

                    foreach (DataRow drow in _dtable.Rows)
                    {
                        for (int i = 0; i < _dtable.Columns.Count; i++)
                        {
                            if (!Convert.IsDBNull(drow[i]))
                            {
                                if (i == 1)
                                {
                                    _sWriter.Write("|" + drow[i].ToString());
                                }
                                else
                                {
                                    _sWriter.Write(drow[i].ToString());
                                }
                                
                            }
                        }
                        _sWriter.Write(_sWriter.NewLine);
                    }
                    _sWriter.Close();
                }
            }
            
            return View(_dtable);
        }

        public FileResult TaglistDownload()
        {
            string _DownloadFile = Path.Combine(Server.MapPath("~/App_Data"),"taglist.txt");
                
            return File(_DownloadFile, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(_DownloadFile));
        }

        private DataSet ConvertHTMLTablesToDataSet(string HTML)
        {
                DataSet _ds = new DataSet();
                DataTable _dt = null;
                DataRow _dr = null;

                string TableExpression = "<table[^>]*(.*?)</table>";
                string HeaderExpression = "<th[^>]*>(.*?)</th>";
                string RowExpression = "<tr[^>]*>(.*?)</tr>";
                string ColumnExpression = "<td[^>]*>(.*?)</td>";
                bool HeadersExist = false;
                int iCurrentColumn = 0;
                int iCurrentRow = 0;

                MatchCollection Tables = Regex.Matches(HTML, TableExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase);

                // Loop through each table element    
            foreach (Match Table in Tables)
            {

                // Reset the current row counter and the header flag    
                iCurrentRow = 0;
                HeadersExist = false;

                // Add a new table to the DataSet    
                _dt = new DataTable();

                // Create the relevant amount of columns for this table (use the headers if they exist, otherwise use default names)    
                if (Table.Value.Contains("<th"))
                {
                    // Set the HeadersExist flag    
                    HeadersExist = true;

                    // Get a match for all the rows in the table    
                    MatchCollection Headers = Regex.Matches(Table.Value, HeaderExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase);

                    // Loop through each header element    
                    foreach (Match Header in Headers)
                    {
                        if (!_dt.Columns.Contains(Header.Groups[1].ToString()))
                            _dt.Columns.Add(Header.Groups[1].ToString().Replace("&nbsp;", ""));
                    }
                }
                else
                {
                    for (int iColumns = 1; iColumns <= Regex.Matches(Regex.Matches(Regex.Matches(Table.Value, TableExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase).ToString(), RowExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase).ToString(), ColumnExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase).Count; iColumns++)
                    {
                        _dt.Columns.Add("Column " + iColumns);
                    }
                }

                // Get a match for all the rows in the table    
                MatchCollection Rows = Regex.Matches(Table.Value, RowExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase);

                // Loop through each row element    
                foreach (Match Row in Rows)
                {

                    // Only loop through the row if it isn't a header row    
                    if (!(iCurrentRow == 0 & HeadersExist == true))
                    {

                        // Create a new row and reset the current column counter    
                        _dr = _dt.NewRow();
                        iCurrentColumn = 0;

                        // Get a match for all the columns in the row    
                        MatchCollection Columns = Regex.Matches(Row.Value, ColumnExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase);

                        // Loop through each column element    
                        foreach (Match Column in Columns)
                        {
                            if(Columns.Count -1!=iCurrentColumn)
                            // Add the value to the DataRow    
                                _dr[iCurrentColumn] = Convert.ToString(Column.Groups[1]).Replace("&nbsp;", "");

                            // Increase the current column     
                            iCurrentColumn += 1;
                        }

                        // Add the DataRow to the DataTable    
                        _dt.Rows.Add(_dr);

                    }

                    // Increase the current row counter    
                    iCurrentRow += 1;
                }

                // Add the DataTable to the DataSet    
                _ds.Tables.Add(_dt);

            }

            return _ds;
        }

        public ActionResult LWSupportCalendar()
        {
            return View();
        }

    }
}
