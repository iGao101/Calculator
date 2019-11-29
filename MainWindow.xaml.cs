using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Logic;

namespace Calculator
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        TextBox TempResult = null;  //保存result和input，用于模式切换
        TextBox TempInut = null;
        public MainWindow()
        {
            InitializeComponent();
            TempResult = this.Result;
            TempInut = this.Input;
        }

        //当选项卡发生变化时
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabControl.SelectedIndex == 0)  //界面一，即标准计算器
            {
                this.Result = TempResult;
                this.Input = TempInut;
            }
            if (tabControl.SelectedIndex == 1)  //界面二，即科学计算器
            {
                this.Result = this.Result1;
                this.Input = this.Input1;
            }
        }

        //存储操作数和操作符
        public ArrayList arrayList = new ArrayList();

        //状态栏隐藏后窗体拖动
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)  //解决此报错：调度程序进程已挂起,但消息仍在处理中
            {
                Dispatcher.BeginInvoke(new Action(() => {
                    this.DragMove();
                }));
            }
        }

        //点击Clean数据回退
        private void Clean_Button(object sender, RoutedEventArgs e)
        {
            if (isError)  //点击clean，回复按钮可用性
            {
                this.Input.Text = "";
                return;
            }

            Thread.Sleep(200);       //睡眠0.2s
            if(this.Input.Text != "")  //首先清空输入框
            {
                this.Input.Text = "";
                return;
            }
            
            if (this.Result.Text != "")
            {
                this.Result.Text = "";
                if(arrayList.Count != 0)
                    arrayList.RemoveAt(arrayList.Count - 1);
                for (int i = 0; i < arrayList.Count; i++)
                    this.Result.Text += arrayList[i];
            }
            else
                return;
        }

        //正负号
        private bool isPositive = false;  //点击两次为符号
        private void Symbol_Button(object sender, RoutedEventArgs e)
        {
            if (this.Input.Text == "")
                return;

            isPositive = isPositive == true ? false : true;
            string num = this.Input.Text;  //提取输入框数字
            if (isPositive)  //正数
            {
                if (num.Contains("-"))
                    num = num.Replace("-", "+");
                else
                    num = "+" + num;
            }
            else
            {
                if (num.Contains("+"))
                    num = num.Replace("+", "-");
                else
                    num = "-" + num;
            }
            Input.Text =  num;
        }

        //数字
        private void Number_Button(object sender, RoutedEventArgs e)
        {
            if (this.Result.Text.Contains("="))
                this.Result.Text = "";
            if (this.Input.Text.Contains("!"))
                return;
            var button = sender as Button;
            this.Input.Text = this.Input.Text + button.Content;
        }

        //运算符
        private void Operator_Button(object sender, RoutedEventArgs e)
        {
            string num = this.Input.Text;      //数字
            if ((num == "" && this.Result.Text =="") || ( num != "" && num[num.Length - 1] == '.'))  //不允许以小数点结尾
                return;

            string operation = "";   //操作符
            bool isSquare = false;  //判断是否为开方或次方运算
            string index = "";         //保存指数
            var button = sender as Button;
            switch (button.Content)
            {
                case "+":
                case "-":
                case "%":
                    operation = (string)button.Content;
                    break;
                case "×":
                    operation = "*";
                    break;
                case "÷":
                    operation = "/";
                    break;
                case "x²":
                    isSquare = true;
                    index = "2";
                    break;
                case "x³":
                    isSquare = true;
                    index = "3";
                    break;
                case "√x":
                    isSquare = true;
                    index = "0.5";
                    break;
                case "1/x":
                    isSquare = true;
                    index = "-1";
                    break;
                case "xʸ":
                    if (num != "")
                    {
                        arrayList.Add(num);
                        this.Result.Text = this.Result.Text + num;
                    }
                    arrayList.Add("^");
                    arrayList.Add("(");
                    this.Result.Text = this.Result.Text + "^" + "(";
                    this.Input.Text = "";
                    return;
                case "y√x":
                    if (num != "")
                    {
                        arrayList.Add(num);
                        this.Result.Text = this.Result.Text + num;
                    }
                    arrayList.Add("^");
                    arrayList.Add("(");
                    arrayList.Add("1");
                    arrayList.Add("/");
                    this.Result.Text = this.Result.Text + "^(1/";
                    this.Input.Text = "";
                    return;
            }

            if (isSquare)  //为开方或者次方运算
            {
                if (num != "")
                {
                    this.Result.Text = this.Result.Text + num;
                    arrayList.Add(num);
                    this.Input.Text = "";
                }
                arrayList.Add("^");
                arrayList.Add(index);
                this.Result.Text = this.Result.Text + "^" + index;
                return;
            }

            if (this.Input.Text == "")  //如果此时为空，表示已经点击过运算符，此时进行替换
            {
                if (this.Result.Text == "")
                    return;
                if(IsOperation(this.Result.Text[this.Result.Text.Length - 1]))  //修改result运算符
                {
                    this.Result.Text = "";
                    arrayList.RemoveAt(arrayList.Count - 1);
                    arrayList.Add(operation);
                    for (int i = 0; i < arrayList.Count; i++)
                        this.Result.Text += arrayList[i];
                    return;
                }
                else  //删除数据后，数组中最后一元素为数字的情况
                {
                    this.Result.Text = this.Result.Text + operation;
                    arrayList.Add(operation);
                    return;
                }
            }

            this.Input.Text = "";
            if (this.Result.Text.Contains("="))
                this.Result.Text = "";
            this.Result.Text = this.Result.Text + num + operation;
            if (isN)
            {
                isN = false;  //阶乘阶段已经添加过数据
                arrayList.Add(operation);
                return;
            }
            arrayList.Add(num);
            arrayList.Add(operation);
        }

        //退出程序
        private void Exit_Button(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //小数点
        private void Dot_Button(object sender, RoutedEventArgs e)
        {
            if (this.Input.Text == "" || this.Input.Text.Contains("."))
                return;
            var button = sender as Button;
            this.Input.Text = this.Input.Text + button.Content;
        }

        //等于
        private bool isError = false;
        private void Equal_Button(object sender, RoutedEventArgs e)
        {
            string num = this.Input.Text;
            if(num != "")
                arrayList.Add(num);
            Logical logical = new Logical(); 
            string result = logical.Analysis(arrayList);  //引用dll文件计算方法
            if(result == "")  //计算出错
            {
                this.Input.Text = "Error";
                this.Result.Text = "";
                isError = true;
                arrayList.Clear();
                return;
            }
            this.Result.Text = this.Result.Text + num + "=" + result;
            this.Input.Text = result;
            arrayList.Clear();
        }

        //转换模式
        private bool isShift = false;
        private void Shift_Button(object sender, RoutedEventArgs e)
        {
            isShift = isShift == false ? true : false;
            if (isShift)  //按键名字变换
            {
                Square.Content = "x³";
                MulSquare.Content = "y√x";
                Sin.Content = "sin-1";
                Cos.Content = "cos-1";
                Tan.Content = "tan-1";
                Radical.Content = "1/x";
                NumSquare.Content = "e^x";
            }
            else
            {
                Square.Content = "x²";
                MulSquare.Content = "xʸ";
                Sin.Content = "sin";
                Cos.Content = "cos";
                Tan.Content = "tan";
                Radical.Content = "√x";
                NumSquare.Content = "10^x";
            }
        }

        //左括号、右括号
        private void Bracket_Button(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            string value = (string)button.Content;
            if(value == "(")
            {
                if (this.Input.Text != "")
                    return;
            }
            else
            {
                string num = this.Input.Text;
                if (num != "")
                    arrayList.Add(num);
                this.Result.Text = this.Result.Text + num;
                this.Input.Text = "";
            }
            this.Result.Text = this.Result.Text + value;
            arrayList.Add(value);
        }

        //圆周率和e
        private void Pi_Button(object sender, RoutedEventArgs e)
        {
            if (this.Input.Text != "")
                return;
            var button = sender as Button;
            this.Input.Text = (string)button.Content;
        }

        //阶乘
        bool isN = false;
        private void N_Button(object sender, RoutedEventArgs e)
        {
            string num = this.Input.Text;  //进行阶乘运算的数字
            if (num == "")
                return;
            isN = true;
            this.Input.Text = this.Input.Text + " !";
            arrayList.Add(num);
            arrayList.Add("!");
        }

        //三角函数
        private void Triangle_Button(object sender, RoutedEventArgs e)
        {
            if (this.Input.Text != "")
                return;
            var button = sender as Button;
            arrayList.Add(button.Content + "(");  //三角函数与左括号存储在一起
            this.Result.Text = this.Result.Text + (string)button.Content + "(";
        }

        //固定数的次方
        private void MulSquare_Button(object sender, RoutedEventArgs e)
        {
            if (this.Input.Text != "")
                return;
            var button = sender as Button;
            if ((string)button.Content == "e^x")
            {
                arrayList.Add("e");
                arrayList.Add("^");
                arrayList.Add("(");
                this.Result.Text = this.Result.Text + "e^(";
            }
            else if ((string)button.Content == "10^x")
            {
                arrayList.Add("10");
                arrayList.Add("^");
                arrayList.Add("(");
                this.Result.Text = this.Result.Text + "10^(";
            }
        }

        //判断是否为运算符
        private bool IsOperation(char value)
        {
            if (value == '+' || value == '-' || value == '*' || value == '/' || value == '%')
                return true;
            return false;
        }
    }
}
