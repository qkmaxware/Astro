using System.IO;
using System.Xml;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Text;

namespace Qkmaxware.Astro.IO {

/// <summary>
/// VO Table
/// </summary>
public class VOTable {
    /// <summary>
    /// Name of the table
    /// </summary>
    /// <value>name</value>
    public string TableName {get; private set;}
    /// <summary>
    /// Fields in the table
    /// </summary>
    /// <returns>field names</returns>
    public List<string> Fields {get; private set;} = new List<string>();
    /// <summary>
    /// Rows
    /// </summary>
    /// <returns>rows</returns>
    private List<Dictionary<string, string>> Rows = new List<Dictionary<string, string>>();
    /// <summary>
    /// Total number of rows
    /// </summary>
    public int RowCount => Rows.Count;

    public VOTable(string name) {
        this.TableName = name;
    }

    public Dictionary<string,string> this[int row] => Rows[row];
    public string this[int row, string column] {
        get {
            var data = this[row];
            string value = string.Empty;
            if (data.TryGetValue(column, out value)) {
                return value;
            } else {
                return value;
            }
        }
    }
    public string this[int row, int column] => Rows[row][Fields[column]];

    public string SelectFirstNonEmpty(int row, params string[] columns) {
        int i = 0;
        string str = string.Empty;
        while ( i < columns.Length && string.IsNullOrEmpty(str = this[row, columns[i]]) ) {
            i++;
        }
        return str;
    }

    public void AddRow(Dictionary<string, string> row) {
        this.Rows.Add(row);
    }
}

/// <summary>
/// File containing multiple VO Tables
/// </summary>
public class VOTableFile {
    /// <summary>
    /// Coordinate system for all VO Tables
    /// </summary>
    /// <value>coordinate system</value>
    public string CoordinateSystem {get; private set;}
    /// <summary>
    /// Equinox for all VO Tables
    /// </summary>
    /// <value>equinox</value>
    public string Equinox {get; private set;}
    /// <summary>
    /// Epoch for all VO Tables
    /// </summary>
    /// <value>epoch</value>
    public string Epoch {get; private set;}

    /// <summary>
    /// VO Tables within this file
    /// </summary>
    /// <value>tables</value>
    public VOTable[] Tables {get; private set;}

    /// <summary>
    /// First VO Table
    /// </summary>
    /// <returns>first table if it exists</returns>
    public VOTable FirstTable => Tables.First();

    public VOTableFile(string system, string equinox, string epoch, VOTable[] tables) {
        this.CoordinateSystem = system;
        this.Epoch = epoch;
        this.Equinox = equinox;
        this.Tables = tables;
    }
}

/// <summary>
/// Class to deserialize data from XML formatted VO Table files
/// </summary>
public class VOTableDeserializer {

    public VOTableFile Deserialize(TextReader reader) {
        var xmlReader = new XmlTextReader(reader);
        xmlReader.Namespaces = false;
        XmlDocument doc = new XmlDocument();
        doc.Load(xmlReader);

        if (doc.DocumentElement?.Name != "VOTABLE") {
            throw new ArgumentException("missing VOTABLE root element");
        }

        var raw_coordinate_system = doc.DocumentElement.SelectSingleNode("/VOTABLE/DEFINITIONS/COOSYS");
        if (raw_coordinate_system == null) {
            throw new ArgumentException("missing COOSYS element");
        }
        var equinox = raw_coordinate_system.Attributes.GetNamedItem("equinox").Value;
        var epoch = raw_coordinate_system.Attributes.GetNamedItem("epoch").Value;
        var system = raw_coordinate_system.Attributes.GetNamedItem("system").Value;

        List<VOTable> tableList = new List<VOTable>();
        var tables = doc.SelectNodes("/VOTABLE/RESOURCE/TABLE");
        if (tables == null) {
            throw new ArgumentException("missing TABLE element");
        }

        foreach (XmlNode table in tables) {
            var currentTable = new VOTable(table.Attributes.GetNamedItem("name")?.Value ?? string.Empty);
            
            // Create fields
            var raw_fields = table.SelectNodes("FIELD");
            foreach (XmlNode field in raw_fields) {
                var name = field.Attributes.GetNamedItem("name")?.Value;
                if (name != null) {
                    currentTable.Fields.Add(name);
                }
            }
            // Create rows
            var raw_rows = table.SelectNodes("DATA/TABLEDATA/TR");
            if (raw_rows != null) {
                for (var rowid = 0; rowid < raw_rows.Count; rowid++) {
                    var data = new Dictionary<string,string>();
                    var row = raw_rows.Item(rowid);
                    var cells = row.SelectNodes("TD");

                    for (var cellid = 0; cellid < Math.Min(cells.Count, currentTable.Fields.Count); cellid++) {
                        var cell = cells.Item(cellid);
                        data[currentTable.Fields[cellid]] = cell.InnerText;
                    }

                    currentTable.AddRow(data);
                }
            }

            tableList.Add(currentTable);
        }

        VOTableFile result = new VOTableFile(system, equinox, epoch, tableList.ToArray());
        return result;
    }

}

}