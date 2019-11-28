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

namespace Calculator
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Clean.AddHandler(Button.MouseDownEvent, new RoutedEventHandler(Clean_ButtonDown), true);
        }

        //存储操作数和操作符
        public ArrayList arrayList = new ArrayList();

        //状态栏隐藏后窗体拖动
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Dispatcher.BeginInvoke(new Action(() => {
                    this.DragMove();
                }));
            }
        }

        //点击Clear数据回退
        private void Clean_ButtonDown(object sender, RoutedEventArgs e)
        {
            this.Input.Text = "";
            Thread.Sleep(200);  //睡眠0.2s
            if (this.Result.Text != "")
            {
                this.Result.Text = "";
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
            if (this.Input.Text.Contains("("))
                return;
            var button = sender as Button;
            this.Input.Text = this.Input.Text + button.Content;
        }

        //运算符
        private void Operator_Button(object sender, RoutedEventArgs e)
        {
            string num = this.Input.Text;      //数字
            if (num != "" && num[num.Length - 1] == '.')  //不允许以小数点结尾
                return;

            string operation = "";   //操作符
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
            }

            if (this.Input.Text == "")  //如果此时为空，表示已经点击过运算符，此时进行替换
            {
                if (this.Result.Text == "")
                    return;
                this.Result.Text = "";  //修改result运算符
                arrayList.RemoveAt(arrayList.Count - 1);
                arrayList.Add(operation);
                for (int i = 0; i < arrayList.Count; i++)
                    this.Result.Text += arrayList[i];
                return;
            }

            this.Input.Text = "";
            this.Result.Text = this.Result.Text + num + operation;
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
        private void Equal_Button(object sender, RoutedEventArgs e)
        {
            //此处运算结束后需要重置arraylist
        }
    }
}
