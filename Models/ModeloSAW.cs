using Microsoft.Extensions.Options;
using System.Linq;
using System.Numerics;

namespace MetodoSaw.Models
{
    public class ModeloSAW
    {
        //Creación del modelo de datos a recibir
        public double[][] alternativas { get; set; }
        public int ren { get; set; }

        public int col { get; set; }
        public double[] pesos { get; set; }
        public string[] nombres { get; set; }
        public int[] tipo { get; set; }
        public string rutaArchivo = "Archivo.txt";

        public string[] resolver()
        {
            // Leer los datos desde el archivo
            LeerDatosDeArchivo(rutaArchivo);

            // Proceder con los cálculos si los datos fueron leídos
            if (alternativas != null && pesos != null && nombres != null && tipo != null)
            {
                double[][] matriz = normalizar();
                matriz = Asignarpesos(matriz);
                int[] indicesordenados = Ordenar(matriz);
                string[] opcionesordenadas = new string[nombres.Length];
                for (int i = 0; i < nombres.Length; i++)
                {
                    opcionesordenadas[i] = nombres[indicesordenados[i]];
                }

                // Guardar el resultado al final del archivo especificado
                GuardarAlEOF(rutaArchivo, opcionesordenadas);

                return opcionesordenadas;
            }
            else
            {
                Console.WriteLine("Error: Datos no cargados correctamente.");
                return null;
            }
        }


        public bool validarPesos()
        {
            bool valid = false;
            double total = 0;
            foreach (var p in pesos)
            {
                total += p;
            }
            if (total == 1 || total == 100)
            {
                valid = true;
            }
            return valid;
        }

        //Método para normalizar la matriz
        private double[][] normalizar()
        {
            double[][] matriznormalizada = alternativas;
            double[] valores = new double[tipo.Length];
            for (int i = 0; i < tipo.Length; i++)
            {
                if (tipo[i] == 1)
                {
                    valores[i] = ValorMayor(i);
                    matriznormalizada = Maximizar(i, matriznormalizada, valores[i]);
                }
                if (tipo[i] == 0)
                {
                    valores = ValorMenor();
                    matriznormalizada = Minimizar(i, matriznormalizada, valores[i]);
                }
            }
            return matriznormalizada;
        }

        private double ValorMayor(int pos)
        {
            double temp = alternativas[0][pos];
            for (int i = pos; i < pos + 1; i++)
            {
                for (int j = 0; j < ren; j++)
                {
                    if (alternativas[j][i] > temp)
                    {
                        temp = alternativas[j][i];
                    }
                }
            }
            return temp;
        }

        private double[] ValorMenor()
        {
            double[] valor = new double[col];
            double temp;
            for (int i = 0; i < col; i++)
            {
                temp = alternativas[0][i];
                for (int j = 0; j < ren; j++)
                {
                    if (alternativas[j][i] < temp)
                    {
                        temp = alternativas[j][i];
                    }
                }
                valor[i] = temp;
            }
            return valor;
        }

        private double[][] Maximizar(int pos, double[][] matriz, double val)
        {
            for (int j = pos; j < (pos + 1); j++)
            {
                for (int k = 0; k < ren; k++)
                {
                    matriz[k][j] = alternativas[k][j] / val;
                }
            }
            return matriz;
        }

        private double[][] Minimizar(int pos, double[][] matriz, double val)
        {
            for (int j = pos; j < (pos + 1); j++)
            {
                for (int k = 0; k < ren; k++)
                {
                    matriz[k][j] = val / alternativas[k][j];
                }
            }
            return matriz;
        }

        private double[][] Asignarpesos(double[][] matriz)
        {
            for (int i = 0; i < matriz.GetLength(0); i++)
            {
                for (int j = 0; j < col; j++)
                {
                    matriz[i][j] *= pesos[j];
                }
            }
            return matriz;
        }

        private int[] Ordenar(double[][] matriz)
        {
            double[] sumas = new double[nombres.Length];

            for (int i = 0; i < ren; i++) //recorres las columnas
            {
                double total = 0;
                for (int j = 0; j < col; j++) //recorres los renglones
                {
                    total += matriz[i][j];
                }
                sumas[i] = total;
            }
            int[] ordenado = sumas
            .Select((valor, indice) => new { Valor = valor, Indice = indice })  // Asociar valores con sus índices
            .OrderByDescending(x => x.Valor)  // Ordenar de mayor a menor por el valor
            .Select(x => x.Indice)  // Seleccionar solo los índices
            .ToArray();  // Convertir a array
            return ordenado;
        }
        public void LeerDatosDeArchivo(string rutaArchivo)
        {
            try
            {
                var lineas = File.ReadAllLines(rutaArchivo);

                // Suponiendo que la primera línea contiene las dimensiones
                var dimensiones = lineas[0].Split(',').Select(int.Parse).ToArray();
                ren = dimensiones[0];
                col = dimensiones[1];

                // Alternativas
                alternativas = new double[ren][];
                for (int i = 0; i < ren; i++)
                {
                    alternativas[i] = lineas[i + 1].Split(',').Select(double.Parse).ToArray();
                }

                // Pesos
                pesos = lineas[ren + 1].Split(',').Select(double.Parse).ToArray();

                // Nombres
                nombres = lineas[ren + 2].Split(',');

                // Tipo de criterio (1 = maximizar, 0 = minimizar)
                tipo = lineas[ren + 3].Split(',').Select(int.Parse).ToArray();

                Console.WriteLine("Datos leídos correctamente del archivo.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al leer el archivo: {ex.Message}");
            }
        }


        public void GuardarAlEOF(string rutaArchivo, string[] opcionesOrdenadas)
        {
            try
            {
                // Abrir el archivo en modo de adición para escribir al final
                using (StreamWriter writer = new StreamWriter(rutaArchivo, true)) // true para agregar al final
                {
                    writer.WriteLine("\nResultados del método SAW:"); // Encabezado opcional
                    foreach (var opcion in opcionesOrdenadas)
                    {
                        writer.WriteLine(opcion); // Escribir cada opción
                    }
                }
                Console.WriteLine("Resultados guardados correctamente al final del archivo.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar el archivo: {ex.Message}");
            }
        }


    }
}
