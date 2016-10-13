using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Facturador.Controllers
{
     public static class RC4
    {
        public static string allegedrc4(string codigo, string llavellegada)
        {
            int[] State = new int[256 + 1];
            string Mensaje = String.Empty;
            string llave = String.Empty;
            string MsgCif = String.Empty;
            int X = 0;
            int Y = 0;
            int Index1 = 0;
            int Index2 = 0;
            int NMen = 0;
            int i = 0;
            short op1 = 0;
            int aux = 0;
            int op2 = 0;
            string nrohex = String.Empty;


            X = 0;
            Y = 0;
            Index1 = 0;
            Index2 = 0;
            Mensaje = codigo;
            llave = llavellegada;
            for (i = 0; i <= 255.0; i += 1)
            {
                State[i] = i;
            }
            for (i = 0; i <= 255.0; i += 1)
            {
                op1 = (short)(llave.Substring(Index1 + 1 - 1, 1)[0]);
                Index2 = (op1 + State[i] + Index2) % 256;
                aux = State[i];
                State[i] = State[Index2];
                State[Index2] = aux;
                Index1 = (Index1 + 1) % llave.Length;
            }
            for (i = 0; i <= Mensaje.Length - 1; i += 1)
            {
                X = (X + 1) % 256;
                Y = (State[X] + Y) % 256;
                aux = State[X];
                State[X] = State[Y];
                State[Y] = aux;
                op1 = (short)(Mensaje.Substring(i + 1 - 1, 1)[0]);
                op2 = State[(State[X] + State[Y]) % 256];
                NMen = op1 ^ op2;
                nrohex = NMen.ToString("X");
                if (nrohex.Length == 1)
                {
                    nrohex = "0" + nrohex;
                }
                MsgCif = MsgCif + nrohex;
            }
            MsgCif = MsgCif.Substring(MsgCif.Length - (MsgCif.Length));
            return MsgCif;
        }

        public static string Encrypt(string key, string data)
        {

            Encoding unicode = Encoding.UTF8;

            return Convert.ToBase64String(Encrypt(unicode.GetBytes(key), unicode.GetBytes(data)));
        }

        public static string Decrypt(string key, string data)
        {
            Encoding unicode = Encoding.ASCII;

            return unicode.GetString(Encrypt(unicode.GetBytes(key), Convert.FromBase64String(data)));
        }

        public static byte[] Encrypt(byte[] key, byte[] data)
        {
            return EncryptOutput(key, data).ToArray();
        }

        public static byte[] Decrypt(byte[] key, byte[] data)
        {
            return EncryptOutput(key, data).ToArray();
        }

        private static byte[] EncryptInitalize(byte[] key)
        {
            byte[] s = Enumerable.Range(0, 256)
              .Select(i => (byte)i)
              .ToArray();

            for (int i = 0, j = 0; i < 256; i++)
            {
                j = (j + key[i % key.Length] + s[i]) & 255;

                Swap(s, i, j);
            }

            return s;
        }

        private static IEnumerable<byte> EncryptOutput(byte[] key, IEnumerable<byte> data)
        {
            byte[] s = EncryptInitalize(key);

            int i = 0;
            int j = 0;

            return data.Select((b) =>
            {
                i = (i + 1) & 255;
                j = (j + s[i]) & 255;

                Swap(s, i, j);

                return (byte)(b ^ s[(s[i] + s[j]) & 255]);
            });
        }

        private static void Swap(byte[] s, int i, int j)
        {
            byte c = s[i];

            s[i] = s[j];
            s[j] = c;
        }
    }
    public static class Verhoeff
    {
        // The multiplication table
        static int[,] d = new int[,]
        {
            {0, 1, 2, 3, 4, 5, 6, 7, 8, 9}, 
            {1, 2, 3, 4, 0, 6, 7, 8, 9, 5}, 
            {2, 3, 4, 0, 1, 7, 8, 9, 5, 6}, 
            {3, 4, 0, 1, 2, 8, 9, 5, 6, 7}, 
            {4, 0, 1, 2, 3, 9, 5, 6, 7, 8}, 
            {5, 9, 8, 7, 6, 0, 4, 3, 2, 1}, 
            {6, 5, 9, 8, 7, 1, 0, 4, 3, 2}, 
            {7, 6, 5, 9, 8, 2, 1, 0, 4, 3}, 
            {8, 7, 6, 5, 9, 3, 2, 1, 0, 4}, 
            {9, 8, 7, 6, 5, 4, 3, 2, 1, 0}
        };

        // The permutation table
        static int[,] p = new int[,]
        {
            {0, 1, 2, 3, 4, 5, 6, 7, 8, 9}, 
            {1, 5, 7, 6, 2, 8, 3, 0, 9, 4}, 
            {5, 8, 0, 3, 7, 9, 6, 1, 4, 2}, 
            {8, 9, 1, 6, 0, 4, 3, 5, 2, 7}, 
            {9, 4, 5, 3, 1, 2, 6, 8, 7, 0}, 
            {4, 2, 8, 6, 5, 7, 3, 9, 0, 1}, 
            {2, 7, 9, 3, 8, 0, 6, 4, 1, 5}, 
            {7, 0, 4, 6, 9, 1, 3, 2, 5, 8}
        };

        // The inverse table
        static int[] inv = { 0, 4, 3, 2, 1, 5, 6, 7, 8, 9 };


        /// <summary>
        /// Validates that an entered number is Verhoeff compliant.
        /// NB: Make sure the check digit is the last one!
        /// </summary>
        /// <param name="num"></param>
        /// <returns>True if Verhoeff compliant, otherwise false</returns>
        public static bool validateVerhoeff(string num)
        {
            int c = 0;
            int[] myArray = StringToReversedIntArray(num);

            for (int i = 0; i < myArray.Length; i++)
            {
                c = d[c, p[(i % 8), myArray[i]]];
            }

            return c == 0;

        }

        /// <summary>
        /// For a given number generates a Verhoeff digit
        /// Append this check digit to num
        /// </summary>
        /// <param name="num"></param>
        /// <returns>Verhoeff check digit as string</returns>
        public static string generateVerhoeff(string num)
        {
            int c = 0;
            int[] myArray = StringToReversedIntArray(num);

            for (int i = 0; i < myArray.Length; i++)
            {
                c = d[c, p[((i + 1) % 8), myArray[i]]];
            }

            return inv[c].ToString();
        }

        public static string generateVerhoeff_iterative(string num, int iterations)
        {
            if (iterations == 1)
            {
                return generateVerhoeff(num);
            }
            else
            {
                string verhoeff = generateVerhoeff(num);
                verhoeff = verhoeff + generateVerhoeff_iterative(num + verhoeff, iterations - 1);

                return verhoeff;
            }
        }


        /// <summary>
        /// Converts a string to a reversed integer array.
        /// </summary>
        /// <param name="num"></param>
        /// <returns>Reversed integer array</returns>
        private static int[] StringToReversedIntArray(string num)
        {
            int[] myArray = new int[num.Length];

            for (int i = 0; i < num.Length; i++)
            {
                myArray[i] = int.Parse(num.Substring(i, 1));
            }

            Array.Reverse(myArray);

            return myArray;

        }
    }
    public static class NumLetra
    {
        public static String Convertir(String numero, bool mayusculas)
        {
            Regex r;
            String literal = "";
            String parte_decimal;
            //si el numero utiliza (.) en lugar de (,) -> se reemplaza
            numero = numero.Replace(".", ",");

            //si el numero no tiene parte decimal, se le agrega ,00
            if (numero.IndexOf(",") == -1)
            {
                numero = numero + ",00";
            }
            //se valida formato de entrada -> 0,00 y 999 999 999,00
            r = new Regex(@"\d{1,9},\d{1,2}");
            MatchCollection mc = r.Matches(numero);
            if (mc.Count > 0)
            {
                //se divide el numero 0000000,00 -> entero y decimal
                String[] Num = numero.Split(',');

                //de da formato al numero decimal
                parte_decimal = Num[1] + "/100 ";
                //se convierte el numero a literal
                if (int.Parse(Num[0]) == 0)
                {//si el valor es cero                
                    literal = "cero ";
                }
                else if (int.Parse(Num[0]) > 999999)
                {//si es millon
                    literal = getMillones(Num[0]);
                }
                else if (int.Parse(Num[0]) > 999)
                {//si es miles
                    literal = getMiles(Num[0]);
                }
                else if (int.Parse(Num[0]) > 99)
                {//si es centena
                    literal = getCentenas(Num[0]);
                }
                else if (int.Parse(Num[0]) > 9)
                {//si es decena
                    literal = getDecenas(Num[0]);
                }
                else
                {//sino unidades -> 9
                    literal = getUnidades(Num[0]);
                }
                //devuelve el resultado en mayusculas o minusculas
                if (mayusculas)
                {
                    return (literal + parte_decimal).ToUpper();
                }
                else
                {
                    return (literal + parte_decimal);
                }
            }
            else
            {//error, no se puede convertir
                return literal = null;
            }
        }

        /* funciones para convertir los numeros a literales */

        private static String getUnidades(String numero)
        {   // 1 - 9            
            //si tuviera algun 0 antes se lo quita -> 09 = 9 o 009=9
            String[] UNIDADES = { "", "un ", "dos ", "tres ", "cuatro ", "cinco ", "seis ", "siete ", "ocho ", "nueve " };
            String num = numero.Substring(numero.Length - 1);
            return UNIDADES[int.Parse(num)];
        }

        private static String getDecenas(String num)
        {// 99                  
            String[] DECENAS = {"diez ", "once ", "doce ", "trece ", "catorce ", "quince ", "dieciseis ",
        "diecisiete ", "dieciocho ", "diecinueve", "veinte ", "treinta ", "cuarenta ",
        "cincuenta ", "sesenta ", "setenta ", "ochenta ", "noventa "};
            int n = int.Parse(num);
            if (n < 10)
            {//para casos como -> 01 - 09
                return getUnidades(num);
            }
            else if (n > 19)
            {//para 20...99
                String u = getUnidades(num);
                if (u.Equals(""))
                { //para 20,30,40,50,60,70,80,90
                    return DECENAS[int.Parse(num.Substring(0, 1)) + 8];
                }
                else
                {
                    return DECENAS[int.Parse(num.Substring(0, 1)) + 8] + "y " + u;
                }
            }
            else
            {//numeros entre 11 y 19
                return DECENAS[n - 10];
            }
        }

        private static String getCentenas(String num)
        {// 999 o 099
            String[] CENTENAS = {"", "ciento ", "doscientos ", "trecientos ", "cuatrocientos ", "quinientos ", "seiscientos ",
        "setecientos ", "ochocientos ", "novecientos "};
            if (int.Parse(num) > 99)
            {//es centena
                if (int.Parse(num) == 100)
                {//caso especial
                    return " cien ";
                }
                else
                {
                    return CENTENAS[int.Parse(num.Substring(0, 1))] + getDecenas(num.Substring(1));
                }
            }
            else
            {//por Ej. 099 
                //se quita el 0 antes de convertir a decenas
                return getDecenas(int.Parse(num) + "");
            }
        }

        private static String getMiles(String numero)
        {// 999 999
            //obtiene las centenas
            String c = numero.Substring(numero.Length - 3);
            //obtiene los miles
            String m = numero.Substring(0, numero.Length - 3);
            String n = "";
            //se comprueba que miles tenga valor entero
            if (int.Parse(m) > 0)
            {
                n = getCentenas(m);
                return n + "mil " + getCentenas(c);
            }
            else
            {
                return "" + getCentenas(c);
            }

        }

        private static String getMillones(String numero)
        { //000 000 000        
            //se obtiene los miles
            String miles = numero.Substring(numero.Length - 6);
            //se obtiene los millones
            String millon = numero.Substring(0, numero.Length - 6);
            String n = "";
            if (millon.Length > 1)
            {
                n = getCentenas(millon) + "millones ";
            }
            else
            {
                n = getUnidades(millon) + "millon ";
            }
            return n + getMiles(miles);
        }

    }
    public class Helper
    {
        public static string ConvertToBase64(int numero)
        {
            string[] diccionario = new string[64] {
                "0", "1", "2", "3", "4", "5","6","7","8","9","A","B","C","D","E","F","G","H","I","J","K" ,"L","M","N","O","P","Q","R","S","T","U","V" ,"W","X","Y","Z","a","b","c","d","e","f","g" ,"h","i","j","k","l","m","n","o","p","q","r" ,"s","t","u","v","w","x","y","z","+","/"
              };
            int cociente = 1; int resto; string palabra = "";
            while (cociente > 0)
            {
                cociente = numero / 64;
                resto = numero % 64;
                palabra = diccionario[resto] + palabra;
                numero = cociente;

            }
            return (palabra);
        }
        public static Byte[] PdfSharpConvert(String html)
        {
            Byte[] res = null;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                var pdf = TheArtOfDev.HtmlRenderer.PdfSharp.PdfGenerator.GeneratePdf(html, PdfSharp.PageSize.A4);
                pdf.Save(ms);
                res = ms.ToArray();
            }
            return res;
        }
        public static string Obtener_codigo_control(string numeroAutorizacion, string numeroFactura, string nitCliente, string fechaTransaccion, string montoTransaccion, string llaveTransaccion)
        {
            string codigoControl = "";
            montoTransaccion = Math.Round(Convert.ToDecimal(montoTransaccion), 0).ToString("0.##");

            //1erPaso añadir verhoeff 2 diitos a cada uno
            numeroFactura = numeroFactura + Verhoeff.generateVerhoeff_iterative(numeroFactura, 2);
            //numeroFactura = numeroFactura + Verhoeff.generateVerhoeff(numeroFactura);
            nitCliente = nitCliente + Verhoeff.generateVerhoeff_iterative(nitCliente, 2);
            fechaTransaccion = fechaTransaccion + Verhoeff.generateVerhoeff_iterative(fechaTransaccion, 2);
            montoTransaccion = montoTransaccion + Verhoeff.generateVerhoeff_iterative(montoTransaccion, 2);
            //2. efectuar la sumatoria
            long sumatoria = long.Parse(numeroFactura) + long.Parse(nitCliente) + long.Parse(fechaTransaccion) + long.Parse(montoTransaccion);

            //2. generar 5 digitos verhoeff
            string num5Verhoeff = Verhoeff.generateVerhoeff_iterative(sumatoria.ToString(), 5);
            //3. del numero verhoeff concatenar digitos de la llave de transaccion
            int longitudD1 = Convert.ToInt32(num5Verhoeff.Substring(0, 1)) + 1;
            int longitudD2 = Convert.ToInt32(num5Verhoeff.Substring(1, 1)) + 1;
            int longitudD3 = Convert.ToInt32(num5Verhoeff.Substring(2, 1)) + 1;
            int longitudD4 = Convert.ToInt32(num5Verhoeff.Substring(3, 1)) + 1;
            int longitudD5 = Convert.ToInt32(num5Verhoeff.Substring(4, 1)) + 1;
            numeroAutorizacion = numeroAutorizacion + llaveTransaccion.Substring(0, longitudD1);
            numeroFactura = numeroFactura + llaveTransaccion.Substring(longitudD1, longitudD2);
            nitCliente = nitCliente + llaveTransaccion.Substring(longitudD1 + longitudD2, longitudD3);
            fechaTransaccion = fechaTransaccion + llaveTransaccion.Substring(longitudD1 + longitudD2 + longitudD3, longitudD4);
            montoTransaccion = montoTransaccion + llaveTransaccion.Substring(longitudD1 + longitudD2 + longitudD3 + longitudD4, longitudD5);
            string concatenarV1 = numeroAutorizacion + numeroFactura + nitCliente + fechaTransaccion + montoTransaccion;
            string llaveConcatenadaV1 = llaveTransaccion + num5Verhoeff;
            //4. aplicar AllegedRC4 con la cadena y la llave
            string allegedRC4V1 = RC4.allegedrc4(concatenarV1, llaveConcatenadaV1);
            //string allegedRC4V1 = RC4.Encrypt(llaveConcatenadaV1,concatenarV1);
            //5. sumatoria en ascii y sumatoria de dividendos
            byte[] ASCIIValues = System.Text.Encoding.ASCII.GetBytes(allegedRC4V1);
            int sumatoriaASCII = 0;
            foreach (byte b in ASCIIValues)
            {
                sumatoriaASCII += Convert.ToInt32(b);

            }
            int sumatoria1 = 0;
            int sumatoria2 = 0;
            int sumatoria3 = 0;
            int sumatoria4 = 0;
            int sumatoria5 = 0;
            for (int i = 1; i <= 5; i++)
            {
                for (int j = i - 1; j < ASCIIValues.Length; j += 5)
                {
                    switch (i)
                    {
                        case 1:
                            sumatoria1 += ASCIIValues[j];
                            break;
                        case 2:
                            sumatoria2 += ASCIIValues[j];
                            break;
                        case 3:
                            sumatoria3 += ASCIIValues[j];
                            break;
                        case 4:
                            sumatoria4 += ASCIIValues[j];
                            break;
                        case 5:
                            sumatoria5 += ASCIIValues[j];
                            break;
                        default:
                            break;
                    }

                }
            }
            sumatoria1 = (sumatoriaASCII * sumatoria1) / longitudD1;
            sumatoria2 = (sumatoriaASCII * sumatoria2) / longitudD2;
            sumatoria3 = (sumatoriaASCII * sumatoria3) / longitudD3;
            sumatoria4 = (sumatoriaASCII * sumatoria4) / longitudD4;
            sumatoria5 = (sumatoriaASCII * sumatoria5) / longitudD5;
            int sumatoriaFinal = sumatoria1 + sumatoria2 + sumatoria3 + sumatoria4 + sumatoria5;
            //6. convertir sumatoria en base64
            string base64 = ConvertToBase64(sumatoriaFinal);
            //7. convertir a alleged rc4 llave de cifrado con digitos verhoeff usando como clave el base64 obtenido
            string llaveCifrado = llaveTransaccion + num5Verhoeff;
            codigoControl = RC4.allegedrc4(base64, llaveCifrado);
            return codigoControl;

        }
    }
}