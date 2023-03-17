using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {

        private string expression = "";
        private string last;
        private string old;
        private string updated;
        private int lastNumIndex;
        private int lastPartIndex;
        private double result;

        public Form1()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, EventArgs e)
        {
            warningTextBox.Text = "";
          
            Button button = sender as Button;
            if (button.Name == "buttonChange")
            {             
                lastNumIndex = expression.LastIndexOfAny("0123456789".ToCharArray());  
                lastPartIndex = expression.LastIndexOfAny("+-/*".ToCharArray());
                if (lastNumIndex >= 0)
                {
                    expression = changingSignController(lastPartIndex, this.expression);                
                    // Wstaw odpowiednią wartość do glownego panelu
                    textBoxOutput.Text = expression;
                }
            }
            // Jesli wyrazenie puste to po prostu dopisz i wyswietl w panelu
            else
            {  
                expression = changeOperator((button).Text, expression);
                expression = doubleDotCheck(expression);
                expression = removeInvalidDots(expression);
                expression = doubleDivideCheck(expression);
                expression = doubleMultipleCheck(expression);
                textBoxOutput.Text = expression;
            }
        }

        private void buttonEquals_Click(object sender, EventArgs e)
        {

            string mathOperation = expression.ToString();
      
            try
            {
                mathOperation = transformExpressionToDouble(mathOperation);
                textBoxOutput.Text = new DataTable().Compute(mathOperation, null).ToString();
                checkDivisionByZero(mathOperation);
                expression = textBoxOutput.Text;
            }
            catch (Exception ex)
            {
                warningTextBox.Text = "Niepoprawna forma wyrażenia: " + textBoxOutput.Text;
                textBoxOutput.Text = "0";
                expression = "";
            }

        }

        private string changeOperator(string newSign, string expression)
        {
            if (expression.Length <= 1)
            {
                expression += newSign;
                return expression;
            }
            string lastExpressionSign = expression.Substring(expression.Length - 1);
            if (!lastExpressionSign.Equals(newSign) && "+-/*".Contains(newSign) && "+-*/".Contains(lastExpressionSign))
            {
                int lastIndex = expression.LastIndexOf(lastExpressionSign);
                expression = expression.Substring(0, lastIndex) + newSign;
            }
            else
            {
                expression += newSign;
            }
          
            return expression;
        }


        private string changingSignController(int lastPartIndex, string expression)
        {

            if (lastPartIndex == -1)
            {
                lastPartIndex = 0;
            }
            last = expression.Substring(lastPartIndex);
            old = last;

            if (last == "0")
            {

            }
            else
            {
                if (expression.Contains("+") || expression.Contains("-") || expression.Contains("*") || expression.Contains("/"))
                {
                    if (last.StartsWith("+"))
                    {
                        updated = last.Replace("+", "-");
                    }
                    else if (last.StartsWith("-"))
                    {
                        updated = last.Replace("-", "+");
                    }
                    else if (last.StartsWith("/"))
                    {
                        updated = last.Replace("/", "/-");
                    }
                    else if (last.StartsWith("*"))
                    {
                        updated = last.Replace("*", "*-");
                    }
                    expression = expression.Replace(old, updated);
                }
                else
                {
                    expression = "-" + expression;
                }
            }
           

            return expression;
        }

        private void checkDivisionByZero(string expression)
        {
            int divisionSignIndex = expression.IndexOf("/");
            if (divisionSignIndex != -1 && expression[divisionSignIndex+1] == '0' && expression[divisionSignIndex + 2]!='.')
            {       
                warningTextBox.Text = "Nie można dzielić przez 0!";
                textBoxOutput.Text = "";
          
            }
            else
            {
                warningTextBox.Text = "Nie można dzielić przez 0!";
                warningTextBox.Text = "";
            }
        }


        private string transformExpressionToDouble(string expression)
        {
            for(int i=0; i<expression.Length-1; i++)
            {
                if ("*/+-".Contains(expression[i]) && !expression.Substring(0, i).Contains("."))
                {
                    expression = expression.Substring(0, i) + ".0" + expression.Substring(i);
                    i = i + 2;
                }
            }
            return expression;
        }

        private string doubleDotCheck(string expression)
        {
            if (!expression.Equals(""))
            {
                for (int i = 0; i < expression.Length; i++)
                {
                    if (expression[i] == '.' && i>0)
                    {
                        if (expression[i - 1] == '.')
                        {
                            warningTextBox.Text = "Nie można używać dwóch kropek obok siebie";
                            expression = expression.Substring(0, i);
                        }
                    }
                }
            }
            return expression;
        }
      

        public string removeInvalidDots(string expression)
        {
            int countDots; 
            int secondDotIndex;
            string part;
            string finalString="";
            List<string> parts = splitExpression(expression);

                for (int i = 0; i < parts.Count; i++)
                {
                    part = parts[i];
                    if (!(part.Contains("+") || part.Contains("-") || part.Contains("*") || part.Contains("/")))
                    {
                        countDots = part.Count(c => c == '.');
                        secondDotIndex = part.LastIndexOf(".");
                        if (countDots > 1)
                        {
                            part = part.Substring(0, secondDotIndex);
                        }
                    }
                                    
                    finalString += part;
            }
         
          
            return finalString;
        }

        public List<string> splitExpression(string expression)
        {
            List<string> parts = new List<string>();
            string currentPart = "";

            foreach (char c in expression)
            {
                if (c == '+' || c == '-' || c == '*' || c == '/')
                {
                    if (currentPart != "")
                    {
                        parts.Add(currentPart);
                    }
                    parts.Add(c.ToString());
                    currentPart = "";
                }
                else
                {
                    currentPart += c;
                }
            }

            if (currentPart != "")
            {
                parts.Add(currentPart);
            }

            return parts;
        }




        private string doubleDivideCheck(string expression)
        {
            if (!expression.Equals(""))
            {
                for (int i = 0; i < expression.Length; i++)
                {
                    if (expression[i] == '/' && i > 0)
                    {
                        if (expression[i - 1] == '/')
                        {
                            warningTextBox.Text = "Nie można używać dwóch // obok siebie";
                            expression = expression.Substring(0, i);
                        }
                    }
                }
            }
            return expression;
        }

        private string doubleMultipleCheck(string expression)
        {
            if (!expression.Equals(""))
            {
                for (int i = 0; i < expression.Length; i++)
                {
                    if (expression[i] == '*' && i > 0)
                    {
                        if (expression[i - 1] == '*')
                        {
                            warningTextBox.Text = "Nie można używać dwóch * obok siebie";
                            expression = expression.Substring(0, i);
                        }
                    }
                }
            }
            return expression;
        }


        private void buttonC_Click(object sender, EventArgs e)
        {
            warningTextBox.Text = "";
            expression = "";
            textBoxOutput.Text = "0";
        }

        private void buttonCE_Click(object sender, EventArgs e)
        {
            warningTextBox.Text = "";
            if (expression.Length > 0)
            {
                int newLength = expression.Length - 1;
                expression = string.Concat(expression.Take(newLength));
                textBoxOutput.Text = expression;
            }      
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void warningTextBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
