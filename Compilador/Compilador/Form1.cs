using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;

namespace Compilador
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string unir_cad = "";
        string espacio = "['']";
        string salto = "['\n']";

        string unir_string = "";
        string unir_com = " ";

        int contar_columas = 1;
        int contar_lineas = 1;
        char validar_uno_mas = '0';

        public void Analizar()
        {
            dataGridView2.Rows.Clear();

            contar_columas = 1;
            contar_lineas = 1;


            char validar_com = '0';
            char validar_cad_string = '0';

            string comentario = "@";
            string cad_string = "[\"]";
            string texto = textBox1.Text;

            foreach (char letra in texto) //recorro el texto para extraer los tokens
            {
                string letra2 = letra.ToString(); //convierto a str el objeto para poder manejarlo 


                if (Regex.IsMatch(letra2, cad_string))//uso regex para poder tratar el str con expreciones regulares
                {
                    if (validar_cad_string.Equals('0'))
                    {
                        validar_cad_string = '1';
                    }
                    else
                    {
                        dataGridView2.Rows.Add(unir_string + "\"", "CADENA", contar_lineas, contar_columas);
                        validar_cad_string = '0';
                        unir_string = "";
                    }
                }

                if (validar_cad_string.Equals('1'))
                {
                    unir_string = unir_string + letra2;
                }


                if (Regex.IsMatch(letra2, comentario))//elimino con expresiones regulares symbolos no validos no validos
                {
                    validar_com = '1';
                }

                if (validar_com.Equals('1'))
                {
                    unir_com = unir_com + letra2;

                    if (letra.Equals('\n'))
                    {

                        dataGridView2.Rows.Add(unir_com + "", "COMENTARIO", contar_lineas, contar_columas);
                        contar_lineas += 1;
                        contar_columas = 1;
                        validar_com = '0';
                        unir_com = "";


                    }

                }

                else if (validar_com.Equals('0') & validar_cad_string.Equals('0') & letra2 != "\"" & letra2 != "\r")
                {
                    if (letra2 == " " || letra2 == "\n")
                    {
                        this.AnalizarPalabras();

                        if (Regex.IsMatch(letra2, espacio))// valido espacios 
                        {
                            contar_columas += 1;
                        }
                        if (Regex.IsMatch(letra2, salto))//valido saltos 
                        {
                            contar_lineas += 1;
                            contar_columas = 1;
                        }
                    }
                    else
                    {
                        unir_cad = unir_cad + letra2;
                    }
                }
            } //fin foreach
        }
        public void AnalizarPalabras()
        {
            //esta funcion valida las palabras contengan el formato adecuado 
            string exp_minusculas = "[a-z]+";
            string exp_MAYUSCULAS = "[A-Z]+";
            if (Regex.IsMatch(unir_cad, exp_minusculas) || Regex.IsMatch(unir_cad, exp_MAYUSCULAS))
            {
                dataGridView2.Rows.Add(unir_cad + "", "ERROR", contar_lineas, contar_columas);
                unir_cad = "";
            }
            else
            {
                this.VerificarLexema();
            }
        }
        public void VerificarLexema()
        {
            //en esta funcion valido que las cadenas de caracteres con cuerden con los tokens
            string[] reservado = { "inicio", "proceso", "fin", "si", "ver", "mientras", "entero", "cadena" };
            string exp_numeros = "^[0-9]+$[0-9]?";
            string exp_delimitador = "^[;|(|)|{|}]$";
            string exp_operadores = "^[+|-|/|*]$";
            string asignacion = "^#$";
            string exp_comparador = "^[<|>]$|^==$";
            string variable = "^var[(0-9)?]$";
            char validar_reservado = '0';

            for (int i = 0; i < 8; i++)
            {
                if (unir_cad.Equals(reservado[i]))
                {
                    dataGridView2.Rows.Add(unir_cad + "", "RESERVADO", contar_lineas, contar_columas);
                    validar_reservado = '1';
                    if (Regex.IsMatch(unir_cad, "si"))
                    {
                        validar_uno_mas = '1';
                    }
                }
            }

            if (Regex.IsMatch(unir_cad, exp_numeros))
            {
                dataGridView2.Rows.Add(unir_cad + "", "NUMERO", contar_lineas, contar_columas);
            }
            else if (Regex.IsMatch(unir_cad, exp_delimitador))
            {
                dataGridView2.Rows.Add(unir_cad + "", "DELIMITADOR", contar_lineas, contar_columas);

            }
            else if (Regex.IsMatch(unir_cad, exp_operadores))
            {
                dataGridView2.Rows.Add(unir_cad + "", "OPERADOR", contar_lineas, contar_columas);
            }
            else if (Regex.IsMatch(unir_cad, asignacion))
            {
                dataGridView2.Rows.Add(unir_cad + "", "ASIGNACION", contar_lineas, contar_columas);
            }
            else if (Regex.IsMatch(unir_cad, exp_comparador))
            {
                dataGridView2.Rows.Add(unir_cad + "", "COMPARADOR", contar_lineas, contar_columas);
            }
            else if (Regex.IsMatch(unir_cad, variable))
            {
                dataGridView2.Rows.Add(unir_cad + "", "VARIABLE", contar_lineas, contar_columas);
            }

            else if (validar_reservado.Equals('0') & unir_cad != "" & unir_cad != "\"")
            {

                dataGridView2.Rows.Add(unir_cad + "", "ERROR", contar_lineas, contar_columas);

            }
            unir_cad = "";
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //LEXICO
            dataGridView2.Rows.Clear();
            label3.Text = "";
            this.Analizar();

            int contador_errores = 0;
            for (int x = 0; x < dataGridView1.RowCount; x++)
            {
                if ((dataGridView2.Rows[x].Cells[1].Value.ToString()).Equals("ERROR"))
                {
                    contador_errores += 1;
                    dataGridView2.Rows[x].DefaultCellStyle.BackColor = Color.Pink;

                }
            }
            if (contador_errores > 0)
            {
                button2.Enabled = false;

                this.label3.Text = "ERRORES LEXICOS TIENE: " + contador_errores;
                this.label3.Visible = true;
            }
            else
            {
                button2.Enabled = true;
            }

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            dataGridView2.Rows.Clear();
            button2.Enabled = false;
            this.label3.Visible = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //COMPILACION
        }
    }
}
