using CsvHelper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Windows.Forms.DataVisualization;
using System.Windows.Forms.DataVisualization.Charting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Data;
using Newtonsoft.Json;
using System.Xml;
using System.Reflection;

namespace Ventas_ACT4
{
    public partial class Form1 : Form
    {
        private DataTable dataTable = new DataTable();
        private Dictionary<string, int> ventasPorCategoria = new Dictionary<string, int>();




        public Form1()
        {
            InitializeComponent();
            ConfigurarChart();

        }
        private void ConfigurarChart()
        {
            
                // Configurar Chart
                chart1.ChartAreas.Add("ChartArea");
                chart1.Series.Add("VentasPorCategoria");
                chart1.Series["VentasPorCategoria"].ChartType = SeriesChartType.Column;
                chart1.Titles.Add("Ventas por Categoría");

                // Establecer un tamaño personalizado
                chart1.Width = 600;
                chart1.Height = 400;
            

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            // Configurar DataGridView
            dataGridView1.DataSource = dataTable;

            dataGridView1.Visible = false;
        chart1.Visible = false;
            // Configurar gráfica (puedes usar un control Chart)
          
            dataGridView1.Refresh();
        }

       

        private void btnCargarArchivo_Click(object sender, EventArgs e)
        {

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Archivos CSV (*.csv)|*.csv|Archivos XML (*.xml)|*.xml|Archivos JSON (*.json)|*.json|Todos los archivos (*.*)|*.*";
                openFileDialog.Multiselect = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (string filePath in openFileDialog.FileNames)
                    {
                        ProcesarArchivo(filePath);
                    }
                }
                MostrarDatosEnGrafica(); // Llamada al método después de cargar los datos

            }
        }
        private void ProcesarArchivo(string filePath)
        {
            string extension = Path.GetExtension(filePath);

            switch (extension.ToLower())
            {
                case ".csv":
                    CargarCSV(filePath);
                    break;
                case ".xml":
                    CargarXML(filePath);
                    break;
                case ".json":
                    CargarJSON(filePath);
                    break;
                default:
                    MessageBox.Show("Formato de archivo no compatible: " + extension, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        private void CargarCSV(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);

            if (lines.Length > 0)
            {
                string[] headers = lines[0].Split(',');

                for (int i = 1; i < lines.Length; i++)
                {
                    string[] data = lines[i].Split(',');

                    // Asignar nombres de columnas basados en la primera línea
                    if (dataTable.Columns.Count == 0)
                    {
                        foreach (string header in headers)
                        {
                            dataTable.Columns.Add(header);
                        }
                    }

                    // Agregar datos al DataTable
                    dataTable.Rows.Add(data);

                    // Actualizar ventasPorCategoria
                    string categoria = data[1]; // Suponiendo que la categoría está en la segunda columna
                    int ventas = int.Parse(data[3]); // Suponiendo que las ventas están en la cuarta columna

                    if (ventasPorCategoria.ContainsKey(categoria))
                    {
                        ventasPorCategoria[categoria] += ventas;
                    }
                    else
                    {
                        ventasPorCategoria.Add(categoria, ventas);
                    }
                }
            }
        }


        private void CargarXML(string filePath)
        {
            // Lógica para cargar datos XML en el DataTable
            // Ejemplo básico:
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(filePath);

            // Aquí debes adaptar la lógica según la estructura de tu XML
            // Puedes usar XPath para seleccionar nodos específicos
        }


        public void CargarJSON(string filePath)
        {
            try
            {
                // Lógica para cargar datos JSON en el DataTable
                string jsonContent = File.ReadAllText(filePath);
                List<EstructuraDeJson> data = JsonConvert.DeserializeObject<List<EstructuraDeJson>>(jsonContent);

                if (data.Count > 0)
                {
                    dataTable.Clear();
                    dataTable.Columns.Clear();

                    // Asignar nombres de columnas y tipos basados en propiedades de TuClase
                    PropertyInfo[] propiedades = typeof(EstructuraDeJson).GetProperties();

                    foreach (PropertyInfo propiedad in propiedades)
                    {
                        dataTable.Columns.Add(propiedad.Name, propiedad.PropertyType);
                    }

                    // Agregar datos al DataTable
                    foreach (EstructuraDeJson item in data)
                    {
                        DataRow row = dataTable.NewRow();

                        foreach (PropertyInfo propiedad in propiedades)
                        {
                            row[propiedad.Name] = propiedad.GetValue(item);
                        }

                        dataTable.Rows.Add(row);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar datos desde JSON: {ex.Message}");
            }
        }

        private void MostrarDatosEnGrafica()
        {
            // Limpiar datos anteriores en el gráfico
            chart1.Series["VentasPorCategoria"].Points.Clear();

            // Limpiar títulos existentes antes de agregar uno nuevo
            chart1.Titles.Clear();

            // Agregar un nuevo título
            Title chartTitle = new Title("Ventas por Categoría");
            chartTitle.Font = new Font("Arial", 16, FontStyle.Bold); // Aumenta el tamaño del título
            chart1.Titles.Add(chartTitle);

            // Configurar la gráfica
            chart1.Series["VentasPorCategoria"].IsValueShownAsLabel = true;

            foreach (var kvp in ventasPorCategoria)
            {
                // Agregar puntos con colores distintos para cada categoría
                chart1.Series["VentasPorCategoria"].Points.AddXY(kvp.Key, kvp.Value);
                chart1.Series["VentasPorCategoria"].Points.Last().Color = ObtenerColorPorCategoria(kvp.Key);
                chart1.Series["VentasPorCategoria"].Points.Last().Label = $"{kvp.Key}\n{String.Format("{0:C}", kvp.Value)}"; // Incluir el nombre de la categoría en la etiqueta
            }
        }



        private Color ObtenerColorPorCategoria(string categoria)
        {
            // Asignar colores diferentes según la categoría
            switch (categoria)
            {
                case "Moda":
                    return Color.Blue;
                case "Calzado":
                    return Color.Green;
                case "Accesorio":
                    return Color.Orange;
                case "Deporte":
                    return Color.Red;
                default:
                    return Color.Gray;
            }
        }
        private void EstablecerEstilosTabla()
        {
            // Iterar a través de las filas y aplicar estilos según ciertos criterios
            foreach (DataGridViewRow fila in dataGridView1.Rows)
            {
                // Supongamos que la columna 3 representa las ventas
                int ventas = Convert.ToInt32(fila.Cells[3].Value);

                // Establecer estilos para resaltar ventas altas
                if (ventas > 100)
                {
                    fila.DefaultCellStyle.BackColor = Color.LightGreen;  // Fondo de la fila
                    fila.DefaultCellStyle.ForeColor = Color.Black;       // Texto de la fila
                    fila.DefaultCellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold); // Texto en negrita
                }
            }
        }
        private void OrdenarTablaPorVentas()
        {
            if (dataGridView1.Columns.Count > 3)
            {
                dataGridView1.Sort(dataGridView1.Columns[3], ListSortDirection.Descending);
            }

        }
        private void ResaltarCeldas()
        {
            // Supongamos que la columna 3 representa las ventas
            foreach (DataGridViewRow fila in dataGridView1.Rows)
            {
                int ventas = Convert.ToInt32(fila.Cells[3].Value);

                // Resaltar la celda si las ventas son mayores a 100
                if (ventas > 100)
                {
                    fila.Cells[3].Style.BackColor = Color.LightGreen;
                    fila.Cells[3].Style.ForeColor = Color.Black;
                    fila.Cells[3].Style.Font = new Font(dataGridView1.Font, FontStyle.Bold);
                }
            }
        }



        private void btnShowTable_Click(object sender, EventArgs e)
        {
            EstablecerEstilosTabla();
            ResaltarCeldas();
            OrdenarTablaPorVentas();
            dataGridView1.Refresh();

            chart1.Visible = false;

            dataGridView1.Visible = true;
       
        }

        private void btnShowGraph_Click(object sender, EventArgs e)
        {
            dataGridView1.Visible = false;

            chart1.Visible = true;

        }
    }
}
public class EstructuraDeJson
{
    // Define las propiedades según la estructura de tus datos JSON
    public string Nombre { get; set; }
    public string Categoria { get; set; }

    public int Precio { get; set; } 
    public int Ventas { get; set; }
}